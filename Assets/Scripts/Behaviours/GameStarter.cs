using UnityEngine;

namespace NetworkTicTacToe.Behaviours {
    public class GameStarter : MonoBehaviour {
        public NetworkGameplayUpdater GameplayUpdater;

        GameplayController _gameplayController;
        
        void Start() {
            _gameplayController = new GameplayController();
            GameplayUpdater.Init(_gameplayController);
        }
    }
}
