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
        RecievedFromUnknown = 1,    //Empfang von unbekannt
        SentToUnknown = 2,          //Senden an Unbekannt
        RecievedFromSms = 4,        //Empfang von SMS
        SentToSms = 8,              //Senden an SMS
        RecievedFromEmail = 16,     //Empgang von Email
        SentToEmail = 32,           //Senden an Email  
        SentToEmailAndSMS = 40      //Senden an Email und SMS    
    }

    class DataClass
    {
    }
}
