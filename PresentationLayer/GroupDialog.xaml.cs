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
    /// Tworzenie nowej partii (Przesunięcie partii od partnera).
    /// </summary>
    public partial class GroupDialog : Window   // 17
    {
        private MainWindow mainWindow;
        private CancellationTokenSource tokenSource;
        private List<DatabaseAccess.Product> products;
        private List<DatabaseAccess.Warehouse> internalOnes;
        private List<DatabaseAccess.Warehouse> externalOnes;
        private bool isLoaded;
        private int sectorId;

        public GroupDialog(MainWindow mainWindow, int sectorId)
        {
            this.mainWindow = mainWindow;
            tokenSource = new CancellationTokenSource();

            isLoaded = false;
            InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    products = (from p in context.Products
                                where true
                                select p).ToList();

                    internalOnes = (from w in context.Warehouses.Include("Sectors")
                                    where w.Internal == true
                                    select w).ToList();

                    externalOnes = (from w in context.Warehouses.Include("Sectors")
                                    where w.Internal == false
                                    select w).ToList();

                    return true;
                }, t => Dispatcher.BeginInvoke(new Action(() =>
                {
                    isLoaded = true;
                    InitializeData();
                }
                )), tokenSource);
        }

        private void InitializeData()
        {
            if (!isLoaded)
                return;

            foreach (DatabaseAccess.Warehouse w in externalOnes)
                PartnersComboBox.Items.Add(w);//.Name);

            foreach (DatabaseAccess.Warehouse w in internalOnes)
                foreach (DatabaseAccess.Sector s in w.Sectors)
                {
                    WarehousesComboBox.Items.Add(s);//w.Name + " - #" + s.Number);
                }

            ProductGroupRow productRow = new ProductGroupRow();
            Products.Items.Add(productRow);

            foreach (DatabaseAccess.Product p in products)
            {
                productRow.ProductsComboBox.Items.Add(p.Name);
            }
        }

        private void AddProductClick(object sender, RoutedEventArgs e)
        {
            ProductGroupRow productRow = new ProductGroupRow();
            Products.Items.Add(productRow);

            foreach (DatabaseAccess.Product p in products)
                productRow.ProductsComboBox.Items.Add(p.Name);
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;

            DatabaseAccess.Warehouse senderW = 
                (DatabaseAccess.Warehouse)PartnersComboBox.Items[PartnersComboBox.SelectedIndex];

            DatabaseAccess.Warehouse recipientW =
                ((DatabaseAccess.Sector)WarehousesComboBox.Items[PartnersComboBox.SelectedIndex]).Warehouse;

            DatabaseAccess.Sector sector = (DatabaseAccess.Sector)WarehousesComboBox.SelectedItem;

            // przenieść w to miejsce wszystkie odwołania do UI

            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    DatabaseAccess.Shift s = new DatabaseAccess.Shift();

                    s.Sender = senderW;
                    s.Recipient = recipientW;
                    context.Warehouses.Attach(s.Recipient);
                    context.Warehouses.Attach(s.Sender);
                    s.Date = new DateTime(DateTime.Now.Ticks);
                    s.Latest = true;

                    s.Group = new DatabaseAccess.Group()
                        {
                            Sector = sector,
                            GroupDetails = new List<DatabaseAccess.GroupDetails>()
                        };

                    foreach (ProductGroupRow p in Products.Items) // Tu się wywala. Nie można dać tu odwołania do UI.
                        // trzeba dać to jakoś przed, przypisać do listy czy coś. generalnie nic z UI nie może tutaj być
                    {
                        DatabaseAccess.GroupDetails gd = new DatabaseAccess.GroupDetails()
                        {
                            Product = products.Find(delegate(DatabaseAccess.Product prod)
                            {
                                return prod.Name == (string)p.ProductsComboBox.Text;
                            }),
                            Count = int.Parse(p.Quantity.Text),
                        };

                        context.Products.Attach(gd.Product);

                        s.Group.GroupDetails.Add(gd);
                    }

                    context.Shifts.Add(s);

                    context.SaveChanges();

                    return true;
                }, t => Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mainWindow.ReloadWindow();
                        this.Close();
                    })), tokenSource);           
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
            this.Close();
        }
    }
}
