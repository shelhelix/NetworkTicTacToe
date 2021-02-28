using Mirror;

namespace NetworkTicTacToe.State {
	public struct PlayerState : NetworkMessage {
		public PlayerSide Side;
	}
}