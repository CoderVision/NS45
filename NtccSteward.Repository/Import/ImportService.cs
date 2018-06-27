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
        private readonly EnumMapper mapper = new EnumMapper();
        private readonly Factory factory = new Factory();
        private int churchId = 0;

        public ImportService(string connectingString, IChurchRepository churchRepo
            , ITeamRepository teamRepo
            , IMemberRepository memberRepo)
        {
            this.connectingString = connectingString;
            this.churchRepo = churchRepo;
            this.teamRepo = teamRepo;
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

                    // this.ImportEnums(cn);  // this does not need to be done for each church, just make sure they match the Enums

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
                    if (!reader.HasRows) {
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

            this.churchId = result.Entity.id;

            // add member
            var member = new NewMember();
            member.ChurchId = this.churchId;
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

            // create teammate
            var teammate = new Teammate(); 
            teammate.ChurchId = this.churchId;
            teammate.TeamId = teamResult.Entity.Id;
            teammate.MemberId = member.id;
            teammate.TeamPositionEnumId = 70; // Pastor
            this.teamRepo.SaveTeammate(teammate);
    }

        private void ImportConfigurations(OleDbConnection cn)
        {
            // TBL_Configuration
            /*
                ID
                Config_Name (name of config)
                ROOT_PATH (path for saving reports)
                ACTIVE (is this configuration active?)
                SEND_USING
                SMTP_SERVER
                SMTP_SERVER_PORT
                SMTP_AUTHENTICATE
                SMTP_USESS1
                SMTP_CONNECTIONTIMEOUT
                SENDUSERNAME
                SENDPASSWORD
                SENDMESSAGES
            */
        }


        private void ImportPastors(OleDbConnection cn)
        {
            // Associate Pastor
            /*
                ASSOCID (pk)
                Associate's Initials
                Associate (person's name)
                Current (currently an associate? - active/inactive) 
            */

            // Lay Pastors
            /*
                LayPastorID (pk)
                Lay Pastor (Lay Pastor's name)
                LP Initials (Lay Pastor's initials)
                Current (active/inactive)
                Email
                Phone
            */
        }


        private void ImportSoulwinners(OleDbConnection cn)
        {
            /*
                Soulwinners
                SWID = Soulwinner Id
                SWLSNM = Soulwinner Last Name
                SWFSNM = Soulwinner First Name
                SWMDNM = Soulwinner Middle Name
                SWGNNM = Soulwinner Generation
                SWNMCB = Soulwinner's name combined (Last Name, First Name)
                SPNSR = Sponsor
                SWLPST = Soulwinner Lay Pastor
                SWMNSTR = Soulwinner Minister
                SWGNDR = Soulwinner Gender
                SWHERE = Soulwinner Here? (Are they here now?)
                SWACTV = Soulwinner Active (are they active?)
                SWMRRD = Soulwinner Married? (are they married?)
                SWSPSE = Soulwinner Spouse (spouse's name)
                SWSPTNR = Soulwinner Spouse Partner?
                SWLPTR = Soulwinner Laypastor Id they are working with
                SWDOOR = Soulwinner door to door (Are they doing door to door soulwinner?)
                SWCASUAL = Soulwinner Casual (Are they on casual status?)
                SWPHN = Soulwinner's phone number
                EMAIL = Soulwinner's email address
            */
        }


        private void ImportGuests(OleDbConnection cn)
        {
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