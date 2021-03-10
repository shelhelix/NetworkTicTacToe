using UnityEngine;

using System;
using System.Collections.Generic;

using NetworkTicTacToe.Gameplay.NetworkEvents;
using NetworkTicTacToe.Utils.Network;

namespace NetworkTicTacToe.Gameplay {
	public class Server {
		//Converted "network id" -> "player side" 
		Dictionary<NetAgentState, PlayerSide> _players = new Dictionary<NetAgentState, PlayerSide>();

		BaseServerNetworkManager _baseServerNetworkManager;
		
		public void Init(BaseServerNetworkManager baseClientNetworkManager) {
			if ( baseClientNetworkManager.Clients.Count != 2 ) {
				Debug.LogError("Can't assign sides to clients. Wrong amount of clients");
				return;
			}

			_baseServerNetworkManager = baseClientNetworkManager;

			var index = 0;
			foreach ( PlayerSide side in Enum.GetValues(typeof(PlayerSide))) {
				var client = baseClientNetworkManager.Clients[index++];
				_players.Add(client, side);
			}
		}

		public void Deinit() { }

		public void SendSidesToPlayers() {
			foreach ( var player in _players ) {
				_baseServerNetworkManager.SendDataMessage(player.Key.Connection, new PlayerSideChanged(player.Value));
			}
		}
	}
}