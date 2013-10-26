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
using System.Windows.Threading;
//using System.Data.Entity;
using DatabaseAccess;

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl // 1
    {
        private MainWindow mainWindow;

        private void ShowStats(Object _token)
        {
            CancellationToken token = (CancellationToken)_token;

            using (SystemContext context = new SystemContext())
            {
                if (token.IsCancellationRequested)
                    return;

                int wCount = context.Warehouses.Count();

                if (token.IsCancellationRequested)
                    return;

                Dispatcher.BeginInvoke(new Action(() => StatBlock1.Text = wCount.ToString()));
            }
        }

        private CancellationTokenSource tokenSource;

        public MainMenu(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            mainWindow.ReloadWindow = new Action(() => { });

            InitializeComponent();

            //using (SystemContext c = new SystemContext())
            //{
            //    TestLabel.Content = (from s in c.Shifts
            //                         select s).FirstOrDefault().Id;
            //}

            tokenSource = new CancellationTokenSource();
            Task showStats = Task.Factory.StartNew(ShowStats, tokenSource.Token, tokenSource.Token);
        }

        private void ChangeMenu(UserControl menu)
        {
            Grid content = Parent as Grid;

            tokenSource.Cancel();

            content.Children.Remove(this);
            content.Children.Add(menu);
        }

        private void ButtonWarehouses_Click(object sender, RoutedEventArgs e)
        {
            ChangeMenu(new WarehousesMenu(mainWindow));
        }

        private void ButtonPartners_Click(object sender, RoutedEventArgs e)
        {
            ChangeMenu(new PartnersMenu());
        }

        private void ButtonGroups_Click(object sender, RoutedEventArgs e)
        {
            ChangeMenu(new GroupsMenu(mainWindow));
        }

        private void ButtonProducts_Click(object sender, RoutedEventArgs e)
        {
            ChangeMenu(new ProductsMenu(mainWindow));
        }
    }
}
