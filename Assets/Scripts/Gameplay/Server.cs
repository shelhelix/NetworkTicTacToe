using UnityEngine;

using System;
using System.Collections.Generic;

using NetworkTicTacToe.Behaviours;
using NetworkTicTacToe.Gameplay.NetworkEvents;
using NetworkTicTacToe.State;
using NetworkTicTacToe.Utils.Network;

namespace NetworkTicTacToe.Gameplay {
	public class Server {
		//Converted "network id" -> "player side" 
		Dictionary<NetAgentState, PlayerSide> _players = new Dictionary<NetAgentState, PlayerSide>();

		BaseNetworkManager _baseNetworkManager;
		
		public void Init(BaseNetworkManager baseNetworkManager) {
			if ( baseNetworkManager.Clients.Count != 2 ) {
				Debug.LogError("Can't assign sides to clients. Wrong amount of clients");
				return;
			}

			_baseNetworkManager = baseNetworkManager;

			var index = 0;
			foreach ( PlayerSide side in Enum.GetValues(typeof(PlayerSide))) {
				var client = baseNetworkManager.Clients[index++];
				_players.Add(client, side);
			}
		}

		public void Deinit() { }

		public void SendSidesToPlayers() {
			foreach ( var player in _players ) {
				_baseNetworkManager.SendDataMessage(player.Key.Connection, new PlayerSideChanged(player.Value));
			}
		}
	}
}