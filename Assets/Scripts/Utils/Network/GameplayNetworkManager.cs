using UnityEngine;

using System;
using System.Collections.Generic;

using NetworkTicTacToe.Gameplay.NetworkEvents;
using NetworkTicTacToe.State;

using Mirror;

namespace NetworkTicTacToe.Behaviours {
	public class GameplayNetworkManager : NetworkManager {
		public NetAgentState       Server;
		public List<NetAgentState> Clients = new List<NetAgentState>();

		bool _isServerReadyLocally;

		// Server-only network manager is not supported 
		public bool IsReady => (IsClient) && Server.IsReady;

		public bool                             IsClient => Server != null;
		public bool                             IsServer => Clients.Count != 0;
		
		public event Action OnServerReadyToPlay;

		public void RegisterClientCallback<T>(Action<T> callback) where T : struct, NetworkMessage {
			if ( callback == null ) {
				return;
			}
			NetworkServer.RegisterHandler(callback);
			NetworkClient.RegisterHandler(callback);
		}

		public void UnregisterClientCallbacks<T>() where T : struct, NetworkMessage {
			NetworkClient.UnregisterHandler<T>();
			NetworkServer.UnregisterHandler<T>();
		}
		
		public void SendDataMessageToServer<T>(T data) where T : struct, NetworkMessage {
			SendDataMessage(Server.Connection, data);
		}
		
		public void SendDataMessageToAllNonHostClients<T>(T data) where T : struct, NetworkMessage {
			foreach ( var client in Clients ) {
				//Don't send info to the local client 
				if ( IsLocalClient(client) ) {
					continue;
				}
				SendDataMessage(client.Connection, data);
			}
		}

		public void SendDataMessage<T>(NetworkConnection connection, T data) where T : struct, NetworkMessage {
			connection.Send(data);
		}

		public override void OnStartHost() {
			base.OnStartHost();
			NetworkServer.RegisterHandler<NetAgentIsReadyToPlayNetEvent>(OnNetAgentIsReadyToPlay);
		}

		public override void OnStartClient() {
			base.OnStartClient();
			NetworkClient.RegisterHandler<NetAgentIsReadyToPlayNetEvent>(OnNetAgentIsReadyToPlay);
		}

		public override void OnClientConnect(NetworkConnection conn) {
			base.OnClientConnect(conn);
			Debug.Log("Client: connected to server");
			Server = new NetAgentState {Connection = conn};
		}

		public override void OnClientDisconnect(NetworkConnection conn) {
			base.OnClientDisconnect(conn);
			Debug.Log("Client: disconnected from server");
			Server = null;
		}

		public override void OnServerAddPlayer(NetworkConnection conn) {
			base.OnServerAddPlayer(conn);
			var client = Clients.Find(x => x.Connection == conn);
			if ( client != null ) {
				client.AddedPlayer = true;
				Debug.Log($"Server: changed player state {conn.connectionId}");
			}
			else {
				Clients.Add(new NetAgentState{Connection = conn});
				Debug.Log($"Server: added player with id {conn.connectionId}");
			}
		}

		public override void OnServerSceneChanged(string sceneName) {
			base.OnServerSceneChanged(sceneName);
			foreach ( var client in Clients ) {
				client.AddedPlayer = false;
			}
			Debug.Log($"Server: removed all players flags");
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

		bool IsLocalClient(NetAgentState client) {
			return client.Connection.connectionId == 0;
		}

		void OnNetAgentIsReadyToPlay(NetworkConnection conn, NetAgentIsReadyToPlayNetEvent e) {
			var client = Clients.Find(x => x.Connection == conn);
			if ( client != null ) {
				client.IsReady = true;
				TrySendServerReadyMessage();
				Debug.Log($"Server: set client as ready {conn.connectionId}");
			} else {
				Server.IsReady = true;
				OnServerReadyToPlay?.Invoke();
				Debug.Log($"Client: set server as ready {conn.connectionId}");
			}
		}

		void TrySendServerReadyMessage() {
			if ( !AreAllClientsReady() || !_isServerReadyLocally ) {
				return;
			}
			Debug.Log($"Send server ready message to all clients");
			foreach ( var client in Clients ) {
				client.Connection.Send(new NetAgentIsReadyToPlayNetEvent());
			}
		}

		bool AreAllClientsReady() {
			var res = !(Clients.Exists(x => !x.IsReady));
			Debug.Log($"Total clients {Clients.Count}");
			return res;
		}
	}
}	