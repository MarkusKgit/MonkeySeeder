using FluentScheduler;
using GalaSoft.MvvmLight.CommandWpf;
using MonkeySeeder.Helpers;
using MonkeySeeder.Services;
using MonkeySeeder.Services.SteamServerQuery;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MonkeySeeder.ViewModels
{
    public class GameServerVM : BaseVM
    {
        private SteamServerQueryService steamServerQueryService;

        public string ServerAdress
        {
            get { return GetValue(() => ServerAdress); }
            set
            {
                if (SetValue(() => ServerAdress, value))
                    Connected = false;
            }
        }

        public bool Connected
        {
            get { return GetValue(() => Connected); }
            private set { SetValue(() => Connected, value); }
        }

        public bool AutoUpdate
        {
            get { return GetValue(() => AutoUpdate); }
            set
            {
                if (SetValue(() => AutoUpdate, value))
                    ToggleAutoUpdate(value);
            }
        }

        public bool ConnectError
        {
            get { return GetValue(() => ConnectError); }
            set { SetValue(() => ConnectError, value); }
        }

        public int PlayerCount
        {
            get { return GetValue(() => PlayerCount); }
            private set
            {
                if (SetValue(() => PlayerCount, value))
                    NotifyPropertyChanged(() => HasPlayers);
            }
        }

        public int MaxPlayerCount
        {
            get { return GetValue(() => MaxPlayerCount); }
            private set { SetValue(() => MaxPlayerCount, value); }
        }

        public string ServerHeading
        {
            get { return GetValue(() => ServerHeading); }
            private set { SetValue(() => ServerHeading, value); }
        }

        public bool HasPlayers
        {
            get { return PlayerCount > 0; }
        }

        public ObservableRangeCollection<PlayerInfo> OnlinePlayers
        {
            get { return GetValue(() => OnlinePlayers); }
            private set { SetValue(() => OnlinePlayers, value); }
        }

        public RelayCommand ConnectCommand { get; private set; }

        public GameServerVM(SteamServerQueryService queryService)
        {
            steamServerQueryService = queryService;
            Initialize();
        }

        public GameServerVM()
        {
            steamServerQueryService = new SteamServerQueryService();
            Initialize();
        }

        private void Initialize()
        {
            ConnectCommand = new RelayCommand(UpdateGameServer, CanConnectGameServer);
            GetData();
            JobManager.Initialize(new Registry());
        }

        private bool CanConnectGameServer()
        {
            return Parsers.TryParseIPEndpoint(ServerAdress, out var endPoint);
        }

        private async void UpdateGameServer()
        {
            Connected = false;
            await UpdateGameServerAsync();
        }

        private async Task UpdateGameServerAsync()
        {
            if (steamServerQueryService == null)
                return;
            IsBusy = true;
            try
            {
                Parsers.TryParseIPEndpoint(ServerAdress, out var endPoint);
                steamServerQueryService.EndPoint = endPoint;
                SteamServerInfo serverInfo = null;
                List<PlayerInfo> playerInfo = null;
                try
                {
                    serverInfo = await steamServerQueryService.GetServerInfoAsync().ConfigureAwait(false);
                    playerInfo = (await steamServerQueryService.GetPlayersAsync().ConfigureAwait(false))?.Where(x => !string.IsNullOrEmpty(x.Name)).OrderBy(x => x.Name).ToList();
                }
                catch (Exception ex)
                {
                    ConnectError = true;
                    //Connected = false;
                    return;
                }
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ConnectError = false;
                    Connected = true;

                    PlayerCount = serverInfo.Players;
                    if (playerInfo != null)
                    {
                        if (OnlinePlayers == null)
                            OnlinePlayers = new ObservableRangeCollection<PlayerInfo>(playerInfo);
                        else
                            OnlinePlayers.ReplaceRange(playerInfo);
                        PlayerCount = OnlinePlayers.Count();
                    }
                    MaxPlayerCount = serverInfo.MaxPlayers;
                    ServerHeading = $"{serverInfo.Name} ({PlayerCount}/{MaxPlayerCount})";
                }));
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ToggleAutoUpdate(bool value)
        {
            if (value)
                JobManager.AddJob(async () => await UpdateGameServerAsync(), (x) => x.WithName("AutoUpdate").ToRunNow().AndEvery(5).Seconds());
            else
                JobManager.RemoveJob("AutoUpdate");
        }

        protected override void GetDesignTimeData()
        {
            ServerAdress = "172.93.105.98:27165";
            ConnectCommand.Execute(null);
        }

        protected override void GetRealData()
        {
        }
    }
}