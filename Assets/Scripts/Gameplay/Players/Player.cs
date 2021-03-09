using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

using NetworkTicTacToe.Behaviours;
using NetworkTicTacToe.State;
using NetworkTicTacToe.Utils.Network;

namespace NetworkTicTacToe.Gameplay.Players {
	public class Player {
		public PlayerSide PlayerSide;
		
		GameplayController     _gameplayController;
		GameplayNetworkManager _gameplayNetworkManager;

		MemoryStream    _stream    = new MemoryStream();
		BinaryFormatter _formatter = new BinaryFormatter();
		
		public Player(GameplayNetworkManager gameplayNetworkManager, GameplayController gameplayController) {
			_gameplayController                           =  gameplayController;
			_gameplayNetworkManager                       =  gameplayNetworkManager;
			_gameplayNetworkManager.OnReceivedDataMessage += OnMessageReceived;
			if ( _gameplayNetworkManager.IsClient ) {
				_gameplayController.OnTurnChanged += SendGameStateToServer;
			}
		}

		public void Deinit() {
			_gameplayNetworkManager.OnReceivedDataMessage -= OnMessageReceived;
				_gameplayController.OnTurnChanged         -= SendGameStateToServer;
		}
		
		void SendGameStateToAllClients() {
			_gameplayNetworkManager.SendDataMessageToAllNonHostClients(_gameplayController.State);	
		}
		
		void SendGameStateToServer() {
			_gameplayNetworkManager.SendDataMessageToServer(_gameplayController.State);	
		}

		void OnMessageReceived(DataNetworkMessage dataNetworkMessage) {
			_stream.Write(dataNetworkMessage.Bytes, 0, dataNetworkMessage.Bytes.Length);
			_stream.Position = 0;
			
			var obj     = _formatter.Deserialize(_stream);
			if ( obj is GameplayControllerState testObj ) {
				_gameplayController.State = testObj;
			}
				
			if ( _gameplayNetworkManager.IsServer ) {
				SendGameStateToAllClients();
			}
		}
	}
}