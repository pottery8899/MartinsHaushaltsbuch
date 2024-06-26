using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using System.IO;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Collections.Generic;
using System.Text;


namespace MartinsHaushaltsbuch
{

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            

            // Bauen eines individuellen Pfades
            string documentsPath = Directory.GetCurrentDirectory();
            string relativePath = @"Datenbank\dbHaushaltsbuch.sqlite";
            // Setzen des DataDirectory auf das Verzeichnis, das die Datenbankdatei enthält

            //Vollständige Pfad
            string fullPath = System.IO.Path.Combine(documentsPath, relativePath);

            string directoryPath = documentsPath + @"\Datenbank";

            // Erstellen des Ordners wenn noch nicht vorhanden
            if (!Directory.Exists(directoryPath))
            { 
                Directory.CreateDirectory(directoryPath);
            }

            // Festlegen connection string
            string connectionString = @$"Data Source = {fullPath}";

            // Prüfen auf Vorhandensein der Datenbank, alternativ Neuerstellen
            if (!System.IO.File.Exists(fullPath))
            {
                SQLiteConnection.CreateFile(fullPath);                          

                using (var sqlite2 = new SQLiteConnection(connectionString))
                {
                    sqlite2.Open();
                    CreateTables(sqlite2);
                    CreateData(sqlite2);
                }
            }     
                     
            SaveConnectionString("HaushaltsbuchDB", connectionString);

            // Setzen des Standardwerts
            Singleton_Filter.Instance.Konto = "Alle Konten";
        }

        private static void CreateTables(SQLiteConnection sqlite2)
        {
            string sqlKat = "CREATE TABLE [Kategorie]" + "(" +
                "[IdKategorie] INTEGER PRIMARY KEY," +
                "[nameKategorie] NVARCHAR (50) NULL)";
            string sqlKonto = "CREATE TABLE [Konto] (" +
                "[Id_Konto] INTEGER PRIMARY KEY," +
                "[name_Konto] NVARCHAR (50) NULL, " +
                "[gesamtsumme_Konto] FLOAT (53) NULL," +
                "[Kontonummer] NVARCHAR (50) NULL)";
            string sqlBuchung = "CREATE TABLE [Buchung] (" +
                "[Id_Buchung] INTEGER PRIMARY KEY," +
                "[Titel_Buchung] NVARCHAR (50) NULL," +
                "[Konto_Buchung] NVARCHAR (50) NULL," +
                "[Kategorie_Buchung] NVARCHAR (50) NULL," +
                "[Betrag_Buchung] FLOAT (53) NULL," +
                "[Datum_Buchung] DATE NULL," +
                "[Kommentar_Buchung] NVARCHAR (50) NULL)";

            SQLiteCommand commandKat = new SQLiteCommand(sqlKat, sqlite2);
            SQLiteCommand commandKonto = new SQLiteCommand(sqlKonto, sqlite2);
            SQLiteCommand commandBuchung = new SQLiteCommand(sqlBuchung, sqlite2);
            commandKat.ExecuteNonQuery();
            commandKonto.ExecuteNonQuery();
            commandBuchung.ExecuteNonQuery();
        }

        private static void CreateData(SQLiteConnection sqlite2)
        {
            StringBuilder sqlKategorie = new StringBuilder();
            sqlKategorie.AppendLine("INSERT INTO[Kategorie] ([IdKategorie], [nameKategorie]) VALUES(1, 'Einkaufen');");
            sqlKategorie.AppendLine("INSERT INTO[Kategorie] ([IdKategorie], [nameKategorie]) VALUES(2, 'Freizeit');");
            sqlKategorie.AppendLine("INSERT INTO[Kategorie] ([IdKategorie], [nameKategorie]) VALUES(3, 'Essen gehen');");
            sqlKategorie.AppendLine("INSERT INTO[Kategorie] ([IdKategorie], [nameKategorie]) VALUES(4, 'Hobbys');");
            sqlKategorie.AppendLine("INSERT INTO[Kategorie] ([IdKategorie], [nameKategorie]) VALUES(5, 'Sonstiges');");
            sqlKategorie.AppendLine("INSERT INTO[Kategorie] ([IdKategorie], [nameKategorie]) VALUES(4, 'Einnahmen');");
            SQLiteCommand commandInsertKat = new SQLiteCommand(sqlKategorie.ToString(), sqlite2);
            commandInsertKat.ExecuteNonQuery();

            StringBuilder sqlKonto = new StringBuilder();
            sqlKonto.AppendLine("INSERT INTO [Konto] ([Id_Konto],[name_Konto],[gesamtsumme_Konto],[Kontonummer]) VALUES (1,'Girokonto',0,'DE xxx xxx xx1');");
            sqlKonto.AppendLine("INSERT INTO [Konto] ([Id_Konto],[name_Konto],[gesamtsumme_Konto],[Kontonummer]) VALUES (2,'Tagesgeldkonto',0,'DE xxx xxx xx2');");
            sqlKonto.AppendLine("INSERT INTO [Konto] ([Id_Konto],[name_Konto],[gesamtsumme_Konto],[Kontonummer]) VALUES (3,'Depot',0,'DE xxx xxx xx3');");
            SQLiteCommand commandInsertKonto = new SQLiteCommand(sqlKonto.ToString(), sqlite2);
            commandInsertKonto.ExecuteNonQuery();

           
        }

        private void SaveConnectionString(string name, string connectionString)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringsSection section = config.ConnectionStrings;

            if (section.ConnectionStrings[name] != null)
            {
                section.ConnectionStrings[name].ConnectionString = connectionString;
            }
            else
            {
                section.ConnectionStrings.Add(new ConnectionStringSettings(name, connectionString));
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");
        }

    }



    //  Singleton für die globale Verwendung von Filtern. Soll beim Start der App auf bestimmte Default-Werte gesetzt und später vom Nutzer beliebig angepasst werden, 
    //  woraufhin die Analysen und Diagramme neu berechnet werden.

    public class Singleton_Filter : Page, INotifyPropertyChanged
    {
        private static Singleton_Filter instance;
        private string _konto;
        private string _kategorie;
        private float _betrag;
        private string _datum = DateTime.Now.ToString();
        private string _einausgang;
        private int _id;

        // Privater Kontruktor für Singleton
        private Singleton_Filter() { }

        // Statische Eigenschaft für den Zugriff auf die einzelne Instanz
        public static Singleton_Filter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Singleton_Filter();
                }
                return instance;
            }
        }

        // Eigenschaften mit PropertyChanged-Ereignisaufruf
        public string Konto
        {
            get => _konto;
            set
            {
                if (_konto != value)
                {
                    _konto = value;
                    OnPropertyChanged(nameof(Konto));
                }
            }
        }

        public string Kategorie
        {
            get => _kategorie;
            set
            {
                _kategorie = value;
                OnPropertyChanged(nameof(Kategorie));
            }
        }

        public float Betrag
        {
            get => _betrag;
            set
            {
                _betrag = value;
                OnPropertyChanged(nameof(Betrag));
            }
        }

        public string Datum
        {
            get => _datum;
            set
            {
                _datum = value;
                OnPropertyChanged(nameof(Datum));
            }
        }

        public string Einausgang
        {
            get => _einausgang;
            set
            {
                _einausgang = value;
                OnPropertyChanged(nameof(Einausgang));
            }
        }

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
