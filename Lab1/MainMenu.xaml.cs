using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;

namespace Lab1
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl // 1
    {
        public MainMenu()
        {
            InitializeComponent();

            using (SystemContext context = new SystemContext())
            {
                int wCount = context.Warehouses.Count();
                //var xx = (from x in context.Warehouses
                //          where x.Id == 1 && x.Deleted == false
                //          select x).Distinct();

                StatBlock1.Text = wCount.ToString();

                //Warehouse w = new Warehouse();
                //w.Tel = "123";
                //w.Street = "aaa";
                //w.Num = "23";
                //w.Name = "Test2";
                //w.Mail = "qqq";
                //w.Internal = true;
                //w.Deleted = false;
                //w.Code = "12345";
                //w.City = "qqq";

                //context.Warehouses.Add(w);
                //context.SaveChanges();

                //Sector s1 = new Sector();
                //s1.Number = 4;
                //s1.Limit = 15;
                //s1.Deleted = false;


                //Warehouse w = (from x in context.Warehouses.Include(y => y.Sectors) where x.WarehouseId == 2 select x).FirstOrDefault();
                //w.Sectors.Add(s1);
                

                //context.SaveChanges();

                TestLabel.Content = (from x in context.Warehouses.Include(w => w.Sectors) // ???????
                                     where x.WarehouseId == 2
                                     select x).FirstOrDefault().Sectors.FirstOrDefault().Limit;
            }
        }

        private void ButtonWarehouses_Click(object sender, RoutedEventArgs e)
        {
            Grid content = Parent as Grid;

            content.Children.Remove(this);
            content.Children.Add(new WarehousesMenu());
        }
    }
}
