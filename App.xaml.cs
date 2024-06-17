using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;


namespace MartinsHaushaltsbuch
{

    public partial class App : Application
    {
    }

    /// <summary>
    ///  Singleton für die globale Verwendung von Filtern. Soll beim Start der App auf bestimmte Default-Werte gesetzt und später vom Nutzer beliebig angepasst werden, 
    ///  woraufhin die Analysen und Diagramme neu berechnet werden.
    /// </summary>
    public class Singleton_Filter : Page, INotifyPropertyChanged
    {
        private static Singleton_Filter instance;
        private string _konto;
        private string _kategorie;
        private float _betrag;
        private string _datum = DateTime.Now.ToString();
        private string _einausgang;
        private int _id;

        // Private constructor to prevent direct instantiation
        private Singleton_Filter() { }

        // Static property to provide access to the single instance
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

        // Properties with PropertyChanged event invocation
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
