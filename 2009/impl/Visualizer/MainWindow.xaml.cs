using System.Windows;

namespace ICFP2009.Visualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            _portsListBox.Items.Add("123");
        }
    }
}