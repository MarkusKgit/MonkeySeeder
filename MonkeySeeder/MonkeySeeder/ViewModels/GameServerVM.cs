using GalaSoft.MvvmLight.CommandWpf;
using MonkeySeeder.Helpers;
using MonkeySeeder.Services;
using MonkeySeeder.Services.SteamServerQuery;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MonkeySeeder.ViewModels
{
    public class GameServerVM : BaseVM
    {
        private SteamServerQueryService steamServerQueryService;

        private CancellationTokenSource autoUpdateCanceller;

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
        }

        private bool CanConnectGameServer()
        {
            return Parsers.TryParseIPEndpoint(ServerAdress, out var endPoint);
        }

        private async void UpdateGameServer()
        {
            await UpdateGameServerAsync();
        }

        private async Task UpdateGameServerAsync()
        {
            IsBusy = true;
            try
            {
                Parsers.TryParseIPEndpoint(ServerAdress, out var endPoint);
                steamServerQueryService.EndPoint = endPoint;
                SteamServerInfo serverInfo = null;
                List<PlayerInfo> playerInfo = null;
                try
                {
                    serverInfo = await steamServerQueryService?.GetServerInfoAsync();
                    playerInfo = (await steamServerQueryService?.GetPlayersAsync())?.Where(x => !string.IsNullOrEmpty(x.Name)).OrderBy(x => x.Name).ToList();
                }
                catch (Exception)
                {
                    ConnectError = true;
                    Connected = false;
                    return;
                }
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
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ToggleAutoUpdate(bool value)
        {
            if (value)
            {
                autoUpdateCanceller = new CancellationTokenSource();
                var task = PeriodicUpdateAsync(TimeSpan.FromSeconds(10), autoUpdateCanceller.Token);
            }
            else
            {
                autoUpdateCanceller?.Cancel();
            }
        }

        private async Task PeriodicUpdateAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (Connected)
                    await UpdateGameServerAsync();
                await Task.Delay(interval, cancellationToken);
            }
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