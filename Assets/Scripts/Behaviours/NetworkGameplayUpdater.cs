using Mirror;

using UnityEngine;

using NetworkTicTacToe.State;

namespace NetworkTicTacToe.Behaviours {
	public class NetworkGameplayUpdater : NetworkBehaviour {
		GameplayController _gameplayController;
		
		public void Init(GameplayController gameplayController) {
			_gameplayController = gameplayController;
		}

		[ClientRpc]
		public void RpcOnBoardUpdated(CellType newType, int x, int y) {
			Debug.Log("Board updated");
		}

		[TargetRpc]
		public void RpcTurnDenied() {
			Debug.LogWarning("turn denied");
		}
		
		[Command]
		public void CmdTrySetCell(CellType newType, int x, int y) {
			if ( !_gameplayController.TrySetCellValue(newType, x, y) ) {
				RpcTurnDenied();
			}
			else {
				RpcOnBoardUpdated(newType, x, y);
			}
		}
	}
}