using MahApps.Metro.Controls;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace MartinsHaushaltsbuch
{
    public partial class Window_Settings : MetroWindow
    {
        public Window_Settings()
        {
            InitializeComponent();
        }

        private void Button_MainPage_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new MainWindow();
            newForm.Show();
            this.Close();
        }

        private void Button_Analysis_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Window_Analysis();
            newForm.Show();
            this.Close();
        }

        private void Button_NewEntry_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Window_NewEntry();
            newForm.Show();
            this.Close();
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Tabelle_Konto (name_Konto, Kontonummer) VALUES (@Name, @Kontonummer)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", TxtName.Text);
                command.Parameters.AddWithValue("@Kontonummer", TxtKontonummer.Text);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Konto erfolgreich gespeichert.");

                    // Leere die Eingabefelder
                    TxtName.Text = string.Empty;
                    TxtKontonummer.Text = string.Empty;

                    // Liste neu laden
                    Load_List();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler: " + ex.Message);
                }
            }
        }

        private void Window_Settings_Load(object sender, RoutedEventArgs e)
        {
            Load_List();
        }

        private void Load_List()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Tabelle_Konto", conn);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable datatable_konto = new DataTable();
                dataAdapter.Fill(datatable_konto);

                ListBoxKonten.ItemsSource = datatable_konto.DefaultView;
                ListBoxKontenBearbeiten.ItemsSource = datatable_konto.DefaultView;
            }
        }

        private void ListBoxKontenBearbeiten_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxKontenBearbeiten.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)ListBoxKontenBearbeiten.SelectedItem;
                TxtNameBearbeiten.Text = selectedRow["name_Konto"].ToString();
                TxtKontonummerBearbeiten.Text = selectedRow["Kontonummer"].ToString();
            }
        }

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
                        Load_List();
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
                        Load_List();
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
