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
        public WarehouseDialog(int id, Action<Object> evt, CancellationToken token) : this(evt, token)
        {
           
        }

        public WarehouseDialog(Action<Object> evt, CancellationToken token)
        {
            InitializeComponent();
        }
    }
}
