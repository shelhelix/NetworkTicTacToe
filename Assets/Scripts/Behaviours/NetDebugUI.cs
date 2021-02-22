using UnityEngine;
using UnityEngine.UI;

using Mirror;

namespace NetworkTicTacToe.Behaviours {
	public class NetDebugUI : MonoBehaviour {
		public Button StartServer;
		public Button StartClient;

		public NetworkManager NetworkManager;
		
		void Start() {
			StartServer.onClick.AddListener(() => NetworkManager.StartHost());
			StartClient.onClick.AddListener(() => NetworkManager.StartClient());
		}
	}
}