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
    /// Interaction logic for WarehouseDialog.xaml
    /// </summary>
    public partial class WarehouseDialog : Window   // 13
    {
        private MainWindow mainWindow;

        public WarehouseDialog(MainWindow mainWindow, int id) : this(mainWindow)
        {
            
        }

        public WarehouseDialog(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }
    }
}
