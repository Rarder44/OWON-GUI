using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWON_GUI.Classes
{
    internal class OwonFastReadingService 
    {
        // Istanza statica privata e readonly (Lazy<T> garantisce thread-safety)
        private static readonly Lazy<OwonFastReadingService> instance = new Lazy<OwonFastReadingService>(() => new OwonFastReadingService());

        // Costruttore privato per evitare istanziazione esterna
        private OwonFastReadingService()
        {
        }
        // Proprietà pubblica per accedere all'istanza singleton
        public static OwonFastReadingService Instance
        {
            get { return instance.Value; }
        }

        //--------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------

        public class ReadingNotStartedException : Exception
        { }


        public delegate void ReadingUpdateEventHandler(OwonFastReadingService sender, FastDataRawEntry[] rawSpeedData, FastReadType type);
        public event ReadingUpdateEventHandler? ReadingUpdate;



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


                    //mentre aspetto la risposta ( circa 50ms ) invio i dati alla GUI

                    Task.Run(() => ReadingUpdate?.Invoke(this, rawSpeedData.ToArray(), type) );


                    


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
            else if (type == (FastReadType.Current| FastReadType.Voltage))
                return "MEAS:ALL?";
            else if (type == (FastReadType.Current | FastReadType.Voltage| FastReadType.Power))
                return "MEAS:ALL:INFO?";

            //combinazione non prevista, leggo tutto
            return "MEAS:ALL:INFO?";
        }





    }


    public enum FastReadType
    {
        //[Description("A")]
        Current = 1,
        //[Description("V")]
        Voltage = 2,
        //[Description("W")]
        Power = 4,
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
        public long Millis {  get; private set; }
        public long Micros { get; private set; }
        public double Current { get; private set; }
        public double Voltage { get; private set; }
        public double Power { get; private set; }

        public FastDataEntry(FastDataRawEntry raw, FastReadType type)
        {
            Micros = (long)(raw.tick * (1_000_000.0 / Stopwatch.Frequency));
            Millis = Micros / 1000;

            double v, v2, v3;
            String[] splitted;
            switch (type)
            {
                case FastReadType.Current:
                    v = double.Parse(raw.row,System.Globalization.CultureInfo.InvariantCulture);
                    Current = v;
                    break;
                case FastReadType.Voltage:
                    v = double.Parse(raw.row, System.Globalization.CultureInfo.InvariantCulture);
                    Voltage = v;
                    break;
                case FastReadType.Power:
                    v = double.Parse(raw.row, System.Globalization.CultureInfo.InvariantCulture);
                    Power = v;
                    break;

                case FastReadType.Current | FastReadType.Voltage:
                    splitted = raw.row.Split(',');
                    v = double.Parse(splitted[0], System.Globalization.CultureInfo.InvariantCulture);
                    v2 = double.Parse(splitted[1], System.Globalization.CultureInfo.InvariantCulture);
                    Voltage = v;
                    Current = v2;
                    break;
                case FastReadType.Current | FastReadType.Voltage| FastReadType.Power:
                default:
                    splitted = raw.row.Split(',');
                    v = double.Parse(splitted[0], System.Globalization.CultureInfo.InvariantCulture);
                    v2 = double.Parse(splitted[1], System.Globalization.CultureInfo.InvariantCulture);
                    v3 = double.Parse(splitted[2], System.Globalization.CultureInfo.InvariantCulture);
                    Voltage = v;
                    Current = v2;
                    Power = v3;
                    break;
                

            }
        }


        /// <summary>
        /// Calculates the electric charge (capacity) accumulated between two current measurements over time.
        /// </summary>
        /// <param name="end">The <see cref="FastDataEntry"/> instance representing the end measurement in time.</param>
        /// <returns>
        /// The calculated capacity as a <see cref="float"/> value. This is the integral approximation of the current
        /// between the current measurement (this instance) and the end measurement, using the trapezoidal rule:
        /// 
        /// where I1 and I2 are the currents of the start (this) and end entries respectively,
        ///and  Δt is the time interval in seconds.
        /// capacity = (I1+I2)/2 * deltaT
        /// </returns>
        /// <remarks>
        /// - Time difference is computed in microseconds and converted to seconds by dividing by 1,000,000.
        /// - The formula uses the average of the two current values to approximate the integral of current over time.
        /// </remarks>
        public double calculateCapacity(FastDataEntry end)
        {
            double deltaT = (end.Micros - this.Micros)/1_000_000d;    //from micros to seconds
            double ampSeconds = ((this.Current + end.Current) / 2.0d * deltaT );
            return ampSeconds*1000 / 3600d;


        }
        public override string ToString()
        {
            return Millis + " - " + Current + " | " + Voltage + " | " + Power;
        }



        public static string CreateCsv(IEnumerable<FastDataEntry> entrys)
        {
            var separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            var sb = new StringBuilder();


            sb.AppendLine($"Volt{separator}Ampere{separator}Watt");
            foreach (var row in entrys)
            {
                var fields = new List<String>(){
                     row.Voltage.ToString(),
                     row.Current.ToString(),
                     row.Power.ToString(),
                };

                //escape
                for (int i = 0; i < fields.Count; i++)
                {
                    if (fields[i].Contains(separator) || fields[i].Contains("\"") || fields[i].Contains("\n"))
                    {
                        // Escape doppie virgolette e racchiudi il campo tra virgolette
                        fields[i] = fields[i].Replace("\"", "\"\"");
                        fields[i] =  $"\"{fields[i]}\"";
                    }
                }

                sb.AppendLine(string.Join(separator, fields));
            }

            return sb.ToString();
        }

    }


   
}
