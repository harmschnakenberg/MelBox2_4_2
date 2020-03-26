using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                        "CREATE TABLE \"Log\"(\"ID\" INTEGER NOT NULL PRIMARY KEY UNIQUE,\"Time\" INTEGER NOT NULL, \"Topic\" TEXT , \"Prio\" INTEGER NOT NULL, \"ContentNo\" INTEGER NOT NULL, \"Content\" TEXT);",

                        "CREATE TABLE \"Company\" (\"ID\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \"Name\" TEXT NOT NULL, \"Address\" TEXT, \"ZipCode\" INTEGER,\"City\" TEXT); ",

                        "INSERT INTO \"Company\" (\"ID\", \"Name\", \"Address\", \"ZipCode\", \"City\") VALUES (0, '_UNBEKANNT_', 'Musterstraße 123', 12345, 'Modellstadt' );",

                        "INSERT INTO \"Company\" (\"ID\", \"Name\", \"Address\", \"ZipCode\", \"City\") VALUES (1, 'Kreutzträger Kältetechnik GmbH & Co. KG', 'Theodor-Barth-Str. 21', 28307, 'Bremen' );",

                        "CREATE TABLE \"Contact\"(\"ID\" INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, \"Time\" INTEGER NOT NULL, \"Name\" TEXT NOT NULL, " +
                        "\"CompanyID\" INTEGER, \"Email\" TEXT, \"Phone\" INTEGER, \"KeyWord\" TEXT, \"MaxInactiveHours\" INTEGER, \"SendWay\" INTEGER );",

                        "INSERT INTO \"Contact\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"SendWay\" ) VALUES (1, " + HelperClass.ConvertToUnixTime(DateTime.Now) + ", 'SMSZentrale', 1, 'smszentrale@kreutztraeger.de', 4915142265412," + (ushort)MessageType.NoCategory + ");",
                        
                        "INSERT INTO \"Contact\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"SendWay\" ) VALUES (2, " + HelperClass.ConvertToUnixTime(DateTime.Now) + ", 'MelBox2Admin', 1, 'harm.schnakenberg@kreutztraeger.de', 0," + (ushort)MessageType.SentToEmail + ");",

                        "INSERT INTO \"Contact\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"SendWay\" ) VALUES (3, " + HelperClass.ConvertToUnixTime(DateTime.Now) + ", 'Bereitschaftshandy', 1, 'bereitschaftshandy@kreutztraeger.de', 491728362586," + (ushort)MessageType.SentToSms + ");",

                        "INSERT INTO \"Contact\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"SendWay\" ) VALUES (4, " + HelperClass.ConvertToUnixTime(DateTime.Now) + ", 'Kreutzträger Service', 1, 'service@kreutztraeger.de', 0," +  (ushort)MessageType.SentToEmail + ");",

                        "INSERT INTO \"Contact\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"SendWay\" ) VALUES (5, " + HelperClass.ConvertToUnixTime(DateTime.Now) + ", 'Henry Kreutzträger', 1, 'henry.kreutztraeger@kreutztraeger.de', 491727889419," + (ushort)(MessageType.SentToEmail & MessageType.SentToSms) + ");",

                        "INSERT INTO \"Contact\" (\"ID\", \"Time\", \"Name\", \"CompanyID\", \"Email\", \"Phone\", \"SendWay\" ) VALUES (6, " + HelperClass.ConvertToUnixTime(DateTime.Now) + ", 'Bernd Kreutzträger', 1, 'bernd.kreutztraeger@kreutztraeger.de', 491727875067," + (ushort)(MessageType.SentToEmail & MessageType.SentToSms) + ");",
                        
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

                        "CREATE TABLE \"BlockedMessages\"( \"ID\" INTEGER NOT NULL UNIQUE, \"StartHour\" INTEGER NOT NULL, " +
                        "\"EndHour\" INTEGER NOT NULL, \"WorkdaysOnly\" INTEGER NOT NULL CHECK (\"WorkdaysOnly\" < 2));" +

                        "INSERT INTO \"BlockedMessages\" (\"ID\", \"StartHour\", \"EndHour\", \"WorkdaysOnly\" ) VALUES " +
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

                    try
                    {
                        da.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        MainWindow.Log(MainWindow.Topic.SQL, MainWindow.Prio.Warnung, 2003221804, "SQL-Fehler in " + query + " | " + ex.Message);
                    }
                    finally
                    {
                        da.Dispose();
                    }

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
                MainWindow.Log(MainWindow.Topic.Internal, MainWindow.Prio.Fehler, 2003181644, "Schreiben in SQL-Datenbank fehlgeschlagen. " + query + " | " + ex.Message);
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
            if (dt.Rows.Count == 0) return 0;

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
            string query = "PRAGMA table_info(" + tableName + ")";
            DataTable dt = ExecuteRead(query, null); //args erzeugt SQLLogicError
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

            query = "SELECT " + cols +" FROM " + tableName + " ORDER BY " + sortingColName + " DESC LIMIT " + numberOfRows;

            return ExecuteRead(query, null); //args erzeugt SQLLogicError
        }

        ///// <summary>
        ///// List eine LIste aus der Splate colName aus.
        ///// </summary>
        ///// <param name="tableName">Name der Tabelle.</param>
        ///// <param name="colName">Name der abzufragenden Spalte.</param>
        ///// <param name="where">(optional) Einschränkende Bedingung.</param>
        ///// <returns></returns>
        //public IEnumerable<string> GetListFromColumn(string tableName, string colName, string where = "1=1")
        //{
        //    string query = "SELECT " + colName + " FROM " + tableName + " WHERE " + where;

        //    DataTable dt = ExecuteRead(query, null);
        //    List<string> s = dt.AsEnumerable().Select(x => x[0].ToString()).ToList();

        //    return s;
        //}

        internal Dictionary<string, object> GetRowValues(string query, Dictionary<string, object> args)
        {
            DataTable dt = ExecuteRead(query, args);

            Dictionary<string, object> dict = new Dictionary<string, object>();

            if (dt.Rows.Count > 0)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    dict.Add(col.ColumnName, dt.Rows[0][col.ColumnName]);
                }
            }
            return dict;
        }

        public ObservableCollection<Contact> GetContacts(string where = "1=1")
        {
            string query = "SELECT ID, Name, CompanyID, Email, Phone, MaxInactiveHours, SendWay FROM Contact WHERE " + where;

            DataTable dt = ExecuteRead(query, null);

            if (dt.Rows.Count == 0) return null;

            ObservableCollection<Contact> contactList = new ObservableCollection<Contact>();

            foreach (DataRow row in dt.Rows)
            {

                System.Net.Mail.MailAddress email = null;
                
                if (HelperClass.IsValidEmailAddress(row[3].ToString())) 
                    email = new System.Net.Mail.MailAddress(row[3].ToString(), row[1].ToString());

                ushort.TryParse(row[5].ToString(), out ushort inactiveMax);

                Contact contact = new Contact
                {
                    Id = uint.Parse(row[0].ToString()),
                    Name = row[1].ToString(),
                    CompanyId = uint.Parse(row[2].ToString()),
                    Email = email,
                    PhoneString = row[4].ToString(),
                    MaxInactiveHours = inactiveMax,
                    ContactType = (MessageType)ushort.Parse(row[6].ToString())                    
                };

                
                contactList.Add(contact);
            }

            return contactList;
        }

        #endregion

        #region Logging

        /// <summary>
        /// Schreibt einen neuen Eintrag in die Tabelle 'Log'.
        /// </summary>
        /// <param name="message"></param>
        internal void CreateLogEntry(MainWindow.Topic topic, MainWindow.Prio prio, ulong contentNo, string content)
        {
            const string query = "INSERT INTO Log(Time, Topic, Prio, ContentNo, Content) VALUES (@timeStamp, @topic, @prio, @contentNo, @content)";

            var args = new Dictionary<string, object>
            {
                {"@timeStamp", HelperClass.ConvertToUnixTime( DateTime.Now ) },
                {"@topic", topic.ToString() },
                {"@prio", (ushort)prio},
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

        internal Company GetCompanyFromDb(uint companyId)
        {
            Company company = new Company();

            const string query = "SELECT Name, Address, ZipCode, City FROM Company WHERE ID = @Id";

            var args = new Dictionary<string, object>
            {
                {"@Id", companyId}
            };

            DataTable dt = ExecuteRead(query, args);

            if (dt.Rows.Count == 0)
            {
                MainWindow.Log(MainWindow.Topic.SQL, MainWindow.Prio.Warnung, 2003251227, "Keine Firmeninformationen in DB für ID >" + companyId + "<");
                return company;
            }

            uint.TryParse(dt.Rows[0][2].ToString(), out uint zipCode);

            company.Id = companyId;
            company.Name = dt.Rows[0][0].ToString();
            company.Address = dt.Rows[0][1].ToString();
            company.ZipCode = zipCode;
            company.City = dt.Rows[0][3].ToString();

            return company;
        }

        internal Company GetCompanyFromDb(string companyName)
        {
            Company company = new Company();

            const string query = "SELECT ID, Address, ZipCode, City FROM Company WHERE Name = @name";

            var args = new Dictionary<string, object>
            {
                {"@Name", companyName}
            };

            DataTable dt = ExecuteRead(query, args);

            if (dt.Rows.Count == 0)
            {
                MainWindow.Log(MainWindow.Topic.SQL, MainWindow.Prio.Warnung, 2003251242, "Keine Firmeninformationen in DB für Firmennamen >" + companyName + "<");
                return company;
            }

            uint.TryParse(dt.Rows[0][0].ToString(), out uint companyId);
            uint.TryParse(dt.Rows[0][2].ToString(), out uint zipCode);

            company.Id = companyId;
            company.Name = companyName;
            company.Address = dt.Rows[0][1].ToString();
            company.ZipCode = zipCode;
            company.City = dt.Rows[0][3].ToString();

            return company;
        }

        public ObservableCollection<Company> GetCompanies(string where = "1=1")
        {
            string query = "SELECT ID, Name, Address, ZipCode, City FROM Company WHERE " + where;

            DataTable dt = ExecuteRead(query, null);

            if (dt.Rows.Count == 0) return null;

            ObservableCollection<Company> companyList = new ObservableCollection<Company>();

            foreach (DataRow row in dt.Rows)
            {

                uint.TryParse(row[0].ToString(), out uint companyId);
                uint.TryParse(row[3].ToString(), out uint zipCode);

                Company company= new Company
                {
                    Id = companyId,
                    Name = row[1].ToString(),
                    Address = row[2].ToString(),
                    ZipCode = zipCode,
                    City = row[4].ToString()
                };


                companyList.Add(company);
            }

            return companyList;
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

            return GetLastId("Company");
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

            Messages.Create_NewUnknownContactMessage(message, lastId, keyWord);


            #endregion

            return lastId;
        }

        //internal uint CreatePerson(string Name, string companyName, string email, string cellphone, string keyword, string maxinactive, MessageType role)
        //{
        //    //Schreibe/Aktualisiere Person in DB
        //    const string queryCreate = "INSERT INTO Contact (Time, Name, CompanyID, Cellphone, Email, KeyWord, MaxInactiveHours, SendWay ) " +
        //                                "VALUES ( @UnixTimeStamp, @Name, @CompanyId, @Cellphone, @Email, @KeyWord, @MaxInactive, @MessageType )";

        //    ulong unixTimeStamp = HelperClass.ConvertToUnixTime(DateTime.Now);

        //    uint CompanyId = GetIdFromEntry("Companies", "Name", companyName);

        //    ulong phonenumber = HelperClass.ConvertStringToPhonenumber(cellphone);

        //    if (!HelperClass.IsValidEmailAddress(email))
        //    {
        //        email = null;
        //    }

        //    if (!int.TryParse(maxinactive, out int maxInactiveInt))
        //    {
        //        maxInactiveInt = 0;
        //    }


        //    Dictionary<string, object> args = new Dictionary<string, object>()
        //    {
        //            {"@UnixTimeStamp", unixTimeStamp },
        //            {"@Name", Name},
        //            {"@CompanyId", CompanyId},
        //            {"@Cellphone", phonenumber},
        //            {"@Email", email},
        //            {"@KeyWord", keyword},
        //            {"@MaxInactive", maxInactiveInt},
        //            {"@MessageType", (uint)role}
        //    };

        //    ExecuteWrite(queryCreate, args);

        //    return GetLastId("Persons", "UnixTimeStamp = " + unixTimeStamp);
        //}


        //internal uint GetContactId(Message message)
        //{
        //    string email = message.EMail;
        //    string keyWord = message.CustomerKeyWord;

        //    if (email == null) email = String.Empty;
        //    if (keyWord == null) keyWord = String.Empty;

        //    Dictionary<string, object> personArgs = new Dictionary<string, object>
        //    {
        //        { "@phoneNumber", message.Cellphone },
        //        { "@email",  email.ToLower() },
        //        { "@keyWord", keyWord.ToLower() }
        //    };


        //    DataTable senderIDTable = ExecuteRead("SELECT \"ID\" FROM \"Persons\" WHERE " +
        //                                            "( \"Cellphone\" > 0 AND \"Cellphone\" = @phoneNumber ) " +
        //                                            "OR ( length(\"KeyWord\") > 2 AND \"KeyWord\" = @keyWord ) " +
        //                                            "OR ( length(\"Email\") > 5 AND \"Email\" = @email )", personArgs);
        //    // Keine passende Person gefunden:
        //    if (senderIDTable.Rows.Count < 1)
        //    {
        //        Log.Write(Log.Type.Persons, string.Format("Kein Eintrag gefunden. Neue Person wird angelegt mit >{0}<, >{1}<, >{2}<", message.CustomerKeyWord, message.EMail, message.Cellphone));
        //        return CreatePerson(message);
        //    }
        //    else if (senderIDTable.Rows.Count > 1)
        //    {
        //        string entries = string.Empty;
        //        foreach (string item in senderIDTable.AsEnumerable().Select(x => x[0].ToString()).ToList())
        //        {
        //            entries += item + ",";
        //        }
        //        Log.Write(Log.Type.Persons, string.Format("Es gibt meherer Einträge für eine Person mit KeyWord >{0}<, Email >{1}<, Mobilnummer >{2}< \r\nPersonen-IDs: {3}", message.CustomerKeyWord, message.EMail, message.Cellphone, entries));
        //    }
        //    else
        //    {
        //        Log.Write(Log.Type.Persons, string.Format("Es gibt genau einen Eintrag für Keyword >{0}<, Email >{1}<, Mobilnummer >{2}<", message.CustomerKeyWord, message.EMail, message.Cellphone));
        //    }

        //    string idString = senderIDTable.AsEnumerable().Select(x => x[0].ToString()).ToList().First();

        //    if (!uint.TryParse(idString, out uint senderId))
        //    {
        //        Log.Write(Log.Type.Persons, "Der Eintrag >" + idString + "< konnte nicht als ID für eine Person interpretiert werden.");
        //        return CreatePerson(message);
        //    }

        //    return senderId;
        //}


        /// <summary>
        /// Sucht Einträge mit mindestens einer Übereinstimmung der angegebenen Kontaktdaten.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="phone"></param>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        internal uint GetContactId(string personName, string email, ulong phone, string keyWord)
        {

            Dictionary<string, object> personArgs = new Dictionary<string, object>
            {
                { "@name", personName },
                { "@phoneNumber", phone },
                { "@email",  email },
                { "@keyWord", keyWord }
            };

            DataTable senderIDTable = ExecuteRead("SELECT \"ID\" FROM \"Contact\" WHERE " +
                                                    "( \"Name\" IS NOT NULL AND \"Name\" = @name ) " +
                                                    "OR ( \"Phone\" > 0 AND \"Phone\" = @phoneNumber ) " +
                                                    "OR ( length(\"KeyWord\") > 2 AND \"KeyWord\" = @keyWord ) " +
                                                    "OR ( length(\"Email\") > 5 AND \"Email\" = @email )", personArgs);
            // Keine passende Person gefunden:
            if (senderIDTable.Rows.Count < 1)
            {
                return 0;
            }

            if (senderIDTable.Rows.Count > 1)
            {
                MainWindow.Log(MainWindow.Topic.Contacts, MainWindow.Prio.Warnung, 2003231133, 
                    string.Format("Es gibt " + senderIDTable.Rows.Count  + " Einträge für eine Person mit KeyWord >{0}<, Email >{1}<, Mobilnummer >{2}<", keyWord, email, phone));
            }

            string idString = senderIDTable.AsEnumerable().Select(x => x[0].ToString()).ToList().Last();

            if (!uint.TryParse(idString, out uint senderId))
            {
                MainWindow.Log(MainWindow.Topic.Contacts, MainWindow.Prio.Fehler, 2003231136, 
                    "Der Eintrag >" + idString + "< konnte nicht als ID für einen Kontakt interpretiert werden.");
                return 0;
            }

            return senderId;
        }

        /// <summary>
        /// Erzeugt eine Instanz der Contact Klasse aus der Datenbank.
        /// </summary>
        /// <param name="contactId">Id des Kontakts in der Datenbank</param>
        /// <returns>Instanz von Contact mit den Einträgen aus der Datenbank</returns>
        internal Contact GetContactFromDb(uint contactId)
        {
            const string query = "SELECT Name, CompanyId, Email, Phone, KeyWord, SendWay FROM Contact WHERE ID = @Id";

            var args = new Dictionary<string, object>
            {
                {"@Id", contactId}
            };

            DataTable result =  ExecuteRead(query, args);

            if (result.Rows.Count == 0) return null;

            Contact contact = new Contact
            {
                Id = contactId,
                Name = result.Rows[0][0].ToString(),
                CompanyId = uint.Parse(result.Rows[0][1].ToString()),
                Email = new System.Net.Mail.MailAddress(result.Rows[0][2].ToString(), result.Rows[0][0].ToString()),
                PhoneString = result.Rows[0][3].ToString(),
                KeyWord = result.Rows[0][4].ToString(),
                ContactType = (MessageType)ushort.Parse(result.Rows[0][5].ToString())
            };

            return contact;
        }

        /// <summary>
        /// Legt neuen Kontakt in DB an und gibt dessen ID aus.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="companyId"></param>
        /// <param name="email"></param>
        /// <param name="cellphone"></param>
        /// <param name="keyword"></param>
        /// <param name="maxinactive"></param>
        /// <param name="role"></param>
        /// <returns>ID des neu erstellten Kontakts</returns>
        internal uint CreateContact(Contact contact)
        {            
            const string queryCreate = "INSERT INTO Contact ( Time, Name, CompanyID, Email, Phone, KeyWord, MaxInactiveHours, SendWay ) " +
                                        "VALUES ( @Time, @Name, @CompanyId, @Email, @Phone, @KeyWord, @MaxInactiveHours, @SendWay )";

            ulong unixTimeStamp = HelperClass.ConvertToUnixTime(DateTime.Now);

            Dictionary<string, object> args = new Dictionary<string, object>()
            {
                    {"@Time", unixTimeStamp },
                    {"@Name", contact.Name},
                    {"@CompanyId", contact.CompanyId},
                    {"@Phone", contact.Phone},
                    {"@Email", contact.Email},
                    {"@KeyWord", contact.KeyWord},
                    {"@MaxInactiveHours", contact.MaxInactiveHours},
                    {"@SendWay", (uint)contact.ContactType}
            };

            int affectedRows = ExecuteWrite(queryCreate, args);

            if (affectedRows == 0)
            {
                MainWindow.Log(MainWindow.Topic.SQL, MainWindow.Prio.Fehler, 2003241204, "Der Kontakt mit dem Namen >" + contact.Name + "< konnte nicht in der Datenbank erstellt werden.");
                return 0;
            }

            return GetLastId("Contact", "Time = " + unixTimeStamp);
        }

        /// <summary>
        /// Verändert einen bestenhenden Kontakt in der Datenbank
        /// </summary>
        /// <param name="contact"></param>
        /// <returns>true = es wurde genau eine Zeile in der Datenbank geändert.</returns>
        internal bool UpdateContact(Contact contact)
        {
            const string queryUpdate =  "UPDATE Contact SET  " +
                                        "Time = @UnixTimeStamp, " +
                                        "Name = @Name, " +
                                        "CompanyID = @CompanyId, " +
                                        "Phone = @Phone, " +
                                        "Email = @Email, " +
                                        "KeyWord = @KeyWord," +
                                        "MaxInactiveHours = @MaxInactiveHours, " +
                                        "SendWay = @SendWay " +
                                        "WHERE ID = @Id";

            ulong unixTimeStamp = HelperClass.ConvertToUnixTime(DateTime.Now);

            Dictionary<string, object> args = new Dictionary<string, object>()
            {
                    {"@UnixTimeStamp", unixTimeStamp },
                    {"@Name", contact.Name},
                    {"@CompanyId", contact.CompanyId},
                    {"@Phone", contact.Phone},
                    {"@Email", contact.Email},
                    {"@KeyWord", contact.KeyWord},
                    {"@MaxInactiveHours", contact.MaxInactiveHours},
                    {"@SendWay", (uint)contact.ContactType},
                    {"@Id", contact.Id }
            };

            bool ok = (ExecuteWrite(queryUpdate, args) == 1);

            if (!ok) MainWindow.Log(MainWindow.Topic.SQL, MainWindow.Prio.Fehler, 2003241833, 
                "Der Kontakt mit dem Namen >" + contact.Name + "< konnte nicht in der Datenbank geändert werden.");

            return ok;
        }

        /// <summary>
        /// Löscht einen bestenhenden Kontakt in der Datenbank
        /// </summary>
        /// <param name="contact"></param>
        /// <returns>true = es wurde genau eine Zeile in der Datenbank gelöscht.</returns>
        internal bool DeleteContact(Contact contact)
        {
            string query = "DELETE FROM \"Contact\" WHERE ID = @ID ;";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                { "@ID", contact.Id }
            };

            bool ok = (ExecuteWrite(query, args) == 1);

            if (!ok) MainWindow.Log(MainWindow.Topic.SQL, MainWindow.Prio.Fehler, 2003241835,
                "Der Kontakt mit dem Namen >" + contact.Name + "< konnte nicht aus der Datenbank gelöscht werden.");

            return ok;
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

            int affectedRows = ExecuteWrite(query, args);

            if (affectedRows == 0) 
                MainWindow.Log(MainWindow.Topic.SQL, MainWindow.Prio.Fehler, 2003221749, 
                    "Für Contact-ID [" + contactId + "] konnt keine neue Bereitschaft in die Datenbank eingetragen werden.");

            return GetLastId("Shifts", "EntryTime = " + entryTime);
        }

        /// <summary>
        /// Standard-Schicht, die erstellt wird, wenn kein Eintrag für den aktuellen Tag gefunden wurde.
        /// </summary>
        /// <param name="contact">Kontakt des Bereitschaftnehmers, wie er in der Datenbank abgelegt ist.</param>
        /// <returns>ID der neu erstellten Schicht.</returns>
        internal uint CreateShiftDefault(Contact contact)
        {
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
            MainWindow.Log(MainWindow.Topic.Calendar, MainWindow.Prio.Info, 987654367, 
                "Erstelle automatische Bereitschaft von " + StartTime.ToString("dd.MM.yyyy HH:mm") + " bis " + EndTime.ToString("dd.MM.yyyy HH:mm"));

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

            int affectedRows = ExecuteWrite(query, args);

            if (affectedRows == 0)
                MainWindow.Log(MainWindow.Topic.SQL, MainWindow.Prio.Fehler, 2003221754,
                    "Die Bereitschaft [" + shiftId + "] konnte in der Datenbank nicht verändert werden.");
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

            int affectedRows = ExecuteWrite(query, args);

            if (affectedRows == 0)
                MainWindow.Log(MainWindow.Topic.SQL, MainWindow.Prio.Fehler, 2003221757,
                    "Die Bereitschaft [" + shiftId + "] konnte in der Datenbank nicht verändert werden.");
        }

        /// <summary>
        /// Entfert eine Schicht aus dem Bereitschaftsplan.
        /// </summary>
        /// <param name="shiftId"></param>
        /// <returns></returns>
        internal void DeleteShift(uint shiftId)
        {
            //nur löschen, wenn mindestens eine Schicht vorhanden
            string query = "DELETE FROM \"Shifts\" WHERE ID > 1 AND ID = @ID;";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                { "@ID", shiftId }
            };

            int affectedRows = ExecuteWrite(query, args);

            if (affectedRows == 0)
                MainWindow.Log(MainWindow.Topic.SQL, MainWindow.Prio.Fehler, 2003221800,
                    "Die Bereitschaft [" + shiftId + "] konnt in der Datenbank nicht gelöscht werden.");
        }


        #endregion
    }
}
