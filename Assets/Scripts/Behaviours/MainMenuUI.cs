using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using Mirror;

namespace NetworkTicTacToe.Behaviours {
	public class MainMenuUI : MonoBehaviour {
		public Button     StartServer;
		public Button     StartClient;
		
		public LoadingUI LoadingUI;

		List<GameObject> _allMenus;
		
		public void Init(NetworkManager networkManager) {
			StartServer.onClick.AddListener(() => {
				networkManager.StartHost();
				OpenPrepareMenu();
			});
			StartClient.onClick.AddListener(() => {
				networkManager.StartClient();
				OpenPrepareMenu();
			});
			HideOtherMenus();
		}

		void OpenPrepareMenu() {
			gameObject.SetActive(false);
			LoadingUI.gameObject.SetActive(true);
		}
		
		
		void HideOtherMenus() {
			LoadingUI.gameObject.SetActive(false);
		}
	}
}