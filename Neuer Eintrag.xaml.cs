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
using System.Configuration;

namespace MartinsHaushaltsbuch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class Window_NewEntry : MetroWindow
    {
        public Window_NewEntry()
        {
            InitializeComponent();
        }

        private void Window_NewEntry_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAccounts();
            LoadCategories();
            LoadEntries();
            //LoadAccountsForFiltering();
        }

        string connectionString = ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString;

        private void LoadAccounts()
        {
            List<string> accounts = new List<string>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT name_Konto FROM Tabelle_Konto", conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        accounts.Add(reader["name_Konto"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading accounts: " + ex.Message);
                }
            }
            CmbAccount.ItemsSource = accounts;
        }

        private void LoadCategories()
        {
            List<string> categories = new List<string>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT nameKategorie FROM TabelleKategorie", conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        categories.Add(reader["nameKategorie"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading categories: " + ex.Message);
                }
            }
            CmbCategory.ItemsSource = categories;
        }

        private void LoadEntries()
        {
            List<Entry> entries = new List<Entry>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Titel_Buchung, Konto_Buchung, Kategorie_Buchung, Betrag_Buchung, Datum_Buchung, Kommentar_Buchung FROM TabelleBuchung", conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        entries.Add(new Entry
                        {
                            Titel_Buchung = reader["Titel_Buchung"].ToString(),
                            Konto_Buchung = reader["Konto_Buchung"].ToString(),
                            Kategorie_Buchung = reader["Kategorie_Buchung"].ToString(),
                            Betrag_Buchung = Convert.ToDouble(reader["Betrag_Buchung"]),
                            Datum_Buchung = Convert.ToDateTime(reader["Datum_Buchung"]),
                            Kommentar_Buchung = reader["Kommentar_Buchung"].ToString()
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading entries: " + ex.Message);
                }
            }
            LstEntries.ItemsSource = entries;
        }

        public class Entry
        {
            public string Titel_Buchung { get; set; }
            public string Konto_Buchung { get; set; }
            public string Kategorie_Buchung { get; set; }
            public double Betrag_Buchung { get; set; }
            public DateTime Datum_Buchung { get; set; }
            public string Kommentar_Buchung { get; set; }
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
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString))
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

                        LoadEntries();
                        ClearInputFields();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern des Eintrags: " + ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearInputFields()
        {
            TxtTitle.Text = string.Empty;
            CmbAccount.SelectedIndex = -1; // Setze die Auswahl im ComboBox zurück
            CmbCategory.SelectedIndex = -1; // Setze die Auswahl im ComboBox zurück
            TxtAmount.Text = string.Empty;
            DpDate.SelectedDate = null; // Setze das Datum zurück
            TxtComment.Text = string.Empty;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------

        private void LoadAccountsForFiltering()
        {
            List<string> accounts = new List<string>();
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT name_Konto FROM Tabelle_Konto", connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string accountName = reader["name_Konto"].ToString();
                        accounts.Add(accountName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Laden der Konten: " + ex.Message);
                }
            }
            // Füge "Alle Konten" als erste Option hinzu
            accounts.Insert(0, "Alle Konten");
            // Setze die Datenquelle der ComboBox auf die Liste der Konten
            CmbFilterAccount.ItemsSource = accounts;
        }


    }
}