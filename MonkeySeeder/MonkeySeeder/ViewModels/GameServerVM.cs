using GalaSoft.MvvmLight.CommandWpf;
using MonkeySeeder.Helpers;
using MonkeySeeder.Services.SteamServerQuery;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MonkeySeeder.ViewModels
{
    public class GameServerVM : BaseVM
    {
        public string ServerAdress
        {
            get { return GetValue(() => ServerAdress); }
            set { SetValue(() => ServerAdress, value); }
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

        public ObservableCollection<PlayerInfo> OnlinePlayers
        {
            get { return GetValue(() => OnlinePlayers); }
            private set { SetValue(() => OnlinePlayers, value); }
        }

        public RelayCommand ConnectCommand { get; private set; }

        public GameServerVM()
        {
            ConnectCommand = new RelayCommand(ConnectGameServer, CanConnectGameServer);
            GetData();
        }

        private bool CanConnectGameServer()
        {
            return Parsers.TryParseIPEndpoint(ServerAdress, out var endPoint);
        }

        private async void ConnectGameServer()
        {
            IsBusy = true;
            try
            {
                Parsers.TryParseIPEndpoint(ServerAdress, out var endPoint);
                var server = new SteamGameServer(endPoint);
                SteamServerInfo serverInfo = null;
                List<PlayerInfo> playerInfo = null;
                try
                {
                    serverInfo = await server.GetServerInfoAsync();
                    playerInfo = (await server.GetPlayersAsync())?.Where(x => !string.IsNullOrEmpty(x.Name)).OrderBy(x => x.Name).ToList();
                }
                catch (Exception)
                {
                    ConnectError = true;
                    return;
                }
                ConnectError = false;

                PlayerCount = serverInfo.Players;
                if (playerInfo != null)
                {
                    OnlinePlayers = new ObservableCollection<PlayerInfo>(playerInfo);
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

        protected override void GetDesignTimeData()
        {
        }

        protected override void GetRealData()
        {
        }
    }
}