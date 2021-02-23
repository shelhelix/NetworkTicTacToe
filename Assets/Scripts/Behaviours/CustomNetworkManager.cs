using UnityEngine;

using Mirror;

namespace NetworkTicTacToe.Behaviours {
	public class CustomNetworkManager : NetworkManager {
		public override void OnServerConnect(NetworkConnection conn) {
			base.OnServerConnect(conn);
			if ( !PlayerController.Instance.TryAddPlayer(conn.connectionId) ) {
				conn.Disconnect();
			}

			if ( PlayerController.Instance.ReadyPlayers == PlayerController.NeededPlayers ) {
				ServerChangeScene("TicTacToe");
			}
			Debug.Log($"Added player with id {conn.connectionId}");
		}

		public override void OnServerDisconnect(NetworkConnection conn) {
			PlayerController.Instance.RemovePlayer(conn.connectionId);
			Debug.Log($"Removed player with id {conn.connectionId}");
		}
	}
}	