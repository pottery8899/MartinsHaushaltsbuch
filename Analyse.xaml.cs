using MahApps.Metro.Controls;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace MartinsHaushaltsbuch
{
    public partial class Window_Analysis : MetroWindow
    {
        //---------------------- Initialisieren der Seite ----------------------
        public Window_Analysis()
        {
            InitializeComponent();
            LoadKonten();
        }

        //---------------------- Navigation auf Startseite ----------------------
        private void Button_MainPage_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new MainWindow();
            newForm.Show();
            this.Close();
        }

        //---------------------- Navigation auf Seite "Einstellungen" ----------------------
        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Window_Settings();
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

        //---------------------- Laden der Konten aus der Datenbank ----------------------
        private void LoadKonten()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT name_Konto, Kontonummer, gesamtsumme_Konto FROM Tabelle_Konto", conn);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable datatable_konto = new DataTable();
                dataAdapter.Fill(datatable_konto);

                // Binden der Daten an die horizontale ListView
                ListBoxKonten.ItemsSource = datatable_konto.DefaultView;
            }
        }
    }
}
