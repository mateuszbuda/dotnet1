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
    /// Interaction logic for ProductsMenu.xaml
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

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    isLoaded = true;
                    InitializeData();
                }
                ));
            }

            if (token.IsCancellationRequested)
                return;

            Dispatcher.BeginInvoke(new Action(() => InitializeData()));
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
