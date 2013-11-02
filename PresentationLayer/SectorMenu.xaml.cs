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
    /// Menu sektora.
    /// Podgląd sektora i partii znajdujących się w nim.
    /// </summary>
    public partial class SectorMenu : UserControl   // 4
    {
        struct Group
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string SenderName { get; set; }
            public int SenderId { get; set; }
            public bool InternalSender { get; set; }
        }

        private int sectorId;
        //private int warehouseId;
        private DatabaseAccess.Sector sector;
        private List<Group> groups;

        private bool isLoaded;
        private CancellationTokenSource tokenSource;
        private MainWindow mainWindow;

        public SectorMenu(MainWindow mainWindow, int id)
        {
            this.mainWindow = mainWindow;
            mainWindow.Title = "Podgląd Sektora";
            isLoaded = false;
            sectorId = id;
            tokenSource = new CancellationTokenSource();
            mainWindow.ReloadWindow = LoadData;

            InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    sector = (from s in context.Sectors.Include("Warehouse")
                              where s.Id == sectorId
                              select s).FirstOrDefault();

                    groups = new List<Group>();

                    foreach (var g in sector.Groups)
                        groups.Add(new Group()
                        {
                            Id = g.Id,
                            Date = g.GetLastDate(),
                            SenderId = g.GetLastSenderId(),
                            InternalSender = g.IsSenderInternal(),
                            SenderName = g.GetSenderName()
                        });

                    return 0;
                }, t => Dispatcher.BeginInvoke(new Action(() => InitializeData())), tokenSource);
        }

        private void InitializeData()
        {
            LoadingLabel.Visibility = System.Windows.Visibility.Hidden;

            WarehouseSectorLabel.Content = String.Format("Magazyn '{0}', Sektor #{1}", sector.Warehouse.Name, sector.Number);

            GroupsGrid.Items.Clear();

            foreach (Group g in groups)
            {
                GroupsGrid.Items.Add(new
                {
                    Id = g.Id.ToString(),
                    Date = g.Date.ToString(),
                    Name = g.SenderName,
                    Send = "Wyślij",
                });
            }

            GroupsGrid.Visibility = System.Windows.Visibility.Visible;

            CountLabel.Content = String.Format("Zajęte {0} / {1}", sector.Groups.Count, sector.Limit);

            SectorsButton.IsEnabled = true;
            isLoaded = true;
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            ShiftDialog dlg = new ShiftDialog(mainWindow, int.Parse((sender as Button).Tag as string));
            dlg.Show();
        }

        private void FindSender(int id)
        {
            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    int pId = (from p in context.Partners
                               where p.WarehouseId == id
                               select p.Id).FirstOrDefault();

                    return pId;
                }, t => Dispatcher.BeginInvoke(new Action(() => LoadNewMenu(new PartnerMenu(mainWindow, t)))), tokenSource);
        }

        private void SenderButtonClick(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;

            Group group = groups.FirstOrDefault(g => g.Id == int.Parse((sender as Button).Tag as string));


            if (group.InternalSender)
                LoadNewMenu(new WarehouseMenu(mainWindow, group.SenderId, group.SenderName));
            else
                FindSender(group.Id);
        }

        private void IdButtonClick(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new GroupMenu(mainWindow, int.Parse((sender as Button).Tag as string)));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SectorsDialog dlg = new SectorsDialog(mainWindow, sector.WarehouseId, sectorId);
            dlg.Show();
        }

        private void DeleteSector()
        {
            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    DatabaseAccess.Sector sec = (from s in context.Sectors.Include("Warehouse")
                                                 where s.Id == sectorId
                                                 select s).FirstOrDefault();

                    if (sec.Groups.Count != 0)
                    {
                        MessageBox.Show("Sektor nie jest pusty!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return new { F = false, Name = "", WarehouseId = 0 };
                    }

                    var ret = new { F = true, Name = sec.Warehouse.Name, WarehouseId = sec.WarehouseId };

                    sec.Deleted = true;

                    context.SaveChanges();
                    return ret;
                }, t => Dispatcher.BeginInvoke(new Action(() =>
            {
                if (t.F)
                {
                    mainWindow.MainWindowContent.Children.Clear();
                    mainWindow.MainWindowContent.Children.Add(new WarehouseMenu(mainWindow, t.WarehouseId, t.Name));
                }
            })), tokenSource);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy chcesz usunąć ten sektor?", "Uwaga!",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                DeleteSector();
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
            LoadNewMenu(new WarehouseMenu(mainWindow, sector.Warehouse.Id, sector.Warehouse.Name));
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
