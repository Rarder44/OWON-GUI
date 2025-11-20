using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWON_GUI.Classes
{
    internal class OwnFastReadingService 
    {
        // Istanza statica privata e readonly (Lazy<T> garantisce thread-safety)
        private static readonly Lazy<OwnFastReadingService> instance = new Lazy<OwnFastReadingService>(() => new OwnFastReadingService());

        // Costruttore privato per evitare istanziazione esterna
        private OwnFastReadingService()
        {
        }
        // Proprietà pubblica per accedere all'istanza singleton
        public static OwnFastReadingService Instance
        {
            get { return instance.Value; }
        }

        //--------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------

        public class ReadingNotStartedException : Exception
        { }


        public delegate void ReadingUpdateEventHandler(OwnFastReadingService sender, int dataReceived);
        public event ReadingUpdateEventHandler ReadingUpdate;



        private List<FastDataRawEntry> rawSpeedData = new List<FastDataRawEntry>();
        private CancellableTask FastFetchDataTask = null;

        SerialComunicationManager comManager = null;


        public bool IsRunning { get
            {
                return FastFetchDataTask != null; 
            }  
        }


        public void setCom(SerialComunicationManager comManager)
        { 
            this.comManager = comManager; 
        }





        async public Task Start(FastReadType type)
        {
            
            rawSpeedData.Clear();




            FastFetchDataTask = new CancellableTask(async (ct) =>
            {
                //il manager impedisce che ci siano più processi che lavorino sulla seriale
                Guid? lockToken = await this.comManager.Lock();

                //svuoto il buffer di lettura da precendenti scritture
                this.comManager.RAW_ReadAll(lockToken);


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
                    comManager.RAW_Write(sb.ToString(), lockToken);

                    //mentre aspetto la risposta ( circa 50ms ) aggiorno la GUI per non perdere tempo
                    ReadingUpdate?.Invoke(this, rawSpeedData.Count);


                    //tengo traccia delle righe lette, leggo linea per linea
                    int CountRows = 0;
                    do
                    {

                        String row = await comManager.RAW_ReadLineAsync(ct, lockToken);
                        //while ((row = com.ReadLine()) == null && !ct.IsCancellationRequested) ;      //fin quando non leggo una linea, aspetto

                        if (row == null)                        //cancell request arrivato
                            break;


                        //salvo la riga ed il timestamp
                        rawSpeedData.Add(new FastDataRawEntry(Stopwatch.GetTimestamp(), row));
                        CountRows++;

                    } while (!ct.IsCancellationRequested && CountRows < maxNumberOfSend);       //continuo fino alla NEsima riga 

                }
                while (!ct.IsCancellationRequested);

                this.comManager.Unlock(lockToken);

            });

            FastFetchDataTask.InnerTask.Start();
        }

        public async Task<List<FastDataRawEntry>> Stop()
        {
            if (FastFetchDataTask == null)
                throw new ReadingNotStartedException();

            

            FastFetchDataTask.Cancel();
            await FastFetchDataTask.InnerTask;
            FastFetchDataTask = null;
            return rawSpeedData;

        }




        private string getCommand(FastReadType type)
        {
            if (type == FastReadType.Current)
                return "MEAS:CURR?";
            else if (type == FastReadType.Voltage)
                return "MEAS:VOLT?";
            else if (type == FastReadType.Power)
                return "MEAS:POW?";
            else if (type == FastReadType.Current_Voltage)
                return "MEAS:ALL?";
            else if (type == FastReadType.Current_Voltage_Power)
                return "MEAS:ALL:INFO?";


            return "MEAS:CURR?";
        }





    }


    public enum FastReadType
    {
        Current,
        Voltage,
        Power,
        Current_Voltage,
        Current_Voltage_Power
    }

    public struct FastDataRawEntry
    {
        public long tick;
        public string row;

        public FastDataRawEntry(long tick, string row)
        {
            this.tick = tick;
            this.row = row;
        }

        public override string ToString()
        {
            return tick + " - " + row;
        }
    }

    public class FastDataEntry
    {
        long Millis { get; set; }
        long Micros { get; set; }
        double Current { get; set; }
        double Voltage { get; set; }
        double Power { get; set; }

        public FastDataEntry(FastDataRawEntry raw, FastReadType type)
        {
            Micros = (long)(raw.tick * (1_000_000.0 / Stopwatch.Frequency));
            Millis = Micros / 1000;

            double v, v2, v3;
            String[] splitted;
            switch (type)
            {
                case FastReadType.Current:
                    v = double.Parse(raw.row);
                    Current = v;
                    break;
                case FastReadType.Voltage:
                    v = double.Parse(raw.row);
                    Voltage = v;
                    break;
                case FastReadType.Power:
                    v = double.Parse(raw.row);
                    Power = v;
                    break;

                case FastReadType.Current_Voltage:
                    splitted = raw.row.Split(',');
                    v = double.Parse(splitted[0]);
                    v2 = double.Parse(splitted[1]);
                    Voltage = v;
                    Current = v2;
                    break;
                case FastReadType.Current_Voltage_Power:
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

}
