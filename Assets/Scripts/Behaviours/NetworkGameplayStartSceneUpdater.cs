using Mirror;

using UnityEngine;

using NetworkTicTacToe.State;

namespace NetworkTicTacToe.Behaviours {
	public class NetworkGameplayStartSceneUpdater : NetworkBehaviour {
		CustomNetworkManager _networkManager;
		
		public void Init(CustomNetworkManager networkManager) {
			_networkManager = networkManager;
		}
	}
}