﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OleDb;
using NtccSteward.Core.Import;
using NtccSteward.Core.Models.Church;
using NtccSteward.Core.Models.Import;
using NtccSteward.Repository.Framework;
using NtccSteward.Core.Models.Team;
using NtccSteward.Core.Models.Members;
using NtccSteward.Core.Models.Message;
using NtccSteward.Core.Interfaces.Team;

namespace NtccSteward.Repository.Import
{
    public interface IImportService
    {
        void ImportMdbFile(string filePath);
    }

    public class ImportService : IImportService
    {
        private readonly string connectingString;
        private readonly IChurchRepository churchRepo = null;
        private readonly ITeamRepository teamRepo = null;
        private readonly IMemberRepository memberRepo = null;
        private readonly IMessageRepository messageRepo = null;
        private readonly ICommonRepository commonRepo = null;
        private readonly EnumMapper mapper = new EnumMapper();
        private readonly Factory factory = new Factory();
        private Church church = null;
        private ITeam pastoralTeam = null;
        private List<AssociatePastor> associateList = new List<AssociatePastor>();
        private List<LayPastor> layPastors = new List<LayPastor>();
        private List<Soulwinner> soulwinners = new List<Soulwinner>();

        public ImportService(string connectingString, IChurchRepository churchRepo
            , ITeamRepository teamRepo
            , IMemberRepository memberRepo
            , IMessageRepository messageRepo
            , ICommonRepository commonRepo)
        {
            this.connectingString = connectingString;
            this.churchRepo = churchRepo;
            this.teamRepo = teamRepo;
            this.commonRepo = commonRepo;
        }

        public void ImportMdbFile(string filePath)
        {
            var cnString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};User Id=admin;Password =;";

            using (var cn = new OleDbConnection(cnString))
            {
                try
                {
                    cn.Open();

                    // Note:  Might be able to remove churchId!  Just add the church from the ChurchInfo table

                    this.ImportChurchInfo(cn);

                    this.ImportConfigurations(cn);

                    this.ImportPastors(cn);

                    this.ImportSoulwinners(cn);

                    this.ImportGuests(cn);

                    this.ImportNoVisit(cn);

                    cn.Close();
                }
                catch (Exception ex)
                {
                    var x = ex;
                }
            }

            return;
        }


        private void ImportChurchInfo(OleDbConnection cn)
        {
            var sql = @"SELECT [Church Info].*
                        FROM [Church Info];";

            var churchInfoList = new List<ChurchInfo>();
            using (var cmd = new OleDbCommand(sql, cn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        throw new Exception("Church Info table empty");
                    }

                    while (reader.Read())
                    {
                        var churchInfo = new ChurchInfo();
                        churchInfo.PastorName = reader.ValueOrDefault<string>("Pastor", "");
                        churchInfo.Address = reader.ValueOrDefault<string>("Address", "");
                        churchInfo.City1 = reader.ValueOrDefault<string>("City1", "");
                        churchInfo.State = reader.ValueOrDefault<string>("State", "");
                        churchInfo.Zip1 = reader.ValueOrDefault<string>("Zip1", "");
                        churchInfo.Phone = reader.ValueOrDefault<string>("Phone", "");
                        churchInfoList.Add(churchInfo);
                    }
                }
            }

            var ci = churchInfoList.FirstOrDefault();

            var church = factory.CreateChurch(ci);

            var result = this.churchRepo.Add(church);

            this.church = result.Entity;

            // add member
            var member = new NewMember();
            member.ChurchId = this.church.id;
            member.FirstName = ci.PastorName;
            member.City = ci.City1;
            member.Line1 = ci.Address;
            member.State = ci.State;
            member.Zip = ci.Zip1;

            var memberResult = this.memberRepo.Add(member);

            member.id = memberResult.Entity.id;

            // create team
            var team = new Core.Models.Team.Team();
            team.Id = 0;
            team.Name = church.Name + " Pastoral Team";
            team.Desc = "Pastoral Team for " + church.Name;
            team.ChurchId = church.id;
            team.TeamTypeEnumId = 68;
            team.TeamPositionEnumTypeId = 17;

