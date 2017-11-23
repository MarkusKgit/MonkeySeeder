using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace MonkeySeeder.ViewModels
{
    public class MainWindowVM : BaseVM
    {
        public bool ShowProgressBar
        {
            get
            {
                return (IsBusy || GameServerVM.IsBusy);
            }
        }

        public bool ShutDownPC
        {
            get { return GetValue(() => ShutDownPC); }
            set { SetValue(() => ShutDownPC, value); }
        }

        public ObservableRangeCollection<Process> RunningApplications
        {
            get { return GetValue(() => RunningApplications); }
            set { SetValue(() => RunningApplications, value); }
        }

        public Process SelectedProcess
        {
            get { return GetValue(() => SelectedProcess); }
            set { SetValue(() => SelectedProcess, value); }
        }

        public string SelectedProcessName
        {
            get { return GetValue(() => SelectedProcessName); }
            set { SetValue(() => SelectedProcessName, value); }
        }

        public RelayCommand RefreshApplicationsCommand { get; private set; }

        public GameServerVM GameServerVM
        {
            get { return GetValue(() => GameServerVM); }
            set { SetValue(() => GameServerVM, value); }
        }

        public MainWindowVM()
        {
            RefreshApplicationsCommand = new RelayCommand(RefreshApplications);
            GameServerVM = new GameServerVM();
            GameServerVM.PropertyChanged += GameServerVM_PropertyChanged;
            GetData();
        }

        private void GameServerVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("");
        }

        private void RefreshApplications()
        {
            var processes = Process.GetProcesses().Where(x => x.MainWindowHandle != IntPtr.Zero).OrderBy(x => x.ProcessName);
            if (RunningApplications == null)
                RunningApplications = new ObservableRangeCollection<Process>(processes);
            else
                RunningApplications.ReplaceRange(processes);
            if (!string.IsNullOrEmpty(SelectedProcessName))
            {
                var process = processes.Where(x => x.ProcessName == SelectedProcessName);
                if (process != null && process.Count() == 1)
                    SelectedProcess = process.First();
            }
        }

        protected override void GetDesignTimeData()
        {
            IsBusy = true;
            ShutDownPC = true;
            RunningApplications = new ObservableRangeCollection<Process>
            {
                Process.GetCurrentProcess()
            };
            SelectedProcess = RunningApplications.FirstOrDefault();
            SelectedProcessName = SelectedProcess.ProcessName;
        }

        protected override void GetRealData()
        {
            RefreshApplications();
        }
    }
}