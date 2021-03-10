using UnityEngine;

using NetworkTicTacToe.Behaviours;
using NetworkTicTacToe.Gameplay.NetworkEvents;

namespace NetworkTicTacToe.Gameplay.Players {
	public class ClientPlayer {
		public PlayerSide PlayerSide;

		readonly GameplayController     _gameplayController;
		readonly GameplayNetworkManager _gameplayNetworkManager;
		
		public ClientPlayer(GameplayNetworkManager gameplayNetworkManager, GameplayController gameplayController) {
			_gameplayController                           =  gameplayController;
			_gameplayNetworkManager                       =  gameplayNetworkManager;
			_gameplayNetworkManager.RegisterClientCallback<GameStateChanged>(OnUpdatedStateReceived);
			_gameplayNetworkManager.RegisterClientCallback<PlayerSideChanged>(OnUpdatedPlayerSide);
			if ( _gameplayNetworkManager.IsClient ) {
				_gameplayController.OnTurnChanged += SendGameStateToServer;
			}
		}

		public void Deinit() {
			_gameplayNetworkManager.UnregisterClientCallbacks<GameStateChanged>();
			_gameplayNetworkManager.UnregisterClientCallbacks<PlayerSideChanged>();
			_gameplayController.OnTurnChanged -= SendGameStateToServer;
		}

		void SendGameStateToServer() {
			_gameplayNetworkManager.SendDataMessageToServer(new GameStateChanged(_gameplayController.State));	
		}

		void OnUpdatedStateReceived(GameStateChanged e) {
			_gameplayController.State = e.State;
			Debug.Log("Updated game state");
			if ( _gameplayNetworkManager.IsServer ) {
				_gameplayNetworkManager.SendDataMessageToAllNonHostClients(new GameStateChanged(_gameplayController.State));	
			}
		}

		void OnUpdatedPlayerSide(PlayerSideChanged e) {
			Debug.Log($"Client side is {e.Side}");
			PlayerSide = e.Side;
		}
	}
}