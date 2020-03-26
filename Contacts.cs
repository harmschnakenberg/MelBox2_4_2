using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelBox2_4
{
    public static class Contacts
    {
        #region Felder

        private static Contact _SmsCenter = null;
        private static Contact _MelBox2Admin = null;
        private static Contact _Bereitschaftshandy = null;
        private static Contact _KreuService = null;


        #endregion

        #region Properties
        //public static ulong GsmModemPhoneNumber { get; set; } = 4915142265412;

        public static string UnknownName { get; } = "_UNBEKANNT_";

        public static IEnumerable<Contact> PermanentSubscribers { get; set; }
                

        public static Contact SmsCenter
        {
            get
            {
                if (_SmsCenter == null)
                    _SmsCenter = MainWindow.Sql.GetContactFromDb(0); //siehe Sql.CreateNewDataBase()
                return _SmsCenter;
            }
        }

        public static Contact MelBox2Admin
        {
            get
            {
                if (_MelBox2Admin == null)
                    _MelBox2Admin = MainWindow.Sql.GetContactFromDb(1); //siehe Sql.CreateNewDataBase()
                return _MelBox2Admin;
            }
        }

        public static Contact Bereitschaftshandy
        {
            get
            {
                if (_Bereitschaftshandy == null)
                {
                    Sql sql = new Sql();
                    _Bereitschaftshandy = sql.GetContactFromDb(2); //siehe Sql.CreateNewDataBase()
                }
                return _Bereitschaftshandy;
            }
        }

        public static Contact KreuService
        {
            get
            {
                if (_KreuService == null)
                    _KreuService = MainWindow.Sql.GetContactFromDb(3); //siehe Sql.CreateNewDataBase()
                return _KreuService;
            }
        }

        #endregion

        #region Methods

        public static MessageType SetMessageType(bool recEmail = false, bool recSms = false, bool sentEmail = false, bool sentSms = false)
        {
            MessageType type = 0;

            if (recSms) type &= MessageType.RecievedFromSms;        //Empfang von SMS
            if (sentSms) type &= MessageType.SentToSms;             //Senden an SMS
            if (recEmail) type &= MessageType.RecievedFromEmail;    //Empgang von Email
            if (sentEmail) type &= MessageType.SentToEmail;         //Senden an Email 

            return type;
        }

        #endregion
    }
}
