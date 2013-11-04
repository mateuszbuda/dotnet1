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
    /// Menu Magazynów.
    /// Wyświetla wszystkie magazyny w systemie.
    /// </summary>
    public partial class WarehousesMenu : UserControl   // 2
    {
        private MainWindow mainWindow;

        /// <summary>
        /// Magazyn do wyświetlenia na ekranie
        /// </summary>
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

        /// <summary>
        /// Ładowanie danych
        /// </summary>
        private void LoadWarehouses()
        {
            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    List<DatabaseAccess.Warehouse> war = context.GetWarehouses();
                    List<Warehouse> _warehouses = new List<Warehouse>();

                    foreach (DatabaseAccess.Warehouse w in war)
                        _warehouses.Add(new Warehouse()
                        {
                            Id = w.Id,
                            Name = w.Name,
                            AllSectors = w.GetAllSectorCount(),
                            EmptySectors = w.GetFreeSectorCount()
                        });

                    return _warehouses;
                },
                t => Dispatcher.BeginInvoke(new Action(() =>
                    {
                        warehouses.Clear();
                        isLoaded = true;
                        warehouses = t;
                        InitializeButtons();
                    })), tokenSource);
        }

        /// <summary>
        /// Magazyn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Inicjalizacja danych
        /// </summary>
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

        /// <summary>
        /// Inicjalizacja manu
        /// </summary>
        /// <param name="mainWindow">Referencja do okna głównego</param>
        public WarehousesMenu(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            mainWindow.Title = "Magazyny";
            tokenSource = new CancellationTokenSource();
            mainWindow.ReloadWindow = LoadWarehouses;

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

            LoadWarehouses();
        }

        /// <summary>
        /// Usunięcie magazynu
        /// </summary>
        /// <param name="id">Id magazynu</param>
        private void DeleteWarehouse(int id)
        {
            DatabaseAccess.SystemContext.Transaction(context =>
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
                        return false;
                    }

                    w.Deleted = true;

                    context.SaveChanges();

                    return true;
                }, t => LoadWarehouses());
        }

        /// <summary>
        /// Usunięcie magazynu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DeleteClick(object sender, RoutedEventArgs e)
        {
            int id = (int)(((e.Source as MenuItem).Parent as ContextMenu).PlacementTarget as Button).Tag;
            string name = (from w in warehouses
                           where w.Id == id
                           select w.Name).FirstOrDefault();

            if (MessageBox.Show("Czy chcesz usunąć magazyn '" + name + "'?", "Uwaga!",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                    DeleteWarehouse(id);
        }

        /// <summary>
        /// Edycja magazynu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditClick(object sender, RoutedEventArgs e)
        {
            int id = (int)(((e.Source as MenuItem).Parent as ContextMenu).PlacementTarget as Button).Tag;

            WarehouseDialog dlg = new WarehouseDialog(mainWindow, id);
            dlg.Show();
        }

        /// <summary>
        /// Menu główne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Grid content = Parent as Grid;

            tokenSource.Cancel();

            content.Children.Remove(this);
            content.Children.Add(new MainMenu(mainWindow));
        }

        /// <summary>
        /// Wyświetlenie danych
        /// </summary>
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

        /// <summary>
        /// Nowy magazyn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            WarehouseDialog dlg = new WarehouseDialog(mainWindow);
            dlg.Show();
        }

        /// <summary>
        /// Zmiana rozmiaru kontrolki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ShowButtons();
        }
    }
}
