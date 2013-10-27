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
using System.Windows.Shapes;

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for PartnerDialog.xaml
    /// </summary>
    public partial class PartnerDialog : Window     // 18
    {
        private MainWindow mainWindow;
        private CancellationTokenSource tokenSource;
        private int partnerId = -1;
        private DatabaseAccess.Partner partner;
        private ContextMenu contextMenu;

        public PartnerDialog(MainWindow mainWindow, int id)
            : this(mainWindow)
        {
            Header.Content = "Edytuj dane:";
            Title = "Edycja partnera";

            partnerId = id;

            Task.Factory.StartNew(LoadData, tokenSource.Token, tokenSource.Token);
        }

        public PartnerDialog(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            tokenSource = new CancellationTokenSource();

            InitializeComponent();

            Header.Content = "Wprowadź dane:";
            Title = "Tworzenie nowego partnera";
        }

        private void LoadData(Object _token)
        {
            CancellationToken token = (CancellationToken)_token;

            if (token.IsCancellationRequested)
                return;

            using (var context = new DatabaseAccess.SystemContext())
            {
                partner = (from w in context.Partners.Include("Warehouse")
                           where w.Id == partnerId
                           select w).FirstOrDefault();

                if (token.IsCancellationRequested)
                    return;

                Dispatcher.BeginInvoke(new Action(() => InitializeData()));
            }
        }

        private void InitializeData()
        {
            NameTB.Text = partner.Warehouse.Name;
            CityTB.Text = partner.City;
            CodeTB.Text = partner.Code;
            StreetTB.Text = partner.Street;
            NumberTB.Text = partner.Num;
            PhoneTB.Text = partner.Tel;
            MailTB.Text = partner.Mail;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            using (var context = new DatabaseAccess.SystemContext())
            {
                DatabaseAccess.Partner p;

                if (partnerId == -1)
                {
                    p = new DatabaseAccess.Partner();
                    DatabaseAccess.Warehouse w = new DatabaseAccess.Warehouse();

                    w.Name = NameTB.Text;
                    w.Internal = false;
                    w.City = CityTB.Text;
                    w.Code = CodeTB.Text;
                    w.Street = StreetTB.Text;
                    w.Num = NumberTB.Text;
                    w.Tel = PhoneTB.Text;
                    w.Mail = MailTB.Text;
                    w.Deleted = false;

                    p.Warehouse = w;
                    p.City = CityTB.Text;
                    p.Code = CodeTB.Text;
                    p.Street = StreetTB.Text;
                    p.Num = NumberTB.Text;
                    p.Tel = PhoneTB.Text;
                    p.Mail = MailTB.Text;

                    context.Partners.Add(p);
                }
                else
                {
                    p = (from partner in context.Partners.Include("Warehouse") where partner.Id == partnerId select partner).FirstOrDefault();
                    p.Warehouse.Name = NameTB.Text;
                    p.City = CityTB.Text;
                    p.Code = CodeTB.Text;
                    p.Street = StreetTB.Text;
                    p.Num = NumberTB.Text;
                    p.Tel = PhoneTB.Text;
                    p.Mail = MailTB.Text;
                }

                context.SaveChanges();
            }

            mainWindow.ReloadWindow();
            this.Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
