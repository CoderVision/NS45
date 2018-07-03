using System;
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
        void ImportMdbFile(string filePath, int userId);
    }

    public class ImportService : IImportService
    {
        private readonly string connectingString;
        private readonly IChurchRepository churchRepo = null;
        private readonly ITeamRepository teamRepo = null;
        private readonly IMemberRepository memberRepo = null;
        private readonly IMessageRepository messageRepo = null;
        private readonly ICommonRepository commonRepo = null;
        private readonly ILogger logger = null;
        private readonly EnumMapper mapper = new EnumMapper();
        private readonly Factory factory = new Factory();
        private Church church = null;
        private ITeam pastoralTeam = null;
        private List<AssociatePastor> associateList = new List<AssociatePastor>();
        private List<LayPastor> layPastors = new List<LayPastor>();
        private List<Soulwinner> soulwinners = new List<Soulwinner>();
        private int userId = 0;

        public ImportService(string connectingString, IChurchRepository churchRepo
            , ITeamRepository teamRepo
            , IMemberRepository memberRepo
            , IMessageRepository messageRepo
            , ICommonRepository commonRepo
            , ILogger logger)
        {
            this.connectingString = connectingString;
            this.churchRepo = churchRepo;
            this.teamRepo = teamRepo;
            this.commonRepo = commonRepo;
            this.memberRepo = memberRepo;
            this.messageRepo = messageRepo;
            this.logger = logger;
        }

        private void Initialize()
        {
            this.church = null;
            this.pastoralTeam = null;
            this.associateList = new List<AssociatePastor>();
            this.layPastors = new List<LayPastor>();
            this.soulwinners = new List<Soulwinner>();
            this.userId = 0;
        }

        public void ImportMdbFile(string filePath, int userId)
        {
            Initialize();

            this.userId = userId;

            var cnString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};User Id=admin;Password =;";

            using (var cn = new OleDbConnection(cnString))
            {
                try
                {
                    cn.Open();

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
                    this.logger.LogInfo(Core.Framework.LogLevel.Critical, ex.Message, ex.StackTrace, this.userId);

                    throw;
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

            if (result.Status == RepositoryActionStatus.Error)
                LogError(result.Exception);

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

            if (memberResult.Status == RepositoryActionStatus.Error)
                LogError(memberResult.Exception);

            member.id = memberResult.Entity.id;

            // create team
            var team = new Core.Models.Team.Team();
            team.Id = 0;
            team.Name = church.Name + " Pastoral Team";
            team.Desc = "Pastoral Team for " + church.Name;
            team.ChurchId = church.id;
            team.TeamTypeEnumId = 68;
            team.TeamPositionEnumTypeId = 17;
            team.Comment = "";

            var teamResult = this.teamRepo.SaveTeam(team);

            if (teamResult.Status == RepositoryActionStatus.Error)
                LogError(teamResult.Exception);

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
                        config.Port = Convert.ToInt32(reader.ValueOrDefault<string>("SMTP_SERVER_PORT", "25")); // 25 is a default port for email
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
            var sql = @"SELECT *
                        FROM [Associate Pastor];";

            using (var cmd = new OleDbCommand(sql, cn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var config = new AssociatePastor();
                        config.Name = reader.ValueOrDefault<string>("Associate", "");
                        config.Initials = reader.ValueOrDefault<string>("Associate's Initials", "");
                        config.AssocId = reader.ValueOrDefault<int>("ASSOCID", 0);
                        config.Current = reader.ValueOrDefault<bool>("Current", false);

                        this.associateList.Add(config);
                    }
                }
            }

            // add member
            foreach (var assoc in this.associateList)
            {
                var member = new MemberProfile();
                member.ChurchId = this.church.id;
                member.FirstName = assoc.Name;
                member.StatusId = assoc.Current ? 49 : 51; // 49 = Active, 51 = Inactive

                var memberResult = this.memberRepo.SaveProfile(member);

                if (memberResult.Status == RepositoryActionStatus.Error)
                    LogError(memberResult.Exception);

                assoc.IdentityId = memberResult.Entity.MemberId;

                this.SaveAddress(assoc.IdentityId, this.church.Line1, this.church.City, this.church.State, this.church.Zip);

                this.SaveTeammate(assoc.IdentityId, this.pastoralTeam.Id, 71); // Associate Pastor
            }
        }

        private void LogError(Exception ex)
        {
            var details = $"churchId:  {this.church.id}.  Stacktrace:  { ex.StackTrace }";
            this.logger.LogInfo(Core.Framework.LogLevel.Error, ex.Message, details, this.userId);
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

            var sql = @"SELECT *
                        FROM [Lay Pastors];";

            using (var cmd = new OleDbCommand(sql, cn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var pastor = new LayPastor();
                        pastor.LayPastorID = reader.ValueOrDefault<int>("LayPastorID", 0);
                        pastor.LayPastorName = reader.ValueOrDefault<string>("Lay Pastor", "");
                        pastor.Initials = reader.ValueOrDefault<string>("LP Initials", "");
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
                team.Comment = "";

                var teamResult = this.teamRepo.SaveTeam(team);

                if (teamResult.Status == RepositoryActionStatus.Error)
                    LogError(teamResult.Exception);

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
            var result = this.teamRepo.SaveTeammate(teammate);

            if (result.Status == RepositoryActionStatus.Error)
                LogError(result.Exception);
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
                memberProfile.StatusId = soulwinner.IsActive ? 49 : 51; // 49 = Active, 51 = Inactive
                //memberProfile.MemberStatusEnumType = soulwinner.IsActive ? 49 : 51; // 49 = Active, 51 = Inactive
                memberProfile.MemberTypeEnumId = soulwinner.IsMinister ? 64 : 62; // 64 = minister, 62 = member
                memberProfile.Married = soulwinner.MarriedStatus == "M";  // (M)arried, (O)ther or (S)ingle   They are either married or they are not
                memberProfile.IsHere = soulwinner.IsHere;
                memberProfile.ChurchId = this.church.id;
                memberProfile.LanguageTypeEnumId = 107; // default to English

                if (!string.IsNullOrWhiteSpace(soulwinner.SpouseName))
                    comments = $"Spouse:  {soulwinner.SpouseName}, " + (soulwinner.IsSpousePartner ? "Is soulwinning partner" : "Is not soulwinning partner");

                memberProfile.Comments = comments;  // last before saving

                var result = this.memberRepo.SaveProfile(memberProfile);

                if (result.Status == RepositoryActionStatus.Error)
                    LogError(result.Exception);

                soulwinner.IdentityId = result.Entity.MemberId;

                // Save the addresses after the profile is saved, because they don't get saved with the profile
                this.SaveEmail(soulwinner.IdentityId, soulwinner.Email);

                this.SavePhone(soulwinner.IdentityId, soulwinner.PhoneNumber);

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

        private void SavePhone(int identityId, string phone)
        {
            var ph = this.factory.ParseNumber(phone);

            if (string.IsNullOrWhiteSpace(ph))
                return;

            var result = this.commonRepo.MergePhone(new Core.Models.Common.Address.Phone
            {
                IdentityId = identityId,
                PhoneNumber = ph,
                ContactInfoLocationType = 8
            });
            if (result.Status == RepositoryActionStatus.Error)
                LogError(result.Exception);
        }

        private void SaveEmail(int identityId, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            var result = this.commonRepo.MergeEmail(new NtccSteward.Core.Models.Common.Address.Email
            {
                IdentityId = identityId,
                EmailAddress = email,
                ContactInfoLocationType = 8
            });

            if (result.Status == RepositoryActionStatus.Error)
                LogError(result.Exception);
        }

        private void SaveAddress(int identityId, string addressLine1, string city, string state, string zip)
        {
            if (string.IsNullOrWhiteSpace(addressLine1)
                && string.IsNullOrWhiteSpace(city)
                && string.IsNullOrWhiteSpace(state)
                && string.IsNullOrWhiteSpace(zip))
            {
                return;
            }

            var result = this.commonRepo.MergeAddress(new Core.Models.Common.Address.Address
            {
                IdentityId = identityId,
                Line1 = addressLine1,
                City = city,
                State = state,
                Zip = this.factory.ParseNumber(zip),
                ContactInfoLocationType = 9
            });
            if (result.Status == RepositoryActionStatus.Error)
                LogError(result.Exception);
        }

        private void ImportGuests(OleDbConnection cn)
        {
            var reasonMap = new ReasonMap();

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
                memberProfile.StatusId = guest.CurrentStatus == "A" ? 49 : guest.CurrentStatus == "F" ? 50 : 51; // 49 = Active, 50 = Faithful, 51 = Inactive
                //memberProfile.MemberStatusEnumType = guest.CurrentStatus == "A" ? 49 : guest.CurrentStatus == "F" ? 50 : 51; // 49 = Active, 50 = Faithful, 51 = Inactive
                memberProfile.MemberTypeEnumId = 62; // 62 = member
                memberProfile.StatusChangeTypeId = reasonMap.Reasons.FirstOrDefault(r => r.ReasonId == guest.ReasonForChange)?.EnumId ?? 0;
                memberProfile.StatusChangeTypeDesc = reasonMap.Reasons.FirstOrDefault(r => r.ReasonId == guest.ReasonForChange)?.Desc;
                memberProfile.Comments = guest.Note;
                memberProfile.LanguageTypeEnumId = guest.LetterTranslation == 2 ? 108 : 107; // 108 = Spanish, 107 = English 
                memberProfile.HasBeenBaptized = guest.HasBeenBaptized;
                memberProfile.NeedsPastoralVisit = guest.NeedsPastorFollowUp;
                memberProfile.ChurchId = this.church.id;
                
                var associatePastor = this.associateList.FirstOrDefault(ap => ap.AssocId == guest.AssocId);
                memberProfile.AssociatePastorId = associatePastor?.IdentityId ?? 0;

                var soulwinner = this.soulwinners.FirstOrDefault(s => s.SoulwinnerId == guest.SponsorId);
                if (soulwinner != null)
                    memberProfile.SponsorId = soulwinner.IdentityId;

                // Don't do anything with these
                // guest.IsLayPastor // don't do anything with this, because there is no way to associate them with a team
                // guest.PendingBaptism  // this is pointless, because if they haven't been baptized, then they are pending
                // guest.IsNew     // not needed because their date came will tell us they are new

                var result = this.memberRepo.SaveProfile(memberProfile);

                if (result.Status == RepositoryActionStatus.Error)
                    LogError(result.Exception);

                guest.IdentityId = result.Entity.MemberId;

                var sponsorId = soulwinner?.IdentityId ?? 0;
                this.memberRepo.AddToGuestbook(this.church.id, guest.IdentityId, sponsorId, guest.DateCameToChurch, guest.MultipleGuestsFirstVisit, guest.Prayed);

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
                this.SaveAddress(guest.IdentityId, guest.AddressLine1, guest.City, guest.State, guest.Zip);

                this.SaveEmail(guest.IdentityId, guest.Email);

                this.SavePhone(guest.IdentityId, guest.Phone);

                this.SavePhone(guest.IdentityId, guest.Phone2);
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

            var result = this.memberRepo.SaveActivity(activity);

            if (result.Status == RepositoryActionStatus.Error)
                LogError(result.Exception);
        }


        private void ImportNoVisit(OleDbConnection cn)
        {
            var sql = @"SELECT [Don't Visit].ID, [Don't Visit].Name, [Don't Visit].Address, [Don't Visit].City, [Don't Visit].Development, [Don't Visit].Date, [Don't Visit].Phone, [Don't Visit].Notes
                        FROM [Don't Visit];";

            var dnvList = new List<DoNotVisit>();
            using (var cmd = new OleDbCommand(sql, cn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var doNotVisit = new DoNotVisit();
                        doNotVisit.Name = reader.ValueOrDefault<string>("Name", "");
                        doNotVisit.Address = reader.ValueOrDefault<string>("Address", "");
                        doNotVisit.City = reader.ValueOrDefault<string>("City", "");
                        doNotVisit.Development = reader.ValueOrDefault<string>("Development", "");
                        doNotVisit.Date = reader.ValueOrDefault<string>("Date", "");
                        doNotVisit.Phone = reader.ValueOrDefault<string>("Phone", "");
                        doNotVisit.Notes = reader.ValueOrDefault<string>("Notes", "");
                        dnvList.Add(doNotVisit);
                    }
                }
            }


            foreach (var dnv in dnvList)
            {
                var member = new MemberProfile();
                member.StatusId = 109; // Do Not Visit
                member.ChurchId = this.church.id;
                member.FirstName = dnv.Name;
                member.LanguageTypeEnumId = 107; // default to English

                if (!string.IsNullOrWhiteSpace(dnv.Date))
                    member.Comments = $"Date Entered: " + dnv.Date + ".  ";

                if (!string.IsNullOrWhiteSpace(dnv.Development))
                    member.Comments += $"Development:  {dnv.Development}.  ";

                if (!string.IsNullOrWhiteSpace(dnv.Notes))
                    member.Comments += dnv.Notes;

                member.Comments = member.Comments?.Trim();

                var result = this.memberRepo.SaveProfile(member);

                if (result.Status == RepositoryActionStatus.Error)
                    LogError(result.Exception);

                dnv.IdentityId = result.Entity.MemberId;

                this.SaveAddress(dnv.IdentityId, dnv.Address, dnv.City, null, null);

                this.SavePhone(dnv.IdentityId, dnv.Phone);
            }
        }
    }
}