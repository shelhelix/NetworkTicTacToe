using UnityEngine;

using System;
using System.Collections.Generic;

using NetworkTicTacToe.Behaviours;
using NetworkTicTacToe.State;
using NetworkTicTacToe.Utils.Network;

namespace NetworkTicTacToe.Gameplay {
	public class Server {
		//Converted "network id" -> "player side" 
		Dictionary<NetAgentState, PlayerSide> _players = new Dictionary<NetAgentState, PlayerSide>();

		GameplayNetworkManager _gameplayNetworkManager;
		
		public void Init(GameplayNetworkManager gameplayNetworkManager) {
			if ( gameplayNetworkManager.Clients.Count != 2 ) {
				Debug.LogError("Can't assign sides to clients. Wrong amount of clients");
				return;
			}

			_gameplayNetworkManager = gameplayNetworkManager;

			var index = 0;
			foreach ( PlayerSide side in Enum.GetValues(typeof(PlayerSide))) {
				var client = gameplayNetworkManager.Clients[index++];
				_players.Add(client, side);
			}
			_gameplayNetworkManager.OnReceivedDataMessage += OnMessageReceived;
		}

		public void Deinit() {
			_gameplayNetworkManager.OnReceivedDataMessage -= OnMessageReceived;
		}

		public void SendSidesToPlayers() {
			foreach ( var player in _players ) {
				_gameplayNetworkManager.SendDataMessage(player.Key.Connection, player.Value);
			}
		}
		
		void OnMessageReceived(DataNetworkMessage dataNetworkMessage) {
			
		}
	}
}