using UnityEngine;

using System;

using NetworkTicTacToe.State;

using Mirror;

namespace NetworkTicTacToe.Utils.Network {
	public class BaseNetworkManager : BaseServerNetworkManager {
		public NetAgentState Server;

		// Server-only network manager is not supported 
		public bool IsReady => (IsClient) && Server.IsReady;

		public bool IsClient => Server != null;
		
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
			if ( Clients.Exists(x => x.Connection == conn) ) {
				return;
			}
			Clients.Add(new NetAgentState{Connection = conn});
			Debug.Log($"Server: added player with id {conn.connectionId}");
		}

		public void SetClientAsReadyToPlay() {
			Server.Connection.Send(new NetAgentIsReadyToPlayNetEvent());
		}

		protected override void OnNetAgentIsReadyToPlay(NetworkConnection conn, NetAgentIsReadyToPlayNetEvent e) {
			base.OnNetAgentIsReadyToPlay(conn, e);
			if ( Server.Connection != conn ) {
				return;
			}
			Server.IsReady = true;
			OnServerReadyToPlay?.Invoke();
			Debug.Log($"Client: set server as ready {conn.connectionId}");
		}
	}
}	