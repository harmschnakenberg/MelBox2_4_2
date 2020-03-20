using System;
using System.Collections.Generic;
using System.Data;
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

        public int MyProperty { get; set; }

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

                        "CREATE TABLE \"Contact\"(\"ID\" INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, \"Time\" INTEGER NOT NULL, \"Name\" TEXT NOT NULL, " +
                        "\"CompanyID\" INTEGER, \"Email\" TEXT, \"Phone\" INTEGER, \"KeyWord\" TEXT, \"MaxInactiveHours\" INTEGER, \"SendWay\" INTEGER );",

                       // "INSERT INTO \"Contact\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"MessageType\" ) VALUES (0, " + HelperClass.ConvertToUnixTime(DateTime.Now.ToUniversalTime()) + ", '" + MainWindow.SmsCenter.Name + "', 1, '" + MainWindow.SmsCenter.Email.Address + "', " + MainWindow.SmsCenter.Phone + "," + (ushort)MainWindow.SmsCenter.ContactType + ");",
                        "INSERT INTO \"Contact\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"MessageType\" ) VALUES (0, " + Contacts.Bereitschaftshandy.GetType().GetProperties().ToArray().ToString() + ");",

                        "INSERT INTO \"Contact\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"MessageType\" ) VALUES (1, " + HelperClass.ConvertToUnixTime(DateTime.Now) + ", '" + Contacts.MelBox2Admin.Name + "', 1, '" + Contacts.MelBox2Admin.Email.Address + "', " + Contacts.MelBox2Admin + "," + (ushort)Contacts.MelBox2Admin.ContactType + ");",

                        "INSERT INTO \"Contact\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"MessageType\" ) VALUES (2, " + HelperClass.ConvertToUnixTime(DateTime.Now) + ", '" + Contacts.Bereitschaftshandy.Name + "', 1, " + Contacts.Bereitschaftshandy.Email.Address +", " + Contacts.Bereitschaftshandy.Phone + "," + (ushort)Contacts.Bereitschaftshandy.ContactType + ");",

                        "INSERT INTO \"Contact\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"MessageType\" ) VALUES (3, " + HelperClass.ConvertToUnixTime(DateTime.Now) + ", '" + Contacts.KreuService.Name + "', 1, '" + Contacts.KreuService.Email.Address + "', 0," + (ushort)Contacts.KreuService.ContactType + ");",

                        "INSERT INTO \"Contact\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"MessageType\" ) VALUES (4, " + HelperClass.ConvertToUnixTime(DateTime.Now) + ", 'Henry Kreutzträger', 1, 'henry.kreutztraeger@kreutztraeger.de', 491727889419," + (ushort)(MessageType.SentToEmail & MessageType.SentToSms) + ");",

                        "INSERT INTO \"Contact\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"MessageType\" ) VALUES (5, " + HelperClass.ConvertToUnixTime(DateTime.Now) + ", 'Bernd Kreutzträger', 1, 'bernd.kreutztraeger@kreutztraeger.de', 491727875067," + (ushort)(MessageType.SentToEmail & MessageType.SentToSms) + ");",
                        
                        //Tabelle MessageTypes wird z.Zt. nicht verwendet! Dient als Dokumentation für die BitCodierung von MessageType.
                        "CREATE TABLE \"MessageType\" (\"ID\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \"Description\" TEXT NOT NULL);",

                        "INSERT INTO \"MessageType\" (\"ID\", \"Description\") VALUES (" + (ushort)MessageType.NoCategory + ", 'keine Zuordnung');",
                        "INSERT INTO \"MessageType\" (\"ID\", \"Description\") VALUES (" + (ushort)MessageType.System + ", 'von System');",
                        "INSERT INTO \"MessageType\" (\"ID\", \"Description\") VALUES (" + (ushort)MessageType.RecievedFromSms + ", 'von SMS');",
                        "INSERT INTO \"MessageType\" (\"ID\", \"Description\") VALUES (" + (ushort)MessageType.SentToSms + ", 'an SMS');",
                        "INSERT INTO \"MessageType\" (\"ID\", \"Description\") VALUES (" + (ushort)MessageType.RecievedFromEmail + ", 'von Email');",
                        "INSERT INTO \"MessageType\" (\"ID\", \"Description\") VALUES (" + (ushort)MessageType.SentToEmail + ", 'an Email');",

                        "CREATE TABLE \"MessageContent\" (\"ID\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \"Content\" TEXT NOT NULL UNIQUE );",

                        "INSERT INTO \"MessageContent\" (\"Content\") VALUES ('Datenbank neu erstellt.');",

                        "CREATE TABLE \"MessageLog\"( \"ID\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \"RecieveTime\" INTEGER NOT NULL, \"FromPersonID\" INTEGER NOT NULL, " +
                        " \"SendTime\" INTEGER, \"ToPersonIDs\" TEXT, \"Type\" INTEGER NOT NULL, \"ContentID\" INTEGER NOT NULL);",

                        "INSERT INTO \"MessageLog\" (\"RecieveTime\", \"FromPersonID\", \"Type\", \"ContentID\") VALUES " +
                        "(" + HelperClass.ConvertToUnixTime(DateTime.Now) + ",0,1,1);",

                        "CREATE TABLE \"Shifts\"( \"ID\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \"EntryTime\" INTEGER NOT NULL, " +
                        "\"ContactID\" INTEGER NOT NULL, \"StartTime\" INTEGER NOT NULL, \"EndTime\" INTEGER NOT NULL, \"SendType\" INTEGER NOT NULL );",

                        "CREATE TABLE \"BlockedMessages\"( \"MessageID\" INTEGER NOT NULL UNIQUE, \"StartHour\" INTEGER NOT NULL, " +
                        "\"EndHour\" INTEGER NOT NULL, \"WorkdaysOnly\" INTEGER NOT NULL CHECK (\"WorkdaysOnly\" < 2));" +

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
                CreateShiftDefault(Contacts.Bereitschaftshandy);
            }
        }

        /// <summary>
        /// Liest aus der SQL-Datenbank und gibt ein DataTable-Object zurück.
        /// </summary>
        /// <param name="query">SQL-Abfrage mit Parametern</param>
        /// <param name="args">Parameter - Wert - Paare</param>
        /// <returns>Abfrageergebnis als DataTable.</returns>
        private DataTable ExecuteRead(string query, Dictionary<string, object> args)
        {
            if (string.IsNullOrEmpty(query.Trim()))
                return null;

            using (var con = new SQLiteConnection(Datasource))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    if (args != null)
                    {
                        //set the arguments given in the query
                        foreach (var pair in args)
                        {
                            cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                        }
                    }


                    var da = new SQLiteDataAdapter(cmd);

                    var dt = new DataTable();
                    da.Fill(dt);
                    da.Dispose();
                    return dt;
                }
            }
        }

        /// <summary>
        /// Führt Schreibaufgaben in DB aus. 
        /// </summary>
        /// <param name="query">SQL-Abfrage mit Parametern</param>
        /// <param name="args">Parameter - Wert - Paare</param>
        /// <returns>Anzahl betroffener Zeilen.</returns>
        private int ExecuteWrite(string query, Dictionary<string, object> args)
        {
            int numberOfRowsAffected = 0;

            try
            {
                //setup the connection to the database
                using (var con = new SQLiteConnection(Datasource))
                {
                    con.Open();

                    //open a new command
                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        //set the arguments given in the query
                        foreach (var pair in args)
                        {
                            cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                        }

                        //execute the query and get the number of row affected
                        numberOfRowsAffected = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                //nichts unternehmen
                Log.Write(Log.LogType.Internal, 2003181644, "Schreiben in SQL-Datenbank fehlgeschlagen. " + query + " | " + ex.Message);
            }

            return numberOfRowsAffected;
        }

        #endregion

        #region Allgemein

        /// <summary>
        /// Listet alle Tabellen in der Datenbank, bis auf übergebene Ausnahmen.
        /// Hinweis: Systemtabellen sind gelistet in Tabelle "sqlite_sequence"
        /// </summary>
        /// <param name="exceptions">Ausnamen: Tabellen, die nicht aufgelistet werden sollen</param>
        /// <returns>List von Tabellennamen in der Datenbank</returns>
        internal IEnumerable<string> GetAllTableNames(string[] exceptions)
        {
            string query = "SELECT name FROM sqlite_master WHERE type=\"table\"";

            DataTable dt = ExecuteRead(query, null);
            List<string> s = dt.AsEnumerable().Select(x => x[0].ToString()).ToList();

            if (exceptions.Length > 0)
            {
                foreach (string exception in exceptions)
                {
                    s.Remove(exception);
                }
            }

            return s;
        }

        /// <summary>
        /// Gibt den letzten Eintrag (letzte ID) der angegebenen Tabelle wieder.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal uint GetLastId(string tableName, string where = "1=1")
        {
            string query = "SELECT ID FROM \"" + tableName + "\" WHERE " + where + " ORDER BY ID DESC LIMIT 1";

            DataTable dt = ExecuteRead(query, null);
            string idString = dt.AsEnumerable().Select(x => x[0].ToString()).ToList().First();
            uint.TryParse(idString, out uint lastId);

            return lastId;
        }


        /// <summary>
        /// Liest die letzten Einträge in der angegebenen Tabelle.
        /// Wandelt UNIX-Zeitstempel in lesbares Format um.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="numberOfRows"></param>
        /// <returns></returns>
        public DataTable GetLastEntries(string tableName, string sortingColName, int numberOfRows)
        {
            var args1 = new Dictionary<string, object>
            {
                {"@tableName", tableName}
            };

            string query = "PRAGMA table_info(@tableName)";
            DataTable dt = ExecuteRead(query, args1);
            List<string> s = dt.AsEnumerable().Select(x => x["name"].ToString()).ToList();

            string cols = string.Empty;
            foreach (string col in s)
            {
                if (col != s.First()) cols += ", ";
                if (col.Contains("Time"))
                {
                    cols += "datetime(" + col + ", 'unixepoch') AS " + col;
                }
                else
                {
                    cols += col;
                }
            }

            var args2 = new Dictionary<string, object>
            {
                {"@cols", cols},
                {"@tableName", tableName},
                {"@sortingColName", sortingColName},
                {"@numberOfRows", numberOfRows}
            };


            query = "SELECT @cols FROM @tableName ORDER BY @sortingColName DESC LIMIT @numberOfRows";

            return ExecuteRead(query, args2);
        }


        #endregion

        #region Logging

        /// <summary>
        /// Schreibt einen neuen Eintrag in die Tabelle 'Log'.
        /// </summary>
        /// <param name="message"></param>
        internal void CreateLogEntry(string type, ulong contentNo, string content)
        {
            const string query = "INSERT INTO Log(Time, Type, ContentNo, Content) VALUES (@timeStamp, @type, @contentNo, @content)";

            var args = new Dictionary<string, object>
            {
                {"@timeStamp", HelperClass.ConvertToUnixTime( DateTime.Now ) },
                {"@type", type},
                {"@contentNo", contentNo},
                {"@content", content}
            };

            ExecuteWrite(query, args);
        }

        #endregion

        #region SQL Firmen

        /// <summary>
        /// Gibt eine Liste aller Firmennamen aus.
        /// </summary>
        /// <param name="where">Einschränkende Bedingung.</param>
        /// <returns>Liste von Firmennamen</returns>
        internal IEnumerable<string> GetListOfCompanies(string where = "1=1")
        {
            string query = "SELECT \"Name\" FROM \"Company\" WHERE " + where;

            DataTable dt = ExecuteRead(query, null);
            List<string> s = dt.AsEnumerable().Select(x => x[0].ToString()).ToList();

            return s;
        }

        /// <summary>
        /// Erzeugt einen neuen Eintrag in der Firmentabelle.
        /// </summary>
        /// <param name="companyName"></param>
        /// <param name="address"></param>
        /// <param name="zipCode"></param>
        /// <param name="city"></param>
        /// <returns>ID des letzten (=erstellten) Eintrags.</returns>
        internal uint CreateCompany(string companyName, string address, uint zipCode, string city)
        {
            string query = "INSERT OR REPLACE INTO \"Company\" (Name, Address, ZipCode, City) VALUES (@Name, @Address, @ZipCode, @City) ";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                { "@Name", companyName },
                { "@Address", address },
                { "@ZipCode", zipCode },
                { "@City", city }
            };

            ExecuteWrite(query, args);

            return GetLastId("Companies");
        }

        /// <summary>
        /// Aktualisiert einen Eintrag in der Firmentabelle.
        /// </summary>
        /// <param name="companyId">ID des EIntrags, der geändert werden soll.</param>
        /// <param name="companyName"></param>
        /// <param name="address"></param>
        /// <param name="zipCode"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        internal int UpdateCompany(uint companyId, string companyName, string address, int zipCode, string city)
        {
            string query = "UPDATE \"Company\" SET Name = @Name, Address = @Address, ZipCode = @ZipCode, City = @City WHERE ID = @ID ;";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                { "@ID", companyId },
                { "@Name", companyName },
                { "@Address", address },
                { "@ZipCode", zipCode },
                { "@City", city }
            };

            return ExecuteWrite(query, args);
        }

        /// <summary>
        /// Entfernt einen Eintrag in der Firmentabelle.
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        internal int DeleteCompany(uint companyId)
        {
            string query = "DELETE FROM \"Company\" WHERE ID = @ID ;";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                { "@ID", companyId }
            };

            return ExecuteWrite(query, args);
        }

        #endregion

        #region SQL Contacts


        /// <summary>
        /// Fügt eine neue Person (Nutzer) in die Datenbank ein
        /// und gibt die ID des Eintrags wieder.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private uint CreateUnknownContact(Message message)
        {
            //Schreibe neue Person in DB
            const string query = "INSERT INTO Contacts( Time, Name, Phone, Email, KeyWord, SendWay ) VALUES ( @Time, @Name, @Phone, @Email, @KeyWord, @SendWay )";

            string keyWord = HelperClass.GetKeyWord(message.Content);

            var args = new Dictionary<string, object>
                {
                    {"@Time", HelperClass.ConvertToUnixTime(DateTime.Now) },
                    {"@Name", Contacts.UnknownName },
                    {"@Phone", message.From.Phone},
                    {"@Email", message.From.Email.Address.ToLower() },
                    {"@KeyWord", keyWord},
                    {"@SendWay", message.Status }
                };

            ExecuteWrite(query, args);

            uint lastId = GetLastId("Contacts");

            #region Email-Benachrichtigung "neue unbekannte Telefonnummer / Emailadresse"

            Messages.Create_NewUnknownContact(message, lastId, keyWord);


            #endregion

            return lastId;
        }




        #endregion

        #region SQL Schichten

        /// <summary>
        /// Erstellt eine neue Schicht im Bereitschaftsplan und gibt die ID der neuen Schicht wieder.
        /// </summary>
        /// <param name="contactId">ID des Bereitschftnehmers</param>
        /// <param name="startTime">Startzeitpunkt in UTC</param>
        /// <param name="endTime">Endzeitpunkt in UTC</param>
        /// <param name="sendingType">Benachrichtigungsweg MessageType.SentToEmail oder MessageType.SentToSms</param>
        /// <returns>Id der erzeugten Schicht</returns>
        internal uint CreateShift(uint contactId, DateTime startTime, DateTime endTime, MessageType sendingType)
        {            
            ulong entryTime = HelperClass.ConvertToUnixTime( DateTime.Now );

            string query = "INSERT INTO Shifts ( EntryTime, ContactID, StartTime, EndTime, SendType ) " +
                           "VALUES ( @EntryTime, @ContactID, @StartTime, @EndTime, @SendType ); ";

            Dictionary<string, object> args = new Dictionary<string, object>()
            {
                    {"@EntryTime", entryTime},
                    {"@ContactID", contactId},
                    {"@StartTime", HelperClass.ConvertToUnixTime(startTime) },
                    {"@EndTime", HelperClass.ConvertToUnixTime(endTime)},
                    {"@SendType", (int)sendingType},
            };

            ExecuteWrite(query, args);

            return GetLastId("Shifts", "EntryTime = " + entryTime);
        }

        /// <summary>
        /// Standard-Schicht, die erstellt wird, wenn kein Eintrag für den aktuellen Tag gefunden wurde.
        /// </summary>
        /// <param name="contact">Kontakt des Bereitschaftnehmers, wie er in der Datenbank abgelegt ist.</param>
        /// <returns>ID der neu erstellten Schicht.</returns>
        internal uint CreateShiftDefault(Contact contact)
        {
            //uint bereitschaftId = GetIdFromEntry("Persons", "Name", GuardName);
            //MessageType type = (MessageType)int.Parse(GetFirstEntryFromColumn("Persons", "MessageType", "Name = '" + GuardName + "'"));

            if (contact.Id < 1) return 0; 
            //TODO: Kontakt mit Datenbankinhalt validieren 

            DateTime date = DateTime.Now.Date;
            List<DateTime> holidays = HelperClass.Feiertage(date);

            int startHour;
            if (holidays.Contains(date) || date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                startHour = HelperClass.NightShiftEndHour; //Start = Ende Vortag.
            }
            else
            {
                if (date.DayOfWeek == DayOfWeek.Friday)
                {
                    startHour = HelperClass.NightShiftStartHourFriday;
                }
                else
                {
                    startHour = HelperClass.NightShiftStartHour;
                }
            }

            DateTime StartTime = date.AddHours(startHour);
            DateTime EndTime = date.AddDays(1).AddHours(HelperClass.NightShiftEndHour);
            Log.Write(Log.LogType.Calendar, 987654367, "Habe automatische Standardschicht erstellt von " + StartTime.ToString("dd.MM.yyyy HH:mm") + " bis " + EndTime.ToString("dd.MM.yyyy HH:mm"));

            return CreateShift(contact.Id, StartTime, EndTime, contact.ContactType);
        }

        /// <summary>
        /// Aktualisiert eine vorhanden Schicht im Bereitschaftsplan.
        /// </summary>
        /// <param name="shiftId"></param>
        /// <param name="personId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        internal void UpdateShift(uint shiftId, uint contactId, DateTime startTime, DateTime endTime, MessageType sendingType)
        {
            ulong entryTime = HelperClass.ConvertToUnixTime( DateTime.Now );

            string query = "UPDATE Shifts SET " +
                           "EntryTime = @EntryTime, ContactID = @ContactID, StartTime = @StartTime, EndTime = @EndTime, SendType = @SendType " +
                           "WHERE ID = @ID ; ";

            Dictionary<string, object> args = new Dictionary<string, object>()
            {
                    {"@ID", shiftId},
                    {"@EntryTime", entryTime},
                    {"@ContactID", contactId},
                    {"@StartTime", HelperClass.ConvertToUnixTime(startTime) },
                    {"@EndTime", HelperClass.ConvertToUnixTime(endTime)},
                    {"@SendType", (ushort)sendingType}
            };

            ExecuteWrite(query, args);
        }

        /// <summary>
        /// Aktualisiert nur den Bereitschaftsnehmer in einer vorhanden Schicht im Bereitschaftsplan.
        /// </summary>
        /// <param name="shiftId">ID der Schicht die geändert werden soll.</param>
        /// <param name="personId">ID der Person, die diese Schicht übernimmt.</param>
        internal void UpdateShift(uint shiftId, uint personId, MessageType messageType)
        {
            string query = "UPDATE Shifts SET " +
                           "ContactID = @ContactID, " +
                           "SendType = @SendType, " +
                           "WHERE ID = @ID ; ";

            Dictionary<string, object> args = new Dictionary<string, object>()
            {
                    {"@ID", shiftId},
                    {"@ContactID", personId},
                    {"@SendType", (uint)messageType}
            };

            ExecuteWrite(query, args);
        }

        /// <summary>
        /// Entfert eine Schicht aus dem Bereitschaftsplan.
        /// </summary>
        /// <param name="shiftId"></param>
        /// <returns></returns>
        internal int DeleteShift(uint shiftId)
        {
            //nur löschen, wenn mindestens eine Schicht vorhanden
            string query = "DELETE FROM \"Shifts\" WHERE ID = @ID;";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                { "@ID", shiftId }
            };

            return ExecuteWrite(query, args);
        }


        #endregion
    }
}
