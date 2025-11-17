using HarfBuzzSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OWON_GUI.Classes
{
    public class MemoryStreamMutex
    {

        System.IO.MemoryStream inter;

        readonly object _locker = new object();

        #region Constructor

        public MemoryStreamMutex()
        {
            inter = new System.IO.MemoryStream();
            EventAssotiation();
        }

        public MemoryStreamMutex(Int32 capacity)
        {
            inter = new System.IO.MemoryStream(capacity);
            EventAssotiation();
        }

        public MemoryStreamMutex(byte[] buffer)
        {
            inter = new System.IO.MemoryStream(buffer);
            EventAssotiation();
        }

        public MemoryStreamMutex(byte[] buffer, bool writable)
        {
            inter = new System.IO.MemoryStream(buffer, writable);
            EventAssotiation();
        }

        public MemoryStreamMutex(byte[] buffer, Int32 index, Int32 count)
        {
            inter = new System.IO.MemoryStream(buffer, index, count);
            EventAssotiation();
        }

        public MemoryStreamMutex(byte[] buffer, Int32 index, Int32 count, bool writable)
        {
            inter = new System.IO.MemoryStream(buffer, index, count, writable);
            EventAssotiation();
        }

        public MemoryStreamMutex(byte[] buffer, Int32 index, Int32 count, bool writable, bool publiclyVisible)
        {
            inter = new System.IO.MemoryStream(buffer, index, count, writable, publiclyVisible);
            EventAssotiation();
        }

        public MemoryStreamMutex(System.IO.MemoryStream inter)
        {
            this.inter = inter;
        }

        #endregion

        #region Fields

        #endregion

        #region Properties

        public bool CanRead
        {
            get
            {
                lock (_locker)
                {
                    return inter.CanRead;
                }
            }
        }
        public bool CanSeek
        {
            get
            {
                lock (_locker)
                {
                    return inter.CanSeek;
                }
            }
        }
        public bool CanWrite
        {
            get
            {
                lock (_locker)
                {
                    return inter.CanWrite;
                }
            }
        }
        public Int32 Capacity
        {
            get
            {
                lock (_locker)
                {
                    return inter.Capacity;
                }
            }
            set
            {
                lock (_locker)
                {
                    inter.Capacity = value;
                }
            }
        }
        public Int64 Length
        {
            get
            {
                lock (_locker)
                {
                    return inter.Length;
                }
            }
        }
        public Int64 Position
        {
            get
            {
                lock (_locker)
                {
                    return inter.Position;
                }
            }
            set
            {
                lock (_locker)
                {
                    inter.Position = value;
                }
            }
        }
        public bool CanTimeout
        {
            get
            {
                lock (_locker)
                {
                    return inter.CanTimeout;
                }
            }
        }
        public Int32 ReadTimeout
        {
            get
            {
                lock (_locker)
                {
                    return inter.ReadTimeout;
                }
            }
            set
            {
                lock (_locker)
                {
                    inter.ReadTimeout = value;
                }
            }
        }
        public Int32 WriteTimeout
        {
            get
            {
                lock (_locker)
                {
                    return inter.WriteTimeout;
                }
            }
            set
            {
                lock (_locker)
                {
                    inter.WriteTimeout = value;
                }
            }
        }
        #endregion

        #region Methods

        public void Flush()
        {
            lock (_locker)
            {
                inter.Flush();
            }
        }

        public Task FlushAsync(System.Threading.CancellationToken cancellationToken)
        {
            lock (_locker)
            {
                return inter.FlushAsync(cancellationToken);
            }
        }

        public byte[] GetBuffer()
        {
            lock (_locker)
            {
                return inter.GetBuffer();
            }
        }

        public byte[] GetSizedBuffer()
        {
            lock (_locker)
            {
                return inter.GetBuffer().SubArray(0, (int)inter.Position);
            }
        }

        public bool TryGetBuffer(out ArraySegment<byte> buffer)
        {
            lock (_locker)
            {
                return inter.TryGetBuffer(out buffer);
            }
        }

        public Int32 Read(byte[] buffer, Int32 offset, Int32 count)
        {
            lock (_locker)
            {
                return inter.Read(buffer, offset, count);
            }
        }

        public Task<Int32> ReadAsync(byte[] buffer, Int32 offset, Int32 count, System.Threading.CancellationToken cancellationToken)
        {
            lock (_locker)
            {
                return inter.ReadAsync(buffer, offset, count, cancellationToken);
            }
        }

        public Int32 ReadByte()
        {
            lock (_locker)
            {
                return inter.ReadByte();
            }
        }

        public Task CopyToAsync(System.IO.Stream destination, Int32 bufferSize, System.Threading.CancellationToken cancellationToken)
        {
            lock (_locker)
            {
                return inter.CopyToAsync(destination, bufferSize, cancellationToken);
            }
        }

        public Int64 Seek(Int64 offset, System.IO.SeekOrigin loc)
        {
            lock (_locker)
            {
                return inter.Seek(offset, loc);
            }
        }

        public void SetLength(Int64 value)
        {
            lock (_locker)
            {
                inter.SetLength(value);
            }
        }

        public byte[] ToArray()
        {
            lock (_locker)
            {
                return inter.ToArray();
            }
        }

        public void Write(byte[] buffer, Int32 offset, Int32 count)
        {
            lock (_locker)
            {
                inter.Write(buffer, offset, count);
            }
        }

        public Task WriteAsync(byte[] buffer, Int32 offset, Int32 count, System.Threading.CancellationToken cancellationToken)
        {
            lock (_locker)
            {
                return inter.WriteAsync(buffer, offset, count, cancellationToken);
            }
        }

        public void WriteByte(byte value)
        {
            lock (_locker)
            {
                inter.WriteByte(value);
            }
        }

        public void WriteTo(System.IO.Stream stream)
        {
            lock (_locker)
            {
                inter.WriteTo(stream);
            }
        }

        public Task CopyToAsync(System.IO.Stream destination)
        {
            lock (_locker)
            {
                return inter.CopyToAsync(destination);
            }
        }

        public Task CopyToAsync(System.IO.Stream destination, Int32 bufferSize)
        {
            lock (_locker)
            {
                return inter.CopyToAsync(destination, bufferSize);
            }
        }

        public void CopyTo(System.IO.Stream destination)
        {
            lock (_locker)
            {
                inter.CopyTo(destination);
            }
        }

        public void CopyTo(System.IO.Stream destination, Int32 bufferSize)
        {
            lock (_locker)
            {
                inter.CopyTo(destination, bufferSize);
            }
        }

        public void Close()
        {
            lock (_locker)
            {
                inter.Close();
            }
        }

        public void Dispose()
        {
            lock (_locker)
            {
                inter.Dispose();
            }
        }

        public Task FlushAsync()
        {
            lock (_locker)
            {
                return inter.FlushAsync();
            }
        }

        public System.IAsyncResult BeginRead(byte[] buffer, Int32 offset, Int32 count, System.AsyncCallback callback, object state)
        {
            lock (_locker)
            {
                return inter.BeginRead(buffer, offset, count, callback, state);
            }
        }

        public Int32 EndRead(System.IAsyncResult asyncResult)
        {
            lock (_locker)
            {
                return inter.EndRead(asyncResult);
            }
        }

        public Task<Int32> ReadAsync(byte[] buffer, Int32 offset, Int32 count)
        {
            lock (_locker)
            {
                return inter.ReadAsync(buffer, offset, count);
            }
        }

        public System.IAsyncResult BeginWrite(byte[] buffer, Int32 offset, Int32 count, System.AsyncCallback callback, object state)
        {
            lock (_locker)
            {
                return inter.BeginWrite(buffer, offset, count, callback, state);
            }
        }

        public void EndWrite(System.IAsyncResult asyncResult)
        {
            lock (_locker)
            {
                inter.EndWrite(asyncResult);
            }
        }

        public Task WriteAsync(byte[] buffer, Int32 offset, Int32 count)
        {
            lock (_locker)
            {
                return inter.WriteAsync(buffer, offset, count);
            }
        }

      

        public String ToString()
        {
            lock (_locker)
            {
                return inter.ToString();
            }
        }

        public bool Equals(object obj)
        {
            lock (_locker)
            {
                return inter.Equals(obj);
            }
        }

        public Int32 GetHashCode()
        {
            lock (_locker)
            {
                return inter.GetHashCode();
            }
        }

        public System.Type GetType()
        {
            lock (_locker)
            {
                return inter.GetType();
            }
        }


        public byte[] Remove(int from, int numBytes)
        {
            //prendpo il buffer di memoria
            //sposto tutti i dati DOPO quelli da rimuovere a 0
            //imposto la nuova lunghezza del buffer pari ai dati rimanenti

            if (numBytes <= 0 || from > inter.Length || from+numBytes>inter.Length)
                return new byte[0];

            byte[] removed = new byte[numBytes];
            lock (_locker)
            {
                byte[] buf = inter.GetBuffer();

                //mi salvo i che poi rimuoverò
                System.Buffer.BlockCopy(buf, from, removed, 0, numBytes);

                int byteAfterSectionToRemove = (int)inter.Length - (from + numBytes);
                // sposto i dati successivi alla sezione da cancellare sopra la sezione da cancellare
                System.Buffer.BlockCopy(buf, from + numBytes, buf, from, byteAfterSectionToRemove);

                //cambio la lunghezza del buffer
                int remaining = (int)inter.Length - numBytes;
                inter.SetLength(remaining);

                //sposto la posizione alla fine
                inter.Seek(0, SeekOrigin.End);

            }

            return removed;
        }


        public byte[] RemoveAll()
        {
            lock (_locker)
                return Remove(0, (int)inter.Position);
        }


        public byte[] RemoveUntil(byte[] searchPattern)
        {
            lock (this)
            {
                int indice = IndexOf(searchPattern);
                if (indice == -1)
                    return new byte[0];

                //prendi i dati prima dell'indice
                return  Remove(0, indice+1); 
            }
        }



        public int IndexOf(byte[] data)
        {
            lock (this)
            {
                byte[] buffer = this.GetSizedBuffer();
                return buffer.IndexOf(data);
            }
        }

        /// <summary>
        /// Permette di leggere qualcosa sul flusso di dati in maniera sincrona, impostando un tempo massimo
        /// </summary>
        /// <param name="timeout"> 0 = infinito, espresso in millisecondi</param>
        public byte[] Read(int count, int timeoutMillisecond = 0)
        {
            lock (_locker)
            {
                //Dichiaro un array lungo quanto passato ( count ) 
                byte[] temp = new byte[count];
                if (timeoutMillisecond == 0)    //se è 0, aspetta all'infinito
                {
                    inter.Read(temp, 0, temp.Length);
                }
                else
                {
                    var result = inter.BeginRead(temp, 0, temp.Length, null, null);        //inizio la lettura
                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeoutMillisecond));        //la interrompo dopo TOT millisecondi
                    if (!success)       //se ha sforato il tempo sollevo un eccezione
                    {
                        throw new SocketException((int)SocketError.TimedOut);
                    }
                }

                return temp;
            }
        }

        public void Write(byte[] buffer)
        {
            lock (_locker)
            {
                inter.Write(buffer, 0, buffer.Length);
            }
        }



        #endregion

        #region Events

        private void EventAssotiation()
        {
        }
        #endregion


    }
}
