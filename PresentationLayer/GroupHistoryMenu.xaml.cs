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
    /// Interaction logic for GroupHistoryMenu.xaml
    /// </summary>
    public partial class GroupHistoryMenu : UserControl     // 6
    {
        struct HistoryEntry
        {
            public int SenderId { get; set; }
            public string SenderName { get; set; }
            public int RecipientId { get; set; }
            public string RecipientName { get; set; }
            public string Date { get; set; }
            public Brush SenderColor { get; set; }
            public Brush RecipientColor { get; set; }
        }

        private int groupId;
        private CancellationTokenSource tokenSource;
        private MainWindow mainWindow;
        private List<HistoryEntry> history;

        public GroupHistoryMenu(MainWindow mainWindow, int id)
        {
            groupId = id;
            this.mainWindow = mainWindow;
            tokenSource = new CancellationTokenSource();
            mainWindow.ReloadWindow = new Action(() => { LoadData(tokenSource.Token); });

            InitializeComponent();

            GroupLabel.Content = String.Format("Historia partii #{0}", groupId);
            GroupButton.Content = String.Format("Partia #{0}", groupId);

            Task.Factory.StartNew(LoadData, tokenSource.Token, tokenSource.Token);
            //LoadData(tokenSource.Token);
        }

        private void LoadData(Object _token)
        {
            CancellationToken token = (CancellationToken)_token;

            if (token.IsCancellationRequested)
                return;

            using (var context = new DatabaseAccess.SystemContext())
            {
                var shifts = (from s in context.Shifts.Include("Sender").Include("Recipient")
                              where s.GroupId == groupId
                              orderby s.Date
                              select s).ToList();

                history = new List<HistoryEntry>();

                foreach (var s in shifts)
                    history.Add(new HistoryEntry()
                    {
                        SenderId = s.SenderId.Value,
                        RecipientId = s.RecipientId.Value,
                        SenderName = s.Sender.Name,
                        RecipientName = s.Recipient.Name,
                        Date = s.Date.ToString(),
                        SenderColor = s.Sender.Internal ? Brushes.Silver : Brushes.Green,
                        RecipientColor = s.Recipient.Internal ? Brushes.Silver : Brushes.Green
                    });
            }

            if (token.IsCancellationRequested)
                return;

            Dispatcher.BeginInvoke(new Action(() => InitializeData()));
        }

        private void InitializeData()
        {
            LoadingLabel.Visibility = System.Windows.Visibility.Hidden;

            HistoryGrid.Items.Clear();

            foreach (HistoryEntry h in history)
            {
                HistoryGrid.Items.Add(h);
            }

            HistoryGrid.Visibility = System.Windows.Visibility.Visible;
        }

        private void RecipientSenderClick(CancellationToken token, int id)
        {
            if (token.IsCancellationRequested)
                return;

            bool i;
            int realId = id;
            string name = null;;

            using (var context = new DatabaseAccess.SystemContext())
            {
                i = (from w in context.Warehouses
                     where w.Id == id
                     select w.Internal).FirstOrDefault();

                if (!i)
                    realId = (from p in context.Partners
                              where p.WarehouseId == id
                              select p.Id).FirstOrDefault();
                else
                    name = (from w in context.Warehouses
                            where w.Id == realId
                            select w.Name).FirstOrDefault();

            }

            if (token.IsCancellationRequested)
                return;

            Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (i)
                        LoadNewMenu(new WarehouseMenu(mainWindow, realId, name));
                    else
                        LoadNewMenu(new PartnerMenu(mainWindow, realId));
                }
            ));
        }

        private void SenderButtonClick(object sender, RoutedEventArgs e)
        {
            int id = (int)(sender as Button).Tag;
            (sender as Button).IsEnabled = false;

            Task.Factory.StartNew((object _token) => RecipientSenderClick((CancellationToken)_token, id), 
                tokenSource.Token, tokenSource.Token);
        }

        private void RecipientButtonClick(object sender, RoutedEventArgs e)
        {
            int id = (int)(sender as Button).Tag;
            (sender as Button).IsEnabled = false;

            Task.Factory.StartNew((object _token) => RecipientSenderClick((CancellationToken)_token, id),
                tokenSource.Token, tokenSource.Token);
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new MainMenu(mainWindow));
        }

        private void GroupsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new GroupsMenu(mainWindow));
        }

        private void GroupButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new GroupMenu(mainWindow, groupId));
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
