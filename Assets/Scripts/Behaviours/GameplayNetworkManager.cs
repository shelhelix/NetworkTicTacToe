using UnityEngine;

using System.Collections.Generic;

using NetworkTicTacToe.State;

using Mirror;

namespace NetworkTicTacToe.Behaviours {
	public class GameplayNetworkManager : NetworkManager {
		public NetAgentState       Server;
		public List<NetAgentState> Clients = new List<NetAgentState>();

		bool _isServerReadyLocally;
		
		// Server-only network manager is not supported 
		public bool IsReady => (IsClient) && Server.IsReady; 
		
		public bool IsClient => Server != null;
		public bool IsServer => Clients.Count != 0;

		public override void OnStartHost() {
			NetworkServer.RegisterHandler<NetAgentIsReadyToPlayNetEvent>(OnNetAgentIsReadyToPlay);
			base.OnStartHost();
		}

		public override void OnStartClient() {
			NetworkClient.RegisterHandler<NetAgentIsReadyToPlayNetEvent>(OnNetAgentIsReadyToPlay);
			base.OnStartClient();
		}

		public override void OnClientConnect(NetworkConnection conn) {
			base.OnClientConnect(conn);
			Debug.Log("Client: connected to server");
			Server = new NetAgentState {Connection = conn, IsReady = false};
		}

		public override void OnClientDisconnect(NetworkConnection conn) {
			base.OnClientDisconnect(conn);
			Debug.Log("Client: disconnected from server");
			Server = null;
		}

		public override void OnServerConnect(NetworkConnection conn) {
			base.OnServerConnect(conn);
			Debug.Log($"Server: added player with id {conn.connectionId}");
			Clients.Add(new NetAgentState{Connection = conn, IsReady = false});
			if ( Clients.Count == 2 ) {
				ServerChangeScene("TicTacToe");
			}
		}
		
		public override void OnServerDisconnect(NetworkConnection conn) {
			base.OnServerDisconnect(conn);
			Debug.Log($"Server: removed player with id {conn.connectionId}");
			Clients.RemoveAll(x => x.Connection == conn);
		}

		public void SetClientAsReadyToPlay() {
			Server.Connection.Send(new NetAgentIsReadyToPlayNetEvent());
		}

		public void SetServerAsReadyToPlay() {
			_isServerReadyLocally = true;
			TrySendServerReadyMessage();
		}

		void OnNetAgentIsReadyToPlay(NetworkConnection conn, NetAgentIsReadyToPlayNetEvent e) {
			var client = Clients.Find(x => x.Connection == conn);
			if ( client != null ) {
				client.IsReady = true;
				TrySendServerReadyMessage();
				Debug.Log($"Server: set client as ready {conn.connectionId}");
			} else {
				Server.IsReady = true;
				Debug.Log($"Client: set server as ready {conn.connectionId}");
			}
		}

		void TrySendServerReadyMessage() {
			if ( AreAllClientsReady() && _isServerReadyLocally ) {
				foreach ( var client in Clients ) {
					client.Connection.Send(new NetAgentIsReadyToPlayNetEvent());
				}
			}
		}

		bool AreAllClientsReady() {
			return !(Clients.Exists(x => !x.IsReady));
		}
	}
}	