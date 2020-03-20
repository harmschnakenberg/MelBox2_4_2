using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MelBox2_4
{
    class Master_Visual
    {
    }

    public partial class MainWindow : Window
    {

        private ObservableCollection<Contact> _Master_ContactCollection;

        public ObservableCollection<Contact> Master_ContactCollection
        {
            get { return _Master_ContactCollection; }
            set 
            { 
                _Master_ContactCollection = value;
                OnPropertyChanged();
            }
        }

    }
}
