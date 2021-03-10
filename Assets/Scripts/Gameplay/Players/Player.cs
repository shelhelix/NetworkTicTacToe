using UnityEngine;

using NetworkTicTacToe.Gameplay.NetworkEvents;
using NetworkTicTacToe.Utils.Network;

namespace NetworkTicTacToe.Gameplay.Players {
	public class Player {
		public PlayerSide PlayerSide;

		readonly GameplayController    _gameplayController;
		readonly ComplexNetworkManager _networkManager;
		
		public Player(ComplexNetworkManager networkManager, GameplayController gameplayController) {
			_gameplayController = gameplayController;
			_networkManager     = networkManager;
			_networkManager.RegisterCallback<GameStateChanged>(OnUpdatedStateReceived);
			_networkManager.RegisterCallback<PlayerSideChanged>(OnUpdatedPlayerSide);
			if ( _networkManager.IsClient ) {
				_gameplayController.OnTurnChanged += SendGameStateToServer;
			}
		}

		public void Deinit() {
			_networkManager.UnregisterAllCallbacks<GameStateChanged>();
			_networkManager.UnregisterAllCallbacks<PlayerSideChanged>();
			_gameplayController.OnTurnChanged -= SendGameStateToServer;
		}

		void SendGameStateToServer() {
			_networkManager.Client.SendDataMessageToServer(new GameStateChanged(_gameplayController.State));	
		}

		void OnUpdatedStateReceived(GameStateChanged e) {
			_gameplayController.State = e.State;
			Debug.Log("Updated game state");
			_networkManager.Server?.SendDataMessageToAllNonHostClients(new GameStateChanged(_gameplayController.State));
		}

		void OnUpdatedPlayerSide(PlayerSideChanged e) {
			Debug.Log($"Client side is {e.Side}");
			PlayerSide = e.Side;
		}
	}
}