using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Services
{
    public interface IImportService
    {
        void ImportMdbFile(int churchId, string filePath);
    }

    public class ImportService : IImportService
    {
        private readonly string connectingString;

        public ImportService(string connectingString)
        {
            this.connectingString = connectingString;
        }

        public void ImportMdbFile(int churchId, string filePath)
        {
            var cnString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};User Id=admin;Password =;";

            using (var cn = new System.Data.OleDb.OleDbConnection(cnString))
            {
                try
                {
                    cn.Open();

                    this.ImportEnums(cn);

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

        private void ImportEnums(System.Data.OleDb.OleDbConnection cn)
        {
            // Reasons for Status Change (need to create temp table to map these reasons to ones in EnumLookup or make sure they are the same)
            // ReasonID
            // Reason for Status Change
        }

        private void ImportChurchInfo(System.Data.OleDb.OleDbConnection cn)
        {
            //Church Info
            /*
                (no pk)
                Pastor (pastor's name)
                Address
                City1
                State
                Zip1
                Phone
            */
        }

        private void ImportConfigurations(System.Data.OleDb.OleDbConnection cn)
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


        private void ImportPastors(System.Data.OleDb.OleDbConnection cn)
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


        private void ImportSoulwinners(System.Data.OleDb.OleDbConnection cn)
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


        private void ImportGuests(System.Data.OleDb.OleDbConnection cn)
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

        private void ImportNoVisit(System.Data.OleDb.OleDbConnection cn)
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