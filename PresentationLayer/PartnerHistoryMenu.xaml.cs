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
    /// Historia partnera.
    /// </summary>
    public partial class PartnerHistoryMenu : UserControl   // 9
    {
        private CancellationTokenSource tokenSource;
        private int partnerId;
        private DatabaseAccess.Partner partner;
        private List<DatabaseAccess.Shift> shifts;
        private MainWindow mainWindow;

        /// <summary>
        /// Inicjalizacja menu
        /// </summary>
        /// <param name="mainWindow">Referencja do okna głównego</param>
        /// <param name="partnerId">Id Partnera</param>
        public PartnerHistoryMenu(MainWindow mainWindow, int partnerId)
        {
            this.mainWindow = mainWindow;
            mainWindow.Title = "Historia Partnera";
            this.partnerId = partnerId;
            tokenSource = new CancellationTokenSource();

            mainWindow.ReloadWindow = LoadData;

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
                    partner = (from p in context.Partners.Include("Warehouse")
                               where p.Id == partnerId
                               select p).FirstOrDefault();

                    shifts = (from s in context.Shifts.Include("Sender").Include("Recipient")
                              where (int)s.RecipientId == partner.WarehouseId || (int)s.SenderId == partner.WarehouseId
                              select s).ToList();

                    return true;
                }, t => Dispatcher.BeginInvoke(new Action(() => InitializeData())), tokenSource);
        }

        /// <summary>
        /// Wyświetlanie danych
        /// </summary>
        private void InitializeData()
        {
            InfoLabel1.Content = String.Format("{0} {1}, {2} {3}",
                            partner.Street, partner.Num, partner.Code, partner.City);

            InfoLabel2.Content = String.Format("Telefon: {0}, e-mail: {1}",
                partner.Tel == null ? "Brak" : partner.Tel,
                partner.Mail == null ? "Brak" : partner.Mail);

            PartnerNameLabel.Content = String.Format("Partner '{0}' - Historia", partner.Warehouse.Name);

            foreach (DatabaseAccess.Shift s in shifts)
                ShiftsGrid.Items.Add(s);

            LoadingLabel.Visibility = System.Windows.Visibility.Hidden;
            ShiftsGrid.Visibility = System.Windows.Visibility.Visible;
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {

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
        /// Partnerzy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PartnersButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new PartnersMenu(mainWindow));
        }

        /// <summary>
        /// Partia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IdButtonClick(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new GroupMenu(mainWindow, (int)(sender as Button).Tag));
        }
    }
}
