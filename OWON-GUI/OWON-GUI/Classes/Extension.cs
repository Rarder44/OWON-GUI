using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Core;
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



        private class BindingInfo
        {
            public string Path { get; set; }
            public object DataContext { get; set; }
            public BindingMode Mode { get; set; }

            public AvaloniaProperty controlPropertyToBind { get; set;  }

         
            public  BindingExpressionBase BindingExpression { get; set; } = null;
            public bool isAttached { get
                {
                    return BindingExpression != null; 
                } 
            }


        }


        private static Dictionary<Control, BindingInfo> _suspendableBindings = new();
        public static BindingExpressionBase CreateSuspendableBind(this Control ctr, AvaloniaProperty controlPropertyToBind, String propertyPath, BindingMode mode=BindingMode.TwoWay)
        {
            if (_suspendableBindings.ContainsKey(ctr))
                throw new InvalidOperationException("Suspendable binding already binded");

            _suspendableBindings.Add(ctr, new BindingInfo()
            {
                Path = propertyPath,
                Mode = mode,
                DataContext = null,
                controlPropertyToBind = controlPropertyToBind,
            }) ;

  
            _suspendableBindings[ctr].BindingExpression= ctr.Bind(controlPropertyToBind, new Avalonia.Data.Binding
            {
                Path = propertyPath,
                Mode = mode
            });
            return _suspendableBindings[ctr].BindingExpression;
        }


        public static void BindingSuspend(this Control ctr)
        {
            if (!_suspendableBindings.ContainsKey(ctr))
                throw new InvalidOperationException("No suspendable binding attached");

            if (!_suspendableBindings[ctr].isAttached) return; //already detached

            object data = ctr[_suspendableBindings[ctr].controlPropertyToBind];

            // Sostituisci il binding con uno statico
            ctr.Bind(_suspendableBindings[ctr].controlPropertyToBind, new Avalonia.Data.Binding
            {
                Source = data,
                Mode = BindingMode.OneTime
            });

            _suspendableBindings[ctr].BindingExpression = null;

        }

        public static BindingExpressionBase BindingRestore(this Control ctr)
        {
            if (!_suspendableBindings.ContainsKey(ctr))
                throw new InvalidOperationException("No suspendable binding attached");

            if (_suspendableBindings[ctr].isAttached) return _suspendableBindings[ctr].BindingExpression; //already attached



            // Crea un NUOVO binding con source esplicita
            _suspendableBindings[ctr].BindingExpression = ctr.Bind(_suspendableBindings[ctr].controlPropertyToBind, new Avalonia.Data.Binding
            {
                Path = _suspendableBindings[ctr].Path,
                Mode = _suspendableBindings[ctr].Mode
            });
            return _suspendableBindings[ctr].BindingExpression;


        }











        /// <summary>
        /// Clona il control, lo sostituisce nell'UI nella stessa posizione e nasconde l'originale
        /// </summary>
        /// <returns>Il control clonato senza binding</returns>
        public static T CloneAndReplace<T>(this T original) where T : Control, new()
        {
            if (original == null)
                throw new ArgumentNullException(nameof(original));

            var parent = original.Parent;
            if (parent == null)
                throw new InvalidOperationException("Il control non ha un parent nell'UI");

            // Crea il clone
            var clone = new T();

            // Copia le proprietà principali (senza binding)
            CopyProperties(original, clone);

            // Sostituisci nell'UI in base al tipo di parent
            if (parent is Panel panel)
            {
                var index = panel.Children.IndexOf(original);
                panel.Children.Insert(index, clone);
                original.IsVisible = false;
            }
            else if (parent is Decorator decorator)
            {
                decorator.Child = clone;
                original.IsVisible = false;
            }
            else if (parent is ContentControl contentControl)
            {
                contentControl.Content = clone;
                original.IsVisible = false;
            }
            else if (parent is ItemsControl itemsControl)
            {
                var index = itemsControl.Items.IndexOf(original);
                if (index >= 0)
                {
                    if (itemsControl.Items != null)
                    {
                        itemsControl.Items.Insert(index, clone);
                        original.IsVisible = false;
                    }
                }
            }
            else
            {
                throw new NotSupportedException($"Parent type {parent.GetType().Name} non supportato");
            }

            return clone;
        }

        /// <summary>
        /// Ripristina il control originale nascondendo il clone
        /// </summary>
        public static void RestoreOriginal<T>(this T clone, T original) where T : Control
        {
            if (clone == null || original == null) return;

            var parent = clone.Parent;
            if (parent == null) return;

            // Rimuovi il clone e mostra l'originale
            if (parent is Panel panel)
            {
                panel.Children.Remove(clone);
                original.IsVisible = true;
            }
            else if (parent is Decorator decorator)
            {
                decorator.Child = original;
                original.IsVisible = true;
            }
            else if (parent is ContentControl contentControl)
            {
                contentControl.Content = original;
                original.IsVisible = true;
            }
            else if (parent is ItemsControl itemsControl)
            {
                if (itemsControl.Items != null)
                {
                    itemsControl.Items.Remove(clone);
                    original.IsVisible = true;
                }
            }
        }

        private static void CopyProperties<T>(T source, T target) where T : Control
        {
            if (source == null || target == null) return;

            // Proprietà comuni da copiare (senza binding)
            target.Width = source.Width;
            target.Height = source.Height;
            target.MinWidth = source.MinWidth;
            target.MinHeight = source.MinHeight;
            target.MaxWidth = source.MaxWidth;
            target.MaxHeight = source.MaxHeight;
            target.Margin = source.Margin;
            target.HorizontalAlignment = source.HorizontalAlignment;
            target.VerticalAlignment = source.VerticalAlignment;
            target.IsEnabled = source.IsEnabled;
            target.Opacity = source.Opacity;
            target.Name = source.Name + "_clone";

            // Per TextBox copia anche il testo corrente
            if (source is TextBox sourceTb && target is TextBox targetTb)
            {
                targetTb.Text = sourceTb.Text;
                targetTb.Watermark = sourceTb.Watermark;
                targetTb.MaxLength = sourceTb.MaxLength;
                targetTb.IsReadOnly = sourceTb.IsReadOnly;
            }

            // Per altri tipi di control aggiungi qui le proprietà specifiche
            if (source is Button sourceBtn && target is Button targetBtn)
            {
                targetBtn.Content = sourceBtn.Content;
            }

            // Copia Grid.Row, Grid.Column, ecc. se presenti
            if (Grid.GetRow(source) != 0)
                Grid.SetRow(target, Grid.GetRow(source));
            if (Grid.GetColumn(source) != 0)
                Grid.SetColumn(target, Grid.GetColumn(source));
            if (Grid.GetRowSpan(source) != 1)
                Grid.SetRowSpan(target, Grid.GetRowSpan(source));
            if (Grid.GetColumnSpan(source) != 1)
                Grid.SetColumnSpan(target, Grid.GetColumnSpan(source));
        }






    }
}
