using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace DatabaseAccess.Tests
{
    /// <summary>
    /// Metody pomocnicze, wymuszające ponowne wczytanie danych z bazy.
    /// Niezbędne w testach jednostkowych.
    /// </summary>
    static class ExtensionMethods
    {
        /// <summary>
        /// Odłącza wszystkie rekordy z DbContext.
        /// </summary>
        /// <param name="objectContext">ObjectContext</param>
        public static void DetachAll(this ObjectContext objectContext)
        {
            unchecked
            {
                foreach (var entry in objectContext.ObjectStateManager.GetObjectStateEntries(
                    EntityState.Added | EntityState.Deleted |
                    EntityState.Modified | EntityState.Unchanged))
                {
                    if (entry.Entity != null)
                    {
                        objectContext.Detach(entry.Entity);
                    }
                }
            }
        }
        
        /// <summary>
        /// Zwraca ObjectContext z DbContext.
        /// </summary>
        /// <param name="dbContext">Wrapper ObjectContext</param>
        /// <returns>ObjectContext, który chcemy uzyskać</returns>
        public static ObjectContext ObjectContext(this DbContext dbContext)
        {
            return ((IObjectContextAdapter)dbContext).ObjectContext;
        }
    }
}
