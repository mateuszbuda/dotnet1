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
    /// Interaction logic for GroupsMenu.xaml
    /// </summary>
    public partial class GroupsMenu : UserControl   // 10
    {
        private MainWindow mainWindow;
        private CancellationTokenSource tokenSource;
        private List<DatabaseAccess.Group> groups;
        private bool isLoaded;
        private bool showInternal = false;
        private bool showExternal = false;

        public GroupsMenu(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            tokenSource = new CancellationTokenSource();

            mainWindow.ReloadWindow = new Action(() => { LoadData(tokenSource.Token); });

            isLoaded = false;
            InitializeComponent();

            showInternal = (bool)Internal.IsChecked;
            showExternal = (bool)External.IsChecked;

            Task.Factory.StartNew(LoadData, tokenSource.Token, tokenSource.Token);
        }

        private void LoadData(Object _token)
        {
            CancellationToken token = (CancellationToken)_token;

            if (token.IsCancellationRequested)
                return;

            using (var context = new DatabaseAccess.SystemContext())
            {
                groups = (from g in context.Groups.Include("Sector.Warehouse")
                          where true
                          select g).ToList();

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    isLoaded = true;
                    InitializeData();
                }
                ));
            }

            if (token.IsCancellationRequested)
                return;

            Dispatcher.BeginInvoke(new Action(() => InitializeData()));
        }

        private void InitializeData()
        {
            if (!isLoaded)
                return;

            GroupsGrid.Items.Clear();

            foreach (DatabaseAccess.Group g in groups)
            {
                if ((showInternal && g.InInternal()) || (showExternal && !g.InInternal()))
                    GroupsGrid.Items.Add(g);
            }

            LoadingLabel.Visibility = System.Windows.Visibility.Hidden;
            GroupsGrid.Visibility = System.Windows.Visibility.Visible;
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new MainMenu(mainWindow));
        }

        private void LoadNewMenu(UserControl menu)
        {
            Grid content = Parent as Grid;

            tokenSource.Cancel();

            content.Children.Remove(this);
            content.Children.Add(menu);
        }

        private void Internal_Checked(object sender, RoutedEventArgs e)
        {
            if (Internal != null)
            {
                showInternal = (bool)Internal.IsChecked;
                Task.Factory.StartNew(LoadData, tokenSource.Token, tokenSource.Token);
            }
        }

        private void Internal_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Internal != null)
            {
                showInternal = (bool)Internal.IsChecked;
                Task.Factory.StartNew(LoadData, tokenSource.Token, tokenSource.Token);
            }
        }

        private void External_Unchecked(object sender, RoutedEventArgs e)
        {
            if (External != null)
            {
                showExternal = (bool)External.IsChecked;
                Task.Factory.StartNew(LoadData, tokenSource.Token, tokenSource.Token);
            }
        }

        private void External_Click(object sender, RoutedEventArgs e)
        {
            if (External != null)
            {
                showExternal = (bool)External.IsChecked;
                Task.Factory.StartNew(LoadData, tokenSource.Token, tokenSource.Token);
            }
        }
    }
}
