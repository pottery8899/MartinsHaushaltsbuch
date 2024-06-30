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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

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
            Loaded += NeuerEintragWindow_Loaded;
            Accounts = new ObservableCollection<AccountViewModel>();
            DataContext = this; // Setze den DataContext hier
            SetCorrectButtonStatus();
        }


        //---------------------- Übersicht über die Konten ----------------------

        public ObservableCollection<AccountViewModel> Accounts { get; set; }


        private void NeuerEintragWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAccounts(); // Lade Accounts aus der Datenbank oder anderer Quelle
            DataContext = this; // DataContext auf das aktuelle Fenster setzen, um Accounts zu binden
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
            Accounts.Clear(); // Sicherstellen, dass die Sammlung vor dem Hinzufügen neuer Elemente geleert wird

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT name_Konto, gesamtsumme_Konto FROM Konto", conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string accountName = reader["name_Konto"].ToString();
                        double totalAmount = Convert.ToDouble(reader["gesamtsumme_Konto"]);

                        Accounts.Add(new AccountViewModel { AccountName = accountName, TotalAmount = totalAmount });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Laden der Konten: " + ex.Message);
                }
            }
        }


        public class AccountViewModel : INotifyPropertyChanged
        {
            private string _accountName;
            public string AccountName
            {
                get { return _accountName; }
                set
                {
                    if (_accountName != value)
                    {
                        _accountName = value;
                        OnPropertyChanged(nameof(AccountName));
                    }
                }
            }

            private double _totalAmount;
            public double TotalAmount
            {
                get { return _totalAmount; }
                set
                {
                    if (_totalAmount != value)
                    {
                        _totalAmount = value;
                        OnPropertyChanged(nameof(TotalAmount));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
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
                    SQLiteCommand cmd = new SQLiteCommand("SELECT name_Konto FROM Konto", conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        incomingAccounts.Add(reader["name_Konto"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Laden der Eingangskonten: " + ex.Message);
                }
            }
            CmbIncomingAccount.ItemsSource = incomingAccounts;
            CmbAccount.ItemsSource = incomingAccounts;
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
                    MessageBox.Show("Fehler beim Laden der Kategorien: " + ex.Message);
                }
            }
            CmbCategory.ItemsSource = categories;
        }

        //---------------------- Laden der Einträge links im Listview ----------------------
        private void LoadEntries()
        {
            ObservableCollection<Entry> entries = new ObservableCollection<Entry>();
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

                    entries = new ObservableCollection<Entry>(entries.OrderByDescending(e => e.Datum_Buchung));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Laden der Einträge: " + ex.Message);
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
                if (string.IsNullOrWhiteSpace(TxtTitle.Text) ||
                    (CmbAccount.SelectedIndex == -1 && BtnToggleTransactionType.Content.ToString() != "Umbuchung") ||
                    (CmbAccount.SelectedIndex == -1 && string.IsNullOrWhiteSpace(CmbIncomingAccount.Text) && BtnToggleTransactionType.Content.ToString() == "Umbuchung") ||
                    CmbCategory.SelectedIndex == -1 ||
                    string.IsNullOrWhiteSpace(TxtAmount.Text) ||
                    DpDate.SelectedDate == null ||
                    string.IsNullOrWhiteSpace(TxtComment.Text))
                {
                    MessageBox.Show("Bitte füllen Sie alle Felder aus.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    DateTime selectedDate = DpDate.SelectedDate ?? DateTime.Now;

                    if (selectedDate.Date > DateTime.Today)
                    {
                        MessageBox.Show("Das ausgewählte Datum kann nicht in der Zukunft liegen.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (LstEntries.SelectedItem != null)
                    {
                        Entry selectedEntry = (Entry)LstEntries.SelectedItem;

                        string deleteQuery = "DELETE FROM Buchung WHERE Titel_Buchung = @Titel";
                        SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, connection);
                        deleteCommand.Parameters.AddWithValue("@Titel", selectedEntry.Titel_Buchung);
                        deleteCommand.ExecuteNonQuery();

                        UpdateAccountTotal(connection, selectedEntry.Konto_Buchung, -selectedEntry.Betrag_Buchung);
                    }

                    double transactionAmount = Convert.ToDouble(TxtAmount.Text);
                    string fromAccount = CmbAccount.SelectedItem.ToString();
                    string toAccount = BtnToggleTransactionType.Content.ToString() == "Umbuchung" ? CmbIncomingAccount.Text : fromAccount;

                    double fromAmount;
                    double toAmount;

                    if (BtnToggleTransactionType.Content.ToString() == "Eingang")
                    {
                        fromAmount = transactionAmount;
                        toAmount = 0;
                    }
                    else if (BtnToggleTransactionType.Content.ToString() == "Ausgang")
                    {
                        fromAmount = -transactionAmount;
                        toAmount = 0;
                    }
                    else
                    {
                        fromAmount = -transactionAmount;
                        toAmount = transactionAmount;
                    }

                    string insertQueryFrom = @"INSERT INTO Buchung (Titel_Buchung, Konto_Buchung, Kategorie_Buchung, Betrag_Buchung, Datum_Buchung, Kommentar_Buchung) 
                                       VALUES (@Titel, @Konto, @Kategorie, @Betrag, @Datum, @Kommentar)";
                    using (SQLiteCommand commandFrom = new SQLiteCommand(insertQueryFrom, connection))
                    {
                        commandFrom.Parameters.AddWithValue("@Titel", TxtTitle.Text);
                        commandFrom.Parameters.AddWithValue("@Konto", fromAccount);
                        commandFrom.Parameters.AddWithValue("@Kategorie", CmbCategory.SelectedItem);
                        commandFrom.Parameters.AddWithValue("@Betrag", fromAmount);
                        commandFrom.Parameters.AddWithValue("@Datum", selectedDate);
                        commandFrom.Parameters.AddWithValue("@Kommentar", TxtComment.Text);
                        commandFrom.ExecuteNonQuery();
                    }

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
                            commandTo.Parameters.AddWithValue("@Datum", selectedDate);
                            commandTo.Parameters.AddWithValue("@Kommentar", TxtComment.Text);
                            commandTo.ExecuteNonQuery();
                        }
                    }

                    UpdateAccountTotal(connection, fromAccount, fromAmount);
                    if (BtnToggleTransactionType.Content.ToString() == "Umbuchung")
                        UpdateAccountTotal(connection, toAccount, toAmount);

                    MessageBox.Show("Eintrag erfolgreich gespeichert!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Aktualisieren der ListBox
                    LoadEntries();
                    SetCorrectButtonStatus();
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
            string selectTotalQuery = "SELECT gesamtsumme_Konto FROM Konto WHERE name_Konto = @Konto";
            SQLiteCommand selectTotalCommand = new SQLiteCommand(selectTotalQuery, connection);
            selectTotalCommand.Parameters.AddWithValue("@Konto", accountName);
            double currentTotal = Convert.ToDouble(selectTotalCommand.ExecuteScalar());

            currentTotal += transactionAmount;

            string updateTotalQuery = "UPDATE Konto SET gesamtsumme_Konto = @Gesamtsumme WHERE name_Konto = @Konto";
            SQLiteCommand updateTotalCommand = new SQLiteCommand(updateTotalQuery, connection);
            updateTotalCommand.Parameters.AddWithValue("@Gesamtsumme", currentTotal);
            updateTotalCommand.Parameters.AddWithValue("@Konto", accountName);
            updateTotalCommand.ExecuteNonQuery();

            // Aktualisiere die ObservableCollection Accounts
            var accountToUpdate = Accounts.FirstOrDefault(acc => acc.AccountName == accountName);
            if (accountToUpdate != null)
            {
                accountToUpdate.TotalAmount = currentTotal;
            }
        }

        //---------------------- Bereinigen der Felder im Formular ----------------------
        private void ClearInputFields()
        {
            TxtTitle.Clear();
            CmbAccount.SelectedIndex = -1;
            CmbIncomingAccount.SelectedIndex = -1;
            CmbCategory.SelectedIndex = -1;
            TxtAmount.Clear();
            DpDate.SelectedDate = DateTime.Now;
            TxtComment.Clear();
            BtnToggleTransactionType.Content = "Ausgang";
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

        // ---------------------- Button Löschen ----------------------
        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (LstEntries.SelectedItem == null)
            {
                MessageBox.Show("Bitte wählen Sie einen Eintrag zum Löschen aus.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Sind Sie sicher, dass Sie diesen Eintrag löschen möchten?", "Bestätigen Sie das Löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        Entry selectedEntry = (Entry)LstEntries.SelectedItem;

                        // Löschen der Buchung aus der Datenbank
                        string deleteQuery = "DELETE FROM Buchung WHERE Titel_Buchung = @Titel";
                        SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, connection);
                        deleteCommand.Parameters.AddWithValue("@Titel", selectedEntry.Titel_Buchung);
                        deleteCommand.ExecuteNonQuery();

                        // Bestimme den Transaction-Typ basierend auf dem Vorzeichen des Betrags
                        string transactionType = selectedEntry.Betrag_Buchung >= 0 ? "Eingang" : "Ausgang";

                        // Aktualisiere die Gesamtsumme des Kontos
                        UpdateAccountTotalDelete(connection, selectedEntry.Konto_Buchung, Math.Abs(selectedEntry.Betrag_Buchung), transactionType);

                        MessageBox.Show("Eintrag erfolgreich gelöscht.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Entferne den Eintrag aus der ObservableCollection
                        var entries = (ObservableCollection<Entry>)LstEntries.ItemsSource;
                        entries.Remove(selectedEntry);

                        // Lade die Einträge und Konten neu
                        LoadAccounts();
                        SetCorrectButtonStatus();
                        ClearInputFields();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Löschen des Eintrags: " + ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Aktualisierung der Account-Summe nach Löschung
        private void UpdateAccountTotalDelete(SQLiteConnection connection, string accountName, double amount, string transactionType)
        {
            try
            {
                // Bestimme den Operator basierend auf dem Transaction-Typ
                string operatorSign = transactionType == "Eingang" ? "-" : "+";

                string updateQuery = $"UPDATE Konto SET gesamtsumme_Konto = gesamtsumme_Konto {operatorSign} @Amount WHERE name_Konto = @AccountName";
                SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, connection);
                updateCommand.Parameters.AddWithValue("@Amount", Math.Abs(amount)); // Betrag muss positiv sein
                updateCommand.Parameters.AddWithValue("@AccountName", accountName);
                updateCommand.ExecuteNonQuery();

                // Aktualisiere die ObservableCollection Accounts
                var accountToUpdate = Accounts.FirstOrDefault(acc => acc.AccountName == accountName);
                if (accountToUpdate != null)
                {
                    accountToUpdate.TotalAmount -= amount; // Subtrahiere den Betrag vom aktuellen Total
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Aktualisieren der Gesamtsumme des Kontos: " + ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
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

        //---------------------- Setzen des richtigen Buttonstatus OnStart ------------------------------------------
        private void SetCorrectButtonStatus()
        {
            // Setze den initialen Zustand des Buttons beim Laden der Seite
            BtnToggleTransactionType.Content = "Eingang"; // Setze den Text auf "Eingang"
            BtnToggleTransactionType.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A5D6A7")); // Setze die Hintergrundfarbe entsprechend

            // Führe die Anpassungen für den initialen Zustand aus
            LblAccount.Text = "Konto der Buchung";
            CmbAccount.Visibility = Visibility.Visible;
            LblIncomingAccount.Visibility = Visibility.Collapsed;
            CmbIncomingAccount.Visibility = Visibility.Collapsed;
        }
    }
}