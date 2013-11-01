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
    /// Interaction logic for WarehousesMenu.xaml
    /// </summary>
    public partial class WarehousesMenu : UserControl   // 2
    {
        private MainWindow mainWindow;

        private struct Warehouse
        {
            public string Name { get; set; }
            public int EmptySectors { get; set; }
            public int AllSectors { get; set; }
            public int Id { get; set; }
        }

        private List<Button> buttons;
        private List<Warehouse> warehouses;
        private CancellationTokenSource tokenSource;
        private bool isLoaded;
        private ContextMenu contextMenu;

        private void LoadWarehouses(Object _token)
        {
            CancellationToken token = (CancellationToken)_token;

            if (token.IsCancellationRequested)
                return;

            using (var context = new DatabaseAccess.SystemContext())
            {
                List<DatabaseAccess.Warehouse> war = context.GetWarehouses();

                warehouses.Clear();

                foreach (DatabaseAccess.Warehouse w in war)
                    warehouses.Add(new Warehouse()
                    {
                        Id = w.Id,
                        Name = w.Name,
                        AllSectors = w.GetAllSectorCount(),
                        EmptySectors = w.GetFreeSectorCount()
                    });

                if (token.IsCancellationRequested)
                    return;

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    isLoaded = true;
                    InitializeButtons();
                }
                ));
            }
        }

        private void WarehouseClick(Object sender, RoutedEventArgs e)
        {
            int id = (int)(e.Source as Button).Tag;
            string name = (from w in warehouses
                           where w.Id == id
                           select w.Name).FirstOrDefault();

            Grid content = Parent as Grid;

            tokenSource.Cancel();

            content.Children.Remove(this);
            content.Children.Add(new WarehouseMenu(mainWindow, id, name));
        }

        private void InitializeButtons()
        {
            buttons = new List<Button>();

            foreach (Warehouse w in warehouses)
            {
                Button b = new Button();
                b.Content = String.Format("{0}\n{1} / {2}", w.Name, w.EmptySectors, w.AllSectors);
                b.Width = 100;
                b.Height = 100;
                b.Tag = w.Id;
                b.Margin = new Thickness(5);
                b.Click += WarehouseClick;
                b.ContextMenu = contextMenu;
                b.Background = w.EmptySectors == 0 ? (w.AllSectors == 0 ? Brushes.Silver : Brushes.Red) : Brushes.Green;

                buttons.Add(b);
            }

            LoadingLabel.Visibility = System.Windows.Visibility.Hidden;
            ShowButtons();
        }

        public WarehousesMenu(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            mainWindow.Title = "Magazyny";
            tokenSource = new CancellationTokenSource();
            mainWindow.ReloadWindow = new Action(() => Task.Factory.StartNew(LoadWarehouses, tokenSource.Token, tokenSource.Token));

            isLoaded = false;
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

            warehouses = new List<Warehouse>();

            Task.Factory.StartNew(LoadWarehouses, tokenSource.Token, tokenSource.Token);
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

            LoadWarehouses(_token);
        }

        void DeleteClick(object sender, RoutedEventArgs e)
        {
            int id = (int)(((e.Source as MenuItem).Parent as ContextMenu).PlacementTarget as Button).Tag;
            string name = (from w in warehouses
                           where w.Id == id
                           select w.Name).FirstOrDefault();

            if (MessageBox.Show("Czy chcesz usunąć magazyn '" + name + "'?", "Uwaga!",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                Task.Factory.StartNew((Object _token) => DeleteWarehouse(_token, id), tokenSource.Token, tokenSource.Token);
        }

        private void EditClick(object sender, RoutedEventArgs e)
        {
            int id = (int)(((e.Source as MenuItem).Parent as ContextMenu).PlacementTarget as Button).Tag;

            WarehouseDialog dlg = new WarehouseDialog(mainWindow, id);
            dlg.Show();
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Grid content = Parent as Grid;

            tokenSource.Cancel();

            content.Children.Remove(this);
            content.Children.Add(new MainMenu(mainWindow));
        }

        private void ShowButtons()
        {
            if (!isLoaded)
                return;

            WarehousesGrid.ColumnDefinitions.Clear();
            WarehousesGrid.RowDefinitions.Clear();
            WarehousesGrid.Children.Clear();

            int n = (int)WarehousesGrid.ActualWidth / 112;

            for (int i = 0; i <= n; ++i)
                WarehousesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(110) });

            WarehousesGrid.ColumnDefinitions.Last().Width = new GridLength(1, GridUnitType.Star);

            for (int i = 0; i <= warehouses.Count / n + 1; ++i)
                WarehousesGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(110) });

            WarehousesGrid.RowDefinitions.Last().Height = new GridLength(1, GridUnitType.Star);

            for (int i = 0; i < buttons.Count; ++i)
            {
                Grid.SetColumn(buttons[i], i % n);
                Grid.SetRow(buttons[i], i / n);

                WarehousesGrid.Children.Add(buttons[i]);
            }
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            WarehouseDialog dlg = new WarehouseDialog(mainWindow);
            dlg.Show();
        }

        private void MenuSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ShowButtons();
        }
    }
}
