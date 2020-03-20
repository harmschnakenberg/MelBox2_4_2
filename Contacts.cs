using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelBox2_4
{
    public static class Contacts
    {
        #region Properties
        public static ulong GsmModemPhoneNumber { get; set; } = 4915142265412;

        public static string UnknownName { get; set; } = "_UNBEKANNT_";


        public static IEnumerable<Contact> PermanentSubscribers { get; set; }

        #endregion

        public static Contact SmsCenter = new Contact
        {
            Id = 0,
            Name = "SMSZentrale",
            CompanyId = 1,
            Email = new System.Net.Mail.MailAddress("smszentrale@kreutztraeger.de", SmsCenter.Name),
            Phone = GsmModemPhoneNumber,
            KeyWord = SmsCenter.Name,
            ContactType = MessageType.SentToEmail
        };

        public static Contact MelBox2Admin { get; } = new Contact()
        {
            Id = 0,
            Name = "MelBox2Admin",
            CompanyId = 1,
            Email = new System.Net.Mail.MailAddress("harm.schnakenberg@kreutztraeger.de", SmsCenter.Name),
            Phone = 4915142265412,
            ContactType = MessageType.SentToEmail
        };

        public static Contact Bereitschaftshandy { get; } = new Contact()
        {
            Id = 0,
            Name = "Bereitschaftshandy",
            CompanyId = 1,
            Email = new System.Net.Mail.MailAddress("bereitschaftshandy@kreutztraeger.de", SmsCenter.Name),
            Phone = 491728362586,
            ContactType = MessageType.SentToSms
        };

        public static Contact KreuService { get; } = new Contact()
        {
            Id = 0,
            Name = "Kreutzträger Service",
            CompanyId = 1,
            Email = new System.Net.Mail.MailAddress("service@kreutztraeger.de", SmsCenter.Name),
            Phone = 0,
            ContactType = MessageType.SentToEmail
        };


    }
}
