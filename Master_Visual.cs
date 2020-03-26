using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MelBox2_4
{
    class Master_Visual
    {
    }

    public partial class MainWindow : Window
    {

        private static ObservableCollection<Contact> _Master_ContactCollection = new ObservableCollection<Contact>(); 

        public static ObservableCollection<Contact> Master_ContactCollection
        {
            get { return _Master_ContactCollection; }
            set 
            { 
                _Master_ContactCollection = value;
                NotifyStaticPropertyChanged();
            }
        }

        //private static ObservableCollection<Company> _Master_CompanyCollection = new ObservableCollection<Company>();

        //public static ObservableCollection<Company> Master_CompanyCollection
        //{
        //    get { return _Master_CompanyCollection; }
        //    set
        //    {
        //        _Master_CompanyCollection = value;
        //        NotifyStaticPropertyChanged();
        //    }
        //}


        #region Methoden

        private void Master_TabItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Master_ListBox_ContactCollection.SelectedIndex == -1)  
                Master_ListBox_ContactCollection.SelectedIndex = Master_ListBox_ContactCollection.Items.Count - 1;

                Master_ComboBox_Companies.ItemsSource = Sql.GetListOfCompanies();
        }




        //private void Mast_ComboBox_UnknownContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    string identifier = Contacts.UnknownName;

        //    if (Mast_ComboBox_UnknownContacts.SelectedIndex != -1)
        //    {
        //        identifier = Mast_ComboBox_UnknownContacts.SelectedValue.ToString();
        //    }

        //    string query = "SELECT Contact.ID, Contact.Name, CompanyID, Company.Name, Email, Phone, KeyWord, MaxInactiveHours, SendWay FROM Contact LEFT JOIN Company ON Contact.CompanyID = Company.ID WHERE Email =@email OR KeyWord = @keyword OR Phone = @phone";

        //    Dictionary<string, object> dictArgs = new Dictionary<string, object>
        //    {
        //        { "@email", identifier },
        //        { "@keyword", identifier },
        //        { "@phone", identifier }
        //    };

        //    Dictionary<string, object> dict = sql.GetRowValues(query, dictArgs);

        //    if (dict.Count == 0) return;

        //    if (!ushort.TryParse(dict["SendWay"].ToString(), out ushort roleId))
        //    {
        //        roleId = (ushort)MessageType.NoCategory;
        //    }

        //    Mast_TextBox_Name.Text = dict["Contact.Name"].ToString();
        //    Mast_ComboBox_Company.SelectedValue = dict["Company.Name"].ToString();
        //    Mast_TextBox_Email.Text = dict["Email"].ToString();
        //    Mast_TextBox_Cellphone.Text = "+" + dict["Cellphone"].ToString();
        //    Mast_TextBox_KeyWord.Text = dict["KeyWord"].ToString();
        //    Mast_TextBox_MaxInactivity.Text = dict["MaxInactive"].ToString();
        //    Mast_CheckBox_RecievesEmail.IsChecked = ((ushort)MessageType.SentToEmail & roleId) == (ushort) MessageType.SentToEmail;
        //    Mast_CheckBox_RecievesSMS.IsChecked = ((ushort)MessageType.SentToSms & roleId) == (ushort)MessageType.SentToSms;


        //    //FillTabMasterData(dict);
        //}

        //private void Mast_Button_SearchName_Click(object sender, RoutedEventArgs e)
        //{

        //}


        #region Company

        private void Master_ComboBox_Companies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Master_ComboBox_Companies.SelectedIndex == -1) return; 

            string companyName = Master_ComboBox_Companies.SelectedValue.ToString();
            int selectedIndex = Master_ListBox_ContactCollection.SelectedIndex;

            Company company = Sql.GetCompanyFromDb(companyName);

            Contact currentContact = (Contact)Master_ListBox_ContactCollection.SelectedItem;

            currentContact.CompanyId = company.Id;

            //Die Ansicht wird nicht automatisch aktualisiert, daher:
            Master_ListBox_ContactCollection.SelectedIndex = -1;
            Master_ListBox_ContactCollection.SelectedIndex = selectedIndex;
        }

        private void Master_Button_CreateCompany_Click(object sender, RoutedEventArgs e)
        {
            //TODO: FUnktioniert nicht: im XAML sind Company 
            MessageBoxResult r = MessageBox.Show("Wirklich neue Firmeninformation erstellen?", "MelBox2 - Neue Firmenadresse anlegen?", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (r != MessageBoxResult.Yes) return;

            string companyName = Master_TextBox_Company_Name.Text;
            string address = Master_TextBox_Company_Address.Text;

            if (!uint.TryParse(Master_TextBox_Company_ZipCode.Text, out uint zipCode))
            {
                zipCode = 0;
            }

            string city = Master_TextBox_Company_City.Text;

            uint affectedRows = sql.CreateCompany(companyName, address, zipCode, city);

            if (affectedRows > 0)
            {
                _ = MessageBox.Show("Neuer Firmeneintrag >" + companyName + "< wurde erstellt.", "MelBox2 - Firmenadresse neu erstelt.", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow.Log(MainWindow.Topic.Contacts, MainWindow.Prio.Info, 2003251812, "Neuer Eintrag Firma >" + companyName + "< in Tabelle Company");
            }

            Master_ComboBox_Companies.ItemsSource = sql.GetListOfCompanies();
        }




        #endregion

        #region Contacts

        /// <summary>
        /// Erzeugt nur eine neue Instanz von Contact, ohne in die Datenbank zu speichern.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Master_Button_CreateContact_Click(object sender, RoutedEventArgs e)
        {
            Master_ContactCollection.Add(new Contact());
            Master_ListBox_ContactCollection.SelectedIndex = Master_ListBox_ContactCollection.Items.Count - 1;
            //Master_TextBlock_Contact_Id.Text = "-nicht gespeichert-";
            Master_TextBox_Contact_Name.Focus();
        }

        /// <summary>
        /// Speichert den ausgewählten Contact in die Datenbank (erzeugen / ändern)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Master_Button_UpdateContact_Click(object sender, RoutedEventArgs e)
        {
            Contact currentContact = (Contact)Master_ListBox_ContactCollection.SelectedItem;

            if (currentContact == null) return;

            MessageBoxResult boxResult = MessageBox.Show("Wirklich den Benutzer " + currentContact.Name + " speichern?", "MelBox2 - Benutzer speichern", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (boxResult != MessageBoxResult.Yes) return;

            string companyName = Master_ComboBox_Companies.SelectedValue.ToString();

            if (companyName.Length > 3)
            {
                currentContact.CompanyId = Sql.GetCompanyFromDb(companyName).Id;
            }

            if (currentContact.Id == 0)
            {
                //Neuen Contact in der Datenbank erzeugen
                uint newId = Sql.CreateContact(currentContact);

                if (newId == 0)                
                    MessageBox.Show("Es ist ein Fehler aufgetreten bei der Neuerstellung von\r\n" + currentContact.Name,
                        "MelBox2 - Fehler bei Benutzer erstellen", MessageBoxButton.OK, MessageBoxImage.Error);
                
                currentContact.Id = newId;                
            }
            else
            {
                //Vorhandenen Contact in der Datenbank ändern
                if (!Sql.UpdateContact(currentContact))
                {
                    MessageBox.Show("Es ist ein Fehler aufgetreten bei der Änderung von " + currentContact.Name,
                        "MelBox2 - Fehler bei Benutzer ändern", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Master_Button_DeleteContact_Click(object sender, RoutedEventArgs e)
        {
            if (Master_ListBox_ContactCollection.Items.Count == 0) return;
            
            Contact currentContact = (Contact)Master_ListBox_ContactCollection.SelectedItem;

            if (currentContact == null) return;

            if (currentContact.Id < 1)
            {
                MessageBox.Show("Benutzer ohne ID (ID=0) können nicht gelöscht werden.",
                    "MelBox2 - Benutzer nicht löschbar", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (currentContact.Id < 4)
            {
                MessageBox.Show("Die Benutzer mit der ID 1,2,3 können nicht gelöscht werden.",
                    "MelBox2 - Benutzer nicht löschbar", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult boxResult = MessageBox.Show("Wirklich den Benutzer" + currentContact.Name + " löschen?", "MelBox2 - Benutzer löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (boxResult != MessageBoxResult.Yes) return;

            if (!Sql.DeleteContact(currentContact))
            {
                MessageBox.Show("Es ist ein Fehler aufgetreten beim Löschen von " + currentContact.Name,
                    "MelBox2 - Fehler bei Benutzer löschen", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                //Aus ListBox entfernen
                int index = Master_ListBox_ContactCollection.SelectedIndex;
                Master_ContactCollection.RemoveAt(index);

                if (index > 0) Master_ListBox_ContactCollection.SelectedIndex = index - 1;
                else Master_ListBox_ContactCollection.SelectedIndex = 0;
            }
        }

        private void Master_Button_ResetContacts_Click(object sender, RoutedEventArgs e)
        {
            //foreach (Contact contact in Sql.GetContacts())
            //{
            //    Master_ContactCollection.Add(contact);
            //}

            Master_ContactCollection = Sql.GetContacts();
            Master_ListBox_ContactCollection.SelectedIndex = 0;
        }

        #endregion

        #endregion
    }
}
