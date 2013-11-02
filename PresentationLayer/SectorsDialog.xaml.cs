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
    /// Dodawanie i edycja sektorów w magazynie.
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

                LoadData();
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

        private void LoadData()
        {
            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    sector = (from w in context.Sectors
                              where w.Id == sectorId
                              select w).FirstOrDefault();

                    return true;
                }, t => Dispatcher.BeginInvoke(new Action(() => InitializeData())), tokenSource);
        }

        private void InitializeData()
        {
            NumberTB.Text = sector.Number.ToString();
            CapacityTB.Text = sector.Limit.ToString();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;

            int number = int.Parse(NumberTB.Text);
            int limit = int.Parse(CapacityTB.Text);

            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    DatabaseAccess.Sector s;

                    if (sectorId == -1)
                    {
                        s = new DatabaseAccess.Sector();
                        s.WarehouseId = warehouseId;
                        s.Number = number;
                        s.Limit = limit;

                        context.Sectors.Add(s);
                    }
                    else
                    {
                        s = (from sector in context.Sectors where sector.Id == sectorId select sector).FirstOrDefault();
                        s.Number = number;
                        s.Limit = limit;
                    }

                    context.SaveChanges();

                    return true;
                }, t => Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mainWindow.ReloadWindow();
                        this.Close();
                    })), tokenSource);            
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
            this.Close();
        }
    }
}
