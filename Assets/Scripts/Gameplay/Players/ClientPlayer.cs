using UnityEngine;

using NetworkTicTacToe.Behaviours;
using NetworkTicTacToe.Gameplay.NetworkEvents;
using NetworkTicTacToe.Utils.Network;

namespace NetworkTicTacToe.Gameplay.Players {
	public class ClientPlayer {
		public PlayerSide PlayerSide;

		readonly GameplayController     _gameplayController;
		readonly BaseNetworkManager _baseNetworkManager;
		
		public ClientPlayer(BaseNetworkManager baseNetworkManager, GameplayController gameplayController) {
			_gameplayController                           =  gameplayController;
			_baseNetworkManager                       =  baseNetworkManager;
			_baseNetworkManager.RegisterClientCallback<GameStateChanged>(OnUpdatedStateReceived);
			_baseNetworkManager.RegisterClientCallback<PlayerSideChanged>(OnUpdatedPlayerSide);
			if ( _baseNetworkManager.IsClient ) {
				_gameplayController.OnTurnChanged += SendGameStateToServer;
			}
		}

		public void Deinit() {
			_baseNetworkManager.UnregisterClientCallbacks<GameStateChanged>();
			_baseNetworkManager.UnregisterClientCallbacks<PlayerSideChanged>();
			_gameplayController.OnTurnChanged -= SendGameStateToServer;
		}

		void SendGameStateToServer() {
			_baseNetworkManager.SendDataMessageToServer(new GameStateChanged(_gameplayController.State));	
		}

		void OnUpdatedStateReceived(GameStateChanged e) {
			_gameplayController.State = e.State;
			Debug.Log("Updated game state");
			if ( _baseNetworkManager.IsServer ) {
				_baseNetworkManager.SendDataMessageToAllNonHostClients(new GameStateChanged(_gameplayController.State));	
			}
		}

		void OnUpdatedPlayerSide(PlayerSideChanged e) {
			Debug.Log($"Client side is {e.Side}");
			PlayerSide = e.Side;
		}
	}
}