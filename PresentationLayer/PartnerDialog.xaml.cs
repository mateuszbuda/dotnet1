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
    /// Dodawanie i edycja partnerów.
    /// </summary>
    public partial class PartnerDialog : Window     // 18
    {
        private MainWindow mainWindow;
        private CancellationTokenSource tokenSource;
        private int partnerId = -1;
        private DatabaseAccess.Partner partner;

        /// <summary>
        /// Konstruktor wykorzystywany przy edycji partnera.
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="id">Id edytowanego partnera</param>
        public PartnerDialog(MainWindow mainWindow, int id)
            : this(mainWindow)
        {
            Header.Content = "Edytuj dane:";
            Title = "Edycja partnera";

            partnerId = id;

            LoadData();
        }

        /// <summary>
        /// Konstruktor wykorzystywany przy tworzeniu nowego partnera.
        /// </summary>
        /// <param name="mainWindow"></param>
        public PartnerDialog(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            tokenSource = new CancellationTokenSource();

            InitializeComponent();
            this.DataContext = new WarehouseValidationRule();

            Header.Content = "Wprowadź dane:";
            Title = "Tworzenie nowego partnera";
        }

        /// <summary>
        /// Ładowanie danych
        /// </summary>
        private void LoadData()
        {
            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    partner = (from w in context.Partners.Include("Warehouse")
                               where w.Id == partnerId
                               select w).FirstOrDefault();

                    return true;
                }, t => Dispatcher.BeginInvoke(new Action(() => InitializeData())), tokenSource);
        }

        /// <summary>
        /// Wyświetlanie danych
        /// </summary>
        private void InitializeData()
        {
            WarehouseValidationRule rule = DataContext as WarehouseValidationRule;

            rule.Name = NameTB.Text = partner.Warehouse.Name;
            rule.City = CityTB.Text = partner.City;
            rule.Code = CodeTB.Text = partner.Code;
            rule.Street = StreetTB.Text = partner.Street;
            rule.Number = NumberTB.Text = partner.Num;
            rule.Phone = PhoneTB.Text = partner.Tel;
            MailTB.Text = partner.Mail;
        }

        /// <summary>
        /// Zapis danych i zamknięcie okna
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;

            var data = new
                {
                    Name = NameTB.Text,
                    City = CityTB.Text,
                    Code = CodeTB.Text,
                    Street = StreetTB.Text,
                    Num = NumberTB.Text,
                    Tel = PhoneTB.Text,
                    Mail = MailTB.Text
                };

            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    DatabaseAccess.Partner p;

                    if (partnerId == -1)
                    {
                        p = new DatabaseAccess.Partner();
                        DatabaseAccess.Warehouse w = new DatabaseAccess.Warehouse();
                        DatabaseAccess.Sector s = new DatabaseAccess.Sector();

                        s.Limit = 0;
                        s.Deleted = false;
                        s.Number = 1;

                        w.Name = data.Name;
                        w.Internal = false;
                        w.City = data.City;
                        w.Code = data.Code;
                        w.Street = data.Street;
                        w.Num = data.Num;
                        w.Tel = data.Tel;
                        w.Mail = data.Mail;
                        w.Deleted = false;
                        w.Sectors = new HashSet<DatabaseAccess.Sector>();
                        w.Sectors.Add(s);

                        p.Warehouse = w;
                        p.City = data.City;
                        p.Code = data.Code;
                        p.Street = data.Street;
                        p.Num = data.Num;
                        p.Tel = data.Tel;
                        p.Mail = data.Mail;

                        context.Partners.Add(p);
                    }
                    else
                    {
                        p = (from partner in context.Partners.Include("Warehouse") where partner.Id == partnerId select partner).FirstOrDefault();
                        p.Warehouse.Name = data.Name;
                        p.City = data.City;
                        p.Code = data.Code;
                        p.Street = data.Street;
                        p.Num = data.Num;
                        p.Tel = data.Tel;
                        p.Mail = data.Mail;

                        p.Warehouse.City = data.City;
                        p.Warehouse.Code = data.Code;
                        p.Warehouse.Street = data.Street;
                        p.Warehouse.Num = data.Num;
                        p.Warehouse.Tel = data.Tel;
                        p.Warehouse.Mail = data.Mail;
                    }

                    context.SaveChanges();

                    return true;
                }, t => Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mainWindow.ReloadWindow();
                        this.Close();
                    })), tokenSource);
        }

        /// <summary>
        /// Zamknięcie okna bez zapisu danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelClick(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
            this.Close();
        }
    }
}
