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
    /// Interaction logic for ProductDialog.xaml
    /// </summary>
    public partial class ProductDialog : Window     // 19
    {
        private MainWindow mainWindow;
        private CancellationTokenSource tokenSource;
        private int productID = -1;
        private DatabaseAccess.Product product;

        public ProductDialog(MainWindow mainWindow, int id)
        {
            this.mainWindow = mainWindow;
            productID = id;
            tokenSource = new CancellationTokenSource();

            InitializeComponent();

            if (productID == -1)
            {
                Header.Content = "Wprowadź dane:";
                Title = "Tworzenie nowego produktu";
            }
            else
            {
                Header.Content = "Edytuj dane:";
                Title = "Edycja produktu";

                Task.Factory.StartNew(LoadData, tokenSource.Token, tokenSource.Token);
            }
        }

        private void LoadData(Object _token)
        {
            CancellationToken token = (CancellationToken)_token;

            if (token.IsCancellationRequested)
                return;

            using (var context = new DatabaseAccess.SystemContext())
            {
                product = (from w in context.Products
                           where w.Id == productID
                           select w).FirstOrDefault();

                if (token.IsCancellationRequested)
                    return;

                Dispatcher.BeginInvoke(new Action(() => InitializeData()));
            }
        }

        private void InitializeData()
        {
            NameTB.Text = product.Name;
            PriceTB.Text = product.Price.ToString();
            DateTB.Text = product.Date.ToString();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            using (var context = new DatabaseAccess.SystemContext())
            {
                DatabaseAccess.Product p;

                if (productID == -1)
                {
                    p = new DatabaseAccess.Product();
                    p.Name = NameTB.Text;
                    p.Price = decimal.Parse(PriceTB.Text);
                    p.Date = DateTime.Parse(DateTB.Text);

                    context.Products.Add(p);
                }
                else
                {
                    p = (from warehouse in context.Products where warehouse.Id == productID select warehouse).FirstOrDefault();
                    p.Name = NameTB.Text;
                    p.Price = decimal.Parse(PriceTB.Text);
                    p.Date = DateTime.Parse(DateTB.Text);
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