            var teamResult = this.teamRepo.SaveTeam(team);
            this.pastoralTeam = teamResult.Entity;

            // create teammate
            this.SaveTeammate(member.id, this.pastoralTeam.Id, 70); // Pastor
        }


        private void ImportConfigurations(OleDbConnection cn)
        {
            var sql = @"SELECT TBL_CONFIGURATION.*
                        FROM TBL_CONFIGURATION;";

            var mailConfigs = new List<MailConfiguration>();
            using (var cmd = new OleDbCommand(sql, cn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var config = new MailConfiguration();
                        config.Name = reader.ValueOrDefault<string>("Config_Name", "");
                        config.Server = reader.ValueOrDefault<string>("SMTP_SERVER", "");
                        config.Port = reader.ValueOrDefault<int>("SMTP_SERVER_PORT", 25); // 25 is a default port for email
                        config.Active = reader.ValueOrDefault<bool>("ACTIVE", false);
                        config.UserName = reader.ValueOrDefault<string>("SENDUSERNAME", "");
                        config.Password = reader.ValueOrDefault<string>("SENDPASSWORD", "");
                        mailConfigs.Add(config);
                    }
                }
            }

            var existingConfigProfiles = this.messageRepo.GetEmailConfigurationProfiles();

            foreach (var config in mailConfigs)
            {
                if (!existingConfigProfiles.Any(p => p.Name == config.Name))
                {
                    var configProfile = new EmailConfigurationProfile { Name = config.Name, Server = config.Server, Port = config.Port };
                    var savedConfig = this.messageRepo.SaveEmailConfigurationProfiles(configProfile);
                    config.EmailConfigurationProfileId = savedConfig.Id;
                }
            }

            // get the updated list after adding the church's
            existingConfigProfiles = this.messageRepo.GetEmailConfigurationProfiles();

            var defaultConfig = mailConfigs.FirstOrDefault(ec => ec.Active == true);
            if (defaultConfig != null)
            {
                var configProfile = existingConfigProfiles.FirstOrDefault(c => c.Id == defaultConfig.EmailConfigurationProfileId);
                if (configProfile != null)
                {
                    // save this config for this church
                    var emailConfig = new EmailConfiguration { ChurchId = this.church.id, EmailConfigProfileId = configProfile.Id, UserName = defaultConfig.UserName, Password = defaultConfig.Password };
                    this.messageRepo.SaveEmailConfiguration(emailConfig);
                }
            }
        }


        private void ImportPastors(OleDbConnection cn)
        {
            this.ImportAssociatePastors(cn);

            this.ImportLayPastors(cn);
        }


        private void ImportAssociatePastors(OleDbConnection cn)
        {
            // Associate Pastor

            // IMPORTANT: THINK ABOUT HOW THE ASSOCID IS RELATED TO THE OTHER TABLES, 
            //     WE NEED TO MAP THEIR OLD ID WITH THEIR NEW ONE

            /*
                ASSOCID (pk)
                Associate's Initials
                Associate (person's name)
                Current (currently an associate? - active/inactive) 
            */
            var sql = @"SELECT [Associate Pastor].ASSOCID, [Associate Pastor].[Associate's Initials] as Initials, [Associate Pastor].Associate, [Associate Pastor].Current
                        FROM [Associate Pastor];";

            using (var cmd = new OleDbCommand(sql, cn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var config = new AssociatePastor();
                        config.Name = reader.ValueOrDefault<string>("Associate", "");
                        config.Initials = reader.ValueOrDefault<string>("Initials", "");
                        config.AssocId = reader.ValueOrDefault<int>("ASSOCID", 0);
                        config.Current = reader.ValueOrDefault<bool>("Current", false);
                        this.associateList.Add(config);
                    }
                }
            }

            // add member
            foreach (var assoc in this.associateList)
            {
                var member = new NewMember();
                member.ChurchId = this.church.id;
                member.FirstName = assoc.Name;
                member.City = this.church.City;
                member.Line1 = this.church.Address;
                member.State = this.church.State;
                member.Zip = this.church.Zip;

                var memberResult = this.memberRepo.Add(member);

                assoc.IdentityId = memberResult.Entity.id;

                this.SaveTeammate(assoc.IdentityId, this.pastoralTeam.Id, 71); // Associate Pastor
            }
        }


        private void ImportLayPastors(OleDbConnection cn)
        {
            // IMPORTANT: THINK ABOUT HOW THE LayPastorID IS RELATED TO THE OTHER TABLES, 
            //     WE NEED TO MAP THEIR OLD ID WITH THEIR NEW ONE

            // Lay Pastors
            // ****************************************************
            // The Lay Pastors table are used like Teams, not individual members, for many churches
            // Some churches use this table for individual members.

            /*
                LayPastorID (pk)
                Lay Pastor (Lay Pastor's name)
                LP Initials (Lay Pastor's initials)
                Current (active/inactive)
                Email
                Phone
            */

            var sql = @"SELECT [Lay Pastors].LayPastorID, [Lay Pastors].[Lay Pastor] as LayPastor, [Lay Pastors].[LP Initials] as Initials, [Lay Pastors].Current, [Lay Pastors].Email, [Lay Pastors].Phone
                        FROM [Lay Pastors];";

            using (var cmd = new OleDbCommand(sql, cn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var pastor = new LayPastor();
                        pastor.LayPastorID = reader.ValueOrDefault<int>("LayPastorID", 0);
                        pastor.LayPastorName = reader.ValueOrDefault<string>("LayPastor", "");
                        pastor.Initials = reader.ValueOrDefault<string>("Initials", "");
                        pastor.Current = reader.ValueOrDefault<bool>("Current", false);
                        pastor.Email = reader.ValueOrDefault<string>("Email", "");
                        pastor.Phone = reader.ValueOrDefault<string>("Phone", "");
                        this.layPastors.Add(pastor);
                    }
                }
            }

            // new soulwinning teams out of each of the lay pastor records
            foreach (var pastor in this.layPastors)
            {
                var team = new Core.Models.Team.Team();
                team.Id = 0;
                team.Name = pastor.LayPastorName;  // this is the team name for many churches, but an actualy member name for others
                team.Desc = $"Soulwinning Team for {church.Name}".Trim();
                team.ChurchId = church.id;
                team.TeamTypeEnumId = 69; // Evangelistic
                team.TeamPositionEnumTypeId = 16; // TeamType

                var teamResult = this.teamRepo.SaveTeam(team);
                pastor.IdentityId = teamResult.Entity.Id; // this will be a TeamId
            }

            // add member. - keep, because we may add a feature where they can specify which type of data the table contains:  teams or individual members that are lay pastors
            /*
            foreach (var pastor in this.layPastors)
            {
                var member = new NewMember();
                member.ChurchId = this.church.id;
                member.FirstName = pastor.LayPastorName;
                member.City = this.church.City;
                member.Line1 = this.church.Address;
                member.State = this.church.State;
                member.Zip = this.church.Zip;
                member.Phone = this.factory.ParseNumber(pastor.Phone);

                var memberResult = this.memberRepo.Add(member);

                pastor.IdentityId = memberResult.Entity.id;

                this.SaveTeammate(pastor.IdentityId, 102); // Lay Pastor
            }
            */
        }


        private void SaveTeammate(int memberId, int teamid, int positionEnumId, int teamStatusTypeEnumId = 0)
        {
            var teammate = new Teammate();
            teammate.ChurchId = this.church.id;
            teammate.TeamId = teamid;
            teammate.MemberId = memberId;
            teammate.TeamPositionEnumId = positionEnumId;
            teammate.TeamStatusTypeEnumId = teamStatusTypeEnumId;
            this.teamRepo.SaveTeammate(teammate);
        }


        private void ImportSoulwinners(OleDbConnection cn)
        {
            var sql = @"SELECT [Soul Winners].SWID, [Soul Winners].SWLSNM, [Soul Winners].SWFSNM, [Soul Winners].SWMDNM, [Soul Winners].SWGNNM, [Soul Winners].SWNMCB, [Soul Winners].SWHERE, [Soul Winners].SWGNDR, [Soul Winners].SWMNSTR, [Soul Winners].SWLPST, [Soul Winners].SWACTV, [Soul Winners].SWMRRD, [Soul Winners].SWSPSE, [Soul Winners].SWSPTNR, [Soul Winners].SWLPTR, [Soul Winners].SWDOOR, [Soul Winners].SWCASUAL, [Soul Winners].SWPHN, [Soul Winners].EMAIL
                        FROM [Soul Winners];";

            using (var cmd = new OleDbCommand(sql, cn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var member = new Soulwinner();
                        member.SoulwinnerId = reader.ValueOrDefault<int>("SWID", 0);
                        member.FirstName = reader.ValueOrDefault<string>("SWFSNM", "");
                        member.MiddleName = reader.ValueOrDefault<string>("SWMDNM", "");
                        member.LastName = reader.ValueOrDefault<string>("SWLSNM", "");
                        member.Generation = reader.ValueOrDefault<string>("SWGNNM", "");
                        member.IsLayPastor = reader.ValueOrDefault<bool>("SWLPST", false);
                        member.IsMinister = reader.ValueOrDefault<bool>("SWMNSTR", false);
                        member.Gender = reader.ValueOrDefault<string>("SWGNDR", "");
                        member.IsHere = reader.ValueOrDefault<bool>("SWHERE", false);
                        member.IsActive = reader.ValueOrDefault<bool>("SWACTV", false);
                        member.MarriedStatus = reader.ValueOrDefault<string>("SWMRRD", ""); // (M)arried, (O)ther or (S)ingle
                        member.SpouseName = reader.ValueOrDefault<string>("SWSPSE", "");
                        member.IsSpousePartner = reader.ValueOrDefault<bool>("SWSPTNR", false);
                        member.LayPastorId = reader.ValueOrDefault<int>("SWLPTR", 0);
                        member.IsDoorToDoor = reader.ValueOrDefault<bool>("SWDOOR", false);
                        member.IsCasualStatus = reader.ValueOrDefault<bool>("SWCASUAL", false);
                        member.Email = reader.ValueOrDefault<string>("EMAIL", "");
                        member.PhoneNumber = reader.ValueOrDefault<string>("SWPHN", "");

                        this.soulwinners.Add(member);
                    }
                }
            }

            foreach (var soulwinner in this.soulwinners)
            {
                var comments = string.Empty;

                // create member profile
                var memberProfile = new MemberProfile();
                memberProfile.FirstName = soulwinner.FirstName;
                memberProfile.MiddleName = soulwinner.MiddleName;
                memberProfile.LastName = soulwinner.LastName;
                memberProfile.Gender = soulwinner.Gender;
                memberProfile.MemberStatusEnumType = soulwinner.IsActive ? 49 : 51; // 49 = Active, 51 = Inactive
                memberProfile.MemberTypeEnumId = soulwinner.IsMinister ? 64 : 62; // 64 = minister, 62 = member
                memberProfile.Married = soulwinner.MarriedStatus == "M";  // (M)arried, (O)ther or (S)ingle   They are either married or they are not
                memberProfile.IsHere = soulwinner.IsHere;

                if (!string.IsNullOrWhiteSpace(soulwinner.SpouseName))
                    comments = $"Spouse:  {soulwinner.SpouseName}, " + (soulwinner.IsSpousePartner ? "Is soulwinning partner" : "Is not soulwinning partner");

                memberProfile.Comments = comments;  // last before saving

                var result = this.memberRepo.SaveProfile(memberProfile);

                soulwinner.IdentityId = result.Entity.MemberId;

                // Save the addresses after the profile is saved, because they don't get saved with the profile
                this.commonRepo.MergeEmail(new NtccSteward.Core.Models.Common.Address.Email
                {
                    IdentityId = soulwinner.IdentityId,
                    EmailAddress = soulwinner.Email,
                    ContactInfoLocationType = 8
                });

                this.commonRepo.MergePhone(new Core.Models.Common.Address.Phone
                {
                    IdentityId = soulwinner.IdentityId,
                    PhoneNumber = this.factory.ParseNumber(soulwinner.PhoneNumber),
                    ContactInfoLocationType = 8
                });


                // Save Teammate
                var layPastor = this.layPastors.FirstOrDefault(lp => lp.LayPastorID == soulwinner.LayPastorId);
                if (layPastor != null)
                {
                    var team = new Core.Models.Members.Team { TeamId = layPastor.IdentityId, MemberId = soulwinner.IdentityId };
                    memberProfile.TeamList.Add(team);

                    var positionEnumId = soulwinner.IsLayPastor ? 74 : 75;  // 75 = soulwinner, 74 = TeamLeader

                    var teamStatusTypeEnumId = soulwinner.IsDoorToDoor ? 103 : soulwinner.IsCasualStatus ? 104 : 105; // 103 Door to Door, 104 Casual, 105 Inactive

                    this.SaveTeammate(soulwinner.IdentityId, layPastor.IdentityId, positionEnumId, teamStatusTypeEnumId);
                }
            }
        }


        private void ImportGuests(OleDbConnection cn)
        {
            var reasonMap = new ReasonMap();
            // Guests
            /*
                GUESTID (pk)
                FLUP = Follow up
                ASSOCID = Associate Pastor Id
                LTRMLD = Letter mailed?  (has the 1st time visitor letter been mailed)
                NEW = First time visitor?
                CRNTST = Status (1 = F[aithful], 2 = A[ctive], 3 = I[nactive])
                DTATTND = Date attended
                SPNSR = Sponsor (Sponsor Id)
                MULTI = Multiple guests in one visit?
                FSNM = First Name
                LSNM = Last Name
                PSTFU = Pastor Followup (is a pastor visit needed?)
                ADDR = Address
                City
                St
                Zip
                PHNE = Phone (Home phone)
                PHNE2 = Phone #2 (other phone)
                PRYD? = Prayed?
                NOTE
                DTCHNG = Date Changed
                RSCHID = Reason for Change (ReasonId / Enum)
                OLDST
                NEWST
                CHGD = Changed?  (possibly indicates something has changed)
                PNDBAP = Pending Baptism
                BAPT = Has already been baptized
                LYP = are they a lay pastor?
                EMail
                LetterTranslation = (1 = English, 2 = Spanish)
                PluralTense (bool)
            */

            // SWID links to SPNSR

            var sql = @"SELECT Guests.GUESTID, Guests.FLUP, Guests.ASSOCID, Guests.LTRMLD, Guests.NEW, Guests.CRNTST, Guests.DTATTND, Guests.SPNSR, Guests.MULTI, Guests.FSNM, Guests.LSNM, Guests.PSTFU, Guests.ADDR, Guests.CITY, Guests.ST, Guests.ZIP, Guests.PHNE, Guests.PHNE2, Guests.[PRYD?], Guests.NOTE, Guests.DTCHNG, Guests.RSCHID, Guests.OLDST, Guests.NEWST, Guests.CHGD, Guests.PNDBAP, Guests.BAPT, Guests.LYP, Guests.EMail, Guests.LetterTranslation, Guests.PluralTense
                        FROM Guests;";

            var guests = new List<Guest>();
            using (var cmd = new OleDbCommand(sql, cn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var member = new Guest();
                        member.GuestId = reader.ValueOrDefault<int>("GUESTID", 0);
                        member.FollowUp = reader.ValueOrDefault<bool>("FLUP", false);
                        member.AssocId = reader.ValueOrDefault<int>("ASSOCID", 0);
                        member.LetterMailed = reader.ValueOrDefault<bool>("LTRMLD", false);
                        member.IsNew = reader.ValueOrDefault<bool>("NEW", false);
                        member.CurrentStatus = reader.ValueOrDefault<string>("CRNTST", "");
                        member.DateCameToChurch = reader.ValueOrDefault<DateTime?>("DTATTND", null);
                        member.SponsorId = reader.ValueOrDefault<int>("SPNSR", 0); // Soulwinner Id
                        member.MultipleGuestsFirstVisit = reader.ValueOrDefault<bool>("MULTI", false);
                        member.FirstName = reader.ValueOrDefault<string>("FSNM", "");
                        member.LastName = reader.ValueOrDefault<string>("LSNM", "");
                        member.NeedsPastorFollowUp = reader.ValueOrDefault<bool>("PSTFU", false);
                        member.AddressLine1 = reader.ValueOrDefault<string>("ADDR", "");
                        member.City = reader.ValueOrDefault<string>("CITY", "");
                        member.State = reader.ValueOrDefault<string>("ST", "");
                        member.Zip = reader.ValueOrDefault<string>("ZIP", "");
                        member.Phone = reader.ValueOrDefault<string>("PHNE", "");
                        member.Phone2 = reader.ValueOrDefault<string>("PHNE2", "");
                        member.Prayed = reader.ValueOrDefault<bool>("PRYD?", false);
                        member.Note = reader.ValueOrDefault<string>("NOTE", "");
                        member.DateChanged = reader.ValueOrDefault<DateTime?>("DTCHNG", null);
                        member.ReasonForChange = reader.ValueOrDefault<int>("RSCHID", 0);
                        member.OldStatus = reader.ValueOrDefault<string>("OLDST", "");
                        member.NewStatus = reader.ValueOrDefault<string>("NEWST", "");
                        member.Changed = reader.ValueOrDefault<bool>("CHGD", false);
                        member.PendingBaptism = reader.ValueOrDefault<bool>("PNDBAP", false);
                        member.HasBeenBaptized = reader.ValueOrDefault<bool>("BAPT", false);
                        member.IsLayPastor = reader.ValueOrDefault<bool>("LYP", false);
                        member.Email = reader.ValueOrDefault<string>("EMAIL", "");
                        member.LetterTranslation = reader.ValueOrDefault<int>("LetterTranslation", 1);
                        guests.Add(member);
                    }
                }
            }

            foreach (var guest in guests)
            {
                // create member profile
                var memberProfile = new MemberProfile();
                memberProfile.FirstName = guest.FirstName;
                memberProfile.LastName = guest.LastName;
                memberProfile.MemberStatusEnumType = guest.CurrentStatus == "A" ? 49 : guest.CurrentStatus == "F" ? 50 : 51; // 49 = Active, 50 = Faithful, 51 = Inactive
                memberProfile.MemberTypeEnumId = 62; // 62 = member
                memberProfile.StatusChangeTypeId = reasonMap.Reasons.FirstOrDefault(r => r.ReasonId == guest.ReasonForChange).EnumId;
                memberProfile.StatusChangeTypeDesc = reasonMap.Reasons.FirstOrDefault(r => r.ReasonId == guest.ReasonForChange).Desc;
                memberProfile.Comments = guest.Note;

                var soulwinner = this.soulwinners.FirstOrDefault(s => s.SoulwinnerId == guest.SponsorId);
                if (soulwinner != null)
                    memberProfile.SponsorId = soulwinner.IdentityId;

                // To-Do:  figure out what to do with
                // guest.IsLayPastor // don't do anything with this, because there is no way to associate them with a team
                // guest.NeedsPastorFollowUp
                // guest.AssocId // maybe Membership table
                // guest.PendingBaptism  // this is pointless, because if they haven't been baptized, then they are pending
                // guest.IsNew     // not needed because their date came will tell us they are new

                // guest.DateCameToChurch    // guest book 
                // guest.MultipleGuestsFirstVisit  // guest book
                // guest.Prayed  // guest book

                // guest.HasBeenBaptized // Person table
                // guest.LetterTranslation // Person table - came LanguageEnumId



                var result = this.memberRepo.SaveProfile(memberProfile);

                guest.IdentityId = result.Entity.MemberId;

                if (guest.DateCameToChurch.HasValue)
                    this.CreateActivity(guest.IdentityId, 13, guest.DateCameToChurch, null); // Came to Church 

                if (guest.LetterMailed == true)
                    this.CreateActivity(guest.IdentityId, 15, null, null); // Mailed Letter

                if (!string.IsNullOrWhiteSpace(guest.NewStatus))
                {
                    var newStatus = guest.NewStatus == "A" ? "Active" : guest.NewStatus == "F" ? "Faithful" : "Inactive"; // 49 = Active, 50 = Faithful, 51 = Inactive
                    var oldStatus = string.IsNullOrWhiteSpace(guest.OldStatus) ? "" : guest.OldStatus == "A" ? "Active" : guest.OldStatus == "F" ? "Faithful" : "Inactive"; // 49 = Active, 50 = Faithful, 51 = Inactive
                    var reason = reasonMap.Reasons.FirstOrDefault(r => r.ReasonId == guest.ReasonForChange)?.Desc;
                    var note = $"Status changed from {oldStatus} to {newStatus}, because {reason}";
                    this.CreateActivity(guest.IdentityId, 106, guest.DateChanged, note); // Status Changed
                }

                // the Current Status is king, so if the NewStatus doesn't match the Current Status, then add an activity
                if (!string.IsNullOrWhiteSpace(guest.NewStatus) && !string.IsNullOrWhiteSpace(guest.CurrentStatus))
                {
                    if (guest.NewStatus.ToLower() != guest.CurrentStatus.ToLower())
                    {
                        var newStatus = guest.CurrentStatus == "A" ? "Active" : guest.CurrentStatus == "F" ? "Faithful" : "Inactive"; // 49 = Active, 50 = Faithful, 51 = Inactive
                        var oldStatus = string.IsNullOrWhiteSpace(guest.NewStatus) ? "" : guest.NewStatus == "A" ? "Active" : guest.NewStatus == "F" ? "Faithful" : "Inactive"; // 49 = Active, 50 = Faithful, 51 = Inactive
                        var note = $"Status changed from {oldStatus} to {newStatus}";
                        this.CreateActivity(guest.IdentityId, 106, guest.DateChanged, note); // Status Changed
                    }
                }


                // Save the addresses after the profile is saved, because they don't get saved with the profile
                this.commonRepo.MergeAddress(new Core.Models.Common.Address.Address
                {
                    IdentityId = guest.IdentityId,
                    Line1 = guest.AddressLine1,
                    City = guest.City,
                    State = guest.State,
                    Zip = this.factory.ParseNumber(guest.Zip),
                    ContactInfoLocationType = 9
                });

                this.commonRepo.MergeEmail(new NtccSteward.Core.Models.Common.Address.Email
                {
                    IdentityId = guest.IdentityId,
                    EmailAddress = guest.Email,
                    ContactInfoLocationType = 8
                });

                this.commonRepo.MergePhone(new Core.Models.Common.Address.Phone
                {
                    IdentityId = guest.IdentityId,
                    PhoneNumber = this.factory.ParseNumber(guest.Phone),
                    ContactInfoLocationType = 8
                });

                this.commonRepo.MergePhone(new Core.Models.Common.Address.Phone
                {
                    IdentityId = guest.IdentityId,
                    PhoneNumber = this.factory.ParseNumber(guest.Phone2),
                    ContactInfoLocationType = 8
                });
            }
        }

        private void CreateActivity(int memberId, int activityTypeEnumId, DateTime? activityDate, string note)
        {
            var activity = new Activity();
            activity.ChurchId = this.church.id;
            activity.TargetId = memberId;
            activity.ActivityTypeEnumID = activityTypeEnumId;

            if (activityDate.HasValue)
                activity.ActivityDate = activityDate.Value;

            if (!string.IsNullOrWhiteSpace(note))
                activity.Note = note;

            this.memberRepo.SaveActivity(activity);
        }


        private void ImportNoVisit(OleDbConnection cn)
        {
            // Don't Visit
            /*
                ID pk
                Name
                Address
                City
                Development
                Date = Date added to list
                Phone
                Notes
            */
        }
    }
}