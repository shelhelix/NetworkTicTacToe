using NetworkTicTacToe.State;

using Mirror;

namespace NetworkTicTacToe.Gameplay.NetworkEvents {
	public struct GameStateChanged : NetworkMessage {
		public GameplayControllerState State;

		public GameStateChanged(GameplayControllerState state) {
			State = state;
		}
	}
}