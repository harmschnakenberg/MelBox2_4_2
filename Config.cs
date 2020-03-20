using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MelBox2_4
{
    class Config
    {
        private const string ConfigFileName = "Config.ini";

        /// <summary>
        /// Erstellt eine Konfig-INI mit Default-Werten.
        /// </summary>
        /// <param name="ConfigFileName">Name der Konfig-Datei</param>
        private static void CreateConfig(string ConfigFileName)
        {


            string configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConfigFileName);
            using (StreamWriter w = File.AppendText(configPath))
            {
                try
                {
                    w.WriteLine("[Allgemein]\r\n" +
                                ";DebugWord=0\r\n" +

                                "\r\n[Pfade]\r\n;" +

                                "\r\n[Zeiten]\r\n;" +
                                ";NightShiftStartHour=17" +
                                ";NightShiftStartHourFriday=15" +
                                ";NightShiftEndHour=7" +

                                "\r\n[Kontakte]\r\n;" +
                                ";GsmModemPhoneNumber=+4915142265412" +
                                ";"
                                );
                }
                catch (IOException ex)
                {
                    Console.WriteLine("FEHLER beim Erstellen von {0}. {1}", configPath, ex.ToString());
                }
            }
        }


        /// <summary>
        /// Lädt Werte aus der Konfig-INI.
        /// </summary>
        internal static void LoadConfig()
        {
            string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configPath = Path.Combine(appDir, ConfigFileName);

            if (!File.Exists(configPath))
            {
                CreateConfig(ConfigFileName);
            }
            else
            {
                string configAll = System.IO.File.ReadAllText(configPath);
                char[] delimiters = new char[] { '\r', '\n' };
                string[] configLines = configAll.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, string> dict = new Dictionary<string, string>();
                foreach (string line in configLines)
                {
                    if (line[0] != ';' && line[0] != '[')
                    {
                        string[] item = line.Split('=');
                        string val = item[1].Trim();
                        if (item.Length > 2) val += "=" + item[2].Trim();
                        dict.Add(item[0].Trim(), val);
                    }
                }

                //Dateipfade
                //string configVal = TagValueFromConfig(dict, "XlTemplateDayFilePath");
                //if (File.Exists(configVal))
                //    Excel.XlTemplateDayFilePath = configVal;

                //configVal = TagValueFromConfig(dict, "XlTemplateMonthFilePath");
                //if (File.Exists(configVal))
                //    Excel.XlTemplateMonthFilePath = configVal;

                ////Ordnerpfade
                //configVal = TagValueFromConfig(dict, "XlArchiveDir");
                //if (Directory.Exists(configVal))
                //    Excel.XlArchiveDir = configVal;

                //configVal = TagValueFromConfig(dict, "XlArchiveDir");
                //if (Directory.Exists(configVal))
                //    Excel.XlArchiveDir = configVal;

                //configVal = TagValueFromConfig(dict, "XmlDir");
                //if (Directory.Exists(configVal))
                //    Sql.XmlDir = configVal;

                //configVal = TagValueFromConfig(dict, "PdfConverterPath");
                //if (File.Exists(configVal))
                //    Pdf.PdfConverterPath = configVal;

                //configVal = TagValueFromConfig(dict, "PrintAppPath");
                //if (File.Exists(configVal))
                //    Print.PrintAppPath = configVal;

                ////Integer
                string configVal = TagValueFromConfig(dict, "NightShiftStartHour");
                if (int.TryParse(configVal, out int i))
                    HelperClass.NightShiftStartHour = (ushort)i;

                configVal = TagValueFromConfig(dict, "NightShiftStartHourFriday");
                if (int.TryParse(configVal, out i))
                    HelperClass.NightShiftStartHour = (ushort)i;

                configVal = TagValueFromConfig(dict, "NightShiftEndHour");
                if (int.TryParse(configVal, out i))
                    HelperClass.NightShiftStartHour = (ushort)i;

                ////String
                //configVal = TagValueFromConfig(dict, "MelBoxAdminPhone");
                //if (configVal != null)
                //    Program.XlLogFlag = dict["InTouchFlag"];
                //if (configVal.Length == 0)
                //    Program.XlLogFlag = string.Empty;


                //configVal = TagValueFromConfig(dict, "PdfConverterArgs");
                //if (configVal != null)
                //    Pdf.PdfConverterArgs = configVal;

                //configVal = TagValueFromConfig(dict, "PrintAppArgs");
                //if (configVal != null)
                //    Print.PrinterAppArgs = configVal;

                //configVal = TagValueFromConfig(dict, "PrintFileExtention");
                //if (configVal != null)
                //    Print.PrintFileExtention = configVal;

                configVal = TagValueFromConfig(dict, "GsmModemPhoneNumber");
                if (configVal != null)
                    Contacts.GsmModemPhoneNumber = HelperClass.ConvertStringToPhonenumber(configVal);
                    

            }
        }

        private static string TagValueFromConfig(Dictionary<string, string> dict, string TagName)
        {
            if (dict.TryGetValue(TagName, out string val))
            {
                return val;
            }
            else return null;
        }


    }
}
