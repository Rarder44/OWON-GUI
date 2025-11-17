using Avalonia.Controls;
using Avalonia.Threading;
using OWON_GUI.Classes;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static OWON_GUI.Classes.OwonSerialCom;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OWON_GUI
{
    public partial class MainWindow : Window
    {
        private OwonSerialCom _owonSerialCom = new OwonSerialCom();
        public OwonSerialCom OwonSerialCom { get { return _owonSerialCom; } }




        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            comboBoxPorts.ItemsSource = SerialPort.GetPortNames().ToList();

            _owonSerialCom.init("COM3");

            //_owonSerialCom.com.RawDataReceived += Com_RawDataReceived;

        }

        private void Com_RawDataReceived(object sender, byte[] rawData)
        {
            Dispatcher.UIThread.Post(() => demoText.Text += rawData.ToASCIIString());
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
            //_owonSerialCom.RawWrite("*IDN?\n");

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
        SpeedReadType type = SpeedReadType.Current_Voltage_Power;
        private void t2_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _owonSerialCom.startSpeedRead(type);
        }

        private async void t3_ClickAsync(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            List<SpeedDataRawEntry> res = await _owonSerialCom.stopSpeedRead();
            foreach (var item in res)
            {
                demoText.Text += new SpeedDataEntry(item, type)+"\n";
            }

        }

        private void t4_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _owonSerialCom.RawWrite(demoText1.Text);
        }
    }
}