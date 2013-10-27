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
using System.Windows.Shapes;

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for PartnerDialog.xaml
    /// </summary>
    public partial class PartnerDialog : Window     // 18
    {
        public PartnerDialog(MainWindow mainWindow, int id)
            : this(mainWindow)
        {
        }

        public PartnerDialog(MainWindow mainWindow)
        {
            InitializeComponent();
        }
    }
}
