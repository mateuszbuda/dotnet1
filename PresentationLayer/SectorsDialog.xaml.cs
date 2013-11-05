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
        private DatabaseAccess.Warehouse warehouse;

        /// <summary>
        /// Konstruktor inicjalizujący dane.
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="warehouseId">Id magazynu dla którego tworzymy/edytujemy sektor</param>
        /// <param name="sectorId">Id edytowanego sektora lub -1 jeśli tworzony jest nowy sektor</param>
        public SectorsDialog(MainWindow mainWindow, int warehouseId, int sectorId)
            : this(mainWindow)
        {
            this.warehouseId = warehouseId;

            if (sectorId != -1)
            {
                Header.Content = "Edytuj dane:";
                Title = "Edycja sektora";

                this.sectorId = sectorId;
            }

            LoadData();
        }

        /// <summary>
        /// Podstawowy konstruktor inicjalizujący podstawowe dane wspólne dla tworzenia i edycji sektora.
        /// </summary>
        /// <param name="mainWindow"></param>
        public SectorsDialog(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            tokenSource = new CancellationTokenSource();

            InitializeComponent();
            this.DataContext = new RegexValidationRule();

            Header.Content = "Wprowadź dane:";
            Title = "Tworzenie nowego sektora";
        }

        /// <summary>
        /// Ładowanie danych
        /// </summary>
        private void LoadData()
        {
            if (sectorId != -1)
            {
                DatabaseAccess.SystemContext.Transaction(context =>
                    {
                        sector = (from s in context.Sectors
                                  where s.Id == sectorId
                                  select s).FirstOrDefault();

                        return true;
                    }, t => Dispatcher.BeginInvoke(new Action(() => InitializeData())), tokenSource);
            }
            DatabaseAccess.SystemContext.Transaction(context =>
            {
                warehouse = (from w in context.Warehouses.Include("Sectors")
                             where w.Id == warehouseId
                             select w).FirstOrDefault();

                return true;
            }, t => Dispatcher.BeginInvoke(new Action(() => InitializeData())), tokenSource);

        }

        /// <summary>
        /// Wyświetlanie danych
        /// </summary>
        private void InitializeData()
        {
            NumberTB.Text = sectorId != -1 ? sector.Number.ToString() : (warehouse.Sectors.Count != 0 ?
                (warehouse.Sectors.Max(s => s.Number) + 1).ToString() : "1");
            CapacityTB.Text = sectorId != -1 ? sector.Limit.ToString() : "1";
        }

        /// <summary>
        /// Zapis danych po ich sprawdzeniu i zamknięcie okna
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;

            int number = int.Parse(NumberTB.Text);

            if (CapacityTB.Text.Length > 10 || CapacityTB.Text.Length < 1)
            {
                MessageBox.Show("Wprowadź poprawną pojemność sektora.", "Uwaga");
                (sender as Button).IsEnabled = true;
                return;
            }
            int limit = 1;
            try
            {
                limit = int.Parse(CapacityTB.Text);
            }
            catch
            {
                MessageBox.Show("Wprowadź poprawną pojemność sektora.", "Uwaga");
                (sender as Button).IsEnabled = true;
                return;
            }

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

        /// <summary>
        /// Zamknięcie okna bez zapisu danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelClick(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
            this.Close();
        }
    }
}
