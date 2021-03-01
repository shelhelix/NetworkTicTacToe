using UnityEngine;

using Mirror;

namespace NetworkTicTacToe.Behaviours {
	public class LoadingUI : MonoBehaviour {

		bool isLoading;

		void Update() {
			var networkManager = NetworkManager.singleton as GameplayNetworkManager;
			if ( networkManager.Clients.Count == 2 && !isLoading ) {
				networkManager.ServerChangeScene("TicTacToe");
				isLoading = true;
			}
		}
	}
}