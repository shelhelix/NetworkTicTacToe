using UnityEngine;

using Mirror;

namespace NetworkTicTacToe.Behaviours {
	public class NetworkInteraction : NetworkBehaviour {
		public static NetworkInteraction Instance;
		
		void Awake() {
			if ( !Instance ) {
				Instance = this;
			}
		}


		[Command]
		public void CmdDoSmth() {
			Debug.Log("smth happened");
		}
	}
}