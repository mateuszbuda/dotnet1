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
    /// Umożliwia podgląd historii partii, z uwzględnieniem partnerów.
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
            mainWindow.Title = "Historia Partii";
            tokenSource = new CancellationTokenSource();
            mainWindow.ReloadWindow = LoadData;

            InitializeComponent();

            GroupLabel.Content = String.Format("Historia partii #{0}", groupId);
            GroupButton.Content = String.Format("Partia #{0}", groupId);

            LoadData();
        }

        private void LoadData()
        {
            DatabaseAccess.SystemContext.Transaction(context =>
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

                    return true;
                }, t => Dispatcher.BeginInvoke(new Action(() => InitializeData())), tokenSource);
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

        private void RecipientSenderClick(int id)
        {
            int realId = id;
            string name = null;

            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    bool i = (from w in context.Warehouses
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

                    return i;
                }, t => Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (t)
                        LoadNewMenu(new WarehouseMenu(mainWindow, realId, name));
                    else
                        LoadNewMenu(new PartnerMenu(mainWindow, realId));
                }
            )), tokenSource);           
        }

        private void SenderButtonClick(object sender, RoutedEventArgs e)
        {
            int id = (int)(sender as Button).Tag;
            (sender as Button).IsEnabled = false;

            RecipientSenderClick(id);
        }

        private void RecipientButtonClick(object sender, RoutedEventArgs e)
        {
            int id = (int)(sender as Button).Tag;
            (sender as Button).IsEnabled = false;

            RecipientSenderClick(id);
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
