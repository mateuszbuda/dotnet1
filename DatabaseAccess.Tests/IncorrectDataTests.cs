using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity.Infrastructure;

namespace DatabaseAccess.Tests
{
    [TestClass]
    public class IncorrectDataTests : DatabaseTests
    {
        private readonly string longString = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

        [TestMethod, ExpectedException(typeof(DbUpdateException))]
        public void Warehouse_TooLongTest()
        {
            Warehouse w = CreateWarehouse();
            w.Name = longString;

            TransactionWithRolllback(context =>
            {
                context.Warehouses.Add(w);
                context.SaveChanges();
            });
        }

        [TestMethod, ExpectedException(typeof(DbUpdateException))]
        public void Product_TooLongTest()
        {
            Product p = CreateProduct();
            p.Name = longString;

            TransactionWithRolllback(context =>
            {
                context.Products.Add(p);
                context.SaveChanges();
            });
        }

        [TestMethod, ExpectedException(typeof(DbUpdateException))]
        public void Partner_TooLongTest()
        {
            Partner p = CreatePartner();
            p.City = longString;

            TransactionWithRolllback(context =>
            {
                context.Partners.Add(p);
                context.SaveChanges();
            });
        }
    }
}
