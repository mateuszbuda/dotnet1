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
    /// Interaction logic for GroupMenu.xaml
    /// </summary>
    public partial class GroupMenu : UserControl    // 5
    {
        struct Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Count { get; set; }
            public decimal Price { get; set; }
            public decimal OnePrice { get; set; }
            public string Date { get; set; }
        }

        private MainWindow mainWindow;
        private CancellationTokenSource tokenSource;
        private int groupId;
        private DatabaseAccess.Group group;
        private string warehouseName;
        private bool isInternal;

        private List<Product> products;

        public GroupMenu(MainWindow mainWindow, int id)
        {
            groupId = id;
            this.mainWindow = mainWindow;
            tokenSource = new CancellationTokenSource();

            mainWindow.ReloadWindow = new Action(() => { LoadData(tokenSource.Token); });

            InitializeComponent();

            //Task.Factory.StartNew(LoadData, tokenSource.Token, tokenSource.Token);
            LoadData(tokenSource.Token);
        }

        private void LoadData(Object _token)
        {
            CancellationToken token = (CancellationToken)_token;

            if (token.IsCancellationRequested)
                return;

            using (var context = new DatabaseAccess.SystemContext())
            {
                group = (from g in context.Groups.Include("Sector").Include("GroupDetails.Product")
                         where g.Id == groupId
                         select g).FirstOrDefault();

                int wid = group.Sector.WarehouseId;

                warehouseName = (from w in context.Warehouses
                                 where w.Id == wid
                                 select w.Name).FirstOrDefault();

                isInternal = (from w in context.Warehouses
                              where w.Id == wid
                              select w.Internal).FirstOrDefault();

                products = new List<Product>();

                foreach (var p in group.GroupDetails)
                    products.Add(new Product()
                    {
                        Id = p.ProductId,
                        Name = p.Product.Name,
                        Count = p.Count,
                        Date = p.Product.Date.ToShortDateString(),
                        Price = p.Product.Price * p.Count,
                        OnePrice = p.Product.Price
                    });
            }

            if (token.IsCancellationRequested)
                return;

            Dispatcher.BeginInvoke(new Action(() => InitializeData()));
        }

        private void InitializeData()
        {
            if (!isInternal)
                SendButton.IsEnabled = false;

            LoadingLabel.Visibility = System.Windows.Visibility.Hidden;

            GroupLabel.Content = String.Format("Magazyn '{0}', Sektor #{1}, Partia #{2}", warehouseName, group.Sector.Number, group.Id);

            ProductsGrid.Items.Clear();

            foreach (Product p in products)
            {
                ProductsGrid.Items.Add(p);
            }

            ProductsGrid.Visibility = System.Windows.Visibility.Visible;
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new GroupHistoryMenu(mainWindow, groupId));
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ShiftDialog dlg = new ShiftDialog(mainWindow, groupId);
            dlg.Show();
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new MainMenu(mainWindow));
        }

        private void GroupsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new GroupsMenu(mainWindow));
        }

        private void IdButtonClick(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new ProductMenu(mainWindow, (int)(sender as Button).Tag));
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
