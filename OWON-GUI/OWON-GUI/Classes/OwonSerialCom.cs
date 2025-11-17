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
    public class OwonSerialCom :INotifyPropertyChanged
    {

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
        private Task FastFetchDataTask = null;


        private SerialPortBuffered com= null;




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



        public void RawWrite(String s)
        {
            com.Write(s);
        }


    }
}
