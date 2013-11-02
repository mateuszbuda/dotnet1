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
    /// Okno dialogowe dodawania i edycji magazynów.
    /// </summary>
    public partial class WarehouseDialog : Window   // 13
    {
        private MainWindow mainWindow;
        private CancellationTokenSource tokenSource;
        private int warehouseId = -1;
        private DatabaseAccess.Warehouse warehouse;

        public WarehouseDialog(MainWindow mainWindow, int id)
            : this(mainWindow)
        {
            Header.Content = "Edytuj dane:";
            Title = "Edycja magazynu";

            warehouseId = id;

            LoadData();
        }

        public WarehouseDialog(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            tokenSource = new CancellationTokenSource();

            InitializeComponent();

            Header.Content = "Wprowadź dane:";
            Title = "Tworzenie nowego magazynu";
        }

        private void LoadData()
        {
            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    warehouse = (from w in context.Warehouses
                                 where w.Id == warehouseId
                                 select w).FirstOrDefault();

                    return true;
                }, t => Dispatcher.BeginInvoke(new Action(() => InitializeData())), tokenSource);
        }

        private void InitializeData()
        {
            NameTB.Text = warehouse.Name;
            CityTB.Text = warehouse.City;
            CodeTB.Text = warehouse.Code;
            StreetTB.Text = warehouse.Street;
            NumberTB.Text = warehouse.Num;
            PhoneTB.Text = warehouse.Tel;
            MailTB.Text = warehouse.Mail;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;

            var tmp = new
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
                    DatabaseAccess.Warehouse w;

                    if (warehouseId == -1)
                    {
                        w = new DatabaseAccess.Warehouse();
                        w.Name = tmp.Name;
                        w.City = tmp.City;
                        w.Code = tmp.Code;
                        w.Street = tmp.Street;
                        w.Num = tmp.Num;
                        w.Tel = tmp.Tel;
                        w.Mail = tmp.Mail;
                        w.Internal = true;

                        context.Warehouses.Add(w);
                    }
                    else
                    {
                        w = (from warehouse in context.Warehouses where warehouse.Id == warehouseId select warehouse).FirstOrDefault();
                        w.Name = tmp.Name;
                        w.City = tmp.City;
                        w.Code = tmp.Code;
                        w.Street = tmp.Street;
                        w.Num = tmp.Num;
                        w.Tel = tmp.Tel;
                        w.Mail = tmp.Mail;
                    }

                    context.SaveChanges();

                    return true;
                }, t => Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mainWindow.ReloadWindow();
                        this.Close();
                    })), tokenSource);
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
            this.Close();
        }
    }
}
