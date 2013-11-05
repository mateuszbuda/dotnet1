using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Menu partnerów.
    /// Umożliwia podgląd i edycję danych partnerów.
    /// </summary>
    public partial class PartnersMenu : UserControl // 7
    {
        private MainWindow mainWindow;
        private CancellationTokenSource tokenSource;
        private List<DatabaseAccess.Partner> partners;
        private bool isLoaded;

        /// <summary>
        /// Inicjalizacja menu
        /// </summary>
        /// <param name="mainWindow">Referencja do okna głównego</param>
        public PartnersMenu(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            mainWindow.Title = "Partnerzy";
            tokenSource = new CancellationTokenSource();

            mainWindow.ReloadWindow = LoadData;

            isLoaded = false;
            InitializeComponent();

            LoadData();
        }

        /// <summary>
        /// Ładowanie danych
        /// </summary>
        private void LoadData()
        {
            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    partners = (from p in context.Partners.Include("Warehouse")
                                where true
                                select p).ToList();

                    return true;
                }, t => Dispatcher.BeginInvoke(new Action(() =>
                {
                    isLoaded = true;
                    InitializeData();
                }
                )), tokenSource);
        }

        /// <summary>
        /// Wyświetlanie danych
        /// </summary>
        private void InitializeData()
        {
            if (!isLoaded)
                return;

            PartnersGrid.Items.Clear();

            foreach (DatabaseAccess.Partner p in partners)
            {
                PartnersGrid.Items.Add(p);
            }

            LoadingLabel.Visibility = System.Windows.Visibility.Hidden;
            PartnersGrid.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Nowy partner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            PartnerDialog dlg = new PartnerDialog(mainWindow);
            dlg.Show();
        }

        /// <summary>
        /// Menu główne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new MainMenu(mainWindow));
        }

        /// <summary>
        /// Ładowanie menu
        /// </summary>
        /// <param name="menu"></param>
        private void LoadNewMenu(UserControl menu)
        {
            Grid content = Parent as Grid;

            tokenSource.Cancel();

            content.Children.Remove(this);
            content.Children.Add(menu);
        }

        /// <summary>
        /// Partner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WarehouseNameClick(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new PartnerMenu(mainWindow, (int)(sender as Button).Tag));
        }

        /// <summary>
        /// Historia partnera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PartnerHistoryMenuClick(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new PartnerHistoryMenu(mainWindow, (int)(sender as Button).Tag));
        }

        /// <summary>
        /// Edycja partnera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PartnerEditClick(object sender, RoutedEventArgs e)
        {
            PartnerDialog dlg = new PartnerDialog(mainWindow, (int)(sender as Button).Tag);
            dlg.Show();
        }
    }
}