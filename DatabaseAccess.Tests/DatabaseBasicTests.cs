using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KellermanSoftware.CompareNetObjects;

namespace DatabaseAccess.Tests
{
    [TestClass]
    public class DatabaseBasicTests : DatabaseTests
    {
        [TestMethod]
        public void Warehouse_BasicTest()
        {
            var compare = new CompareObjects();
            compare.IgnoreObjectTypes = true;
            compare.ElementsToIgnore.AddRange(new string[] { "Sectors", "Sent", "Received", "Owners", "Version" });

            TransactionWithRolllback(context =>
            {
                Warehouse w = CreateWarehouse();

                context.Warehouses.Add(w);
                context.SaveChanges();
                context.ObjectContext().DetachAll();
                Warehouse wc = context.Warehouses.Find(w.Id);

                Assert.IsTrue(wc != w);
                Assert.IsTrue(compare.Compare(w, wc));

                wc.Name = "Y";
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                wc = context.Warehouses.Find(w.Id);

                Assert.IsTrue(wc != w);
                Assert.IsTrue(!compare.Compare(w, wc));

                w.Name = "Y";
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                Assert.IsTrue(compare.Compare(w, wc));
            });
        }

        [TestMethod]
        public void Sector_BasicTest()
        {
            var compare = new CompareObjects();
            compare.IgnoreObjectTypes = true;
            compare.ElementsToIgnore.AddRange(new string[] { "Groups", "Version" });

            TransactionWithRolllback(context =>
            {
                Sector s = CreateSector();

                context.Sectors.Add(s);
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                Sector sc = context.Sectors.Find(s.Id);

                Assert.IsTrue(s != sc);
                Assert.IsTrue(compare.Compare(s, sc));

                sc.Limit = 0;
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                sc = context.Sectors.Find(s.Id);

                Assert.IsTrue(s != sc);
                Assert.IsTrue(!compare.Compare(s, sc));

                s.Limit = 0;
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                Assert.IsTrue(compare.Compare(s, sc));
            });
        }

        [TestMethod]
        public void Product_BasicTest()
        {
            var compare = new CompareObjects();
            compare.IgnoreObjectTypes = true;
            compare.ElementsToIgnore.AddRange(new string[] { "GroupsDetails", "Date", "Version" });

            TransactionWithRolllback(context =>
            {
                Product p = CreateProduct();

                context.Products.Add(p);
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                Product pc = context.Products.Find(p.Id);

                Assert.IsTrue(p != pc);
                Assert.IsTrue(compare.Compare(p, pc));

                pc.Price = 10M;
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                pc = context.Products.Find(p.Id);

                Assert.IsTrue(p != pc);
                Assert.IsTrue(!compare.Compare(p, pc));

                p.Price = 10M;
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                Assert.IsTrue(compare.Compare(p, pc));
            });
        }

        [TestMethod]
        public void Partner_BasicTest()
        {
            var compare = new CompareObjects();
            compare.IgnoreObjectTypes = true;
            compare.ElementsToIgnore.AddRange(new string[] { "Version" });

            TransactionWithRolllback(context =>
            {
                Partner p = CreatePartner();

                context.Partners.Add(p);
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                Partner pc = context.Partners.Find(p.Id);

                Assert.IsTrue(p != pc);
                Assert.IsTrue(compare.Compare(p, pc));

                pc.Num = "10";
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                pc = context.Partners.Find(p.Id);

                Assert.IsTrue(p != pc);
                Assert.IsTrue(!compare.Compare(p, pc));

                p.Num = "10";
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                Assert.IsTrue(compare.Compare(p, pc));
            });
        }

        [TestMethod]
        public void Group_BasicTest()
        {
            var compare = new CompareObjects();
            compare.IgnoreObjectTypes = true;
            compare.ElementsToIgnore.AddRange(new string[] { "GroupDetails", "Shfts", "Version" });

            TransactionWithRolllback(context =>
            {
                Group g = CreateGroup();

                context.Groups.Add(g);
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                Group gc = context.Groups.Find(g.Id);

                Assert.IsTrue(g != gc);
                Assert.IsTrue(compare.Compare(g.Sector, gc.Sector));

                gc.Sector = CreateSector(2);
                context.SaveChanges();

                gc = context.Groups.Find(g.Id);

                Assert.IsTrue(g != gc);
                Assert.IsTrue(!compare.Compare(g.Sector, gc.Sector));
            });
        }

        [TestMethod]
        public void GroupDetails_BasicTest()
        {
            var compare = new CompareObjects();
            compare.IgnoreObjectTypes = true;
            compare.ElementsToIgnore.AddRange(new string[] { "Version" });

            TransactionWithRolllback(context =>
            {
                GroupDetails g = CreateGroupDetails();

                context.GroupsDetails.Add(g);
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                GroupDetails gc = context.GroupsDetails.Find(g.ProductId, g.GroupId);

                Assert.IsTrue(g != gc);
                Assert.IsTrue(compare.Compare(g, gc));

                gc.Count = 10;
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                gc = context.GroupsDetails.Find(g.ProductId, g.GroupId);

                Assert.IsTrue(g != gc);
                Assert.IsTrue(!compare.Compare(g, gc));

                g.Count = 10;
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                Assert.IsTrue(compare.Compare(g, gc));
            });
        }

        [TestMethod]
        public void Shift_BasicTest()
        {
            var compare = new CompareObjects();
            compare.IgnoreObjectTypes = true;
            compare.ElementsToIgnore.AddRange(new string[] { "Date", "Version" });

            TransactionWithRolllback(context =>
            {
                Shift s = CreateShift();

                context.Shifts.Add(s);
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                Shift sc = context.Shifts.Find(s.Id);

                Assert.IsTrue(s != sc);
                Assert.IsTrue(compare.Compare(s, sc));

                sc.Latest = false;
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                sc = context.Shifts.Find(s.Id);

                Assert.IsTrue(s != sc);
                Assert.IsTrue(!compare.Compare(s, sc));

                s.Latest = false;
                context.SaveChanges();
                context.ObjectContext().DetachAll();

                Assert.IsTrue(compare.Compare(s, sc));
            });
        }
    }
}
