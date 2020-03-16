using SerialPortListener.Serial;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MelBox2_4
{
    public partial class MainWindow : Window
    {
        #region Fields
        SerialPortManager _spManager;

        private static readonly string TextLogPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Log", string.Format("Log{0:000}.txt", DateTime.Now.DayOfYear));
        #endregion

        /// <summary>
        /// Setzt die Eigenschaften für den seriellen Port, Aboniert das Mitschreiben der COM-Port Antworten
        /// </summary>
        private void InitializeSerialPort()
        {
            _spManager = new SerialPortManager();
            SerialSettings mySerialSettings = _spManager.CurrentSerialSettings;
            
            Gsm_ComboBox_PortName.ItemsSource = mySerialSettings.PortNameCollection;
            Gsm_ComboBox_PortName.SelectedItem = mySerialSettings.PortName;
            Gsm_ComboBox_BaudRate.ItemsSource = mySerialSettings.BaudRateCollection;
            Gsm_ComboBox_BaudRate.SelectedItem = mySerialSettings.BaudRate;
            Gsm_ComboBox_DataBits.ItemsSource = mySerialSettings.DataBitsCollection;
            Gsm_ComboBox_DataBits.SelectedItem = mySerialSettings.DataBits;
            Gsm_ComboBox_Parity.ItemsSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            Gsm_ComboBox_Parity.SelectedItem = System.IO.Ports.Parity.None;
            Gsm_ComboBox_StopBits.ItemsSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));
            Gsm_ComboBox_StopBits.SelectedItem = System.IO.Ports.StopBits.One;

            _spManager.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(GsmTrafficLogger);
          //  _spManager.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(CheckForIncomingSms);

        }

        /// <summary>
        /// Liest die vom COM-Port erhaltenden Daten und schreibt sie in eine TextBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Empfangene Daten</param>
        void GsmTrafficLogger(object sender, SerialDataEventArgs e)
        {
            //Quelle: https://stackoverflow.com/questions/4016921/wpf-invoke-a-control

            int maxTextLength = 1000; // maximum text length in text box
            string textBoxContent = string.Empty; //für Inhalt der TextBox

            this.Gsm_TextBox_SerialPortResponse.Dispatcher.Invoke(DispatcherPriority.Normal,
                    new Action(() => { textBoxContent = this.Gsm_TextBox_SerialPortResponse.Text; }));

            if (textBoxContent.Length > maxTextLength)
                textBoxContent = textBoxContent.Remove(0, textBoxContent.Length - maxTextLength);

            // This application is connected to a GPS sending ASCCI characters, so data is converted to text
            string str = Encoding.ASCII.GetString(e.Data);
            //Gsm_TextBox_SerialPortResponse.Text = text + str;

            this.Gsm_TextBox_SerialPortResponse.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() => { this.Gsm_TextBox_SerialPortResponse.Text = textBoxContent + str; }));

            //Antworten protokollieren
            using (System.IO.StreamWriter file = System.IO.File.AppendText(TextLogPath))
            {
                file.WriteLine("\r\n" + DateTime.Now.ToShortTimeString() +":\r\n" + str );
            }
            
        }

        void CheckForIncomingSms(object sender, SerialDataEventArgs e)
        {
            string str = Encoding.ASCII.GetString(e.Data);

            //Empfangshinweis für eingehende Nachricht ?
            if (str.Contains("+CMTI:"))
            {               
                string smsIdStr = System.Text.RegularExpressions.Regex.Match(str, @"\d+").Value;

                if (int.TryParse(smsIdStr, out int smsId))
                {
                    
                    PortComandExe("AT+CMGR=" + smsId);
                    MessageBox.Show(smsId.ToString());

                }
                else
                {
                    MessageBox.Show("2003160908 Die Id der SMS konnte nicht ausgelesen werden aus >" + str + "<");
                }

            }
        }



        /// <summary>
        /// Schreibt ein AT-Command an das Modem
        /// </summary>
        /// <param name="command"></param>
        private void PortComandExe(string command)
        {
            System.IO.Ports.SerialPort port = _spManager.SerialPort;
            //Port bereit?
            if (port == null)
            {
                MessageBox.Show("2003111425 serieller Port ist unbestimmt. Befehl an GSM-Modem wird abgebrochen.");                   
                return;
            }

            if (!SerialPort.GetPortNames().ToList().Contains(port.PortName))
            {
                MessageBox.Show("2003111551 Der Port >" + port.PortName + " < existiert nicht.");
                return;
            }

            if ( !port.IsOpen)
            {
                //MessageBox.Show("2003110953 Port >" + port.PortName + "< ist nicht offen. Versuche zu öffnen");

                _spManager.SerialPort.Open();
                System.Threading.Thread.Sleep(500);
            }

            port.DiscardOutBuffer();
            port.DiscardInBuffer();

            try
            {
                _spManager.SerialPort.Write(command + "\r");
                //MessageBox.Show("PortComandExe(" + command + ")");
            }
            catch (System.IO.IOException ex_io)
            {
                MessageBox.Show("2003110956 Konnte Befehl nicht an COM-Port senden.\r\n" + ex_io.Message);
                return;
            }

        }


    }
}
