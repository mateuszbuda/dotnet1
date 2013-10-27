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
    /// Interaction logic for PartnerMenu.xaml
    /// </summary>
    public partial class PartnerMenu : UserControl  // 8
    {
        private MainWindow mainWindow;
        private int partnerId;
        private CancellationTokenSource tokenSource;
        private DatabaseAccess.Partner partner;

        public PartnerMenu(MainWindow mainWindow, int id)
        {
            this.mainWindow = mainWindow;
            partnerId = id;
            tokenSource = new CancellationTokenSource();
            mainWindow.ReloadWindow = new Action(() => { LoadData(tokenSource.Token); });

            InitializeComponent();

            Task.Factory.StartNew(LoadData, tokenSource.Token, tokenSource.Token);
        }

        private void LoadData(object _token)
        {
            CancellationToken token = (CancellationToken)_token;

            if (token.IsCancellationRequested)
                return;

            using (var context = new DatabaseAccess.SystemContext())
            {
                partner = (from p in context.Partners.Include("Warehouse")
                           where p.Id == partnerId
                           select p).FirstOrDefault();
            }

            if (token.IsCancellationRequested)
                return;

            Dispatcher.BeginInvoke(new Action(() => InitializeData()));
        }

        private void InitializeData()
        {
            LoadingLabel.Visibility = System.Windows.Visibility.Hidden;
            PartnerLabel.Content = String.Format("Partner {0}", partner.Warehouse.Name);

            PartnerGrid.Visibility = System.Windows.Visibility.Visible;

            AddressLabel1.Content = String.Format("{0} {1}\n{2} {3}", partner.Street, partner.Num, partner.Code, partner.City);
            TelLabel1.Content = partner.Tel;
            MailLabel1.Content = partner.Mail == null ? "Brak" : partner.Mail;

            AddressLabel2.Content = String.Format("{0} {1}\n{2} {3}", partner.Warehouse.Street, 
                partner.Warehouse.Num, partner.Warehouse.Code, partner.Warehouse.City);
            TelLabel2.Content = partner.Warehouse.Tel == null ? "Brak" : partner.Warehouse.Tel;
            MailLabel2.Content = partner.Warehouse.Mail == null ? "Brak" : partner.Warehouse.Mail;
        }

        private void PartnersButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new PartnersMenu(mainWindow));
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new MainMenu(mainWindow));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            PartnerDialog dlg = new PartnerDialog(mainWindow, partnerId);
            dlg.Show();
        }

        private void LoadNewMenu(UserControl menu)
        {
            Grid content = Parent as Grid;

            tokenSource.Cancel();

            content.Children.Remove(this);
            content.Children.Add(menu);
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new PartnerHistoryMenu(mainWindow, partnerId));
        }
    }
}
