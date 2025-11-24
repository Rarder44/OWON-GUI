using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using OWON_GUI.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static OWON_GUI.Classes.OwonSerialCom;

namespace OWON_GUI
{
    public partial class MainWindow : Window
    {
        private OwonSerialCom _owonSerialCom = new OwonSerialCom();
        public OwonSerialCom OwonSerialCom { get { return _owonSerialCom; } }







        public MainWindow()
        {
            InitializeComponent();


            comboBoxPorts.ItemsSource = SerialPort.GetPortNames().ToList();

            if (!Design.IsDesignMode)
            {
                comboBoxPorts.ItemsSource = SerialPort.GetPortNames().ToList();
                _owonSerialCom.init(SerialPort.GetPortNames().ToList().FirstOrDefault("COM3"));
            }



            this.DataContext = this;


            if (!Design.IsDesignMode)
            {
                _owonSerialCom.StartNormalReadData();
            }

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


        private void entryTxt_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            var tb = sender as Avalonia.Controls.TextBox;
            TextBox Cloned = tb.CloneAndReplace();
            Cloned.SetValue(ToolTip.TipProperty, "ENTER to confirm value\nESC to cancel");
            Cloned.KeyDown += (object? sender, Avalonia.Input.KeyEventArgs e) =>
            {
                if (e.Key == Avalonia.Input.Key.Escape)
                {
                    Cloned.RestoreOriginal(tb);
                }
                else if (e.Key == Avalonia.Input.Key.Enter)
                {

                    float v = float.Parse(Cloned.Text.Replace(".", ","));
                    if (tb == entryC)
                        OwonSerialCom.Current = v;
                    if (tb == entryCStop)
                        OwonSerialCom.CurrentLimit = v;
                    if (tb == entryV)
                        OwonSerialCom.Voltage = v;
                    if (tb == entryVStop)
                        OwonSerialCom.VoltageLimit = v;

                    Cloned.RestoreOriginal(tb);
                }
            };
            Cloned.Classes.Add("InEdit");
            Cloned.Classes.Remove("ReadOnly");
            Cloned.IsReadOnly = false;
            Cloned.Focus();

        }


        private async void t1_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
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

            _owonSerialCom.acquireSetupValues();


        }
        FastReadType type = FastReadType.Current_Voltage_Power;
        private void t2_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            
        }

        private async void t3_ClickAsync(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            

        }

        private async void t4_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            //_owonSerialCom.RawWrite(demoText1.Text);

            _owonSerialCom.acquireRTValues();

        }

        async private void FastReadingStartStop_Click(object? sender, RoutedEventArgs e)
        {
            if (FastReadDataTypeCombo.SelectedItem == null)
                return;

            FastReadType type = (FastReadType)FastReadDataTypeCombo.SelectedItem;

            if( !_owonSerialCom.IsFastReadingServiceRunning)
            {
                //START
                FastReadDataTypeCombo.IsEditable= false;
                _owonSerialCom.StartFastReadData(type);

            }
            else
            {
                //STOP
                List<FastDataRawEntry> res = await _owonSerialCom.StopFastReadData();
                FastReadDataTypeCombo.IsEditable = true;


                foreach (var item in res)
                {
                    demoText.Text += new FastDataEntry(item, type) + "\n";
                }
            }

        }
    }
}