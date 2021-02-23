using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;

using NetworkTicTacToe.State;

namespace NetworkTicTacToe.Behaviours {
	public class CellView : MonoBehaviour {
		[Serializable]
		public class CellSpriteContainer {
			public CellType Type;
			public Sprite   Sprite;
		}

		public Image                     Image;
		public Button                    Button;
		public List<CellSpriteContainer> Sprites;

		int _x;
		int _y;

		GameplayController _gameplayController;
		
		public void Init(GameplayController gameplayController, int x, int y) {
			_x                  = x;
			_y                  = y;
			_gameplayController = gameplayController;
		}
		
		public void UpdateView() {
			var curState        = _gameplayController.GetCellState(_x, _y);
			var spriteContainer = Sprites.Find(x => (x.Type == curState));
			Image.sprite        = spriteContainer?.Sprite;
			Button.interactable = (curState == CellType.None);
		}
	}
}
