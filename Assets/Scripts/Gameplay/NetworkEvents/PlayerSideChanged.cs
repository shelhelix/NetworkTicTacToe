using Mirror;

namespace NetworkTicTacToe.Gameplay.NetworkEvents {
	public struct PlayerSideChanged : NetworkMessage {
		public PlayerSide Side;

		public PlayerSideChanged(PlayerSide side) {
			Side = side;
		}
	}
}