using UnityEngine;

using System;
using System.Collections.Generic;

using NetworkTicTacToe.State;

namespace NetworkTicTacToe.Gameplay {
	public class GameplayController {
		public const int BoardSideSize = 3;
		
		GameplayControllerState _state = new GameplayControllerState(BoardSideSize);
		
		readonly Dictionary<PlayerSide, CellType> _playerToCellTypeConverter = new Dictionary<PlayerSide, CellType> {
			{PlayerSide.Circle, CellType.Circle},
			{PlayerSide.Cross,  CellType.Cross}
		};
		
		readonly Dictionary<PlayerSide, PlayerSide> _turnRotator = new Dictionary<PlayerSide, PlayerSide> {
			{PlayerSide.Circle, PlayerSide.Cross},
			{PlayerSide.Cross,  PlayerSide.Circle}
		};

		public GameplayControllerState State {
			get => _state;
			set {
				_state = value;
				OnStateChangedExternally?.Invoke();
			}
		}
		
		public event Action OnStateChangedExternally;
		public event Action OnTurnChanged;
		public event Action OnPartyEnded;

		public void MakeTurn(PlayerSide player, int x, int y) {
			if ( !IsCellEmpty(x, y) ) {
				Debug.LogError($"Can't place item on this cell {x},{y} - cell isn't empty");
				return;
			}
			if ( State.CurrentPlayer != player ) {
				Debug.LogError($"Can't place item on this cell {x},{y} - it's not our turn now");
				return;
			}
			PlaceCurPlayerItemOnCell(x, y);
			if ( CheckWin() ) {
				OnPartyEnded?.Invoke();
			} else {
				RollTurn();
			}
		}

		public CellType GetCellState(int x, int y) {
			if ( !State.IsOnField(x, y) ) {
				Debug.LogError($"Can't get item from the cell {x},{y} - cell coords are out of bounds");
				return CellType.Invalid;
			}
			return State.GetCell(x, y);
		}
		
		bool IsCellEmpty(int x, int y) {
			if ( !State.IsOnField(x, y) ) {
				Debug.LogError($"Can't get item from the cell {x},{y} - cell coords are out of bounds");
				return false;
			}
			return State.GetCell(x, y) == CellType.Empty;
		}

		void PlaceCurPlayerItemOnCell(int x, int y) {
			if ( !State.IsOnField(x, y) ) {
				Debug.LogError($"Can't place item on the cell {x},{y} - cell coords are out of bounds");
				return;
			}
			if ( !_playerToCellTypeConverter.ContainsKey(State.CurrentPlayer) ) {
				Debug.LogError($"Can't get item from the cell {x},{y} - unknown player side {State.CurrentPlayer}");
				return;
			}
			var cellType = _playerToCellTypeConverter[State.CurrentPlayer];
			State.SetCell(x, y, cellType);
		}

		void RollTurn() {
			if ( !_turnRotator.ContainsKey(State.CurrentPlayer) ) {
				Debug.LogError($"Can't rotate turn - unknown player side {State.CurrentPlayer}");
				return;
			}
			State.CurrentPlayer = _turnRotator[State.CurrentPlayer];
			OnTurnChanged?.Invoke();
		}

		bool CheckWin() {
			var curPlayerCellType = _playerToCellTypeConverter[State.CurrentPlayer];
			//Check horizontal
			for ( var y = 0; y < State.RowSize; y++ ) {
				var isOk = true;
				for ( var x = 0; x < State.RowSize && isOk; x++ ) {
					if ( State.GetCell(x, y) != curPlayerCellType) {
						isOk = false;
					}	
				}
				if ( isOk ) {
					return true;
				}
			}
			//Check vertical
			for ( var x = 0; x < State.RowSize; x++ ) {
				var isOk = true;
				for ( var y = 0; y < State.RowSize && isOk; y++ ) {
					if ( State.GetCell(x, y) != curPlayerCellType) {
						isOk = false;
					}	
				}
				if ( isOk ) {
					return true;
				}
			}
			//left diagonal check
			{
				var isOk = true;
				for ( var x = 0; x < State.RowSize; x++ ) {
					if ( State.GetCell(x, x) != curPlayerCellType ) {
						isOk = false;
					}

					if ( isOk ) {
						return true;
					}
				}
			}
			//right diagonal check
			{
				var isOk = true;
				for ( var x = 0; x < State.RowSize; x++ ) {
					if ( State.GetCell(x, State.RowSize-1-x) != curPlayerCellType ) {
						isOk = false;
					}
					if ( isOk ) {
						return true;
					}
				}
			}
			return false;
		}
	}
}