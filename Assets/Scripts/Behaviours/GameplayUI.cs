using UnityEngine;

using System.Collections.Generic;

using NetworkTicTacToe.Gameplay;
using NetworkTicTacToe.Gameplay.Players;

namespace NetworkTicTacToe.Behaviours {
	public class GameplayUI : MonoBehaviour {
		public List<Cell> Cells;
		
		GameplayController _gameplayController;
		
		void OnDestroy() {
			if ( _gameplayController != null ) {
				_gameplayController.OnStateChangedExternally -= RefreshUI;
			}

			foreach ( var cell in Cells ) {
				cell.Deinit();
			}
		}

		public void Init(GameStarter starter) {
			_gameplayController =  starter.GameplayController;
			_gameplayController.OnStateChangedExternally += RefreshUI;
			InitCells(_gameplayController, starter.Player);
			RefreshUI();
		}

		void InitCells(GameplayController gameplayController, Player player) {
			// Init cells 
			for ( var index = 0; index < Cells.Count; index++ ) {
				var y = index / GameplayController.BoardSideSize;
				var x = index % GameplayController.BoardSideSize;
				Cells[index].Init(gameplayController, player, x, y);
			}
		}
		
		void RefreshUI() {
			foreach ( var cell in Cells ) {
				cell.Refresh();
			}
		}
	}
}