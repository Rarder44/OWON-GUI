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
using OWON_GUI.Converter;

namespace OWON_GUI.Classes
{
   
    public class OwonSerialCom :INotifyPropertyChanged
    {
        public const int DEVICE_BUFFER_SIZE = 200;  //find by try and error



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
        public String Firmware { 
            get
            {
                if( _firmware!=null )
                    return _firmware;


                if (comManager == null)
                    return null;


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


                if (comManager == null)
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


                if (comManager == null)
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



        DateTime invalidationDate= DateTime.MinValue;

        private float _voltageRT = 0;
        public float VoltageRT
        {
            get
            {
                if (DateTime.Now > invalidationDate)
                {
                    //prendi i dati 
                    acquireRTValues();
                }
                return _voltageRT;
            }
        }
        private float _currentRT = 0;
        public float CurrentRT
        {
            get
            {
                if (DateTime.Now > invalidationDate)
                {
                    //prendi i dati 
                    acquireRTValues();
                }
                return _currentRT;
            }
        }
        private float _powerRT = 0;
        public float PowerRT
        {
            get
            {
                if (DateTime.Now > invalidationDate)
                {
                    //prendi i dati 
                    acquireRTValues();
                }
                return _powerRT;
            }
        }





        private float _current = 0;
        public float Current
        {
            get
            {
                return _current;
            }
            set
            {
                setCurrent(value);
                OnPropertyChanged();
            }
        }
        private float _voltage = 0;
        public float Voltage
        {
            get
            {
                return _voltage;
            }
            set
            {
                setVoltage(value);
                OnPropertyChanged();
            }
        }

        private float _currentLimit = 0;
        public float CurrentLimit
        {
            get
            {
                return _currentLimit;
            }
            set
            {
                setCurrentLimit(value);
                _currentLimit = value;
                OnPropertyChanged();
            }
        }
        private float _voltageLimit = 0;
        public float VoltageLimit
        {
            get
            {
                return _voltageLimit;
            }
            set
            {
                setVoltageLimit(value);
                OnPropertyChanged();
            }
        }





        private bool _overVoltage;
        public bool OverVoltage
        {
            get
            {
                if (DateTime.Now > invalidationDate)
                {
                    //prendi i dati 
                    acquireRTValues();
                }
                return _overVoltage;
            }
        }

        private bool _overCurrent;
        public bool OverCurrent
        {
            get
            {
                if (DateTime.Now > invalidationDate)
                {
                    //prendi i dati 
                    acquireRTValues();
                }
                return _overCurrent;
            }
        }

        private bool _overTemperature;
        public bool OverTemperature
        {
            get
            {
                if (DateTime.Now > invalidationDate)
                {
                    //prendi i dati 
                    acquireRTValues();
                }
                return _overTemperature;
            }
        }


        private OperatingModeEnum _operatingMode;
        public OperatingModeEnum OperatingMode
        {
            get
            {
                if (DateTime.Now > invalidationDate)
                {
                    //prendi i dati 
                    acquireRTValues();
                }
                return _operatingMode;
            }
        }







        #endregion



        //------------------------------------------------------------------------
        //------------------------------------------------------------------------
        //------------------------------------------------------------------------



        private CancellableTask ContinuosLockTask = null;
        private CancellableTask NormalFetchDataTask = null;

                

        SerialComunicationManager comManager = null;
         

        async public void init(String COM)
        {
            comManager = new SerialComunicationManager(COM,115200,Parity.None,8,StopBits.One);

            await acquireDeviceInfo();
            await acquireRTValues();
            await acquireSetupValues();
            IsLocked = false;

        }
        async private Task acquireDeviceInfo()
        {

            String s =  await comManager.makeRequest("*IDN?\n");

            String[] tmp = s.Split(",");
            if (tmp.Length != 4)
                throw new OWONProtocolException("IDN ( info ) string not correctly formatted");

            DeviceType = tmp[1];
            SerialNumber = tmp[2];

            Firmware = tmp[3];

        }
        private async Task setDeviceLockStatus(bool toLock)
        {
            if (toLock)
            {
                
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
                            await comManager.makeRequestWithoutResponse("SYST:REM\n"); //SYSTem: REMote
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

                await comManager.makeRequestWithoutResponse("SYST:LOC\n"); //SYSTem: REMote
            }
        }
        private async void setDevicePowered(bool toPower)
        {
            String command;
            if(toPower)
            {
                command = "OUTP 1\n";
            }
            else
            {
                command = "OUTP 0\n";
            }

            await comManager.makeRequestWithoutResponse(command);
        }




        //TODO: mettere private
        async public Task acquireRTValues()
        {
            if (DateTime.Now > invalidationDate)
            {
                String str = await comManager.makeRequest("MEAS:ALL:INFO?\n");
                String[] parts = str.Split(',');
                _voltageRT = float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                _currentRT = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                _powerRT = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                //1 -> current
                //2 -> watt
                //OFF,OFF,OFF,0

                _overVoltage = parts[3] == "ON" || parts[3] == "1";
                _overCurrent = parts[4] == "ON" || parts[4] == "1";
                _overTemperature = parts[5] == "ON" || parts[5] == "1";


                _operatingMode = (OperatingModeEnum)int.Parse(parts[6]);

                if (_operatingMode == OperatingModeEnum.StandBy || _operatingMode == OperatingModeEnum.Failure)
                    _isPowered = false;
                else
                    _isPowered = true;

                invalidationDate = DateTime.Now + TimeSpan.FromSeconds(1);

                OnPropertyChanged(nameof(VoltageRT));
                OnPropertyChanged(nameof(CurrentRT));
                OnPropertyChanged(nameof(PowerRT));
                OnPropertyChanged(nameof(OverVoltage));
                OnPropertyChanged(nameof(OverCurrent));
                OnPropertyChanged(nameof(OverTemperature));
                OnPropertyChanged(nameof(OperatingMode));
                OnPropertyChanged(nameof(IsPowered));
            }
        }

        async public Task acquireSetupValues()
        {
            //CURRent?
            _current = await acquireGenericValue("CURR?");
            OnPropertyChanged(nameof(Current));

            //CURRent:LIMit?
            _currentLimit = await acquireGenericValue("CURR:LIM?");
            OnPropertyChanged(nameof(CurrentLimit));

            //VOLTage?
            _voltage = await acquireGenericValue("VOLT?");
            OnPropertyChanged(nameof(Voltage));

            //VOLTage:LIMit?
            _voltageLimit = await acquireGenericValue("VOLT:LIM?");
            OnPropertyChanged(nameof(VoltageLimit));

        }

        async Task<float> acquireGenericValue(String command)
        {
            command = command.Trim();
            String s = await comManager.makeRequest(command+"\n");
            float v;
            s = s.Trim();
            if (float.TryParse(s, System.Globalization.CultureInfo.InvariantCulture, out v))
                return v;
            else
                throw new OWONProtocolException("CURR? ( CURRent? ) string not correctly formatted");
             
        }


        async public Task acquireOutputStatus()
        {
            String s = await comManager.makeRequest("OUTP?\n");   //OUTPut?

            s = s.Trim();
            
            if (s=="ON" || s=="1")
                _isPowered = true;
            else if (s == "OFF" || s == "0")
                _isPowered = false;
            else
                throw new OWONProtocolException("OUTP? ( OUTPut? ) string not correctly formatted");

            OnPropertyChanged(nameof(IsPowered));

        }


        private async void setCurrent(float current)
        {
           
        }
        private async void setCurrentLimit(float currentLimit)
        {

        }

        private async void setVoltage(float voltage)
        {

        }
        private async void setVoltageLimit(float voltageLimit)
        {

        }




        async public void StartFastReadData(FastReadType type)
        {
            OwnFastReadingService frs = OwnFastReadingService.Instance;
            if (frs.IsRunning )
                throw new InvalidOperationException("Can't start FastReadData because the service is already running.");

            frs.setCom(comManager);
            await frs.Start(type);

        }

        async public Task<List<FastDataRawEntry>> StopFastReadData()
        {
            OwnFastReadingService frs = OwnFastReadingService.Instance;
            return await frs.Stop();
        }


    }
}
