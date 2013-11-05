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
    /// Okno dialogowe odpowiedzialne za realizację przesunięć partii.
    /// </summary>
    public partial class ShiftDialog : Window   // 16
    {
        private MainWindow mainWindow;
        private CancellationTokenSource tokenSource;
        private DatabaseAccess.Group group;
        private List<DatabaseAccess.Warehouse> warehouses;
        private bool isLoaded;
        private int groupId;

        /// <summary>
        /// Konstruktor inicjalizujący dane
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="groupId">Id przesuwanej partii</param>
        public ShiftDialog(MainWindow mainWindow, int groupId)
        {
            this.mainWindow = mainWindow;
            this.groupId = groupId;
            tokenSource = new CancellationTokenSource();

            isLoaded = false;
            InitializeComponent();

            LoadData();
        }

        /// <summary>
        /// Ładowanie danych
        /// </summary>
        private void LoadData()
        {
            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    group = (from g in context.Groups.Include("Sector")
                             where g.Id == this.groupId
                             select g).FirstOrDefault();

                    warehouses = (from w in context.Warehouses.Include("Sectors.Groups")
                                  where w.Deleted == false
                                  select w).ToList();

                    return true;
                }, t => Dispatcher.BeginInvoke(new Action(() =>
                {
                    isLoaded = true;
                    InitializeData();
                }
                )), tokenSource);
        }

        /// <summary>
        /// Wyświetlanie danych
        /// </summary>
        private void InitializeData()
        {
            if (!isLoaded)
                return;

            Header.Content = "Przesuwanie partii " + groupId.ToString();

            foreach (DatabaseAccess.Warehouse w in warehouses)
                foreach (DatabaseAccess.Sector s in w.Sectors)
                    if (!s.IsFull())
                        WarehousesComboBox.Items.Add(s);
        }

        /// <summary>
        /// Zapis danych po ich sprawdzeniu i zamknięcie okna
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;

            if (WarehousesComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("Wybierz miejsce docelowe przesunięcia.", "Uwaga");
                (sender as Button).IsEnabled = true;
                return;
            }

            DatabaseAccess.Warehouse recipient =
                ((DatabaseAccess.Sector)WarehousesComboBox.Items[WarehousesComboBox.SelectedIndex]).Warehouse;

            DatabaseAccess.Sector sector = (DatabaseAccess.Sector)WarehousesComboBox.SelectedValue;

            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    List<DatabaseAccess.Shift> shifts = (from sh in context.Shifts
                                                         where sh.GroupId == groupId
                                                         select sh).ToList();

                    foreach (DatabaseAccess.Shift shift in shifts)
                        shift.Latest = false;

                    context.SaveChanges();


                    context.Groups.Attach(group);

                    DatabaseAccess.Shift s = new DatabaseAccess.Shift();

                    s.Sender = group.Sector.Warehouse;
                    s.Recipient = recipient;
                    context.Warehouses.Attach(s.Recipient);
                    s.Date = new DateTime(DateTime.Now.Ticks);
                    s.Latest = true;
                    s.Group = group;

                    group.Sector = sector;

                    context.Shifts.Add(s);

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
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
            this.Close();
        }
    }
}
