using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelBox2_4
{
    /// <summary>
    /// Die Nachrichten werden hiermit in Kategorien eingeordnet.
    /// Kategorien können bitweise vergeben werden z.B. MessageType = 36 (= 4 + 32) heisst: Nachricht empfangen als SMS, weitergeleitet als Email.
    /// Die hier vergebenen Texte tauchen in der Visualisierung auf.
    /// </summary>
    [Flags]
    public enum MessageType : short
    {
        NoCategory = 0,             //Nicht zugeordnet
        System = 1,                 //Vom System erzeugt
        RecievedFromSms = 2,        //Empfang von SMS
        SentToSms = 4,              //Senden an SMS
        RecievedFromEmail = 8,      //Empgang von Email
        SentToEmail = 16            //Senden an Email     
    }


    /// <summary>
    /// Abbildung von Einträgen aus der Datenbanktabelle "Contacts"
    /// </summary>
    public class Contact
    {
        public uint Id { get; set; }

        public string Name { get; set; }

        public string KeyWord { get; set; }

        public uint CompanyId { get; set; }

        public System.Net.Mail.MailAddress Email { get; set; }

        public ulong Phone { get; set; }

        public MessageType ContactType { get; set; }

    }

    public class Message
    {
        public uint Id { get; set; }

        //Inhalt
        public string Subject { get; set; }

        public string Content{ get; set; }

        public uint ContentId { get; set; }

        //Von
        public Contact From { get; set; }

        public DateTime RecieveTime { get; set; }

        //An
        public List<Contact> To { get; set; }

        public DateTime SentTime { get; set; }

        public int SendingApproches { get; set; }

        //Weg der Nachricht
        public MessageType Status { get; set; }
    }

    public static class Messages
    {
        #region Fields

        private static List<Message> inBox = new List<Message>();
        private static List<Message> outBox = new List<Message>();

        #endregion

        #region Properties
        /// <summary>
        /// Warteschlange der eingegangenen, zu verarbeiteten Nachrichten
        /// </summary>
        public static List<Message> InBox { get => inBox; set => inBox = value; }

        /// <summary>
        /// Warteschlange der zu sendenden Nachrichten
        /// </summary>
        public static List<Message> OutBox { get => outBox; set => outBox = value; }

        #endregion

        #region Methoden

        internal static void Create_SignalQuality(int signalQuality)
        {
            //Mögliche Werte: 2 - 9 marginal, 10 - 14 OK, 15 - 19 Good, 20 - 30 Excellent, 99 = kein Signal
            string signalStrength = "unbekannt";

            if (signalQuality < 3 || signalQuality >= 30)
            {
                signalStrength = "kein Signal";
            }
            else if (signalQuality < 9)
            {
                signalStrength = "marginal";
            }
            else if (signalQuality < 15)
            {
                signalStrength = "ok";
            }
            else if (signalQuality < 20)
            {
                signalStrength = "gut";
            }
            else if (signalQuality < 30)
            {
                signalStrength = "exzellent";
            }

            StringBuilder body = new StringBuilder();
            body.Append("MelBox2: Die Signalqualität am GSM-Modem für die Störungsweitermeldung wird eingestuft als -" + signalStrength + "-.");

            Message notification = new Message
            {
                Content = body.ToString(),
                From = Contacts.SmsCenter,
                Status = MessageType.System,
                Subject = "MelBox2 - Signalqualität " + signalStrength,
                To = new List<Contact>() { Contacts.MelBox2Admin }
            };

            Messages.InBox.Add(notification);
        }

        internal static void Create_NewUnknownContact(Message recievedMessage, uint newContactId, string keyWord)
        {
            if (recievedMessage is null)
            {
                throw new ArgumentNullException(nameof(recievedMessage));
            }

            StringBuilder body = new StringBuilder();
            body.Append("Es wurde ein neuer Absender in die Datenbank von MelBox2 eingetragen.\r\n\r\n");
            body.Append("Neue Nachricht empfangen am " + recievedMessage.SentTime.ToShortDateString() + " um " + recievedMessage.SentTime.ToLongTimeString() + "\r\n\r\n");

            body.Append("Benutzerschlüsselwort ist\t\t>" + keyWord + "<\r\n");
            body.Append("Empfangene Emailadresse war\t\t>" + recievedMessage.From.Email + "<\r\n");
            body.Append("Empfangene Telefonnummer war\t>+" + recievedMessage.From.Phone + "<\r\n\r\n");

            body.Append("Empfangenen Nachricht war [" + recievedMessage.ContentId + "]\t\t>" + recievedMessage.Content + "<\r\n\r\n");

            body.Append("Bitte die Absenderdaten in MelBox2 im Reiter >Stammdaten< für die ID [" + newContactId + "] vervollständigen .\r\n");
            body.Append("Dies ist eine automatische Nachricht von MelBox2.");

            Message notification = new Message
            {
                Content = body.ToString(),
                From = Contacts.SmsCenter,
                Status = MessageType.System,
                Subject = "MelBox2 - neuer Absender",
                To = new List<Contact>() { Contacts.MelBox2Admin }
            };

            Messages.InBox.Add(notification);
        }

        internal static void Create_Startup()
        {
            
            StringBuilder body = new StringBuilder();
            body.Append("MelBox2: Die Anwendung wurde neu gestartet am " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + " Uhr");

            Message notification = new Message
            {
                Content = body.ToString(),
                From = Contacts.SmsCenter,
                Status = MessageType.System,
                Subject = "MelBox2 - Neustart " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + " Uhr",
                To = new List<Contact>() { Contacts.MelBox2Admin }
            };

            Messages.InBox.Add(notification);
        }


        #endregion
    }

    class Shift
    {
    }
}
