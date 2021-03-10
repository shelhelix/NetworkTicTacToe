using UnityEngine;

using System.Collections.Generic;

using NetworkTicTacToe.Gameplay;
using NetworkTicTacToe.Gameplay.Players;
using TMPro;

namespace NetworkTicTacToe.Behaviours {
	public class GameplayUI : MonoBehaviour {
		public List<Cell> Cells;

		public GameObject MessageRoot;
		public TMP_Text   MessageText;
		
		GameplayController _gameplayController;
		Player             _player;
		
		void OnDestroy() {
			if ( _gameplayController != null ) {
				_gameplayController.OnStateChangedExternally -= RefreshUI;
				_gameplayController.OnPartyEnded             -= OnPartyEnded;
			}

			foreach ( var cell in Cells ) {
				cell.Deinit();
			}
		}

		public void Init(GameStarter starter) {
			_gameplayController                          =  starter.GameplayController;
			_gameplayController.OnStateChangedExternally += RefreshUI;
			_gameplayController.OnPartyEnded             += OnPartyEnded;
			_player                                      =  starter.Player;
			
			InitCells(_gameplayController, starter.Player);
			RefreshUI();
			
			MessageRoot.SetActive(false);
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

		void OnPartyEnded() {
			RefreshUI();
			MessageRoot.SetActive(true);
			MessageText.text = _gameplayController.State.CurrentPlayer == _player.PlayerSide ? "You win" : "You died";
		}
	}
}