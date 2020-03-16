using SerialPortListener.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

/*
 * NOTIZEN:
 * 1) Es soll eine möglichst Event-gesteuerter Ablauf realisiert werden.
 * 2) Senden und Empfangen sollte asynchron erfolgen
 * 3) Fehler beim senden und empfangen sollen sicher protokolliert werden. (Netzempfang, smtp-Erreichbarkeit,..)
 * 4) Die Datenbank soll keine wiederholten Datensätze speichern (Speicherplatz)
 * 5) Telefonanrufe umsetzen (WIE?) Rufumleitung **61*RUFNUMMER*11*05#    
 * 6) Bei Neustart Meldung an einstellbaren Empfänger
 * 7) HeartBeat-Meldung Uhrzeit einstellbar
 * 8) Test "SMSAbruf" einrichten
 * 9) Mailverteiler Variabel gestalten
 * 10) Empfangsberietschaft Bereitschaftshandy feststellen und ggf. an jmd. weiterleiten.
 * 11) Datenbanksicherung
 */


namespace MelBox2_4
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
     
        #endregion

        #region Properties

        #endregion

        #region Methods
        public MainWindow()
        {
            InitializeComponent();

            InitializeSerialPort();
          
            _spManager.StartListening();

            StartSignalQualityCheckTimer();

        }

        /// <summary>
        /// Beim Schließen des Fensters, getriggert von XAML <Window ... Closing="Window_Closing"> 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _spManager.Dispose();
        }







        #endregion


    }
}
