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
    /// Interaction logic for SectorsDialog.xaml
    /// </summary>
    public partial class SectorsDialog : Window     // 15
    {
        private MainWindow mainWindow;
        private CancellationTokenSource tokenSource;
        private int sectorId = -1;
        private int warehouseId = -1;
        private DatabaseAccess.Sector sector;

        public SectorsDialog(MainWindow mainWindow, int warehouseId, int sectorId)
            : this(mainWindow)
        {
            this.warehouseId = warehouseId;

            if (sectorId != -1)
            {
                Header.Content = "Edytuj dane:";
                Title = "Edycja sektora";

                this.sectorId = sectorId;

                Task.Factory.StartNew(LoadData, tokenSource.Token, tokenSource.Token);
            }
        }

        public SectorsDialog(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            tokenSource = new CancellationTokenSource();

            InitializeComponent();

            Header.Content = "Wprowadź dane:";
            Title = "Tworzenie nowego sektora";
        }

        private void LoadData(Object _token)
        {
            CancellationToken token = (CancellationToken)_token;

            if (token.IsCancellationRequested)
                return;

            using (var context = new DatabaseAccess.SystemContext())
            {
                sector = (from w in context.Sectors
                          where w.Id == sectorId
                          select w).FirstOrDefault();

                if (token.IsCancellationRequested)
                    return;

                Dispatcher.BeginInvoke(new Action(() => InitializeData()));
            }
        }

        private void InitializeData()
        {
            NumberTB.Text = sector.Number.ToString();
            CapacityTB.Text = sector.Limit.ToString();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            using (var context = new DatabaseAccess.SystemContext())
            {
                DatabaseAccess.Sector s;

                if (sectorId == -1)
                {
                    s = new DatabaseAccess.Sector();
                    s.WarehouseId = warehouseId;
                    s.Number = int.Parse(NumberTB.Text);
                    s.Limit = int.Parse(CapacityTB.Text);

                    context.Sectors.Add(s);
                }
                else
                {
                    s = (from sector in context.Sectors where sector.Id == sectorId select sector).FirstOrDefault();
                    s.Number = int.Parse(NumberTB.Text);
                    s.Limit = int.Parse(CapacityTB.Text);
                }

                context.SaveChanges();
            }

            mainWindow.ReloadWindow();
            this.Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
