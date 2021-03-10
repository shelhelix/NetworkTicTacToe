using UnityEngine;

using Mirror;
using NetworkTicTacToe.Utils.Network;

namespace NetworkTicTacToe.Behaviours {
	public class LoadingUI : MonoBehaviour {

		bool isLoading;

		void Update() {
			var networkManager = NetworkManager.singleton as BaseNetworkManager;
			if ( networkManager.Clients.Count == 2 && !isLoading ) {
				networkManager.ServerChangeScene("TicTacToe");
				isLoading = true;
			}
		}
	}
}