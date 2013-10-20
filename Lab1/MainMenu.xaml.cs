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

namespace Lab1
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();

            using (SystemContext context = new SystemContext())
            {
                int wCount = context.Warehouses.Count();
                StatBlock1.Text = wCount.ToString();
            }
        }

        private void ButtonWarehouses_Click(object sender, RoutedEventArgs e)
        {
            Grid content = Parent as Grid;

            content.Children.Remove(this);
            content.Children.Add(new WarehouseMenu());
        }
    }
}
