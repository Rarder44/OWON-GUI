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
using System.Diagnostics;
using System.IO;
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
            if(comboBoxPorts.Items.Count > 0 )
                comboBoxPorts.SelectedIndex = 0;




            this.DataContext = this;


            OwonFastReadingService.Instance.ReadingUpdate += Instance_ReadingUpdate; ;

        }

        private void Instance_ReadingUpdate(OwonFastReadingService sender, FastDataRawEntry[] rawSpeedData, FastReadType type)
        {
            if (rawSpeedData.Length == 0)
                return;

            FastDataEntry first = new FastDataEntry(rawSpeedData[0], type);
            double capacity = 0;
            String UM = "mAh";

            for (int i = 1; i < rawSpeedData.Length; i++)
            {
                FastDataEntry second=new FastDataEntry(rawSpeedData[i], type);
                capacity += first.calculateCapacity(second);

                first = second;
            }


            if(capacity>1000)
            {
                capacity /= 1000d;
                UM = "Ah";
            }

            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                FastReadValuesNumber.Content = rawSpeedData.Length.ToString();
                FastReadConsumedValue.Content = capacity.ToString("F3");
                FastReadConsumedUM.Content = UM;
            });
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
            if (comboBoxPorts.SelectedItem == null)
                return;


            _owonSerialCom.init((String)comboBoxPorts.SelectedItem);

            btnConnect.IsEnabled=false;
            comboBoxPorts.IsEnabled=false;
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





     


        async private void FastReadingStartStop_Click(object? sender, RoutedEventArgs e)
        {
            if (FastReadDataTypeCombo.SelectedItem == null)
                return;

            FastReadType type = (FastReadType)FastReadDataTypeCombo.SelectedItem;

            if( !_owonSerialCom.IsFastReadingServiceRunning)
            {
                //START
                FastReadDataTypeCombo.IsEnabled = false;


                _owonSerialCom.StartFastReadData(type);

            }
            else
            {
                //STOP
                List<FastDataRawEntry> res = await _owonSerialCom.StopFastReadData();
                FastReadDataTypeCombo.IsEnabled = true;




                List<FastDataEntry> AllData = new List<FastDataEntry>();

                foreach (var item in res)
                {
                    var tmp = new FastDataEntry(item, type);
                    AllData.Add(tmp);                  
                }

                var now = DateTime.Now;
                string fileName = Path.Combine("FastReads", $"{now:yyyy-MM-dd} _ {now:HH-mm-ss}.csv");

                var folder = System.IO.Path.GetDirectoryName(fileName);
                if (!string.IsNullOrEmpty(folder) && !System.IO.Directory.Exists(folder))
                    System.IO.Directory.CreateDirectory(folder);


                File.WriteAllText(fileName, FastDataEntry.CreateCsv(AllData));

                
            }

        }
    }
}