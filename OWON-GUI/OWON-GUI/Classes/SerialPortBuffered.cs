using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OWON_GUI.Classes
{
    public class SerialPortBuffered : IDisposable
    {

        private System.IO.Ports.SerialPort _inter;
        private MemoryStreamMutex _serialStream = new MemoryStreamMutex();


        //see commonInitializer() for the initializzation
        private String endLineChars = null;
        private byte[] endLineCharByte = null;

        public String EndLineChars
        {
            get
            {
                return endLineChars;
            }
            set
            {
                endLineChars = value;
                endLineCharByte = endLineChars.ToByteArrayASCII();
            }
        }



        #region Constructor

        public SerialPortBuffered(System.ComponentModel.IContainer container)
        {
            _inter = new System.IO.Ports.SerialPort(container);
            EventAssotiation();
            commonInitializer();
        }

        public SerialPortBuffered()
        {
            _inter = new System.IO.Ports.SerialPort();
            EventAssotiation();
            commonInitializer();
        }

        public SerialPortBuffered(String portName)
        {
            _inter = new System.IO.Ports.SerialPort(portName);
            EventAssotiation();
            commonInitializer();
        }

        public SerialPortBuffered(String portName, Int32 baudRate)
        {
            _inter = new System.IO.Ports.SerialPort(portName, baudRate);
            EventAssotiation();
            commonInitializer();
        }

        public SerialPortBuffered(String portName, Int32 baudRate, System.IO.Ports.Parity parity)
        {
            _inter = new System.IO.Ports.SerialPort(portName, baudRate, parity);
            EventAssotiation();
            commonInitializer();
        }

        public SerialPortBuffered(String portName, Int32 baudRate, System.IO.Ports.Parity parity, Int32 dataBits)
        {
            _inter = new System.IO.Ports.SerialPort(portName, baudRate, parity, dataBits);
            EventAssotiation();
            commonInitializer();
        }

        public SerialPortBuffered(String portName, Int32 baudRate, System.IO.Ports.Parity parity, Int32 dataBits, System.IO.Ports.StopBits stopBits)
        {
            _inter = new System.IO.Ports.SerialPort(portName, baudRate, parity, dataBits, stopBits);
            EventAssotiation();
            commonInitializer();
        }

        private SerialPortBuffered(System.IO.Ports.SerialPort inter)
        {
            this._inter = inter;
        }


        private void commonInitializer()
        {
            EndLineChars = "\n";
        }
        #endregion

        #region Fields

        static public Int32 InfiniteTimeout;
        #endregion

        #region Properties

        public System.IO.Stream BaseStream
        {
            get
            {
                return _inter.BaseStream;
            }
        }
        public Int32 BaudRate
        {
            get
            {
                return _inter.BaudRate;
            }
            set
            {
                _inter.BaudRate = value;
            }
        }
        public bool BreakState
        {
            get
            {
                return _inter.BreakState;
            }
            set
            {
                _inter.BreakState = value;
            }
        }
        public Int32 BytesToWrite
        {
            get
            {
                return _inter.BytesToWrite;
            }
        }
        public Int32 BytesToRead
        {
            get
            {
                return _inter.BytesToRead;
            }
        }
        public bool CDHolding
        {
            get
            {
                return _inter.CDHolding;
            }
        }
        public bool CtsHolding
        {
            get
            {
                return _inter.CtsHolding;
            }
        }
        public Int32 DataBits
        {
            get
            {
                return _inter.DataBits;
            }
            set
            {
                _inter.DataBits = value;
            }
        }
        public bool DiscardNull
        {
            get
            {
                return _inter.DiscardNull;
            }
            set
            {
                _inter.DiscardNull = value;
            }
        }
        public bool DsrHolding
        {
            get
            {
                return _inter.DsrHolding;
            }
        }
        public bool DtrEnable
        {
            get
            {
                return _inter.DtrEnable;
            }
            set
            {
                _inter.DtrEnable = value;
            }
        }
        public System.Text.Encoding Encoding
        {
            get
            {
                return _inter.Encoding;
            }
            set
            {
                _inter.Encoding = value;
            }
        }
        public System.IO.Ports.Handshake Handshake
        {
            get
            {
                return _inter.Handshake;
            }
            set
            {
                _inter.Handshake = value;
            }
        }
        public bool IsOpen
        {
            get
            {
                return _inter.IsOpen;
            }
        }
        public String NewLine
        {
            get
            {
                return _inter.NewLine;
            }
            set
            {
                _inter.NewLine = value;
            }
        }
        public System.IO.Ports.Parity Parity
        {
            get
            {
                return _inter.Parity;
            }
            set
            {
                _inter.Parity = value;
            }
        }
        public byte ParityReplace
        {
            get
            {
                return _inter.ParityReplace;
            }
            set
            {
                _inter.ParityReplace = value;
            }
        }
        public String PortName
        {
            get
            {
                return _inter.PortName;
            }
            set
            {
                _inter.PortName = value;
            }
        }
        public Int32 ReadBufferSize
        {
            get
            {
                return _inter.ReadBufferSize;
            }
            set
            {
                _inter.ReadBufferSize = value;
            }
        }
        public Int32 ReadTimeout
        {
            get
            {
                return _inter.ReadTimeout;
            }
            set
            {
                _inter.ReadTimeout = value;
            }
        }
        public Int32 ReceivedBytesThreshold
        {
            get
            {
                return _inter.ReceivedBytesThreshold;
            }
            set
            {
                _inter.ReceivedBytesThreshold = value;
            }
        }
        public bool RtsEnable
        {
            get
            {
                return _inter.RtsEnable;
            }
            set
            {
                _inter.RtsEnable = value;
            }
        }
        public System.IO.Ports.StopBits StopBits
        {
            get
            {
                return _inter.StopBits;
            }
            set
            {
                _inter.StopBits = value;
            }
        }
        public Int32 WriteBufferSize
        {
            get
            {
                return _inter.WriteBufferSize;
            }
            set
            {
                _inter.WriteBufferSize = value;
            }
        }
        public Int32 WriteTimeout
        {
            get
            {
                return _inter.WriteTimeout;
            }
            set
            {
                _inter.WriteTimeout = value;
            }
        }
        public ISite Site
        {
            get
            {
                return _inter.Site;
            }
            set
            {
                _inter.Site = value;
            }
        }
        public System.ComponentModel.IContainer Container
        {
            get
            {
                return _inter.Container;
            }
        }
        #endregion

        #region Methods

        public void Close()
        {
            _inter.Close();
        }

        public void DiscardInBuffer()
        {
            _inter.DiscardInBuffer();
        }

        public void DiscardOutBuffer()
        {
            _inter.DiscardOutBuffer();
        }

        public static System.String[] GetPortNames()
        {
            return System.IO.Ports.SerialPort.GetPortNames();
        }

        public void Open()
        {
            _inter.Open();
        }


        public byte[] Read(int offset, int count)
        {
            return _serialStream.Remove(offset, count);
        }

        public byte[] Read(int count)
        {
            return Read(0, count);
        }

        public byte[] ReadAll()
        {
            return Read(0, (int)_serialStream.Position);
        }


        /*
         
        public byte ReadByte()
        {

            serialStream.Position = 0;
            byte car = (byte)serialStream.ReadByte();         //TODO: testare!
            serialStream.Remove(sizeof(byte));        //TODO: testare!
            serialStream.Seek(0, SeekOrigin.End);
            return car;
        }
        public char ReadChar()
        {


            serialStream.Position = 0;
            char car = (char)serialStream.ReadByte();         //TODO: testare!
            serialStream.Remove(sizeof(char));        //TODO: testare!
            serialStream.Seek(0, SeekOrigin.End);
            return car;
        }
        public char? ReadChar(int TimeOutMillis)
        {
            var token = new CancellationTokenSource(TimeOutMillis);
            Task<int?> t = Task<int?>.Factory.StartNew(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (serialStream.Position > 0)
                    {
                        return ReadChar();
                    }
                    Task.Delay(1);
                }
                return null;
            });
            t.Wait();
            return (char?)t.Result;
        }


        
        public byte? ReadByte(int TimeOutMillis)
        {
            var token = new CancellationTokenSource(TimeOutMillis);
            Task<int?> t = Task<int?>.Factory.StartNew(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (serialStream.Position > 0)
                    {
                        return ReadByte();
                    }
                    Task.Delay(1);
                }
                return null;
            });
            t.Wait();
            return (byte?)t.Result;
        }*/

        public String ReadExisting()
        {
            return _serialStream.RemoveAll().ToASCIIString();
        }

        public String ReadLine()
        { 
            return ReadTo(endLineCharByte);
        }

        public String ReadTo(String value)
        {
            return ReadTo(value.ToByteArrayASCII());
        }
        public String ReadTo(byte[] pattern)
        {
            byte[] data= _serialStream.RemoveUntil(pattern);

            if (data.Length>0)
                return data.ToASCIIString();
            else
                return null;
        }


        public int IndexOf(String text)
        {
            return IndexOf(text.ToByteArrayASCII());
        }
        public int IndexOf(byte[] data)
        {
            return _serialStream.IndexOf(data);
        }

        public void Write(String text)
        {
            lock(this)
                _inter.Write(text);
        }

        public void Write(System.Char[] buffer, Int32 offset, Int32 count)
        {
            lock (this)
                _inter.Write(buffer, offset, count);
        }

        public void Write(System.Byte[] buffer, Int32 offset, Int32 count)
        {
            lock (this)
                _inter.Write(buffer, offset, count);
        }

        public void Write(byte b)
        {
            lock (this)
                _inter.Write(new byte[] { b }, 0, 1);
        }

        public void WriteLine(String text)
        {
            lock (this)
                _inter.WriteLine(text);
        }

        public void Dispose()
        {
            _inter.Dispose();
            _serialStream.Dispose();

            /* if (t != null && t.IsAlive)
                 t.Abort();*/
        }

        public String ToString()
        {
            return _inter.ToString();
        }




        public bool Equals(object obj)
        {
            return _inter.Equals(obj);
        }

        public Int32 GetHashCode()
        {
            return _inter.GetHashCode();
        }
       
        public void Write(byte[] buffer)
        {
            _inter.Write(buffer, 0, buffer.Length);
        }


        public SerialPort getInternalSerialPort()
        {
            return _inter;
        }


        #endregion

        #region Events

        public event System.IO.Ports.SerialErrorReceivedEventHandler ErrorReceived;
        public event System.IO.Ports.SerialPinChangedEventHandler PinChanged;
        public event System.IO.Ports.SerialDataReceivedEventHandler LineReceived;
        public delegate void rawDataReceivedEventHandler (object sender, byte[] rawData);
        public event rawDataReceivedEventHandler RawDataReceived;

        public event EventHandler Disposed;
        private void EventAssotiation()
        {
            _inter.ErrorReceived += ErrorReceived;
            _inter.PinChanged += PinChanged;
            _inter.DataReceived += Inter_DataReceived;
            _inter.Disposed += Disposed;
        }

        


        #endregion

        
        private void Inter_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (BytesToRead <= 0)
                return;
      
            byte[] buff = _inter.Read(BytesToRead);

            lock (this)
            {
                _serialStream.Write(buff);
            }

            RawDataReceived?.Invoke(this, buff);

            if (buff.IndexOf(endLineCharByte) != -1)
            {
                LineReceived?.Invoke(this, e);
            }
        }
        

    }
}
