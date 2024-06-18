using MahApps.Metro.Controls;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace MartinsHaushaltsbuch
{
    public partial class Window_Settings : MetroWindow
    {
        //---------------------- Initialisieren der Seite ----------------------
        public Window_Settings()
        {
            InitializeComponent();
        }

        //---------------------- Navigation auf Startseite ----------------------
        private void Button_MainPage_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new MainWindow();
            newForm.Show();
            this.Close();
        }

        //---------------------- Navigation auf Seite "Analyse" ----------------------
        private void Button_Analysis_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Window_Analysis();
            newForm.Show();
            this.Close();
        }

        //---------------------- Navigation auf Seite "Buchungen" ----------------------
        private void Button_NewEntry_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Window_NewEntry();
            newForm.Show();
            this.Close();
        }

        //---------------------- Funktion die hinter dem Speichern-Button des Kontos liegt, Speichert Informationen in DB ----------------------
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Überprüfen, ob Konto bereits existiert
                string checkQuery = "SELECT COUNT(*) FROM Tabelle_Konto WHERE name_Konto = @Name OR Kontonummer = @Kontonummer";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@Name", TxtName.Text);
                checkCommand.Parameters.AddWithValue("@Kontonummer", TxtKontonummer.Text);

                int count = (int)checkCommand.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Ein Konto mit diesem Namen oder dieser Kontonummer existiert bereits.");
                    return;
                }

                // Hier setzen wir gesamtsumme_Konto auf 0 direkt im INSERT-Statement, damit später keine Felder mit einem NULL-Eintrag in der DB entstehen
                //Außerdem ist so der Wert in der Liste unter "Konto bearbeiten" direkt anzeigbar, nämlich als initial "0€"
                string query = "INSERT INTO Tabelle_Konto (name_Konto, Kontonummer, gesamtsumme_Konto) VALUES (@Name, @Kontonummer, 0)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", TxtName.Text);
                command.Parameters.AddWithValue("@Kontonummer", TxtKontonummer.Text);

                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Konto erfolgreich gespeichert.");

                    // Leere die Eingabefelder
                    TxtName.Text = string.Empty;
                    TxtKontonummer.Text = string.Empty;

                    // Liste neu laden
                    Load_List_Konten();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler: " + ex.Message);
                }
            }
        }

        //---------------------- Funktion die hinter dem Button für das Speichern einer neuen Kategorie liegt ----------------------
        private void Button_Kategorie_Save_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Überprüfen, ob Kategorie bereits existiert
                string checkQuery = "SELECT COUNT(*) FROM TabelleKategorie WHERE nameKategorie = @Name";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@Name", TxtNameKategorie.Text);

                int count = (int)checkCommand.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Eine Kategorie mit diesem Namen existiert bereits.");
                    return;
                }

                string query = "INSERT INTO TabelleKategorie (nameKategorie) VALUES (@Name)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", TxtNameKategorie.Text);

                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Kategorie erfolgreich gespeichert.");

                    // Leere die Eingabefelder
                    TxtNameKategorie.Text = string.Empty;

                    // Liste neu laden
                    Load_List_Kategorie();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler: " + ex.Message);
                }
            }
        }

        //---------------------- Lädt die Listen für Kategorien und Konten neu, bswp. wenn ein neues Konto erstellt wurde ----------------------
        private void Window_Settings_Load(object sender, RoutedEventArgs e)
        {
            Load_List_Konten();
            Load_List_Kategorie();
        }

        //---------------------- Lädt die Liste der Konten aus der Datenbank für die Anzeige in der Listview ----------------------
        private void Load_List_Konten()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT name_Konto, Kontonummer, gesamtsumme_Konto FROM Tabelle_Konto", conn);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable datatable_konto = new DataTable();
                dataAdapter.Fill(datatable_konto);

                ListBoxKonten.ItemsSource = datatable_konto.DefaultView;
                ListBoxKontenBearbeiten.ItemsSource = datatable_konto.DefaultView;
            }
        }

        //---------------------- Lädt die Liste der Kategorien für die Anzeige in der Listview ----------------------
        private void Load_List_Kategorie()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM TabelleKategorie", conn);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable datatable_kategorie = new DataTable();
                dataAdapter.Fill(datatable_kategorie);

                ListBoxKategorie.ItemsSource = datatable_kategorie.DefaultView;
                ListBoxKategorieBearbeiten.ItemsSource = datatable_kategorie.DefaultView;
            }
        }

        //---------------------- Funktion die aufgerufen wird, wenn in der Liste mit den Konten ein anderes Item ausgewählt wird ----------------------
        private void ListBoxKontenBearbeiten_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxKontenBearbeiten.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)ListBoxKontenBearbeiten.SelectedItem;
                TxtNameBearbeiten.Text = selectedRow["name_Konto"].ToString();
                TxtKontonummerBearbeiten.Text = selectedRow["Kontonummer"].ToString();
            }
        }

        //---------------------- Funktion die aufgerufen wird, wenn in der Liste mit den Kategorien ein anderes Item ausgewählt wird ----------------------
        private void ListBoxKategorieBearbeiten_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxKategorieBearbeiten.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)ListBoxKategorieBearbeiten.SelectedItem;
                TxtNameKategorieBearbeiten.Text = selectedRow["nameKategorie"].ToString();
            }
        }

        //---------------------- Löschen-Button unter "Konto löschen"  ----------------------
        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxKonten.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)ListBoxKonten.SelectedItem;
                string kontonummer = selectedRow["Kontonummer"].ToString();

                string connectionString = ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Tabelle_Konto WHERE Kontonummer = @Kontonummer";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Kontonummer", kontonummer);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("Konto erfolgreich gelöscht.");
                        Load_List_Konten();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fehler: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie ein Konto aus der Liste aus.");
            }
        }

        //---------------------- Löschen-Button für Kategorien ----------------------
        private void Button_Delete_Kategorie_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxKategorie.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)ListBoxKategorie.SelectedItem;
                string kategorie = selectedRow["IdKategorie"].ToString();

                string connectionString = ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM TabelleKategorie WHERE IdKategorie = @KategorieID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@KategorieID", kategorie);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("Kategorie erfolgreich gelöscht.");
                        Load_List_Kategorie();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fehler: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie eine Kategorie aus der Liste aus.");
            }
        }

        //---------------------- Funktion für das Speichern von Änderungen an Kontoeinträgen ----------------------
        private void Button_SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxKontenBearbeiten.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)ListBoxKontenBearbeiten.SelectedItem;
                string originalKontonummer = selectedRow["Kontonummer"].ToString();
                string newName = TxtNameBearbeiten.Text;
                string newKontonummer = TxtKontonummerBearbeiten.Text;

                string connectionString = ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Tabelle_Konto SET name_Konto = @Name, Kontonummer = @Kontonummer WHERE Kontonummer = @OriginalKontonummer";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", newName);
                    command.Parameters.AddWithValue("@Kontonummer", newKontonummer);
                    command.Parameters.AddWithValue("@OriginalKontonummer", originalKontonummer);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("Konto erfolgreich aktualisiert.");
                        TxtNameBearbeiten.Text = string.Empty;
                        TxtKontonummerBearbeiten.Text = string.Empty;
                        Load_List_Konten();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fehler: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie ein Konto aus der Liste aus.");
            }
        }

        //---------------------- Funktion für Speichern von Änderungen an Kategorien ----------------------
        private void Button_SaveChanges_Kategorie_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxKategorieBearbeiten.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)ListBoxKategorieBearbeiten.SelectedItem;
                string originalKategorie = selectedRow["IdKategorie"].ToString();
                string newName = TxtNameKategorieBearbeiten.Text;

                string connectionString = ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE TabelleKategorie SET nameKategorie = @NameKategorie WHERE IdKategorie = @IdKategorie";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@NameKategorie", newName);
                    command.Parameters.AddWithValue("@IdKategorie", originalKategorie);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("Kategorie erfolgreich aktualisiert.");
                        Load_List_Kategorie();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fehler: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie ein Konto aus der Liste aus.");
            }
        }
    }
}
