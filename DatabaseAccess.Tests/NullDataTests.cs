using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace DatabaseAccess.Tests
{
    [TestClass]
    public class NullDataTests : DatabaseTests
    {
        [TestMethod, ExpectedException(typeof(DbEntityValidationException))]
        public void Warehouse_EmptyTest()
        {
            Warehouse w = CreateWarehouse();
            w.Street = null;

            TransactionWithRolllback(context =>
                {
                    context.Warehouses.Add(w);
                    context.SaveChanges();
                });
        }

        [TestMethod, ExpectedException(typeof(DbUpdateException))]
        public void Sector_EmptyTest()
        {
            Sector s = CreateSector();
            s.Warehouse = null;

            TransactionWithRolllback(context =>
            {
                context.Sectors.Add(s);
                context.SaveChanges();
            });
        }

        [TestMethod, ExpectedException(typeof(DbEntityValidationException))]
        public void Product_EmptyTest()
        {
            Product p = CreateProduct();
            p.Name = null;

            TransactionWithRolllback(context =>
            {
                context.Products.Add(p);
                context.SaveChanges();
            });
        }

        [TestMethod, ExpectedException(typeof(DbUpdateException))]
        public void Group_EmptyTest()
        {
            Group g = CreateGroup();
            g.Sector = null;

            TransactionWithRolllback(context =>
            {
                context.Groups.Add(g);
                context.SaveChanges();
            });
        }

        [TestMethod, ExpectedException(typeof(DbEntityValidationException))]
        public void Partner_EmptyTest()
        {
            Partner p = CreatePartner();
            p.City = null;

            TransactionWithRolllback(context =>
            {
                context.Partners.Add(p);
                context.SaveChanges();
            });
        }

        [TestMethod, ExpectedException(typeof(DbUpdateException))]
        public void Shift_EmptyTest()
        {
            Shift s = CreateShift();
            s.Group = null;

            TransactionWithRolllback(context =>
            {
                context.Shifts.Add(s);
                context.SaveChanges();
            });
        }

        [TestMethod, ExpectedException(typeof(DbUpdateException))]
        public void GroupDetails_EmptyTest()
        {
            GroupDetails g = CreateGroupDetails();
            g.Product = null;

            TransactionWithRolllback(context =>
            {
                context.GroupsDetails.Add(g);
                context.SaveChanges();
            });
        }
    }
}
