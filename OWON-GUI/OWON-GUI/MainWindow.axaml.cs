using Avalonia.Controls;
using OWON_GUI.Classes;

namespace OWON_GUI
{
    public partial class MainWindow : Window
    {
        OwonSerialCom owonSerialCom = new OwonSerialCom();
        public MainWindow()
        {
            InitializeComponent();
            svgLockStatus.DataContext = owonSerialCom;
            btnLock.DataContext = owonSerialCom;


        }

        private void btnLock_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            owonSerialCom.IsLocked = ! owonSerialCom.IsLocked;
            //svgLockStatus.IsLocked =! svgLockStatus.IsLocked;
        }

       
    }
}