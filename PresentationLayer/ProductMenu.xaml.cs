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
    /// Wyświetla informacje o produkcie.
    /// </summary>
    public partial class ProductMenu : UserControl // 12
    {
        private MainWindow mainWindow;
        private int productId;
        private CancellationTokenSource tokenSource;
        private DatabaseAccess.Product product;

        public ProductMenu(MainWindow mainWindow, int id)
        {
            this.mainWindow = mainWindow;
            mainWindow.Title = "Podgląd Produktu";
            productId = id;
            tokenSource = new CancellationTokenSource();
            mainWindow.ReloadWindow = LoadData;

            InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    product = (from p in context.Products
                               where p.Id == productId
                               select p).FirstOrDefault();

                    return true;
                }, t => Dispatcher.BeginInvoke(new Action(() => InitializeData())), tokenSource);
        }

        private void InitializeData()
        {
            LoadingLabel.Visibility = System.Windows.Visibility.Hidden;
            ProductLabel.Content = String.Format("{0}", product.Name);

            ProductGrid.Visibility = System.Windows.Visibility.Visible;

            DateLabel.Content = product.Date == null ? "Nieznana" : product.Date.ToShortDateString();
            PriceLabel.Content = product.Price.ToString();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            ProductDialog dlg = new ProductDialog(mainWindow, productId);
            dlg.Show();
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new MainMenu(mainWindow));
        }

        private void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new ProductsMenu(mainWindow));
        }

        private void LoadNewMenu(UserControl menu)
        {
            Grid content = Parent as Grid;

            tokenSource.Cancel();

            content.Children.Remove(this);
            content.Children.Add(menu);
        }
    }
}
