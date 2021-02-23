using UnityEngine;

using System;

using NetworkTicTacToe.State;

namespace NetworkTicTacToe {
	public class GameplayController {
		CellType[,] _gameBoard = new CellType[3, 3];

		public int GridSideSize => _gameBoard.GetLength(0);
		
		public event Action OnStateChanged;
		
		public bool TrySetCellValue(CellType newType, int x, int y) {
			if ( !IsCellAvailable(x, y) ) {
				Debug.LogError($"Can't set cell value - coords are invalid ({x},{y}) or cell isn't empty");
				return false;
			}
			_gameBoard[x, y] = newType;
			return true;
		}

		public CellType GetCellState(int x, int y) {
			if ( !IsCellAvailable(x, y) ) {
				Debug.LogError($"Can't set cell value - coords are invalid ({x},{y}) or cell isn't empty");
				return CellType.Invalid;
			}
			return _gameBoard[x, y];
		}

		bool AreCoordsCorrect(int x, int y) {
			return (x >= 0) && (x < _gameBoard.GetLength(0)) && (y >= 0) && (y < _gameBoard.GetLength(1));
		}
		
		bool IsCellAvailable(int x, int y) {
			if ( AreCoordsCorrect(x, y) ) {
				return false;
			}
			return _gameBoard[x, y] == CellType.None;
		}
	}
}