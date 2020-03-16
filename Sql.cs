using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelBox2_4
{
    class Sql
    {

        #region Felder
        public static string DbPath { get; set; } = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "DB", "MelBox2.db");
        private readonly string Datasource = "Data Source=" + DbPath;
        #endregion

        #region SQL- Basismethoden
        public Sql()
        {
            if (!System.IO.File.Exists(DbPath))
            {
                CreateNewDataBase();
            }
        }


        /// <summary>
        /// Erzeugt eine neue Datenbankdatei, erzeugt darin Tabellen, Füllt diverse Tabellen mit Defaultwerten.
        /// </summary>
        private void CreateNewDataBase()
        {
            //Erstelle Datenbank-Datei
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(DbPath));
            FileStream stream = File.Create(DbPath);
            stream.Close();

            //Erzeuge Tabellen in neuer Datenbank-Datei
            using (var con = new SQLiteConnection(Datasource))
            {
                con.Open();

                List<String> TableCreateQueries = new List<string>
                    {
                        "CREATE TABLE \"Log\"(\"ID\" INTEGER NOT NULL PRIMARY KEY UNIQUE,\"Time\" INTEGER NOT NULL, \"Type\" TEXT , \"ContentNo\" INTEGER NOT NULL, \"Content\" TEXT);",

                        "CREATE TABLE \"Company\" (\"ID\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \"Name\" TEXT NOT NULL, \"Address\" TEXT, \"ZipCode\" INTEGER,\"City\" TEXT); ",

                        "INSERT INTO \"Company\" (\"ID\", \"Name\", \"Address\", \"ZipCode\", \"City\") VALUES (0, '_UNBEKANNT_', 'Musterstraße 123', 12345, 'Modellstadt' );",

                        "INSERT INTO \"Company\" (\"ID\", \"Name\", \"Address\", \"ZipCode\", \"City\") VALUES (1, 'Kreutzträger Kältetechnik GmbH & Co. KG', 'Theodor-Barth-Str. 21', 28307, 'Bremen' );",

                        "CREATE TABLE \"User\"(\"ID\" INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, \"Time\" INTEGER NOT NULL, \"Name\" TEXT NOT NULL, " +
                        "\"CompanyID\" INTEGER, \"Email\" TEXT, \"Phone\" INTEGER, \"KeyWord\" TEXT, \"MaxInactiveHours\" INTEGER, \"SendWay\" INTEGER );",

                        "INSERT INTO \"User\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"MessageType\" ) VALUES (0, " + HelperClass.ConvertToUnixTime(DateTime.Now.ToUniversalTime()) + ", '" + MainWindow.SMSCenter.DisplayName + "', 1, '" + MainWindow.SMSCenter.Address + "', " + MainWindow.GsmModemPhoneNumber + "," + (ushort)MessageType.NoCategory + ");",

                        "INSERT INTO \"User\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"MessageType\" ) VALUES (1, " + HelperClass.ConvertToUnixTime(DateTime.Now.ToUniversalTime()) + ", '" + MainWindow.MelBox2AdminName + "', 1, '" + MainWindow.MelBoxAdminMail + "', 4915142265412," + (ushort)MessageType.SentToEmail + ");",

                        "INSERT INTO \"User\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"MessageType\" ) VALUES (2, " + HelperClass.ConvertToUnixTime(DateTime.Now.ToUniversalTime()) + ", '" + MainWindow.DefaultGuardName + "', 1, 'bereitschaftshandy@kreutztraeger.de', 491728362586," + (ushort)MessageType.SentToSms + ");",

                        "INSERT INTO \"User\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"MessageType\" ) VALUES (3, " + HelperClass.ConvertToUnixTime(DateTime.Now.ToUniversalTime()) + ", '" + MainWindow.KreuService.DisplayName + "', 1, '" + MainWindow.KreuService.Address + "', 0," + (ushort)MessageType.SentToEmail + ");",

                        "INSERT INTO \"User\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"MessageType\" ) VALUES (4, " + HelperClass.ConvertToUnixTime(DateTime.Now.ToUniversalTime()) + ", 'Henry Kreutzträger', 1, 'henry.kreutztraeger@kreutztraeger.de', 491727889419," + (ushort)MessageType.SentToEmailAndSMS + ");",

                        "INSERT INTO \"User\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"MessageType\" ) VALUES (5, " + HelperClass.ConvertToUnixTime(DateTime.Now.ToUniversalTime()) + ", 'Bernd Kreutzträger', 1, 'bernd.kreutztraeger@kreutztraeger.de', 491727875067," + (ushort)MessageType.SentToEmailAndSMS + ");",

                        //Tabelle MessageTypes wird z.Zt. nicht verwendet! Dient als Dokumentation für die BitCodierung von MessageType.
                        "CREATE TABLE \"MessageType\" (\"ID\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \"Description\" TEXT NOT NULL);",

                        "INSERT INTO \"MessageType\" (\"ID\", \"Description\") VALUES (" + (ushort)MessageType.NoCategory + ", 'keine Zuordnung');",
                        "INSERT INTO \"MessageType\" (\"ID\", \"Description\") VALUES (" + (ushort)MessageType.RecievedFromUnknown + ", 'von System');",
                        "INSERT INTO \"MessageType\" (\"ID\", \"Description\") VALUES (" + (ushort)MessageType.SentToUnknown + ", 'an unbekannt');",
                        "INSERT INTO \"MessageType\" (\"ID\", \"Description\") VALUES (" + (ushort)MessageType.RecievedFromSms + ", 'von SMS');",
                        "INSERT INTO \"MessageType\" (\"ID\", \"Description\") VALUES (" + (ushort)MessageType.SentToSms + ", 'an SMS');",
                        "INSERT INTO \"MessageType\" (\"ID\", \"Description\") VALUES (" + (ushort)MessageType.RecievedFromEmail + ", 'von Email');",
                        "INSERT INTO \"MessageType\" (\"ID\", \"Description\") VALUES (" + (ushort)MessageType.SentToEmail + ", 'an Email');",

                        "CREATE TABLE \"MessageContent\" (\"ID\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \"Content\" TEXT NOT NULL UNIQUE );",

                        "INSERT INTO \"MessageContent\" (\"Content\") VALUES ('Datenbank neu erstellt.');",

                        "CREATE TABLE \"MessageLog\"( \"ID\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \"RecieveTime\" INTEGER NOT NULL, \"FromPersonID\" INTEGER NOT NULL, " +
                                            " \"SendTime\" INTEGER, \"ToPersonIDs\" TEXT, \"Type\" INTEGER NOT NULL, \"ContentID\" INTEGER NOT NULL);",

                        "INSERT INTO \"MessageLog\" (\"RecieveTime\", \"FromPersonID\", \"Type\", \"ContentID\") VALUES " +
                                            "(" + ConvertToUnixTime(DateTime.Now) + ",0,1,1);",

                        "CREATE TABLE \"Shifts\"( \"ID\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \"EntryTime\" INTEGER NOT NULL, " +
                                            "\"PersonID\" INTEGER NOT NULL, \"StartTime\" INTEGER NOT NULL, \"EndTime\" INTEGER NOT NULL, \"SendType\" INTEGER NOT NULL );",

                        //"INSERT INTO \"Shifts\" ( \"ID\", \"EntryTime\", \"PersonID\", \"StartTime\", \"EndTime\", \"SendType\" ) VALUES (0, " +
                        // ConvertToUnixTime( DateTime.Now ) + ", 0," + ConvertToUnixTime( DateTime.Now.Date.AddHours(17) ) + ", " + ConvertToUnixTime( DateTime.Now.Date.AddDays(1).AddHours(7)  ) +", 0 );",

                        "CREATE TABLE \"BlockedMessages\"( \"MessageID\" INTEGER NOT NULL UNIQUE, \"StartHour\" INTEGER NOT NULL, " +
                        " \"EndHour\" INTEGER NOT NULL, \"WorkdaysOnly\" INTEGER NOT NULL CHECK (\"WorkdaysOnly\" < 2));" +

                        "INSERT INTO \"BlockedMessages\" (\"MessageID\", \"StartHour\", \"EndHour\", \"WorkdaysOnly\" ) VALUES " +
                        "(1,8,8,0);"

                };

                foreach (string query in TableCreateQueries)
                {
                    SQLiteCommand sQLiteCommand = new SQLiteCommand(query, con);
                    using (SQLiteCommand cmd = sQLiteCommand)
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                //Es muss mindestens ein EIntrag in der Tabelle "Shifts" vorhanden sein.
                CreateShiftDefault(MainWindow.DefaultGuardName);
            }
        }


    }
}
