using UnityEngine;

using System.Collections.Generic;

namespace NetworkTicTacToe.Behaviours {
	public class GridView : MonoBehaviour{
		public List<CellView> Cells;

		GameplayController _gameplayController;
		
		void Init(GameplayController gameplayController) {
			_gameplayController = gameplayController;
			var gridSideSize = gameplayController.GridSideSize;
			for (var index = 0; index < Cells.Count; index++ ) {
				Cells[index].Init(gameplayController, index % gridSideSize, index / gridSideSize);
			}

			_gameplayController.OnStateChanged += UpdateGrid;
		}

		void OnDestroy() {
			_gameplayController.OnStateChanged -= UpdateGrid;
		}

		void UpdateGrid() {
			foreach ( var cell in Cells ) {
				cell.UpdateView();
			}
		}
	}
}