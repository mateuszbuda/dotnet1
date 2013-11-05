using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseAccess.Tests
{
    [TestClass]
    public class RelationshipTests : DatabaseTests
    {
        [TestMethod]
        public void Partner_Warehouse_RelationshipTest()
        {
            Warehouse w = CreateWarehouse();
            Sector s1 = CreateSector();
            s1.Warehouse = null;
            Sector s2 = CreateSector();
            s2.Warehouse = null;

            w.Sectors = new HashSet<Sector>(new Sector[] { s1, s2 });

            TransactionWithRolllback(context =>
                {
                    context.Warehouses.Add(w);
                    context.SaveChanges();
                    context.ObjectContext().DetachAll();

                    Warehouse wx = context.Warehouses.Find(w.Id);
                    Assert.IsTrue(wx.Sectors.Count == 2);
                    Assert.AreEqual(wx.Sectors.FirstOrDefault().Id, s1.Id);
                });
        }

        [TestMethod]
        public void Group_Product_RelationshipTest()
        {
            Product p = CreateProduct();
            Group g = CreateGroup();
            GroupDetails gd = CreateGroupDetails();
            gd.Product = p;
            gd.Group = g;

            TransactionWithRolllback(context =>
                {
                    context.GroupsDetails.Add(gd);
                    context.SaveChanges();
                    context.ObjectContext().DetachAll();

                    Group gx = (from q in context.Groups.Include("GroupDetails.Product") where q.Id == g.Id select q).FirstOrDefault();
                    Assert.IsTrue(gx.GroupDetails.Count == 1);

                    Assert.AreEqual(gx.GroupDetails.FirstOrDefault().Product.Id, p.Id);
                });
        }

        [TestMethod]
        public void Shift_Group_Sector_Warehouse_RelationshipTest()
        {
            Warehouse w = CreateWarehouse();
            Sector s = CreateSector();
            s.Warehouse = w;
            Group g = CreateGroup();
            g.Sector = s;
            Shift h = CreateShift();
            h.Group = g;
            h.Recipient = w;
            h.Sender = CreateWarehouse();

            TransactionWithRolllback(context =>
                {
                    context.Shifts.Add(h);
                    context.SaveChanges();
                    context.ObjectContext().DetachAll();

                    Shift hx = (from q in context.Shifts.Include("Group.Sector.Warehouse") where q.Id == h.Id select q).FirstOrDefault();
                    Assert.AreEqual(hx.Group.Sector.Warehouse.Id, hx.Recipient.Id);
                    context.ObjectContext().DetachAll();

                    Warehouse wx = (from q in context.Warehouses.Include("Sectors.Groups.Shifts") where q.Id == w.Id select q).FirstOrDefault();
                    Assert.AreEqual(wx.Sectors.FirstOrDefault().Groups.FirstOrDefault().Shifts.FirstOrDefault().Id, h.Id);
                });
        }
    }
}
