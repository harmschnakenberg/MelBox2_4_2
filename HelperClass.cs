using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelBox2_4
{
    internal static class HelperClass
    {

        #region Properties

        public static ushort NightShiftStartHour { get; set; } = 17;
        public static ushort NightShiftStartHourFriday { get; set; } = 15;
        public static ushort NightShiftEndHour { get; set; } = 7;

        #endregion

        /// <summary>
        /// Kovertiert einen String mit Zahlen und Zeichen in eine Telefonnumer als Zahl mit führender  
        /// Ländervorwahl z.B. +49 (0) 4201 123 456 oder 0421 123 456 wird zu 49421123456 
        /// </summary>
        /// <param name="str_phone">String, der eine Telefonummer enthält.</param>
        /// <returns>Telefonnumer als Zahl mit führender  
        /// Ländervorwahl (keine führende 00). Bei ungültigem str_phone Rückgabewert 0.</returns>
        internal static ulong ConvertStringToPhonenumber(string str_phone)
        {
            // Entferne (0) aus +49 (0) 421...
            str_phone = str_phone.Replace("(0)", string.Empty);

            // Entferne alles ausser Zahlen
            System.Text.RegularExpressions.Regex regexObj = new System.Text.RegularExpressions.Regex(@"[^\d]");
            str_phone = regexObj.Replace(str_phone, "");

            // Wenn zu wenige Zeichen übrigbleiben gebe 0 zurück.
            if (str_phone.Length < 2) return 0;

            // Wenn am Anfang 0 steht, aber nicht 00 ersetze führende 0 durch 49
            string firstTwoDigits = str_phone.Substring(0, 2);

            if (firstTwoDigits != "00" && firstTwoDigits[0] == '0')
            {
                str_phone = "49" + str_phone.Substring(1, str_phone.Length - 1);
            }

            ulong number = ulong.Parse(str_phone);

            if (number > 0)
            {
                return number;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Erzeugt einen Unix-Zeitstempel (UTC) aus einem DateTime-Objekt (LocalTime)
        /// </summary>
        /// <param name="datetime">Die Locale Zeit</param>
        /// <returns></returns>
        public static ulong ConvertToUnixTime(DateTime datetime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (ulong)(datetime.ToUniversalTime() - sTime).TotalSeconds;
        }

        // Aus VB konvertiert
        private static DateTime DateOsterSonntag(DateTime pDate)
        {
            int viJahr, viMonat, viTag;
            int viC, viG, viH, viI, viJ, viL;

            viJahr = pDate.Year;
            viG = viJahr % 19;
            viC = viJahr / 100;
            viH = (viC - viC / 4 - (8 * viC + 13) / 25 + 19 * viG + 15) % 30;
            viI = viH - viH / 28 * (1 - 29 / (viH + 1) * (21 - viG) / 11);
            viJ = (viJahr + viJahr / 4 + viI + 2 - viC + viC / 4) % 7;
            viL = viI - viJ;
            viMonat = 3 + (viL + 40) / 44;
            viTag = viL + 28 - 31 * (viMonat / 4);

            return new DateTime(viJahr, viMonat, viTag);
        }

        // Aus VB konvertiert
        public static List<DateTime> Feiertage(DateTime pDate)
        {
            int viJahr = pDate.Year;
            DateTime vdOstern = DateOsterSonntag(pDate);

            List<DateTime> feiertage = new List<DateTime>
            {
                new DateTime(viJahr, 1, 1),    // Neujahr
                new DateTime(viJahr, 5, 1),    // Erster Mai
                vdOstern.AddDays(-2),          // Karfreitag
                vdOstern.AddDays(1),           // Ostermontag
                vdOstern.AddDays(39),          // Himmelfahrt
                vdOstern.AddDays(50),          // Pfingstmontag
                new DateTime(viJahr, 10, 3),   // TagderDeutschenEinheit
                new DateTime(viJahr, 10, 31),  // Reformationstag
                new DateTime(viJahr, 12, 24),  // Heiligabend
                new DateTime(viJahr, 12, 25),  // Weihnachten 1
                new DateTime(viJahr, 12, 26),  // Weihnachten 2
                new DateTime(viJahr, 12, DateTime.DaysInMonth(viJahr, 12)) // Silvester
            };
            return feiertage;
        }


        public static MessageType SetMessageType(bool recEmail = false, bool recSms = false, bool sentEmail = false, bool sentSms = false)
        {
            MessageType type = 0;

            if (recSms) type &= MessageType.RecievedFromSms;        //Empfang von SMS
            if (sentSms) type &= MessageType.SentToSms;             //Senden an SMS
            if (recEmail) type &= MessageType.RecievedFromEmail;    //Empgang von Email
            if (sentEmail) type &= MessageType.SentToEmail;         //Senden an Email 

            return type;
        }

        /// <summary>
        /// Ermittelt KeyWord aus dem Inhalt einer Nachricht
        /// </summary>
        /// <param name="messageContent">Inhalt einer Nachricht</param>
        /// <returns>KeyWord der Nachricht</returns>
        public static string GetKeyWord(string messageContent)
        {
            string[] words = messageContent.Split(' ', ',', '.', '-', '_');

            string keyWord = words[0];

            if (words.Length > 1) keyWord += words[1];

            return keyWord.ToLower();
        }
    }
}
