using UnityEngine;

using Mirror;

namespace NetworkTicTacToe.Behaviours {
	public class StartSceneStarter : MonoBehaviour{
		public NetworkManager NetworkManager;

		public MainMenuUI MainMenu; 
		
		void Start() {
			MainMenu.Init(NetworkManager);
		}
	}
}