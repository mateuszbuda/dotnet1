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

        /// <summary>
        /// Nowe okno dialogowe
        /// </summary>
        /// <param name="mainWindow">Referencja do okna głównego</param>
        /// <param name="sectorId"></param>
        public GroupDialog(MainWindow mainWindow, int sectorId)
        {
            this.mainWindow = mainWindow;
            tokenSource = new CancellationTokenSource();

            isLoaded = false;
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
                    products = (from p in context.Products
                                where true
                                select p).ToList();

                    internalOnes = (from w in context.Warehouses.Include("Sectors")
                                    where w.Internal == true
                                    select w).ToList();

                    externalOnes = (from w in context.Warehouses.Include("Sectors.Groups")
                                    where w.Internal == false && w.Deleted == false
                                    select w).ToList();

                    return true;
                }, t => Dispatcher.BeginInvoke(new Action(() =>
                {
                    isLoaded = true;
                    InitializeData();
                }
                )), tokenSource);
        }

        /// <summary>
        /// Przygotowywanie danych do wyświetlania
        /// </summary>
        private void InitializeData()
        {
            if (!isLoaded)
                return;

            foreach (DatabaseAccess.Warehouse w in externalOnes)
                PartnersComboBox.Items.Add(w);

            foreach (DatabaseAccess.Warehouse w in internalOnes)
                foreach (DatabaseAccess.Sector s in w.Sectors)
                    if (!s.IsFull() && s.Deleted == false)
                        WarehousesComboBox.Items.Add(s);

            ProductGroupRow productRow = new ProductGroupRow();
            Products.Items.Add(productRow);

            foreach (DatabaseAccess.Product p in products)
            {
                productRow.ProductsComboBox.Items.Add(p.Name);
            }
        }

        /// <summary>
        /// Dodaje nowy wiersz do wprowadzenia danych produktu w partii.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddProductClick(object sender, RoutedEventArgs e)
        {
            ProductGroupRow productRow = new ProductGroupRow();
            Products.Items.Add(productRow);

            foreach (DatabaseAccess.Product p in products)
                productRow.ProductsComboBox.Items.Add(p.Name);
        }

        /// <summary>
        /// Zapisuje partię, po wcześniejszym sprawdzeniu danych i zamyka okno.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;

            if (PartnersComboBox.SelectedIndex < 0 || WarehousesComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("Wypełnij poprawnie wszystkie dane.", "Uwaga");
                (sender as Button).IsEnabled = true;
                return;
            }
            DatabaseAccess.Warehouse senderW =
                (DatabaseAccess.Warehouse)PartnersComboBox.Items[PartnersComboBox.SelectedIndex];

            DatabaseAccess.Warehouse recipientW =
                ((DatabaseAccess.Sector)WarehousesComboBox.Items[WarehousesComboBox.SelectedIndex]).Warehouse;

            DatabaseAccess.Sector sector = (DatabaseAccess.Sector)WarehousesComboBox.SelectedItem;

            int productsCount = Products.Items.Count;
            String[,] productsInfo = new String[2, productsCount];
            // productsInfo[0,*] - name
            // productsInfo[1,*] - quantity
            int i = 0;
            foreach (ProductGroupRow p in Products.Items)
            {
                if (p.ProductsComboBox.SelectedIndex < 0)
                {
                    MessageBox.Show("Wypełnij poprawnie wszystkie dane.", "Uwaga");
                    (sender as Button).IsEnabled = true;
                    return;
                }
                productsInfo[0, i] = (string)p.ProductsComboBox.Text;
                productsInfo[1, i] = p.Quantity.Text;
                i++;
            }

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

                    for (int k = 0; k < productsCount; k++)
                    {
                        DatabaseAccess.GroupDetails gd = null;
                        try
                        {
                            gd = new DatabaseAccess.GroupDetails()
                            {
                                Product = products.Find(delegate(DatabaseAccess.Product prod)
                                {
                                    return prod.Name == productsInfo[0, k];
                                }),
                                Count = int.Parse(productsInfo[1, k]),
                            };
                        }
                        catch
                        {
                            MessageBox.Show("Wypełnij poprawnie wszystkie dane.", "Uwaga");
                            return false;
                        }
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

        /// <summary>
        /// Anuluje tworzenie nowej partii i zamyka okna bez zapisu danych.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
            this.Close();
        }
    }
}
