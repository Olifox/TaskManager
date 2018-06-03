using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Dis
{
    public class ProcessModel : INotifyPropertyChanged
    {
        private string _name;
        private string _ram;
        private int _pid;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Ram
        {
            get { return _ram; }
            set
            {
                _ram = Math.Round(Convert.ToDouble(value) / 1024, 1).ToString() + " КБ";
                OnPropertyChanged("Ram");
            }
        }

        public int PID
        {
            get { return _pid; }
            set
            {
                _pid = value;
                OnPropertyChanged("PID");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
