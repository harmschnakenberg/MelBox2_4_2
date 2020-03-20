using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelBox2_4
{
    public static class Log
    {
       
        public enum LogType
        {
            General,
            Contacts,
            Calendar,
            Email,
            SMS,
            Internal
        }

        #region Felder
        
        private static readonly string TextLogPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Log", string.Format("Log{0:000}.txt", DateTime.Now.DayOfYear));
        #endregion

        public static void Write(LogType type, ulong contentId, string content)
        {
                Sql sql = new Sql();
                sql.CreateLogEntry(type.ToString(), contentId, content);
        }

        public static bool IsBitSet(int b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }

        public static void Text(LogType type, string message)
        {

            using (System.IO.StreamWriter file = System.IO.File.AppendText(TextLogPath))
            {
                file.WriteLine(DateTime.Now.ToShortTimeString() + " - " + type + " - " + message);
            }
        }

    }

}
