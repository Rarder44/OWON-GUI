using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OWON_GUI.Classes
{
    public class OwonSerialCom :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        
        private bool _isLocked  = false;
        /**
         * Gets or sets the state of the power supply's physical keypad
         */
        public bool IsLocked
        {
            get
            { 
                return _isLocked;
            }
            set
            {
                if (_isLocked != value)
                {
                    _isLocked = value;
                    OnPropertyChanged();
                }
            }
        }

        
        private bool _isPowered = false;
        /**
         * Gets or sets the output state ( true -> supplies current )
         */
        public bool IsPowered
        {
            get
            {
                return _isPowered;
            }
            set
            {

                if (_isPowered != value)
                {
                    _isPowered = value;
                    OnPropertyChanged();
                }
            }
        }




    }
}
