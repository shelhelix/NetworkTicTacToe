
using UnityEngine;
using UnityEngine.UI;

using NetworkTicTacToe.Gameplay;
using NetworkTicTacToe.Gameplay.Players;
using TMPro;

namespace NetworkTicTacToe.Behaviours {
	public class Cell : MonoBehaviour {
		public Button   Button;
		public TMP_Text Text;

		GameplayController _gameplayController;
		Vector2Int         _cellCoords;
		
		public void Init(GameplayController gameplayController, Player player, int x, int y) {
			_gameplayController = gameplayController;
			_cellCoords = new Vector2Int(x, y);
			
			Button.onClick.AddListener( () => gameplayController.MakeTurn(player.PlayerSide, x, y) );
		}

		public void Deinit() {
			Button.onClick.RemoveAllListeners();
		}

		public void Refresh() {
			Text.text = _gameplayController.GetCellState(_cellCoords.x, _cellCoords.y).ToString();
		}
	}
}