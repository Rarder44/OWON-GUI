using Avalonia;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OWON_GUI.Controls
{
    public class Padlock : Avalonia.Controls.Shapes.Path
    {

        static Dictionary<bool, String> datas = new Dictionary<bool, String>(){
            { true, "M8 0a4 4 0 0 1 4 4v2.05a2.5 2.5 0 0 1 2 2.45v5a2.5 2.5 0 0 1-2.5 2.5h-7A2.5 2.5 0 0 1 2 13.5v-5a2.5 2.5 0 0 1 2-2.45V4a4 4 0 0 1 4-4m0 1a3 3 0 0 0-3 3v2h6V4a3 3 0 0 0-3-3" },
            { false, "M12 0a4 4 0 0 1 4 4v2.5h-1V4a3 3 0 1 0-6 0v2h.5A2.5 2.5 0 0 1 12 8.5v5A2.5 2.5 0 0 1 9.5 16h-7A2.5 2.5 0 0 1 0 13.5v-5A2.5 2.5 0 0 1 2.5 6H8V4a4 4 0 0 1 4-4" },
        };
        static Dictionary<bool, IImmutableSolidColorBrush> colors = new Dictionary<bool, IImmutableSolidColorBrush>() {
            { true, Brushes.Red },
            { false, Brushes.Green }
        };

        public static readonly StyledProperty<bool> IsLockedProperty = AvaloniaProperty.Register<Padlock, bool>(nameof(IsLocked), defaultValue: false);


        public Padlock()
        {
            update();
        }


        public bool IsLocked
        {
            get => GetValue(IsLockedProperty);
            set => SetValue(IsLockedProperty, value);
        }
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsLockedProperty)
            {
                update();
            }
        }



        private void update()
        {
            var key = GetValue(IsLockedProperty);
            Fill = colors[key];
            Data = Geometry.Parse(datas[key]);
        }
       
        
    }
}
