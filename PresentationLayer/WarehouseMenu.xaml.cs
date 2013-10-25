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
    /// Interaction logic for WarehouseMenu.xaml
    /// </summary>
    public partial class WarehouseMenu : UserControl    // 3
    {
        private CancellationTokenSource tokenSource;
        private int warehouseId;
        private DatabaseAccess.Warehouse warehouse;
        private List<DatabaseAccess.Sector> sectors;
        private List<int> sectorsInfo;
        private bool isLoaded;
        private List<Button> buttons;
        private ContextMenu contextMenu;
        private MainWindow mainWindow;

        private void LoadData(Object _token)
        {
            CancellationToken token = (CancellationToken)_token;

            if (token.IsCancellationRequested)
                return;

            using (var context = new DatabaseAccess.SystemContext())
            {
                warehouse = (from w in context.Warehouses
                             where w.Id == warehouseId
                             select w).FirstOrDefault();

                sectors = warehouse.GetSectors();

                sectorsInfo = new List<int>();

                foreach (var s in sectors)
                    sectorsInfo.Add(s.Groups.Count);

                if (token.IsCancellationRequested)
                    return;

                Dispatcher.BeginInvoke(new Action(() => InitializeData()));
            }
        }

        private void InitializeData()
        {
            AddressLabel1.Content = String.Format("{0} {1}, {2} {3}",
                            warehouse.Street, warehouse.Num, warehouse.Code, warehouse.City);

            AddressLabel2.Content = String.Format("Telefon: {0}, e-mail: {1}",
                warehouse.Tel == null ? "Brak" : warehouse.Tel,
                warehouse.Mail == null ? "Brak" : warehouse.Mail);

            WarehouseNameLabel.Content = String.Format("Magazyn '{0}'", warehouse.Name);

            buttons = new List<Button>();

            int i = 0;

            foreach (var s in sectors)
            {
                Button b = new Button();
                b.Content = String.Format("{0}\n{1} / {2}", s.Number, sectorsInfo[i], s.Limit);
                b.Width = 100;
                b.Height = 100;
                b.Tag = s.Id;
                b.Margin = new Thickness(5);
                b.Click += SectorClick;
                b.ContextMenu = contextMenu;
                b.Background = sectorsInfo[i] == s.Limit ? Brushes.Red : Brushes.Green;

                buttons.Add(b);

                ++i;
            }

            LoadingLabel.Visibility = System.Windows.Visibility.Hidden;
            isLoaded = true;

            ShowData();
        }

        private void SectorClick(Object sender, RoutedEventArgs e)
        {
            int id = (int)(e.Source as Button).Tag;
            LoadNewMenu(new SectorMenu(mainWindow, id));
        }

        private void ShowData()
        {
            if (!isLoaded)
                return;

            SectorsGrid.Children.Clear();
            SectorsGrid.ColumnDefinitions.Clear();
            SectorsGrid.RowDefinitions.Clear();

            int n = (int)SectorsGrid.ActualWidth / 112;

            for (int i = 0; i <= n; ++i)
                SectorsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(110) });

            SectorsGrid.ColumnDefinitions.Last().Width = new GridLength(1, GridUnitType.Star);

            for (int i = 0; i <= sectors.Count / n + 1; ++i)
                SectorsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(110) });

            SectorsGrid.RowDefinitions.Last().Height = new GridLength(1, GridUnitType.Star);

            for (int i = 0; i < buttons.Count; ++i)
            {
                Grid.SetColumn(buttons[i], i % n);
                Grid.SetRow(buttons[i], i / n);

                SectorsGrid.Children.Add(buttons[i]);
            }
        }

        public WarehouseMenu(MainWindow mainWindow, int warehouseId, string name)
        {
            this.mainWindow = mainWindow;
            isLoaded = false;
            this.warehouseId = warehouseId;
            tokenSource = new CancellationTokenSource();

            mainWindow.ReloadWindow = new Action(() => LoadData(tokenSource.Token));

            InitializeComponent();

            contextMenu = new System.Windows.Controls.ContextMenu();

            MenuItem item1 = new MenuItem();
            item1.Click += EditClick;
            item1.Header = "Edytuj...";

            MenuItem item2 = new MenuItem();
            item2.Click += DeleteClick;
            item2.Header = "Usuń...";

            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);

            WarehouseNameLabel.Content = String.Format("Magazyn '{0}'", name);

            Task.Factory.StartNew(LoadData, tokenSource.Token, tokenSource.Token);
        }

        private void DeleteSector(Object _token, int id)
        {
            CancellationToken token = (CancellationToken)_token;

            if (token.IsCancellationRequested)
                return;

            using (var context = new DatabaseAccess.SystemContext())
            {
                DatabaseAccess.Sector sec = (from s in context.Sectors
                                             where s.Id == id
                                             select s).FirstOrDefault();

                if (sec.Groups.Count != 0)
                {
                    MessageBox.Show("Sektor nie jest pusty!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                sec.Deleted = true;

                context.SaveChanges();
            }

            if (token.IsCancellationRequested)
                return;

            LoadData(_token);
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            int id = (int)(((e.Source as MenuItem).Parent as ContextMenu).PlacementTarget as Button).Tag;

            if (MessageBox.Show("Czy chcesz usunąć ten sektor?", "Uwaga!",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                Task.Factory.StartNew((Object _token) => DeleteSector(_token, id), tokenSource.Token, tokenSource.Token);
        }

        private void EditClick(object sender, RoutedEventArgs e)
        {
            int id = (int)(((e.Source as MenuItem).Parent as ContextMenu).PlacementTarget as Button).Tag;

            SectorsDialog dlg = new SectorsDialog(mainWindow, id);
            dlg.Show();
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            SectorsCountDialog dlg = new SectorsCountDialog(mainWindow);
            dlg.Show();
        }

        private void DeleteWarehouse(Object _token, int id)
        {
            CancellationToken token = (CancellationToken)_token;

            if (token.IsCancellationRequested)
                return;

            using (var context = new DatabaseAccess.SystemContext())
            {
                DatabaseAccess.Warehouse w = (from x in context.Warehouses
                                              where x.Id == id
                                              select x).FirstOrDefault();

                int c = (from s in w.Sectors
                         where s.Deleted == false
                         where s.Groups.Count != 0
                         select s).Count();

                if (c != 0)
                {
                    MessageBox.Show("Magazyn nie jest pusty!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                w.Deleted = true;

                context.SaveChanges();
            }

            Dispatcher.BeginInvoke(new Action(() => { 
                mainWindow.MainWindowContent.Children.Clear();
                mainWindow.MainWindowContent.Children.Add(new WarehousesMenu(mainWindow));
            }));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy chcesz usunąć magazyn '" + warehouse.Name + "'?", "Uwaga!",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                Task.Factory.StartNew((Object _token) => DeleteWarehouse(_token, warehouse.Id), tokenSource.Token, tokenSource.Token);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            WarehouseDialog dlg = new WarehouseDialog(mainWindow, warehouseId);
            dlg.Show();
        }

        private void LoadNewMenu(UserControl menu)
        {
            Grid content = Parent as Grid;

            tokenSource.Cancel();

            content.Children.Remove(this);
            content.Children.Add(menu);
        }

        private void WarehousesButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new WarehousesMenu(mainWindow));
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new MainMenu(mainWindow));
        }

        private void MenuSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ShowData();
        }
    }
}
