using UnityEngine;

using NetworkTicTacToe.Gameplay;
using NetworkTicTacToe.Gameplay.Players;

using Mirror;

namespace NetworkTicTacToe.Behaviours {
    public class GameStarter : MonoBehaviour {
        public GameplayUI GameplayUI;

        public GameplayController GameplayController;
        public Player             Player;

        GameplayNetworkManager _networkManager;

        void Start() {
            GameplayController            =  new GameplayController();
            _networkManager               =  NetworkManager.singleton as GameplayNetworkManager;
            if ( !_networkManager.IsReady && _networkManager.IsServer) {
                _networkManager.OnServerReadyToPlay += OnServerReady;
            } else {
                OnServerReady();
            }
            Player                        =  new Player(_networkManager, GameplayController);
            TryInitServer();
            TryInitClient();
        }

        void OnDestroy() {
            Player?.Deinit();
            _networkManager.OnServerReadyToPlay -= OnServerReady;
        }

        // Server only callback
        void OnServerReady() {
            print("Game started");
            GameplayUI.Init(this);
        }

        void TryInitClient() {
            if ( (!_networkManager) || (!_networkManager.IsClient) ) {
                return;
            }
            Debug.Log("Register client handlers");
            _networkManager.SetClientAsReadyToPlay();
        }

        void TryInitServer() {
            if ( (!_networkManager) || (!_networkManager.IsServer) ) {
                return;
            }
            Debug.Log("Register server handlers");
            _networkManager.SetServerAsReadyToPlay();
        }
    }
}
