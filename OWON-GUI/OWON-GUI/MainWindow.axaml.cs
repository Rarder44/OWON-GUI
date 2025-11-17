using Avalonia.Controls;
using Avalonia.Threading;
using OWON_GUI.Classes;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWON_GUI
{
    public partial class MainWindow : Window
    {
        private OwonSerialCom _owonSerialCom = new OwonSerialCom();
        public OwonSerialCom OwonSerialCom { get { return _owonSerialCom; } }




        public MainWindow()
        {
            InitializeComponent();
            //svgLockStatus.DataContext = owonSerialCom;
            //btnLock.DataContext = owonSerialCom;

            this.DataContext = this;

            comboBoxPorts.ItemsSource = SerialPort.GetPortNames().ToList();

            _owonSerialCom.init("COM3");

        }

        private void Port_newReceived(SerialPortBuffered obj, byte[] data)
        {
         //    Dispatcher.UIThread.Post(() => demoText.Text += data.ToASCIIString());
        }

      

     

        private void btnLock_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _owonSerialCom.IsLocked = !_owonSerialCom.IsLocked;
            //svgLockStatus.IsLocked =! svgLockStatus.IsLocked;
        }

        private void btnOnOff_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _owonSerialCom.IsPowered = !_owonSerialCom.IsPowered;
        }

       
        private void btnConnect_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

           
        }

        private async  void t1_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _owonSerialCom.RawWrite("*IDN?\n");

            /* Task t = Task.Run(() =>
             {
                 int rep = 10;
                 string s = "";
                 for(int i=0;i<rep;i++)
                 {
                     s += "*IDN?\n";
                 }
                 port.Write(s);

                 for (int i = 0; i < rep; i++)
                 {
                     string resp;

                     while(true)
                     {
                         string line = port.ReadLine();
                         if( line==null)
                             continue;

                         resp = i + ") " + line;
                         break;
                     }


                     Dispatcher.UIThread.Post(() => demoText.Text += resp);
                 }


             });
            */
            //await t;


        }
        private void t2_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            
        }

        private void t3_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

        }

        private void t4_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

        }
    }
}