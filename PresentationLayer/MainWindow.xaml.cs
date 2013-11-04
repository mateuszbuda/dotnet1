using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PresentationLayer
{
    /// <summary>
    /// Główne okno programu, na które wczytywane są okna w postaci kontrolek.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Przeładowywanie kontrolki i jej zawartości w oknie głównym
        /// </summary>
        public Action ReloadWindow { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            MainWindowContent.Children.Add(new MainMenu(this));
        }
    }
}
