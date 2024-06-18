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
    public partial class MainWindow : MetroWindow
    {
        //---------------------- Initialisierung der Seite und Integration des Singleton ----------------------
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Singleton_Filter.Instance;
        }

        //---------------------- Funktion für Navigation auf Seite "Buchungen" ----------------------
        private void Button_NewEntry_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Window_NewEntry();
            newForm.Show();
            this.Close();
        }

        //---------------------- Funktion für Navigation auf Seite "Einstellungen" ----------------------
        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Window_Settings();
            newForm.Show();
            this.Close();
        }

        //---------------------- Funktion für Navigation auf Seite "Analyse" ----------------------
        private void Button_Analysis_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Window_Analysis();
            newForm.Show();
            this.Close();
        }
    }
}