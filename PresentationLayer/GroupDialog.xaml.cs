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
    /// Interaction logic for GroupDialog.xaml
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

            mainWindow.ReloadWindow = new Action(() => { LoadData(tokenSource.Token); });

            isLoaded = false;
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
                products = (from p in context.Products
                            where true
                            select p).ToList();

                internalOnes = (from w in context.Warehouses.Include("Sectors")
                                where w.Internal == true
                                select w).ToList();

                externalOnes = (from w in context.Warehouses.Include("Sectors")
                                where w.Internal == false
                                select w).ToList();

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    isLoaded = true;
                    InitializeData();
                }
                ));
            }

            if (token.IsCancellationRequested)
                return;

            //Dispatcher.BeginInvoke(new Action(() => InitializeData()));
        }

        private void InitializeData()
        {
            if (!isLoaded)
                return;

            foreach (DatabaseAccess.Warehouse w in externalOnes)
                PartnersComboBox.Items.Add(w.Name);

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
            using (var context = new DatabaseAccess.SystemContext())
            {
                DatabaseAccess.Shift s = new DatabaseAccess.Shift();

                s.SenderId = externalOnes.Find(delegate(DatabaseAccess.Warehouse w)
                {
                    return w.Name == (string)PartnersComboBox.SelectedValue;
                }).Id;
                s.RecipientId = internalOnes.Find(delegate(DatabaseAccess.Warehouse w)
                {
                    return w.Name == ((string)WarehousesComboBox.Text).Substring(0, ((string)WarehousesComboBox.Text).LastIndexOf('#') - 3);
                }).Id;
                s.Date = new DateTime(DateTime.Now.Ticks);
                s.Latest = true;

                s.Group = new DatabaseAccess.Group()
                    {
                        SectorId = int.Parse(((string)WarehousesComboBox.Text).Substring(((string)WarehousesComboBox.Text).LastIndexOf('#') + 1)),
                        GroupDetails = new List<DatabaseAccess.GroupDetails>()
                    };

                MessageBox.Show(s.Group.SectorId.ToString());

                foreach (ProductGroupRow p in Products.Items)
                {
                    s.Group.GroupDetails.Add(new DatabaseAccess.GroupDetails()
                    {
                        Product = products.Find(delegate(DatabaseAccess.Product prod)
                        {
                            return prod.Name == (string)p.ProductsComboBox.Text;
                        }),
                        Count = int.Parse(p.Quantity.Text),
                    });
                }

                context.Shifts.Add(s);

                context.SaveChanges();
            }

            mainWindow.ReloadWindow(); // nie działa
            this.Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
