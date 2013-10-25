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
    /// Interaction logic for SectorMenu.xaml
    /// </summary>
    public partial class SectorMenu : UserControl   // 4
    {
        private int sectorId;
        private bool isLoaded;
        private CancellationTokenSource tokenSource;
        private MainWindow mainWindow;

        public SectorMenu(MainWindow mainWindow, int id)
        {
            this.mainWindow = mainWindow;
            isLoaded = false;
            sectorId = id;
            tokenSource = new CancellationTokenSource();
            mainWindow.ReloadWindow = new Action(() => LoadData(tokenSource.Token));

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

            }

            if (token.IsCancellationRequested)
                return;

            Dispatcher.BeginInvoke(new Action(() => InitializeData()));
        }

        private void InitializeData()
        {
            SectorsButton.IsEnabled = true;
            LoadingLabel.Visibility = System.Windows.Visibility.Hidden;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SectorsDialog dlg = new SectorsDialog(mainWindow, sectorId);
            dlg.Show();
        }

        private void DeleteSector(Object _token)
        {
            CancellationToken token = (CancellationToken)_token;

            if (token.IsCancellationRequested)
                return;

            string name = null;
            int warehouseId = 0;

            using (var context = new DatabaseAccess.SystemContext())
            {
                DatabaseAccess.Sector sec = (from s in context.Sectors.Include("Warehouse")
                                             where s.Id == sectorId
                                             select s).FirstOrDefault();

                if (sec.Groups.Count != 0)
                {
                    MessageBox.Show("Sektor nie jest pusty!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                name = sec.Warehouse.Name;
                warehouseId = sec.WarehouseId;

                sec.Deleted = true;

                context.SaveChanges();
            }

            if (token.IsCancellationRequested)
                return;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                mainWindow.MainWindowContent.Children.Clear();
                mainWindow.MainWindowContent.Children.Add(new WarehouseMenu(mainWindow, warehouseId, name));
            }));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy chcesz usunąć ten sektor?", "Uwaga!",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                Task.Factory.StartNew((Object _token) => DeleteSector(_token), tokenSource.Token, tokenSource.Token);
        }

        private void NewGroupButton_Click(object sender, RoutedEventArgs e)
        {
            GroupDialog dlg = new GroupDialog(mainWindow, sectorId);
            dlg.Show();
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new MainMenu(mainWindow));
        }

        private void SectorsButton_Click(object sender, RoutedEventArgs e)
        {
            //LoadNewMenu(new WarehouseMenu(mainWindow, ));
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
