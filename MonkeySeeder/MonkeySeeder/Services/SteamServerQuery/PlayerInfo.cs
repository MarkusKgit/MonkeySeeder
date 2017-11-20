﻿using System;

namespace MonkeySeeder.Services.SteamServerQuery
{
    public class PlayerInfo
    {
        /// <summary>
        /// Name of the player.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Player's score (usually "frags" or "kills".).
        /// </summary>
        public long Score { get; internal set; }

        /// <summary>
        /// Time  player has been connected to the server.(returns TimeSpan instance).
        /// </summary>
        public TimeSpan Time { get; internal set; }
    }
}