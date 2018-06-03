using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Win32;

namespace Dis
{
    public class ProcessViewModel : INotifyPropertyChanged
    {
        private ProcessModel _selectedProcess;
        private ObservableCollection<ProcessModel> _processes;
        private RelayCommand _refreshCommand;
        private RelayCommand _killCommand;
        private RelayCommand _startCommand;
        private Dispatcher _dispatcher;
        private Timer _timer;

        public ObservableCollection<ProcessModel> Processes
        {
            get { return _processes; }
            set
            {
                _processes = value;
                OnPropertyChanged("Processes");
            }
        }
        public ProcessModel SelectedProcess
        {
            get { return _selectedProcess; }
            set
            {
                _selectedProcess = value;
                OnPropertyChanged("SelectedPhone");
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand ??
                  (_refreshCommand = new RelayCommand(obj =>
                  {
                      Processes.Clear();
                      _dispatcher.Invoke(new Action(() => GetAllProcesses()));
                  }));
            }
        }

        public RelayCommand KillCommand
        {
            get
            {
                return _killCommand ??
                    (_killCommand = new RelayCommand(obj =>
                      {
                          try
                          {
                              Process.GetProcessById(SelectedProcess.PID).Kill();
                              Processes.Remove(SelectedProcess);
                          }
                          catch { }
                      }));
            }
        }

        public RelayCommand StartCommand
        {
            get
            {
                return _startCommand ??
                    (_startCommand = new RelayCommand(obj =>
                      {
                          string filePath = "";
                          OpenFileDialog ofd = new OpenFileDialog();
                          if (ofd.ShowDialog() == true)
                          {
                              filePath = ofd.FileName;
                          }
                          Process.Start(filePath);
                      }));
            }
        }

        public ProcessViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _dispatcher.Invoke(new Action(() => GetAllProcesses()));          
            _timer = new Timer(new TimerCallback(RefreshTimer), null, 2000, 2000);
        }

        private void RefreshTimer(object state)
        {
            Timer t = (Timer)state;
            _dispatcher.Invoke(new Action(()=>GetAllProcesses()));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void GetAllProcesses()
        {
            var processList = Process.GetProcesses().ToList();
            Processes = new ObservableCollection<ProcessModel>();
            foreach (Process p in processList)
            {
                    Processes.Add(new ProcessModel { Name = p.ProcessName, Ram = p.WorkingSet64.ToString(), PID = p.Id });
            }
            Processes = new ObservableCollection<ProcessModel>(Processes.OrderBy(i => i.Name));
        }

        private void Refresh()
        {
            var processList = Process.GetProcesses().ToList();
            foreach (var p in Processes)
            {
                try
                {
                    p.Ram = processList
                        .Find(x=>x.Id==p.PID)
                        .PeakWorkingSet64
                        .ToString();
                }
                catch (ArgumentException)
                {
                    Processes.Remove(p);
                }
            }
        }
    }
}
