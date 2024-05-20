using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MartinsHaushaltsbuch
{
    public partial class Window_Analysis : MetroWindow
    {
        public Window_Analysis()
        {
            InitializeComponent();
        }
        private void Button_MainPage_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new MainWindow();             //create your new form.
            newForm.Show();                             //show the new form.
            this.Close();                               //only if you want to close the current form.
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Window_Settings();             //create your new form.
            newForm.Show();                             //show the new form.
            this.Close();                               //only if you want to close the current form.
        }
        private void Button_NewEntry_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Window_NewEntry();        //create your new form.
            newForm.Show();                             //show the new form.
            this.Close();                               //only if you want to close the current form.
        }
    }
}
