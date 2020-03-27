using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
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
            get => _Master_ContactCollection;
            set
            {
                _Master_ContactCollection = value;
                NotifyStaticPropertyChanged();
            }
        }

        #region Methoden

        private void Master_TabItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Master_ListBox_ContactCollection.SelectedIndex == -1)  
                Master_ListBox_ContactCollection.SelectedIndex = Master_ListBox_ContactCollection.Items.Count - 1;

                Master_ComboBox_Companies.ItemsSource = Sql.GetListOfCompanies("1=1 ORDER BY Name");
        }

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

            uint.TryParse(Master_TextBox_Company_ZipCode.Text, out uint zipCode);
            string companyName = Master_TextBox_Company_Name.Text;
            string companyAddress = Master_TextBox_Company_Address.Text;
            string companyCity = Master_TextBox_Company_City.Text;

            MessageBoxResult r = MessageBox.Show("Wirklich einen neuen Firmeneintrag erstellen?\r\n" +
                "\r\nFirma:\t\t" + companyName +
                "\r\nAdressse:\t" + companyAddress +
                "\r\nin:\t\t" + zipCode + " " + companyCity,
                "MelBox2 - Neue Firmenadresse anlegen?", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (r != MessageBoxResult.Yes) return;

            uint lastId = sql.CreateCompany(companyName, companyAddress, zipCode, companyCity);

            Contact currentContact = (Contact)Master_ListBox_ContactCollection.SelectedItem;

            currentContact.CompanyId = lastId;

            Master_ComboBox_Companies.ItemsSource = sql.GetListOfCompanies("1=1 ORDER BY Name");
            Master_ComboBox_Companies.SelectedValue = companyName;
        }

        private void Master_Button_UpdateCompany_Click(object sender, RoutedEventArgs e)
        {
            uint.TryParse(Master_TextBox_Contact_CompanyId.Text, out uint companyId);
            uint.TryParse(Master_TextBox_Company_ZipCode.Text, out uint zipCode);
            string companyName = Master_TextBox_Company_Name.Text;
            string companyAddress = Master_TextBox_Company_Address.Text;
            string companyCity = Master_TextBox_Company_City.Text;
   
            string question = "Diese Firmenadresse ändern?";
                    
            MessageBoxResult r = MessageBox.Show(question +"\r\n" +
                "\r\nFirma[" + companyId + "]:\t\t" + companyName +
                "\r\nAdressse:\t" + companyAddress +
                "\r\nin:\t\t" + zipCode + " " + companyCity,
               "MelBox2 - " + question, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (r != MessageBoxResult.Yes) return;

            bool success = Sql.UpdateCompany(companyId, companyName, companyAddress, zipCode, companyCity);
           
            if (success)
            {
                Master_ComboBox_Companies.ItemsSource = sql.GetListOfCompanies("1=1 ORDER BY Name");
                if (Master_ComboBox_Companies.Items.Contains(companyName))
                    Master_ComboBox_Companies.SelectedValue = companyName;
                else
                    Master_ComboBox_Companies.SelectedIndex = 1;
            }
            else
            {
                _ = MessageBox.Show("Firmenadresse ändern für >" + companyName + "< konnte nicht umgesetzt werden.", "MelBox2 - Fehler Firmenadresse ändern", MessageBoxButton.OK, MessageBoxImage.Warning);
                Log(Topic.Contacts, Prio.Warnung, 2003261300, "Firmenadresse ändern konnte für >" + companyName + "< nicht umgesetzt werden.");
            }
  
        }

        private void Master_Button_DeleteCompany_Click(object sender, RoutedEventArgs e)
        {
            
            Contact currentContact = (Contact)Master_ListBox_ContactCollection.SelectedItem;

            MessageBoxResult r = MessageBox.Show("Wirklich diesen Firmeneintrag löschen?\r\n" +
                "\r\nFirma[" + currentContact.Company.Id + "]:\t\t" + currentContact.Company.Name +
                "\r\nAdressse:\t" + currentContact.Company.Address +
                "\r\nin:\t\t" + currentContact.Company.ZipCode + " " + currentContact.Company.City,
               "MelBox2 - Diese Firmenadresse löschen?", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (r != MessageBoxResult.Yes) return;

            if (!Sql.DeleteCompany(currentContact.Company.Id))
            {               
                _ = MessageBox.Show("Der Firmeneintrag für >" + currentContact.Company.Name + "< [" + currentContact.Company.Id + "] konnte nicht gelöscht werden.", "MelBox2 - Fehler Firmeneintrag löschen.", MessageBoxButton.OK, MessageBoxImage.Warning);
                Log(Topic.Contacts, Prio.Warnung, 2003262000, "Der Firmeneintrag für > " + currentContact.Company.Name + " < [" + currentContact.Company.Id + "] konnte nicht gelöscht werden.");
            }
            else
            {
                Master_ComboBox_Companies.ItemsSource = sql.GetListOfCompanies("1=1 ORDER BY Name");
                Master_ComboBox_Companies.SelectedIndex = 1;
            }
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
            Contact contact = new Contact()
            {
                Name = Contacts.UnknownName + DateTime.Now.Ticks,
                CompanyId = 1,
                EmailAddress = "xyz@abc.de"
            };

            Master_ContactCollection.Add(contact);
            Master_ListBox_ContactCollection.SelectedIndex = Master_ListBox_ContactCollection.Items.Count - 1;            
            Master_TextBox_Contact_Name.Focus();

            //Neuen Contact in der Datenbank erzeugen
            uint newId = Sql.CreateContact(contact);

            if (newId == 0)
            {
                MessageBox.Show("Es ist ein Fehler aufgetreten bei der Neuerstellung von\r\n" + contact.Name,
                    "MelBox2 - Fehler bei Benutzer erstellen", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Contact currentContact = (Contact)Master_ListBox_ContactCollection.SelectedItem;
            currentContact.Id = newId;
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
                //Hierhin sollte es niemals kommen.
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
            Master_ContactCollection = Sql.GetContacts("1=1 ORDER BY Name");
            Master_ListBox_ContactCollection.SelectedIndex = 0;
        }

        #endregion

        #endregion
    }
}
