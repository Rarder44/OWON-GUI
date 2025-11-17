using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWON_GUI.Classes
{
    public static class Extension
    {
        #region Array

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        public static T[] Resize<T>(this T[] data, int NewSize)
        {
            Array.Resize(ref data, NewSize);
            return data;
        }
        public static T[] SubArray<T>(this T[] data, int index)
        {
            int length = data.Count() - index;
            if (length == 0)
                return null;

            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        #endregion

        public static byte[] ToByteArrayASCII(this String n)
        {
            return Encoding.ASCII.GetBytes(n);
        }
        public static String ToASCIIString(this byte[] s)
        {
            return Encoding.ASCII.GetString(s);
        }


        public static int IndexOf(this byte[] s, byte[] pattern)
        {

            int indice = -1;
            for (int i = 0; i < s.Length - (pattern.Length-1); i++)
            {
                bool trovato = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (s[i + j] != pattern[j])
                    {
                        trovato = false;
                        break;
                    }
                }
                if (trovato)
                {
                    indice = i;
                    break;
                }
            }
            return indice;
        }


        public static byte[] Read(this SerialPort sp, int count)
        {
            byte[] temp = new byte[count];
            sp.Read(temp, 0, temp.Length);
            return temp;
        }
        


    }
}
