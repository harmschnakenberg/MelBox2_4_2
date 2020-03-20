using SerialPortListener.Serial;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace MelBox2_4
{
    /// <summary>
    /// Ändert die Farbe der Progressbar in Abhänigkeit der Signalstärke
    /// </summary>
    public class ProgressForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double progress = (double)value;
            Brush foreground = Brushes.Green;

            //Mögliche Werte: 2 - 9 marginal, 10 - 14 OK, 15 - 19 Good, 20 - 30 Excellent, 99 = kein Signal

            if (progress < 3 || progress >= 30)
            {
                foreground = Brushes.Red;
            }
            else if (progress < 9)
            {
                foreground = Brushes.DarkOrange;
            }
            else if (progress < 15)
            {
                foreground = Brushes.YellowGreen;
            }
            else if (progress < 20)
            {
                foreground = Brushes.Green;
            }
            else if (progress < 30)
            {
                foreground = Brushes.DarkGreen;
            }

            return foreground;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public partial class MainWindow : Window
    {

        private void Gsm_TextBox_SerialPortResponse_TextChanged(object sender, TextChangedEventArgs e)
        {
            Gsm_ScrollViewer_SerialPortResponse.ScrollToEnd();
        }

        private void Gsm_Button_GetSignalQuality_Click(object sender, RoutedEventArgs e)
        {
            GetGsmSignalQuality(sender, e);
        }

        /// <summary>
        /// Triggert die Abfrage der momentanen Netzqualität mit ShowGsmSignalQuality(...)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void GetGsmSignalQuality(object sender, EventArgs e)
        {
            //Ereignis abbonieren
            _spManager.NewSerialDataRecieved += ShowGsmSignalQuality;

            //Signalqualität abfragen            
            PortComandExe("AT+CSQ");
        }

        /// <summary>
        /// Ermittelt die momentane Netzqualität für das GSM-Netz und schreibt den Wert in eine ProgressBar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Antwort aus GSM-Modem</param>
        private void ShowGsmSignalQuality(object sender, SerialDataEventArgs e)
        {
            string portResponse = Encoding.ASCII.GetString(e.Data);

            //Antwort OK?
            if (portResponse.Contains("CSQ:"))
            {
                try
                {
                    //Finde Zahlenwerte
                    Regex rS = new Regex(@"CSQ:\s[0-9]+,");
                    Match m = rS.Match(portResponse);
                    if (!m.Success)
                    {
                        MessageBox.Show("Antwort von GSM-Modem auf Signalqualität konnte nicht verarbeitet werden:\r\n" + portResponse);
                        return;
                    }

                    //Mögliche Werte: 2 - 9 marginal, 10 - 14 OK, 15 - 19 Good, 20 - 30 Excellent, 99 = kein Signal
                    int.TryParse(m.Value.Remove(0, 4).Trim(','), out int signalQuality);

                    this.Gsm_TextBox_SerialPortResponse.Dispatcher.Invoke(DispatcherPriority.Normal,
                        new Action(() => { this.Gsm_ProgressBar_SignalQuality.Value = signalQuality; }));

                    //TODO: Wenn signalQuality < 10 Nachricht an Admin
                    if (signalQuality < 10)                    
                        Messages.Create_SignalQuality(signalQuality);                    
                }
                finally
                {
                    //Ereignisabbonement kündigen.
                    _spManager.NewSerialDataRecieved -= ShowGsmSignalQuality;
                }
            }
        }


        private void Gsm_Button_ReadSmsFromId_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(Gsm_TextBox_ReadSmsId.Text, out int smsId)) return;

            PortComandExe("AT+CMGR=" + smsId);
        }

        private void Gsm_Button_ReadAllSms_Click(object sender, RoutedEventArgs e)
        {
            PortComandExe("AT+CMGL=\"ALL\"");
        }

        private void Gsm_Button_SubscribeSms_Click(object sender, RoutedEventArgs e)
        {
            //Setzte Textmodus in GSM-Modem
            PortComandExe("AT+CMGF=1");
            System.Threading.Thread.Sleep(500);

            //Setzte Speicherbereich im GSM-Modem "SM" SIM, "PM" Phone-Memory, "MT" + "SM" + "PM"
            PortComandExe("AT+CPMS=\"MT\"");
            System.Threading.Thread.Sleep(500);

            //TODO: funktioniert nur sporadisch - warum?
            //Aktiviere Benachrichtigung von GSM-Modem, wenn neue SMS ankommt 
            PortComandExe("AT+CNMI=3,1,1,2,1");
        }

        private void Gsm_Button_SendSms_Click(object sender, RoutedEventArgs e)
        {
            string phoneStr = Gsm_TextBox_SendSmsTo.Text;
            ulong phone = HelperClass.ConvertStringToPhonenumber(phoneStr);
            string content = Gsm_TextBox_SendSmsContent.Text;
            
            if (phone == 0)
            {
                MessageBox.Show("Die Telefonnumer ist ungültig:\r\n" + phoneStr);
                return;
            }

            if (content.Length < 5)
            {
                MessageBox.Show("Die zu sendene Nachricht muss mindestens 5 Zeichen haben.");
                return;
            }

            string ctrlz = "\u001a";

           PortComandExe("AT+CSCS=\"GSM\"");
           System.Threading.Thread.Sleep(500);
           PortComandExe("AT+CMGS=\"+" + phone + "\"\r" + content + ctrlz);

        }

    }

}

