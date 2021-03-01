using NetworkTicTacToe.State;

namespace NetworkTicTacToe.Behaviours {
	public class GameplayController {
		public GameplayControllerState State = new GameplayControllerState();

		public GameplayController() {
			State.Field = new CellType[3][];
			for (var i = 0; i < 3; i++) {
				 State.Field[i] = new CellType[3];
			}
		}
	}
}