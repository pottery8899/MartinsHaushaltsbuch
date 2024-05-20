using MahApps.Metro.Controls;
using ModernWpf.Controls;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MartinsHaushaltsbuch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_NewEntry_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Window_NewEntry();        //create your new form.
            newForm.Show();                             //show the new form.
            this.Close();                               //only if you want to close the current form.
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Window_Settings();             //create your new form.
            newForm.Show();                             //show the new form.
            this.Close();                               //only if you want to close the current form.
        }

        private void Button_Analysis_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Window_Analysis();             //create your new form.
            newForm.Show();                             //show the new form.
            this.Close();                               //only if you want to close the current form.
        }
    }
}