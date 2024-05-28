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
using MahApps.Metro.Controls;
using System.Data.SqlClient;

namespace MartinsHaushaltsbuch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class Window_NewEntry : MetroWindow
    {
        string ConnectionString = "Data Source = (localdb)\\MSSQLLocalDB;";
        public Window_NewEntry()
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

        private void Button_Analysis_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Window_Analysis();             //create your new form.
            newForm.Show();                             //show the new form.
            this.Close();                               //only if you want to close the current form.
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    // SQL-Befehl zum Einfügen eines neuen Eintrags
                    string query = @"INSERT INTO TabelleBuchung (Titel_Buchung, Konto_Buchung, Kategorie_Buchung, Betrag_Buchung, Datum_Buchung, Kommentar_Buchung) 
                                     VALUES (@Titel, @Konto, @Kategorie, @Betrag, @Datum, @Kommentar)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Parameterwerte setzen
                        command.Parameters.AddWithValue("@Titel", TxtTitle.Text);
                        command.Parameters.AddWithValue("@Konto", CmbAccount.SelectedItem);
                        command.Parameters.AddWithValue("@Kategorie", CmbCategory.SelectedItem);
                        command.Parameters.AddWithValue("@Betrag", Convert.ToDouble(TxtAmount.Text));
                        command.Parameters.AddWithValue("@Datum", DpDate.SelectedDate);
                        command.Parameters.AddWithValue("@Kommentar", TxtComment.Text);

                        // Befehl ausführen
                        command.ExecuteNonQuery();

                        MessageBox.Show("Eintrag erfolgreich gespeichert!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern des Eintrags: " + ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}