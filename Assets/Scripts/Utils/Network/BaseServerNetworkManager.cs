using UnityEngine;

using System.Collections.Generic;

using NetworkTicTacToe.State;

using Mirror;

namespace NetworkTicTacToe.Utils.Network {
	public class BaseServerNetworkManager : NetworkManager {
		public List<NetAgentState> Clients = new List<NetAgentState>();

		bool _isServerReadyLocally;
		
		public bool IsServer => Clients.Count != 0;
		
		public override void OnStartServer() {
			base.OnStartHost();
			NetworkServer.RegisterHandler<NetAgentIsReadyToPlayNetEvent>(OnNetAgentIsReadyToPlay);
		}

		public override void OnServerDisconnect(NetworkConnection conn) {
			base.OnServerDisconnect(conn);
			Debug.Log($"Server: removed player with id {conn.connectionId}");
			Clients.RemoveAll(x => x.Connection == conn);
		}

		public void SendDataMessage<T>(NetworkConnection connection, T data) where T : struct, NetworkMessage {
			connection.Send(data);
		}
		
		public void SetServerAsReadyToPlay() {
			_isServerReadyLocally = true;
			TrySendServerReadyMessage();
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
		
		protected virtual void OnNetAgentIsReadyToPlay(NetworkConnection conn, NetAgentIsReadyToPlayNetEvent e) {
			var client = Clients.Find(x => x.Connection == conn);
			if ( client == null ) {
				Debug.Log($"Server: Can't set client as ready - client not found {conn.address}");
				return;
			}
			client.IsReady = true;
			TrySendServerReadyMessage();
		}

		void TrySendServerReadyMessage() {
			if ( !AreAllClientsReady() || !_isServerReadyLocally ) {
				return;
			}
			foreach ( var client in Clients ) {
				client.Connection.Send(new NetAgentIsReadyToPlayNetEvent());
			}
		}

		bool IsLocalClient(NetAgentState client) {
			return client.Connection.connectionId == 0;
		}

		bool AreAllClientsReady() {
			var res = !(Clients.Exists(x => !x.IsReady));
			return res;
		}
	}
}