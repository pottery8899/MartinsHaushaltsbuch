using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace MartinsHaushaltsbuch
{

    public partial class App : Application
    {
    }

/// <summary>
///  Singleton für die globale Verwendung von Filtern. Soll beim Start der App auf bestimmte Default-Werte gesetzt und später vom Nutzer beliebig angepasst werden, 
///  woraufhin die Analysen und Diagramme neu berechnet werden.
/// </summary>
    public class Singleton_Filter
    {
        private static Singleton_Filter instance;
        private string _konto;
        private string _kategorie;
        private float _betrag;
        private string _datum;
        private string _einausgang;
        private int _id;

        public string Konto
        {
            get => _konto;
            set
            {
                _konto = value;
            }
        }

        public string Kategorie
        {
            get => _kategorie;
            set 
            {
                _kategorie = value; 
            }
        }

        public float Betrag
        {
            get => _betrag;
            set
            {
                _betrag = value;
            }
        }

        public string Datum
        {
            get => _datum;
            set { 
            _datum = value;
            }
        }

        public string Einausgang
        {
            get => _einausgang;
                set
            {
                _einausgang = value;
            }
        }

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
            }
        }
    }
}
