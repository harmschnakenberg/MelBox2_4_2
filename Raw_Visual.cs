using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MelBox2_4
{
    public partial class MainWindow : Window
    {
        private void Raw_Combobox_Tabels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (int.TryParse(Raw_TextBox_MaxRows.Text, out int numRows))
            {
                Sql sql = new Sql();
                Raw_DataGrid_TableShow.DataContext = sql.GetLastEntries(e.AddedItems[0].ToString(), "ID", numRows).DefaultView;
            }
        }


    }
}
