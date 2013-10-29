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
using System.Windows.Shapes;

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for ShiftDialog.xaml
    /// </summary>
    public partial class ShiftDialog : Window   // 16
    {
        private MainWindow mainWindow;
        private CancellationTokenSource tokenSource;
        private DatabaseAccess.Group group;
        private List<DatabaseAccess.Warehouse> warehouses;
        private bool isLoaded;
        private int groupId;

        public ShiftDialog(MainWindow mainWindow, int groupId)
        {
            this.mainWindow = mainWindow;
            this.groupId = groupId;
            tokenSource = new CancellationTokenSource();

            mainWindow.ReloadWindow = new Action(() => { LoadData(tokenSource.Token); });

            isLoaded = false;
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
                group = (from g in context.Groups.Include("Sector")
                         where g.Id == this.groupId
                         select g).FirstOrDefault();

                warehouses = (from w in context.Warehouses.Include("Sectors")
                              where w.Internal == true
                              select w).ToList();

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    isLoaded = true;
                    InitializeData();
                }
                ));
            }

            if (token.IsCancellationRequested)
                return;
        }

        private void InitializeData()
        {
            if (!isLoaded)
                return;

            Header.Content = "Przesuwanie partii " + groupId.ToString();

            foreach (DatabaseAccess.Warehouse w in warehouses)
                foreach (DatabaseAccess.Sector s in w.Sectors)
                    WarehousesComboBox.Items.Add(w.Name + " - #" + s.Number);
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            using (var context = new DatabaseAccess.SystemContext())
            {
                DatabaseAccess.Shift s = new DatabaseAccess.Shift();

                s.SenderId = group.Sector.WarehouseId;
                s.RecipientId = warehouses.Find(delegate(DatabaseAccess.Warehouse w)
                {
                    return w.Name == ((string)WarehousesComboBox.Text).Substring(0, ((string)WarehousesComboBox.Text).LastIndexOf('#') - 3);
                }).Id;
                MessageBox.Show(s.RecipientId.ToString());
                s.Date = new DateTime(DateTime.Now.Ticks);
                s.Latest = true;

                group.SectorId = int.Parse(((string)WarehousesComboBox.Text).Substring(((string)WarehousesComboBox.Text).LastIndexOf('#') + 1));
                s.Group = group;

                context.Shifts.Add(s);

                context.SaveChanges();
            }

            mainWindow.ReloadWindow();
            this.Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
