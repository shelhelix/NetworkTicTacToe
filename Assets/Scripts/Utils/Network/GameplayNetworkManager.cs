using UnityEngine;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Mirror;

using NetworkTicTacToe.State;
using NetworkTicTacToe.Utils.Network;

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

		public event Action<DataNetworkMessage> OnReceivedDataMessage;

		MemoryStream    _stream    = new MemoryStream();
		BinaryFormatter _formatter = new BinaryFormatter();
		
		public void SendDataMessageToServer(object data) {
			SendDataMessage(Server.Connection, data);
		}
		public void SendDataMessageToAllNonHostClients(object data) {
			foreach ( var client in Clients ) {
				//Don't send info to the local client 
				if ( IsLocalClient(client) ) {
					continue;
				}
				SendDataMessage(client.Connection, data);
			}
		}
		
		public void SendDataMessage(NetworkConnection connection, object data) {
			_stream.SetLength(0);
			_formatter.Serialize(_stream, data);
			var bytes = _stream.ToArray();
			var message = new DataNetworkMessage {Bytes = bytes};
			connection.Send(message);
		}

		public override void OnStartHost() {
			NetworkServer.RegisterHandler<NetAgentIsReadyToPlayNetEvent>(OnNetAgentIsReadyToPlay);
			NetworkServer.RegisterHandler<DataNetworkMessage>(OnDataMessageReceived);
			base.OnStartHost();
		}

		public override void OnStartClient() {
			NetworkClient.RegisterHandler<DataNetworkMessage>(OnDataMessageReceived);
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

		public override void OnServerAddPlayer(NetworkConnection conn) {
			base.OnServerAddPlayer(conn);
			Debug.Log($"Server: added player with id {conn.connectionId}");
			Clients.Add(new NetAgentState{Connection = conn, IsReady = false});
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
			OnServerReadyToPlay?.Invoke();
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

		void OnDataMessageReceived(DataNetworkMessage dataNetworkMessage) {
			OnReceivedDataMessage?.Invoke(dataNetworkMessage);
		}

		void TrySendServerReadyMessage() {
			if ( !AreAllClientsReady() || !_isServerReadyLocally ) {
				return;
			}
			foreach ( var client in Clients ) {
				client.Connection.Send(new NetAgentIsReadyToPlayNetEvent());
			}
		}

		bool AreAllClientsReady() {
			return !(Clients.Exists(x => !x.IsReady));
		}
	}
}	