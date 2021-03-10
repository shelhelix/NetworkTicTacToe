using System;
using UnityEngine;

using System.Collections.Generic;

using Mirror;

namespace NetworkTicTacToe.Utils.Network {
	public class BaseServerNetworkManager : BaseMessageDeliverer {
		public readonly List<NetAgentState> Clients = new List<NetAgentState>();

		bool _isServerReadyLocally;
		
		public void Init() {
			NetworkServer.RegisterHandler<NetAgentIsReadyToPlayNetEvent>(OnNetAgentIsReadyToPlay);
		}
		
		public void RegisterCallback<T>(Action<T> callback) where T : struct, NetworkMessage {
			if ( callback == null ) {
				return;
			}
			NetworkServer.RegisterHandler(callback);
		}

		public void UnregisterAllCallbacks<T>() where T : struct, NetworkMessage {
			NetworkServer.UnregisterHandler<T>();
		}

		public void DisconnectClient(NetworkConnection conn) {
			Debug.Log($"Server: removed player with id {conn.connectionId}");
			Clients.RemoveAll(x => x.Connection == conn);
		}

		public void TryAddPlayer(NetworkConnection conn) {
			if ( Clients.Exists(x => x.Connection == conn) ) {
				return;
			}
			Clients.Add(new NetAgentState{Connection = conn});
			Debug.Log($"Server: added player with id {conn.connectionId}");
		}
		
		public void SetAsReadyToPlay() {
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
		
		void OnNetAgentIsReadyToPlay(NetworkConnection conn, NetAgentIsReadyToPlayNetEvent e) {
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