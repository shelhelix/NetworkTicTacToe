using Mirror;

namespace NetworkTicTacToe.Behaviours {
	public class CustomNetworkManager : NetworkManager {
		public override void OnClientConnect(NetworkConnection conn) {
			base.OnClientConnect(conn);
			print("client connected");
		}
	}
}