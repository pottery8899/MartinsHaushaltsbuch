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
using System.Data.SQLite;
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

        //---------------------- Aufruf der relvanten Funktionen bei Laden der Seite ----------------------

        private void Window_NewEntry_Loaded(object sender, RoutedEventArgs e)
        {
            //Singleton_Filter.Instance.Konto = "Alle Konten"; // Setzen des Standardwerts
            LoadAccounts();
            LoadIncomingAccounts();
            LoadCategories();
            LoadAccountsForFiltering();
            LoadEntries();
            LoadFilteredEntries();
        }

        //---------------------- lokales Festlegen des Connectionstrings in Window_NewEntry für alle enthaltenen Funktionen ----------------------
        string connectionString = ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString;

        
        //---------------------- Laden der Konten für Formular ----------------------
        private void LoadAccounts()
        {
            List<string> accounts = new List<string>();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {                    
                    conn.Open();                    
                    SQLiteCommand cmd = new SQLiteCommand("SELECT name_Konto FROM Konto", conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();
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

        //---------------------- Laden der Eingangskonten für Formular ----------------------
        private void LoadIncomingAccounts()
        {
            List<string> incomingAccounts = new List<string>();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT name_Konto FROM Konto", conn); // Hier sollte eine Abfrage für Eingangskonten stehen, falls diese separat gespeichert sind
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        incomingAccounts.Add(reader["name_Konto"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading incoming accounts: " + ex.Message);
                }
            }
            CmbIncomingAccount.ItemsSource = incomingAccounts;
        }


        //---------------------- Laden der Kategorien für Formular ----------------------
        private void LoadCategories()
        {
            List<string> categories = new List<string>();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT nameKategorie FROM Kategorie", conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();
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

        //---------------------- Laden der Einträge links im Listview ----------------------
        private void LoadEntries()
        {
            List<Entry> entries = new List<Entry>();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT Titel_Buchung, Konto_Buchung, Kategorie_Buchung, Betrag_Buchung, Datum_Buchung, Kommentar_Buchung FROM Buchung", conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();
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

                    // Sortiere die Einträge nach Datum absteigend
                    entries = entries.OrderByDescending(e => e.Datum_Buchung).ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading entries: " + ex.Message);
                }
            }
            LstEntries.ItemsSource = entries;
        }

        //---------------------- Öffentliche Klasse für die Einträge in der Listview ----------------------
        public class Entry
        {
            public string Titel_Buchung { get; set; }
            public string Konto_Buchung { get; set; }
            public string Kategorie_Buchung { get; set; }
            public double Betrag_Buchung { get; set; }
            public DateTime Datum_Buchung { get; set; }
            public string Kommentar_Buchung { get; set; }
        }

        //---------------------- Navigationsbutton Startseite ----------------------
        private void Button_MainPage_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new MainWindow();
            newForm.Show();
            this.Close();
        }

        //---------------------- Navigationsbutton Seite Einstellungen ----------------------
        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Window_Settings();
            newForm.Show();
            this.Close();
        }

        //---------------------- Navigationsbutton Seite Analyse ----------------------
        private void Button_Analysis_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Window_Analysis();
            newForm.Show();
            this.Close();
        }

        //---------------------- Button für das Speichern des neuen Eintrags oder der Änderungen ----------------------
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Überprüfung, ob alle Felder ausgefüllt sind
                if (string.IsNullOrWhiteSpace(TxtTitle.Text) ||
                    (CmbAccount.SelectedIndex == -1 && BtnToggleTransactionType.Content.ToString() != "Umbuchung") ||
                    (CmbAccount.SelectedIndex == -1 && string.IsNullOrWhiteSpace(CmbIncomingAccount.Text) && BtnToggleTransactionType.Content.ToString() == "Umbuchung") ||
                    CmbCategory.SelectedIndex == -1 ||
                    string.IsNullOrWhiteSpace(TxtAmount.Text) ||
                    DpDate.SelectedDate == null ||
                    string.IsNullOrWhiteSpace(TxtComment.Text))
                {
                    MessageBox.Show("Bitte füllen Sie alle Felder aus.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Beende die Methode, wenn ein Feld nicht ausgefüllt ist
                }

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    DateTime selectedDate = DpDate.SelectedDate ?? DateTime.Now; // Default to current date if no date is selected

                    // Überprüfen, ob das ausgewählte Datum in der Zukunft liegt
                    if (selectedDate.Date > DateTime.Today)
                    {
                        MessageBox.Show("Das ausgewählte Datum kann nicht in der Zukunft liegen.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        return; // Verhindere das Speichern der Buchung
                    }

                    // Überprüfe, ob ein Eintrag bearbeitet wird
                    if (LstEntries.SelectedItem != null)
                    {
                        Entry selectedEntry = (Entry)LstEntries.SelectedItem;

                        // Lösche den alten Eintrag
                        string deleteQuery = "DELETE FROM Buchung WHERE Titel_Buchung = @Titel";
                        SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, connection);
                        deleteCommand.Parameters.AddWithValue("@Titel", selectedEntry.Titel_Buchung);
                        deleteCommand.ExecuteNonQuery();
                    }

                    double transactionAmount = Convert.ToDouble(TxtAmount.Text);

                    // Bestimme die Konten basierend auf dem Button-Zustand
                    string fromAccount = CmbAccount.SelectedItem.ToString();
                    string toAccount = BtnToggleTransactionType.Content.ToString() == "Umbuchung" ? CmbIncomingAccount.Text : fromAccount;

                    // Berechne den Betrag für Ausgangskonto (negativ) und Eingangskonto (positiv)
                    double fromAmount = transactionAmount * -1;
                    double toAmount = transactionAmount;

                    // Füge den neuen Eintrag für das Ausgangskonto hinzu
                    string insertQueryFrom = @"INSERT INTO Buchung (Titel_Buchung, Konto_Buchung, Kategorie_Buchung, Betrag_Buchung, Datum_Buchung, Kommentar_Buchung) 
            VALUES (@Titel, @Konto, @Kategorie, @Betrag, @Datum, @Kommentar)";
                    using (SQLiteCommand commandFrom = new SQLiteCommand(insertQueryFrom, connection))
                    {
                        commandFrom.Parameters.AddWithValue("@Titel", TxtTitle.Text);
                        commandFrom.Parameters.AddWithValue("@Konto", fromAccount);
                        commandFrom.Parameters.AddWithValue("@Kategorie", CmbCategory.SelectedItem);
                        commandFrom.Parameters.AddWithValue("@Betrag", fromAmount);
                        commandFrom.Parameters.AddWithValue("@Datum", DpDate.SelectedDate);
                        commandFrom.Parameters.AddWithValue("@Kommentar", TxtComment.Text);
                        commandFrom.ExecuteNonQuery();
                    }

                    // Füge den neuen Eintrag für das Eingangskonto hinzu, falls es eine Umbuchung ist
                    if (BtnToggleTransactionType.Content.ToString() == "Umbuchung")
                    {
                        string insertQueryTo = @"INSERT INTO Buchung (Titel_Buchung, Konto_Buchung, Kategorie_Buchung, Betrag_Buchung, Datum_Buchung, Kommentar_Buchung) 
                VALUES (@Titel, @Konto, @Kategorie, @Betrag, @Datum, @Kommentar)";
                        using (SQLiteCommand commandTo = new SQLiteCommand(insertQueryTo, connection))
                        {
                            commandTo.Parameters.AddWithValue("@Titel", TxtTitle.Text);
                            commandTo.Parameters.AddWithValue("@Konto", toAccount);
                            commandTo.Parameters.AddWithValue("@Kategorie", CmbCategory.SelectedItem);
                            commandTo.Parameters.AddWithValue("@Betrag", toAmount);
                            commandTo.Parameters.AddWithValue("@Datum", DpDate.SelectedDate);
                            commandTo.Parameters.AddWithValue("@Kommentar", TxtComment.Text);
                            commandTo.ExecuteNonQuery();
                        }
                    }

                    // Aktualisieren der Gesamtsumme für das Ausgangskonto
                    UpdateAccountTotal(connection, fromAccount, fromAmount);

                    // Aktualisieren der Gesamtsumme für das Eingangskonto, falls es eine Umbuchung ist
                    if (BtnToggleTransactionType.Content.ToString() == "Umbuchung")
                    {
                        UpdateAccountTotal(connection, toAccount, toAmount);
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


        //---------------------- Updaten des Gesamtbetrags je Konto ----------------------
        private void UpdateAccountTotal(SQLiteConnection connection, string accountName, double transactionAmount)
        {
            // Abrufen der aktuellen Gesamtsumme des Kontos
            string selectTotalQuery = "SELECT gesamtsumme_Konto FROM Konto WHERE name_Konto = @Konto";
            SQLiteCommand selectTotalCommand = new SQLiteCommand(selectTotalQuery, connection);
            selectTotalCommand.Parameters.AddWithValue("@Konto", accountName);
            double currentTotal = Convert.ToDouble(selectTotalCommand.ExecuteScalar());

            // Hinzufügen oder Abziehen des Betrags zur Gesamtsumme des Kontos
            currentTotal += transactionAmount;

            // Aktualisieren der Gesamtsumme in der Tabelle Konto
            string updateTotalQuery = "UPDATE Konto SET gesamtsumme_Konto = @Gesamtsumme WHERE name_Konto = @Konto";
            SQLiteCommand updateTotalCommand = new SQLiteCommand(updateTotalQuery, connection);
            updateTotalCommand.Parameters.AddWithValue("@Gesamtsumme", currentTotal);
            updateTotalCommand.Parameters.AddWithValue("@Konto", accountName);
            updateTotalCommand.ExecuteNonQuery();
        }




        //---------------------- Bereinigen der Felder im Formular ----------------------
        private void ClearInputFields()
        {
            TxtTitle.Text = string.Empty;
            CmbAccount.SelectedIndex = -1; // Setze die Auswahl im ComboBox zurück
            CmbCategory.SelectedIndex = -1; // Setze die Auswahl im ComboBox zurück
            TxtAmount.Text = string.Empty;
            DpDate.SelectedDate = null; // Setze das Datum zurück
            TxtComment.Text = string.Empty;
        }

        //---------------------- Button Editieren ----------------------

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

        //---------------------- Button Löschen ----------------------
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
                    string deleteQuery = "DELETE FROM Buchung WHERE Titel_Buchung = @Titel";
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, connection);
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

        //---------------------- Button Zurücksetzen des Formulars ----------------------
        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            ClearInputFields();
        }

        //---------------------- Laden der Konten für das Filtern im Dropdown ----------------------
        private void LoadAccountsForFiltering()
        {
            List<string> accounts = new List<string>();
            CmbFilterAccount.Items.Add("Alle Konten");
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("SELECT name_Konto FROM Konto", connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string accountName = reader["name_Konto"].ToString();
                        accounts.Add(accountName);
                        CmbFilterAccount.Items.Add(accountName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Laden der Konten: " + ex.Message);
                }
            }
        }

        //---------------------- Funktion für Reaktion auf geändertes Element in Dropdown ------------------------------------------

        private void CmbFilterAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbFilterAccount.SelectedItem != null)
            {
                string selectedAccount = CmbFilterAccount.SelectedItem.ToString();
                Singleton_Filter.Instance.Konto = selectedAccount;

                // Lade die gefilterten Einträge basierend auf dem ausgewählten Konto
                LoadFilteredEntries();
            }
        }

        //---------------------- Funktion für Laden der Elemente in Listview nach Filtern in Dropdown ------------------------------------------

        private void LoadFilteredEntries()
        {
            List<Entry> entries = new List<Entry>();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Titel_Buchung, Konto_Buchung, Kategorie_Buchung, Betrag_Buchung, Datum_Buchung, Kommentar_Buchung FROM Buchung";
                    if (Singleton_Filter.Instance.Konto != "Alle Konten")
                    {
                        query += " WHERE Konto_Buchung = @Konto";
                    }

                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Konto", Singleton_Filter.Instance.Konto);

                    SQLiteDataReader reader = cmd.ExecuteReader();
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

                    // Sortiere die Einträge nach Datum absteigend
                    entries = entries.OrderByDescending(e => e.Datum_Buchung).ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading entries: " + ex.Message);
                }
            }
            LstEntries.ItemsSource = entries;
        }

        //---------------------- Funktion für den Button Ein- und Ausgang ------------------------------------------

        private void BtnToggleTransactionType_Click(object sender, RoutedEventArgs e)
        {
            if (BtnToggleTransactionType.Content.ToString() == "Eingang")
            {
                BtnToggleTransactionType.Content = "Ausgang";
                BtnToggleTransactionType.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCDD2"));

                // Anpassungen für Eingang -> Ausgang
                LblAccount.Text = "Ausgangskonto";
                CmbAccount.Visibility = Visibility.Visible;
                LblIncomingAccount.Visibility = Visibility.Collapsed; // Verstecke Eingangskonto Label
                CmbIncomingAccount.Visibility = Visibility.Collapsed; // Verstecke Eingangskonto ComboBox
            }
            else if (BtnToggleTransactionType.Content.ToString() == "Ausgang")
            {
                BtnToggleTransactionType.Content = "Umbuchung";
                BtnToggleTransactionType.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEB3B"));

                // Anpassungen für Ausgang -> Umbuchung
                LblAccount.Text = "Ausgangskonto";
                CmbAccount.Visibility = Visibility.Visible;
                LblIncomingAccount.Text = "Eingangskonto"; // Setze Eingangskonto Label sichtbar und Text
                LblIncomingAccount.Visibility = Visibility.Visible;
                CmbIncomingAccount.Visibility = Visibility.Visible; // Setze Eingangskonto ComboBox sichtbar

                // Lade die Eingangskonten in die ComboBox
                LoadIncomingAccounts();
            }
            else if (BtnToggleTransactionType.Content.ToString() == "Umbuchung")
            {
                BtnToggleTransactionType.Content = "Eingang";
                BtnToggleTransactionType.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A5D6A7"));

                // Anpassungen für Umbuchung -> Eingang
                LblAccount.Text = "Konto der Buchung";
                CmbAccount.Visibility = Visibility.Visible;
                LblIncomingAccount.Visibility = Visibility.Collapsed; // Verstecke Eingangskonto Label
                CmbIncomingAccount.Visibility = Visibility.Collapsed; // Verstecke Eingangskonto ComboBox
            }
        }
    }
}