using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static OWON_GUI.Classes.OwonSerialCom;

namespace OWON_GUI.Classes
{
    /// <summary>
    /// Permette l'accesso gestito alla comunicazione seriale di tipo "richiesta"->"risposta" 
    // è possibile gestire anche una scrittura e lettura RAW 
    /// </summary>
    internal class SerialComunicationManager
    {
        public class RAW_CommandWithoutManualLock : Exception
        { }


        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private Guid? lockOwnerThreadToken = null;



        public SerialPortBuffered com = null;


        public SerialComunicationManager()
        {

        }
        public SerialComunicationManager(String portName, Int32 baudRate, System.IO.Ports.Parity parity, Int32 dataBits, System.IO.Ports.StopBits stopBits)
        {
            SerialPortBuffered tmp = new SerialPortBuffered(portName, baudRate, parity, dataBits, stopBits);
            init(tmp);
        }
        public void init(SerialPortBuffered com)
        {
            if (this.com != null)
            {
                if( this.com.IsOpen ) 
                    this.com.Close();
            }

            this.com = com;

            if (!this.com.IsOpen)
                this.com.Open();

        }


        /*async public Task<byte[]> readAll()
        {

            await semaphore.WaitAsync();
            Debug.WriteLine("S WAIT readAll");
            try
            {
                return com.ReadAll();
            }
            finally
            {
                semaphore.Release();
                Debug.WriteLine("S RELEASE 1");
            }
        }*/

        async public Task<String> makeRequest(String request)
        {
            

            Debug.WriteLine(request.Trim());
            await semaphore.WaitAsync();
            Debug.WriteLine("S WAIT 1");

            //prima di qualsiasi richiesta cancello tutto ciò che c'è nel buffer che potrebbe sballare 
            com.ReadAll();


            try
            {
                com.Write(request);
                String s = await com.ReadLineAsync();
                Debug.WriteLine(s?.Trim());
                return s;
            }
            finally
            {
                semaphore.Release();
                Debug.WriteLine("S RELEASE 1\n");

            }
        }

        async public Task makeRequestWithoutResponse(String request)
        {

           


            Debug.WriteLine(request.Trim());

            await semaphore.WaitAsync();
            Debug.WriteLine("S WAIT 2");

            //prima di qualsiasi richiesta cancello tutto ciò che c'è nel buffer che potrebbe sballare 
            com.ReadAll();

            try
            {
                com.Write(request);
            }
            finally
            {
                semaphore.Release();
                Debug.WriteLine("S RELEASE 2\n");

            }
        }



        async public Task<Guid> Lock()
        {

            await semaphore.WaitAsync();
            Debug.WriteLine("S WAIT 3");

            lockOwnerThreadToken = Guid.NewGuid();
            return lockOwnerThreadToken.Value;
        }

        public void Unlock(Guid? lockToken)
        {
            if (!lockToken.HasValue || lockToken.Value!=lockOwnerThreadToken)
                    throw new InvalidOperationException("Only the thread that acquired the lock can release it.");

            lockOwnerThreadToken = null;
            semaphore.Release();
            Debug.WriteLine("S RELEASE 3");

        }
        private void CheckOwner(Guid? lockToken)
        {
            if (!lockToken.HasValue || lockToken.Value != lockOwnerThreadToken)
                throw new InvalidOperationException("The current thread is not the one that acquired the lock.");
        }



        public void RAW_Write(String data, Guid? lockToken)
        {
            CheckOwner(lockToken);
            com.Write(data);
        }


        public String RAW_ReadLine(Guid? lockToken)
        {
            CheckOwner(lockToken);
            return com.ReadLine();
        }


        async public Task<String> RAW_ReadLineAsync(CancellationToken? cancellationToken, Guid? lockToken)
        {
            CheckOwner(lockToken);
            return await com.ReadLineAsync(cancellationToken);
        }

        public byte[] RAW_ReadAll(Guid? lockToken)
        {
            CheckOwner(lockToken);
            return com.ReadAll();
        }



    }
}
