using System;

namespace NetworkTicTacToe.State {
	[Serializable]
	public sealed class GameplayControllerState {
		public CellType[][] Field;
	}
}