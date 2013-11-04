using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseAccess.Tests
{
    [TestClass]
    public class ContextMethodsTests : DatabaseTests
    {
        [TestMethod]
        public void GetInternalGroupsCount_Test()
        {
            TransactionWithRolllback(context =>
                {
                    int x = context.GetInternalGroupsCount();

                    Shift s = CreateShift();
                    context.Shifts.Add(s);
                    context.SaveChanges();

                    int y = context.GetInternalGroupsCount();

                    Assert.IsTrue(x + 1 == y);

                    s.Group.Sector.Warehouse.Internal = false;
                    context.SaveChanges();

                    y = context.GetInternalGroupsCount();
                    Assert.IsTrue(x == y);
                });
        }

        [TestMethod]
        public void GetWarehouses_Test()
        {
            TransactionWithRolllback(context =>
                {
                    List<Warehouse> lw = context.GetWarehouses();

                    foreach (Warehouse w in lw)
                    {
                        Assert.IsTrue(w.Internal);
                        Assert.IsFalse(w.Deleted);
                    }
                });
        }

        [TestMethod]
        public void GetWarehousesCount_Test()
        {
            TransactionWithRolllback(context =>
                {
                    int c = 0;

                    foreach (Warehouse w in context.Warehouses)
                        if (w.Deleted == false && w.Internal == true)
                            c++;

                    int x = context.GetWarehousesCount();

                    Assert.AreEqual(c, x);
                });
        }

        [TestMethod]
        public void GetLastDate_Group_Test()
        {
            TransactionWithRolllback(context =>
                {
                    Shift s = CreateShift();
                    context.Shifts.Add(s);
                    context.SaveChanges();

                    Assert.AreEqual(s.Group.GetLastDate(), s.Date);
                });
        }

        [TestMethod]
        public void GetFreeSectorCount_Warehouse_Test()
        {
            TransactionWithRolllback(context =>
            {
                Sector s = CreateSector(5);
                s.Groups = new HashSet<Group>();
                context.Sectors.Add(s);
                context.SaveChanges();

                Assert.IsTrue(s.Warehouse.GetFreeSectorCount() == 1); 
            });
        }

        [TestMethod]
        public void GetAllSectorCount_Warehouse_Test()
        {
            TransactionWithRolllback(context =>
            {
                Sector s = CreateSector(5);
                context.Sectors.Add(s);
                context.SaveChanges();

                int c = 0;
                foreach (Sector sc in s.Warehouse.Sectors)
                    if (sc.Deleted == false)
                        c++;

                Assert.IsTrue(c == s.Warehouse.GetAllSectorCount());
            });
        }

        [TestMethod]
        public void GetSectors_Warehouse_Test()
        {
            TransactionWithRolllback(context =>
            {
                Sector s = CreateSector();
                context.Sectors.Add(s);
                context.SaveChanges();

                List<Sector> ls = s.Warehouse.GetSectors();

                foreach (Sector ss in ls)
                    Assert.IsTrue(ss.Deleted == false);
            });
        }
    }
}
