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
            LoadAccountsForFiltering();
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
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Überprüfe, ob ein Eintrag bearbeitet wird
                    if (LstEntries.SelectedItem != null)
                    {
                        Entry selectedEntry = (Entry)LstEntries.SelectedItem;

                        // Lösche den alten Eintrag
                        string deleteQuery = "DELETE FROM TabelleBuchung WHERE Titel_Buchung = @Titel";
                        SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                        deleteCommand.Parameters.AddWithValue("@Titel", selectedEntry.Titel_Buchung);
                        deleteCommand.ExecuteNonQuery();
                    }

                    // Füge den neuen Eintrag hinzu
                    string insertQuery = @"INSERT INTO TabelleBuchung (Titel_Buchung, Konto_Buchung, Kategorie_Buchung, Betrag_Buchung, Datum_Buchung, Kommentar_Buchung) 
                                 VALUES (@Titel, @Konto, @Kategorie, @Betrag, @Datum, @Kommentar)";
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Titel", TxtTitle.Text);
                        command.Parameters.AddWithValue("@Konto", CmbAccount.SelectedItem);
                        command.Parameters.AddWithValue("@Kategorie", CmbCategory.SelectedItem);
                        command.Parameters.AddWithValue("@Betrag", Convert.ToDouble(TxtAmount.Text));
                        command.Parameters.AddWithValue("@Datum", DpDate.SelectedDate);
                        command.Parameters.AddWithValue("@Kommentar", TxtComment.Text);
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Eintrag erfolgreich gespeichert!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Lade die Liste der Einträge neu
                    LoadEntries();
                    ClearInputFields();
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

//----------------------------------------------------------- Buttons Edit, Clear und Löschen --------------------------------------------------------------------------

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            // Überprüfe, ob ein Eintrag ausgewählt wurde
            if (LstEntries.SelectedItem != null)
            {
                // Lade die ausgewählten Daten in das Formular
                Entry selectedEntry = (Entry)LstEntries.SelectedItem;
                TxtTitle.Text = selectedEntry.Titel_Buchung;
                // Setze die ausgewählte Option in der ComboBox
                CmbAccount.SelectedItem = selectedEntry.Konto_Buchung;
                CmbCategory.SelectedItem = selectedEntry.Kategorie_Buchung;
                TxtAmount.Text = selectedEntry.Betrag_Buchung.ToString();
                DpDate.SelectedDate = selectedEntry.Datum_Buchung;
                TxtComment.Text = selectedEntry.Kommentar_Buchung;
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie einen Eintrag aus der Liste aus, den Sie bearbeiten möchten.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            // Überprüfe, ob ein Eintrag ausgewählt wurde
            if (LstEntries.SelectedItem != null)
            {
                // Frage den Benutzer, ob er den ausgewählten Eintrag wirklich löschen möchte
                MessageBoxResult result = MessageBox.Show("Möchten Sie den ausgewählten Eintrag wirklich löschen?", "Bestätigung", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Lösche den ausgewählten Eintrag aus der Datenbank
                    Entry selectedEntry = (Entry)LstEntries.SelectedItem;
                    string deleteQuery = "DELETE FROM TabelleBuchung WHERE Titel_Buchung = @Titel";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                            deleteCommand.Parameters.AddWithValue("@Titel", selectedEntry.Titel_Buchung);
                            deleteCommand.ExecuteNonQuery();
                            MessageBox.Show("Eintrag erfolgreich gelöscht.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                            // Lade die Liste der Einträge neu
                            LoadEntries();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Fehler beim Löschen des Eintrags: " + ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie einen Eintrag aus der Liste aus, den Sie löschen möchten.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            ClearInputFields();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------

        private void LoadAccountsForFiltering()
        {
            List<string> accounts = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
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