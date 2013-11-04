using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Windows;
using System.Windows.Threading;

namespace DatabaseAccess
{
    /// <summary>
    /// Kontekst bazy danych
    /// </summary>
    public class SystemContext : DbContext
    {
        /// <summary>
        /// Magazyny
        /// </summary>
        public DbSet<Warehouse> Warehouses { get; set; }
        
        /// <summary>
        /// Sektory
        /// </summary>
        public DbSet<Sector> Sectors { get; set; }

        /// <summary>
        /// Partie
        /// </summary>
        public DbSet<Group> Groups { get; set; }

        /// <summary>
        /// Przesunięcia
        /// </summary>
        public DbSet<Shift> Shifts { get; set; }

        /// <summary>
        /// Produkty
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Partnerzy
        /// </summary>
        public DbSet<Partner> Partners { get; set; }

        /// <summary>
        /// Szczegóły grup
        /// </summary>
        public DbSet<GroupDetails> GroupsDetails { get; set; }

        /// <summary>
        /// Transakcja kontekstu
        /// </summary>
        public DbContextTransaction Tran { get; private set; }

        /// <summary>
        /// Metoda wykonująca operacje na bazie danych w transakcji.
        /// Wersja synchroniczna. Przydatna do testów jednostkowych.
        /// </summary>
        /// <typeparam name="T">Typ zwracanej zmiennej</typeparam>
        /// <param name="action">Operacje do wykonania na bazie danych</param>
        /// <returns>Wynik operacji</returns>
        public static T SyncTransaction<T>(Func<SystemContext, T> action)
        {
            using (var context = new SystemContext())
            {
                using (context.Tran = context.Database.BeginTransaction())
                {
                    try
                    {
                        T result = action(context);

                        try { context.Tran.Commit(); }
                        catch { }
                        return result;
                    }
                    catch
                    {
                        context.Tran.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Metoda wykonująca operacje na bazie danych w sposób asynchroniczny.
        /// </summary>
        /// <typeparam name="T">Typ zwracanej wartości</typeparam>
        /// <param name="action">Operacje do wykonania na bazie danych</param>
        /// <param name="continuation">Zadanie do wykonania po zakończeniu transakcji.</param>
        /// <param name="_tokenSource">CancellationTokenSource</param>
        public static void Transaction<T>(Func<SystemContext, T> action, Action<T> continuation = null, CancellationTokenSource _tokenSource = null)
        {
            if (_tokenSource == null)
                _tokenSource = new CancellationTokenSource();

            Task<T> task = new Task<T>((object t) =>
                {
                    CancellationToken token = (CancellationToken)t;

                    try
                    {
                        return SyncTransaction(action);
                    }
                    catch
                    {
                        MessageBox.Show("Błąd wewnętrzny bazy danych.\nAplikacja zostanie zamknięta.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                        throw;
                    }

                }, _tokenSource.Token, _tokenSource.Token);

            if (continuation != null)
                task.ContinueWith((t, o) => { if (!((CancellationToken)o).IsCancellationRequested) continuation(t.Result); },
                    _tokenSource.Token, _tokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
                
            task.Start();
        }

        /// <summary>
        /// Zwraca liczbę partii wewnątrz magazynów
        /// </summary>
        /// <returns>Liczba partii</returns>
        public int GetInternalGroupsCount()
        {
            return (from g in Groups
                    where g.Sector.Warehouse.Internal == true
                    select g).Count();
        }

        /// <summary>
        /// Zapełnienie magazynów
        /// </summary>
        /// <returns>Procent zapełnienia</returns>
        public int GetFillRate()
        {
            int all = 0;
            int full = 0;

            foreach (Warehouse w in Warehouses)
                if (w.Internal == true && w.Deleted == false)
                    foreach (Sector s in w.Sectors)
                    {
                        all += s.Limit;
                        full += s.Groups.Count;
                    }

            return all == 0 ? 0 : (full * 100) / all;
        }

        /// <summary>
        /// Zwraca magazyny wewnętrzne.
        /// </summary>
        /// <returns>Lista magazynów</returns>
        public List<Warehouse> GetWarehouses()
        {
            return (from w in Warehouses
                    where w.Internal == true
                    where w.Deleted == false
                    select w).ToList();
        }

        /// <summary>
        /// Liczba magazynów wewnętrznych
        /// </summary>
        /// <returns>Liczba magazynów</returns>
        public int GetWarehousesCount()
        {
            return (from w in Warehouses
                    where w.Internal == true
                    where w.Deleted == false
                    select w).Count();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shift>()
                .HasRequired(s => s.Recipient)
                .WithMany(w => w.Received)
                .HasForeignKey(s => s.RecipientId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Shift>()
                .HasRequired(s => s.Sender)
                .WithMany(w => w.Sent)
                .HasForeignKey(s => s.SenderId)
                .WillCascadeOnDelete(false);
        }
    }
}
