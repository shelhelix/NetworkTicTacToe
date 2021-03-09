using System;
using Mirror;

namespace NetworkTicTacToe.Utils.Network {
	[Serializable]
	public struct DataNetworkMessage : NetworkMessage {
		public byte[] Bytes;
	}
}