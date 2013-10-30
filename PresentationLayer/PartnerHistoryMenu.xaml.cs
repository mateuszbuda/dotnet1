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
    /// Interaction logic for PartnerHistoryMenu.xaml
    /// </summary>
    public partial class PartnerHistoryMenu : UserControl   // 9
    {
        private CancellationTokenSource tokenSource;
        private int partnerId;
        private DatabaseAccess.Partner partner;
        private List<DatabaseAccess.Shift> shifts;
        private MainWindow mainWindow;

        public PartnerHistoryMenu(MainWindow mainWindow, int partnerId)
        {
            this.mainWindow = mainWindow;
            mainWindow.Title = "Historia Partnera";
            this.partnerId = partnerId;
            tokenSource = new CancellationTokenSource();

            mainWindow.ReloadWindow = new Action(() => LoadData(tokenSource.Token));

            InitializeComponent();

            Task.Factory.StartNew(LoadData, tokenSource.Token, tokenSource.Token);
        }

        private void LoadData(Object _token)
        {
            CancellationToken token = (CancellationToken)_token;

            if (token.IsCancellationRequested)
                return;

            using (var context = new DatabaseAccess.SystemContext())
            {
                partner = (from p in context.Partners.Include("Warehouse")
                           where p.Id == partnerId
                           select p).FirstOrDefault();

                shifts = (from s in context.Shifts.Include("Sender").Include("Recipient")
                          where (int)s.RecipientId == partner.WarehouseId || (int)s.SenderId == partner.WarehouseId
                          select s).ToList();

                if (token.IsCancellationRequested)
                    return;

                Dispatcher.BeginInvoke(new Action(() => InitializeData()));
            }
        }

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

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new MainMenu(mainWindow));
        }

        private void LoadNewMenu(UserControl menu)
        {
            Grid content = Parent as Grid;

            tokenSource.Cancel();

            content.Children.Remove(this);
            content.Children.Add(menu);
        }

        private void PartnersButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new PartnersMenu(mainWindow));
        }

        private void IdButtonClick(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new GroupMenu(mainWindow, (int)(sender as Button).Tag));
        }
    }
}
