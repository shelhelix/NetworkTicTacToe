using UnityEngine;

using Mirror;
using NetworkTicTacToe.Utils.Network;

namespace NetworkTicTacToe.Behaviours {
	public class LoadingUI : MonoBehaviour {
		bool _isLoading;

		void Update() {
			var networkManager = NetworkManager.singleton as ComplexNetworkManager;
			if ( networkManager.IsServer && networkManager.Server.Clients.Count == 2 && !_isLoading ) {
				networkManager.ServerChangeScene("TicTacToe");
				_isLoading = true;
			}
		}
	}
}