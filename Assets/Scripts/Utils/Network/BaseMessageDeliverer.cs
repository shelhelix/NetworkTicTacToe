using Mirror;

namespace NetworkTicTacToe.Utils.Network {
	public class BaseMessageDeliverer {
		public void SendDataMessage<T>(NetworkConnection connection, T data) where T : struct, NetworkMessage {
			connection.Send(data);
		}
	}
}