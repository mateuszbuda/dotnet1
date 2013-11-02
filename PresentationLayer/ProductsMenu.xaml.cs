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
    /// Podgląd i edycja produktów.
    /// </summary>
    public partial class ProductsMenu : UserControl     // 11
    {
        private MainWindow mainWindow;
        private CancellationTokenSource tokenSource;
        private List<DatabaseAccess.Product> products;
        private bool isLoaded;

        public ProductsMenu(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            mainWindow.Title = "Produkty";
            tokenSource = new CancellationTokenSource();

            mainWindow.ReloadWindow = LoadData;

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

            ProductsGrid.Items.Clear();

            foreach (DatabaseAccess.Product p in products)
            {
                ProductsGrid.Items.Add(p);
            }

            LoadingLabel.Visibility = System.Windows.Visibility.Hidden;
            ProductsGrid.Visibility = System.Windows.Visibility.Visible;
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            ProductDialog dlg = new ProductDialog(mainWindow, -1);
            dlg.Show();
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

        private void ProductNameClick(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new ProductMenu(mainWindow, (int)(sender as Button).Tag));
        }

        private void EditProductClick(object sender, RoutedEventArgs e)
        {
            ProductDialog dlg = new ProductDialog(mainWindow, (int)(sender as Button).Tag);
            dlg.Show();
        }
    }
}
