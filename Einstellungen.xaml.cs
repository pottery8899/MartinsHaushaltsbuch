using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

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
            try
            {
                string connectionString = "Data Source = (localdb)\\MSSQLLocalDB;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL-Befehl zum Einfügen eines neuen Kontos
                    string query = @"INSERT INTO [dbo.Tabelle_Konto] ([name_Konto], [gesamtsumme_Konto], [Kontonummer]) 
                                     VALUES (@Name, @Gesamtsumme, @Kontonummer)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Parameterwerte setzen
                        command.Parameters.AddWithValue("@Name", TxtName.Text);
                        command.Parameters.AddWithValue("@Gesamtsumme", Convert.ToDouble(TxtGesamtsumme.Text));
                        command.Parameters.AddWithValue("@Kontonummer", TxtKontonummer.Text);

                        // Befehl ausführen
                        command.ExecuteNonQuery();

                        MessageBox.Show("Konto erfolgreich gespeichert!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern des Kontos: " + ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
