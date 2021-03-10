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
		ClientPlayer             _clientPlayer;
		
		void OnDestroy() {
			if ( _gameplayController != null ) {
				_gameplayController.OnStateChangedExternally -= RefreshUI;
				_gameplayController.OnPartyEnded             -= OnPartyEnded;
				_gameplayController.OnTurnChanged            -= RefreshUI;
			}

			foreach ( var cell in Cells ) {
				cell.Deinit();
			}
		}

		public void Init(GameStarter starter) {
			_gameplayController                          =  starter.GameplayController;
			_gameplayController.OnStateChangedExternally += RefreshUI;
			_gameplayController.OnTurnChanged            += RefreshUI;
			_gameplayController.OnPartyEnded             += OnPartyEnded;
			_clientPlayer                                =  starter.ClientPlayer;
			
			InitCells(_gameplayController, starter.ClientPlayer);
			RefreshUI();
			
			MessageRoot.SetActive(false);
		}

		void InitCells(GameplayController gameplayController, ClientPlayer clientPlayer) {
			// Init cells 
			for ( var index = 0; index < Cells.Count; index++ ) {
				var y = index / GameplayController.BoardSideSize;
				var x = index % GameplayController.BoardSideSize;
				Cells[index].Init(gameplayController, clientPlayer, x, y);
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
			MessageText.text = _gameplayController.State.CurrentPlayer == _clientPlayer.PlayerSide ? "You win" : "You died";
		}
	}
}