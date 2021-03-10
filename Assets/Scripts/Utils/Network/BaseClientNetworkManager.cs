using UnityEngine;

using System;

using Mirror;

namespace NetworkTicTacToe.Utils.Network {
	public sealed class BaseClientNetworkManager : BaseMessageDeliverer {
		NetAgentState _server;

		// Server-only network manager is not supported 
		public bool IsReady => (IsClient) && _server.IsReady;

		public bool IsClient => _server != null;
		
		public event Action OnServerReadyToPlay;

		public void Init() {
			NetworkClient.RegisterHandler<NetAgentIsReadyToPlayNetEvent>(OnNetAgentIsReadyToPlay);
		}
		
		public void RegisterCallback<T>(Action<T> callback) where T : struct, NetworkMessage {
			if ( callback == null ) {
				return;
			}
			NetworkClient.RegisterHandler(callback);
		}

		public void UnregisterAllCallbacks<T>() where T : struct, NetworkMessage {
			NetworkClient.UnregisterHandler<T>();
		}
		
		public void SendDataMessageToServer<T>(T data) where T : struct, NetworkMessage {
			SendDataMessage(_server.Connection, data);
		}
		
		public void ConnectToServer(NetworkConnection conn) {
			Debug.Log("Client: connected to server");
			_server = new NetAgentState {Connection = conn};
		}

		public void DisconnectFromServer() {
			Debug.Log("Client: disconnected from server");
			_server = null;
		}

		public void SetAsReadyToPlay() {
			_server.Connection.Send(new NetAgentIsReadyToPlayNetEvent());
		}

		void OnNetAgentIsReadyToPlay(NetworkConnection conn, NetAgentIsReadyToPlayNetEvent e) {
			if ( _server.Connection != conn ) {
				return;
			}
			_server.IsReady = true;
			OnServerReadyToPlay?.Invoke();
			Debug.Log($"Client: set server as ready {conn.connectionId}");
		}
	}
}	