using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OWON_GUI.Classes
{
    internal class OwonSerialCom :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        private bool _isLocked  = false;
        public bool IsLocked
        {
            get
            { 
                return _isLocked;
            }
            set
            {
                //TODO: invio il comando di lock/unlock
                //richiedo lo stato corrente
                //se è corretto allora cambio il valore, altrimenti...??? eccezione?

                if (_isLocked != value)
                {
                    _isLocked = value;
                    OnPropertyChanged();
                }
            }
        }

        
    }
}
