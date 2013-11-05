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
            this.DataContext = new WarehouseValidationRule();

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
            WarehouseValidationRule rule = DataContext as WarehouseValidationRule;

            rule.Name = NameTB.Text = warehouse.Name;
            rule.City = CityTB.Text = warehouse.City;
            rule.Code = CodeTB.Text = warehouse.Code;
            rule.Street = StreetTB.Text = warehouse.Street;
            rule.Number = NumberTB.Text = warehouse.Num;
            rule.Phone = PhoneTB.Text = warehouse.Tel;
            MailTB.Text = warehouse.Mail;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
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
                    DatabaseAccess.Warehouse w;

                    if (warehouseId == -1)
                    {
                        w = new DatabaseAccess.Warehouse();
                        w.Name = data.Name;
                        w.City = data.City;
                        w.Code = data.Code;
                        w.Street = data.Street;
                        w.Num = data.Num;
                        w.Tel = data.Tel;
                        w.Mail = data.Mail;
                        w.Internal = true;

                        context.Warehouses.Add(w);
                    }
                    else
                    {
                        w = (from warehouse in context.Warehouses where warehouse.Id == warehouseId select warehouse).FirstOrDefault();
                        w.Name = data.Name;
                        w.City = data.City;
                        w.Code = data.Code;
                        w.Street = data.Street;
                        w.Num = data.Num;
                        w.Tel = data.Tel;
                        w.Mail = data.Mail;
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

        private void NameTB_LostFocus(object sender, RoutedEventArgs e)
        {
        }
    }
}
