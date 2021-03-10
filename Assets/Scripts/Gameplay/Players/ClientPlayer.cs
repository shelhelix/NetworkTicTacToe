using UnityEngine;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NetworkTicTacToe.Behaviours;
using NetworkTicTacToe.State;
using NetworkTicTacToe.Utils.Network;

namespace NetworkTicTacToe.Gameplay.Players {
	public class ClientPlayer {
		public PlayerSide PlayerSide;
		
		GameplayController     _gameplayController;
		GameplayNetworkManager _gameplayNetworkManager;

		MemoryStream    _stream    = new MemoryStream();
		BinaryFormatter _formatter = new BinaryFormatter();
		
		public ClientPlayer(GameplayNetworkManager gameplayNetworkManager, GameplayController gameplayController) {
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

		void SendGameStateToServer() {
			_gameplayNetworkManager.SendDataMessageToServer(_gameplayController.State);	
		}

		void OnMessageReceived(DataNetworkMessage dataNetworkMessage) {
			_stream.SetLength(0);
			_stream.Write(dataNetworkMessage.Bytes, 0, dataNetworkMessage.Bytes.Length);
			_stream.Position = 0;
			
			var obj = _formatter.Deserialize(_stream);
			switch ( obj ) {
				case GameplayControllerState state:
					Debug.Log("Client gameplay state updated");
					_gameplayController.State = state;
					if ( _gameplayNetworkManager.IsServer ) {
						_gameplayNetworkManager.SendDataMessageToAllNonHostClients(_gameplayController.State);	
					}
					break;
				case PlayerSide side:
					Debug.Log($"Client side is {side}");
					PlayerSide = side;
					break;
			}
		}
	}
}