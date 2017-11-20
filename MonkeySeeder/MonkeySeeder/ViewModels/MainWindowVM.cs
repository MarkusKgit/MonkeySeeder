using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace MonkeySeeder.ViewModels
{
    public class MainWindowVM : BaseVM
    {
        public bool IsBusy
        {
            get { return GetValue(() => IsBusy); }
            set { SetValue(() => IsBusy, value); }
        }

        public bool ShutDownPC
        {
            get { return GetValue(() => ShutDownPC); }
            set { SetValue(() => ShutDownPC, value); }
        }

        public ObservableCollection<Process> RunningApplications
        {
            get { return GetValue(() => RunningApplications); }
            set { SetValue(() => RunningApplications, value); }
        }

        public string SelectedProcess
        {
            get { return GetValue(() => SelectedProcess); }
            set { SetValue(() => SelectedProcess, value); }
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
            GetData();
        }

        private void RefreshApplications()
        {
            RunningApplications = new ObservableCollection<Process>(Process.GetProcesses().Where(x => x.MainWindowHandle != IntPtr.Zero).OrderBy(x => x.ProcessName));
        }

        protected override void GetDesignTimeData()
        {
            IsBusy = true;
            ShutDownPC = true;
            RunningApplications = new ObservableCollection<Process>
            {
                new Process() { }
            };
        }

        protected override void GetRealData()
        {
            RefreshApplications();
        }
    }
}