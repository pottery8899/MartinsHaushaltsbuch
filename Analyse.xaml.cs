using LiveCharts;
using LiveCharts.Wpf;
using MahApps.Metro.Controls;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace MartinsHaushaltsbuch
{
    //---------------------- Viewmodel fuer das erste, nicht-dynamische Element, das "alle Konten" repraesentiert ----------------------
    public class KontoViewModel
    {
        public string name_Konto { get; set; }
        public string Kontonummer { get; set; }
        public float gesamtsumme_Konto { get; set; }
    }

    public partial class Window_Analysis : MetroWindow
    {

        public ObservableCollection<KontoViewModel> KontenListe { get; set; }
        public SeriesCollection LineSeriesCollection { get; set; }
        public string[] Labels { get; set; }

        //---------------------- Initialisieren der Seite ----------------------
        public Window_Analysis()
        {
            InitializeComponent();
            LoadKonten();
            DataContext = this;
            LoadChartData();
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
            KontenListe = new ObservableCollection<KontoViewModel>();

            // Hier fügst du das nicht-dynamische Element "Alle Konten" als erstes Element hinzu
            KontenListe.Add(new KontoViewModel { name_Konto = "Alle Konten", gesamtsumme_Konto = CalculateGesamtsummeAllerKonten() });

            // Hier fügst du die dynamischen Konten hinzu (aus der Datenbank oder anderer Datenquelle)
            string connectionString = ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString;
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand("SELECT name_Konto, Kontonummer, gesamtsumme_Konto FROM Konto", conn);
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd);
                DataTable datatable_konto = new DataTable();
                dataAdapter.Fill(datatable_konto);

                foreach (DataRow row in datatable_konto.Rows)
                {
                    KontenListe.Add(new KontoViewModel
                    {
                        name_Konto = row["name_Konto"].ToString(),
                        Kontonummer = row["Kontonummer"].ToString(),
                        gesamtsumme_Konto = float.Parse(row["gesamtsumme_Konto"].ToString())
                    });
                }
            }

            // Setze die Datenquelle der ListBox
            ListBoxKonten.ItemsSource = KontenListe;
        }

        private float CalculateGesamtsummeAllerKonten()
        {
            float gesamtsumme = 0;

            // Verbindung zur Datenbank herstellen und die Summe abrufen
            string connectionString = ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString;
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand("SELECT SUM(gesamtsumme_Konto) FROM Konto", conn);
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    gesamtsumme = Convert.ToSingle(result);
                }
            }

            return gesamtsumme;
        }

        //---------------------- Speichern des in der Liste ausgewählten Kontos im Singleton ----------------------

        private void ListBoxKonten_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxKonten.SelectedItem != null)
            {
                if (ListBoxKonten.SelectedItem is KontoViewModel selectedKonto)
                {
                    // Der Benutzer hat ein dynamisches Konto ausgewählt
                    Singleton_Filter.Instance.Konto = selectedKonto.name_Konto;
                }
                else if (ListBoxKonten.SelectedItem is ListBoxItem selectedItem && selectedItem.Tag != null && selectedItem.Tag.ToString() == "AlleKonten")
                {
                    // Der Benutzer hat "Alle Konten" ausgewählt
                    Singleton_Filter.Instance.Konto = "Alle Konten";
                }
                LoadChartData();  // Daten neu laden, wenn Konto geändert wird
            }
        }

        //---------------------- Code für das Liniendiagramm ----------------------

        private void LoadChartData()
        {
            string kontoFilter = Singleton_Filter.Instance.Konto;
            string connectionString = ConfigurationManager.ConnectionStrings["HaushaltsbuchDB"].ConnectionString;

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = kontoFilter == "Alle Konten"
                    ? "SELECT Datum_Buchung, SUM(Betrag_Buchung) as Betrag FROM Buchung GROUP BY Datum_Buchung"
                    : "SELECT Datum_Buchung, SUM(Betrag_Buchung) as Betrag FROM Buchung WHERE Konto_Buchung = @konto GROUP BY Datum_Buchung";

                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                if (kontoFilter != "Alle Konten")
                {
                    cmd.Parameters.AddWithValue("@konto", kontoFilter);
                }

                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                var dates = dataTable.AsEnumerable().Select(row => row.Field<DateTime>("Datum_Buchung").ToShortDateString()).ToArray();
                var amounts = dataTable.AsEnumerable().Select(row => row.Field<double>("Betrag")).ToArray();

                LineSeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Buchungen",
                        Values = new ChartValues<double>(amounts)
                    }
                };

                Labels = dates;
                DataContext = null;
                DataContext = this; // Datenbindung aktualisieren
            }
        }
    }
}
