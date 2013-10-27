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
            }

            if (token.IsCancellationRequested)
                return;

            Dispatcher.BeginInvoke(new Action(() => InitializeData()));
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

        private void FindSender(CancellationToken token, int id)
        {
            if (token.IsCancellationRequested)
                return;

            int pId = 0;

            using (var context = new DatabaseAccess.SystemContext())
            {
                pId = (from p in context.Partners
                       where p.WarehouseId == id
                       select p.Id).FirstOrDefault();
            }

            if (token.IsCancellationRequested)
                return;

            Dispatcher.BeginInvoke(new Action(() => LoadNewMenu(new PartnerMenu(mainWindow, pId))));
        }

        private void SenderButtonClick(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;

            Group group = groups.FirstOrDefault(g => g.Id == int.Parse((sender as Button).Tag as string));


            if (group.InternalSender)
                LoadNewMenu(new WarehouseMenu(mainWindow, group.SenderId, group.SenderName));
            else
                Task.Factory.StartNew(new Action(() => FindSender(tokenSource.Token, group.Id)));
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
