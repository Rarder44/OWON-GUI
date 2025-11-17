using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OWON_GUI.Classes
{
    public enum SpeedReadType
    {
        Current,
        Voltage,
        Power,
        Current_Voltage,
        Current_Voltage_Power
    }

    public struct SpeedDataRawEntry
    {
        public long tick;
        public string row;

        public SpeedDataRawEntry(long tick, string row)
        {
            this.tick = tick;
            this.row = row; 
        }

        public override string ToString()
        {
            return tick + " - " + row;
        }
    }

    public class SpeedDataEntry
    {
        long Millis { get; set; }
        long Micros{ get; set; }
        double Current { get; set; }
        double Voltage { get; set; }
        double Power { get; set; }

        public SpeedDataEntry(SpeedDataRawEntry raw, SpeedReadType type)
        {
            Micros = (long)(raw.tick * (1_000_000.0 / Stopwatch.Frequency));
            Millis = Micros / 1000;

            double v,v2,v3;
            String[] splitted;
            switch (type)
            {
                case SpeedReadType.Current:
                    v = double.Parse(raw.row);
                    Current = v;
                    break;
                case SpeedReadType.Voltage:
                    v = double.Parse(raw.row);
                    Voltage = v;
                    break;
                case SpeedReadType.Power:
                    v = double.Parse(raw.row);
                    Power = v;
                    break;

                case SpeedReadType.Current_Voltage:
                    splitted = raw.row.Split(',');
                    v = double.Parse(splitted[0]);
                    v2 = double.Parse(splitted[1]);
                    Voltage = v;
                    Current = v2;
                    break;
                case SpeedReadType.Current_Voltage_Power:
                    splitted = raw.row.Split(',');
                    v = double.Parse(splitted[0]);
                    v2 = double.Parse(splitted[1]);
                    v3 = double.Parse(splitted[2]);
                    Voltage = v;
                    Current = v2;
                    Power = v3;
                    break;

            }
        }

        public override string ToString()
        {
            return Millis + " - " + Current + " | " + Voltage + " | " + Power;
        }

    }

    public class OwonSerialCom :INotifyPropertyChanged
    {
        const int DEVICE_BUFFER_SIZE = 200;  //find by try and error


        #region Exceptions
        public class ComunicationNotStartedException: Exception
        { }
        public class TaskActiveException : Exception
        { }

        public class OWONProtocolException : Exception
        {
            public OWONProtocolException(String  message) : base(message) { }
        }

        #endregion

        #region boilerplate
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Property

        private bool _isLocked  = false;
        /// <summary>
        ///  Gets or sets the state of the power supply's physical keypad
        /// </summary>
        public bool IsLocked
        {
            get
            { 
                return _isLocked;
            }
            set
            {
                Debug.WriteLine("isLocked ->" + (value ? "true" : "false"));
                setDeviceLockStatus(value);
                if (_isLocked != value)
                {
                    _isLocked = value;
                    OnPropertyChanged();
                }
            }
        }

        
        private bool _isPowered = false;
        /// <summary>
        /// Gets or sets the output state ( true -> supplies current )
        /// </summary>
        public bool IsPowered
        {
            get
            {
                return _isPowered;
            }
            set
            {
                setDevicePowered(value);
                if (_isPowered != value)
                {
                    _isPowered = value;
                    OnPropertyChanged();
                }
            }
        }


        private String _firmware = null;
        public String Firmware { get
            {
                if( _firmware!=null )
                    return _firmware;


                if (com == null)
                    return null;
                    //throw new ComunicationNotStartedException();

                if (_taskActive)
                    return null;
                //throw new TaskActiveException();

                acquireDeviceInfo();

                return _firmware; 
            }
            private set
            {
                if (_firmware != value)
                {
                    _firmware = value;
                    OnPropertyChanged();
                }

            }
        }

        private String _type = null;
        public String DeviceType
        {
            get
            {
                if (_type != null)
                    return _type;


                if (com == null)
                    return null;

                if (_taskActive)
                    return null;

                acquireDeviceInfo();

                return _type;
            }
            private set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged();
                }

            }
        }



        private String _sn = null;
        public String SerialNumber
        {
            get
            {
                if (_sn != null)
                    return _sn;


                if (com == null)
                    return null;

                if (_taskActive)
                    return null;

                acquireDeviceInfo();

                return _sn;
            }
            private set
            {
                if (_sn != value)
                {
                    _sn = value;
                    OnPropertyChanged();
                }

            }
        }




        #endregion

        private CancellableTask ContinuosLockTask = null;
        private Task NormalFetchDataTask = null;
        private CancellableTask FastFetchDataTask = null;


        public SerialPortBuffered com= null;




        private bool _taskActive = false;

        public void init(String COM)
        {
            com = new SerialPortBuffered(COM,115200,Parity.None,8,StopBits.One);
            com.Open();

            acquireDeviceInfo();
            IsLocked = false;
        }



        private void acquireDeviceInfo()
        {
            com.Write("*IDN?\n");
            do
            {
                String s = com.ReadLine();
                if (s!=null)
                {
                    String[] tmp= s.Split(",");
                    if (tmp.Length != 4)
                        throw new OWONProtocolException("IDN ( info ) string not correctly formatted");

                    DeviceType = tmp[1];
                    SerialNumber = tmp[2];

                    Firmware = tmp[3];
                    break;
                }
            }
            while (true);

        }


        
        private async Task setDeviceLockStatus(bool toLock)
        {
            if (toLock)
            {
                com.Write("SYST:REM\n"); //SYSTem: REMote

                if (ContinuosLockTask != null && !ContinuosLockTask.InnerTask.IsCompleted)
                {

                    //non fare nulla.. sta già andando?

                }
                else
                {
                    ContinuosLockTask = new CancellableTask(async (ct) =>
                    {
                        do
                        {
                            Debug.WriteLine("bloccato");
                            com.Write("SYST:REM\n");
                            try {
                                await Task.Delay(3000, ct);
                            }
                            catch(Exception _) { }
                            
                        }
                        while (!ct.IsCancellationRequested);

                    });
                    ContinuosLockTask.InnerTask.Start();

                }


            }
            else
            {
                
                if( ContinuosLockTask!= null)
                {
                    ContinuosLockTask.Cancel();
                    await ContinuosLockTask.InnerTask;
                }
                

                com.Write("SYST:LOC\n");// SYSTem: LOCal
            }
        }


        private void setDevicePowered(bool toPower)
        {
            if(toPower)
            {
                com.Write("OUTP 1\n"); 
            }
            else
            {
                com.Write("OUTP 0\n");
            }
        }



        

        
       

        private string getCommand(SpeedReadType type)
        {
            if (type == SpeedReadType.Current)
                return "MEAS:CURR?";
            else if (type == SpeedReadType.Voltage)
                return "MEAS:VOLT?";
            else if (type == SpeedReadType.Power)
                return "MEAS:POW?";
            else if (type == SpeedReadType.Current_Voltage)
                return "MEAS:ALL?";
            else if (type == SpeedReadType.Current_Voltage_Power)
                return "MEAS:ALL:INFO?";

     
            return "MEAS:CURR?";
        }





        private List<SpeedDataRawEntry> rawSpeedData = new List<SpeedDataRawEntry>();
        public int NumRawSpeedData
        {
            get
            {
                return rawSpeedData.Count;
            }
        }

        public void startSpeedRead(SpeedReadType type)
        {
            //interrompo tutto!!!
            

            //svuoto il buffer di lettura da precendenti scritture
            com.ReadAll();


            FastFetchDataTask = new CancellableTask(async (ct) =>
            {
                //calcolo quanto è lungo il comando in byte e in base al buffer del dispositivo so quanti comandi "ripetuti" massimo posso inviare
                String command = getCommand(type);
                int maxNumberOfSend = OwonSerialCom.DEVICE_BUFFER_SIZE / (command.Length + 1);        //+1 per lo \n
                do
                {

                    //invio N comandi 
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < maxNumberOfSend; i++)
                    {
                        sb.Append(command + "\n");
                    }
                    com.Write(sb.ToString());

                    //mentre aspetto la risposta ( circa 50ms ) aggiorno la GUI per non perdere tempo
                    OnPropertyChanged(nameof(NumRawSpeedData));


                    //tengo traccia delle righe lette, leggo linea per linea
                    int CountRows = 0;
                    do
                    {
                        String row;
                        while ((row = com.ReadLine()) == null && !ct.IsCancellationRequested);      //fin quando non leggo una linea, aspetto

                        if (row == null)                        //cancell request arrivato
                            break;


                        //salvo la riga ed il timestamp
                        rawSpeedData.Add(new SpeedDataRawEntry(Stopwatch.GetTimestamp(), row));
                        CountRows++;

                    } while (!ct.IsCancellationRequested && CountRows < maxNumberOfSend);       //continuo fino alla NEsima riga 

                }
                while (!ct.IsCancellationRequested);




            });
            
            FastFetchDataTask.InnerTask.Start();
        }

        public async Task<List<SpeedDataRawEntry>> stopSpeedRead()
        {
            if (FastFetchDataTask != null)
            {

                FastFetchDataTask.Cancel();
                await FastFetchDataTask.InnerTask;
                FastFetchDataTask = null;
            }

            return rawSpeedData;

        }

        



        public void RawWrite(String s)
        {
            if (s == null)
                return;
            com.Write(s);
        }
        

    }
}
