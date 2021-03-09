using System;

using NetworkTicTacToe.Gameplay;

namespace NetworkTicTacToe.State {
	[Serializable]
	public sealed class GameplayControllerState {
		public PlayerSide CurrentPlayer = PlayerSide.Cross;
		public CellType[] Field;
		public int        RowSize;

		public GameplayControllerState() { }
		
		public GameplayControllerState(int rowSize) {
			RowSize = rowSize;
			Field = new CellType[RowSize*RowSize];
		}
		
		public bool IsOnField(int x, int y) {
			return (x >= 0) && (x < RowSize) && (y >= 0) && (y < Field.Length / RowSize);
		}

		public CellType GetCell(int x, int y) {
			return !IsOnField(x, y) ? CellType.Invalid : Field[y * RowSize + x];
		}

		public void SetCell(int x, int y, CellType cell) {
			if ( !IsOnField(x, y) ) {
				return;
			}
			Field[y * RowSize + x] = cell;
		}
	}
}