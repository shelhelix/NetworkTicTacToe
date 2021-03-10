using System;

using Mirror;

namespace NetworkTicTacToe.Utils.Network {
	public class ComplexNetworkManager : NetworkManager {
		public BaseServerNetworkManager Server;
		public BaseClientNetworkManager Client;

		public bool IsServer => Server != null;
		public bool IsClient => Client != null;
		
		public override void OnStartServer() {
			base.OnStartServer();
			Server = new BaseServerNetworkManager();
			Server.Init();
		}

		public override void OnStartClient() {
			base.OnStartClient();
			Client = new BaseClientNetworkManager();
			Client.Init();
		}

		public override void OnServerDisconnect(NetworkConnection conn) {
			base.OnServerDisconnect(conn);
			Server.DisconnectClient(conn);
		}

		public override void OnServerAddPlayer(NetworkConnection conn) {
			base.OnServerAddPlayer(conn);
			Server.TryAddPlayer(conn);
		}

		public override void OnClientConnect(NetworkConnection conn) {
			base.OnClientConnect(conn);
			Client.ConnectToServer(conn);
		}

		public override void OnClientDisconnect(NetworkConnection conn) {
			base.OnClientDisconnect(conn);
			Client.DisconnectFromServer();
		}

		public void RegisterCallback<T>(Action<T> callback) where T : struct, NetworkMessage {
			Client?.RegisterCallback(callback);
			Server?.RegisterCallback(callback);
		}
		
		public void UnregisterAllCallbacks<T>() where T : struct, NetworkMessage {
			Client?.UnregisterAllCallbacks<T>();
			Server?.UnregisterAllCallbacks<T>();
		}
		
	}
}