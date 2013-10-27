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
    /// Interaction logic for WarehouseDialog.xaml
    /// </summary>
    public partial class WarehouseDialog : Window   // 13
    {
        private MainWindow mainWindow;
        private CancellationTokenSource tokenSource;
        private int warehouseId = -1;
        private DatabaseAccess.Warehouse warehouse;
        private ContextMenu contextMenu;

        public WarehouseDialog(MainWindow mainWindow, int id)
            : this(mainWindow)
        {
            Header.Content = "Edytuj dane:";
            Title = "Edycja magazynu";

            warehouseId = id;

            Task.Factory.StartNew(LoadData, tokenSource.Token, tokenSource.Token);
        }

        public WarehouseDialog(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            tokenSource = new CancellationTokenSource();

            InitializeComponent();

            Header.Content = "Wprowadź dane:";
            Title = "Tworzenie nowego magazynu";
        }

        private void LoadData(Object _token)
        {
            CancellationToken token = (CancellationToken)_token;

            if (token.IsCancellationRequested)
                return;

            using (var context = new DatabaseAccess.SystemContext())
            {
                warehouse = (from w in context.Warehouses
                             where w.Id == warehouseId
                             select w).FirstOrDefault();

                if (token.IsCancellationRequested)
                    return;

                Dispatcher.BeginInvoke(new Action(() => InitializeData()));
            }
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
            using (var context = new DatabaseAccess.SystemContext())
            {
                DatabaseAccess.Warehouse w;

                if (warehouseId == -1)
                {
                    w = new DatabaseAccess.Warehouse();
                    w.Name = NameTB.Text;
                    w.City = CityTB.Text;
                    w.Code = CodeTB.Text;
                    w.Street = StreetTB.Text;
                    w.Num = NumberTB.Text;
                    w.Tel = PhoneTB.Text;
                    w.Mail = MailTB.Text;
                    w.Internal = true;

                    context.Warehouses.Add(w);
                }
                else
                {
                    w = (from warehouse in context.Warehouses where warehouse.Id == warehouseId select warehouse).FirstOrDefault();
                    w.Name = NameTB.Text;
                    w.City = CityTB.Text;
                    w.Code = CodeTB.Text;
                    w.Street = StreetTB.Text;
                    w.Num = NumberTB.Text;
                    w.Tel = PhoneTB.Text;
                    w.Mail = MailTB.Text;
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
