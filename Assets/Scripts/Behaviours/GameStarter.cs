using UnityEngine;

using NetworkTicTacToe.Gameplay;
using NetworkTicTacToe.Gameplay.Players;
using NetworkTicTacToe.Utils.Network;

using Mirror;

namespace NetworkTicTacToe.Behaviours {
    public class GameStarter : MonoBehaviour {
        // Exists only on host
        Server _server;
        
        public GameplayUI GameplayUI;

        public GameplayController GameplayController;
        public Player             Player;

        ComplexNetworkManager _networkManager;

        void Start() {
            _networkManager    =  NetworkManager.singleton as ComplexNetworkManager;
            GameplayController =  new GameplayController();
            Player       = new Player(_networkManager, GameplayController);
            if ( _networkManager.IsServer ) {
                _server = new Server();
                _server.Init(_networkManager.Server);
            }
            if ( !_networkManager.Client.IsReady ) {
                _networkManager.Client.OnServerReadyToPlay += OnServerReady;
            } else {
                OnServerReady();
            }
            TryInitServer();
            TryInitClient();
        }

        void OnDestroy() {
            Player?.Deinit();
            _server?.Deinit();
            _networkManager.Client.OnServerReadyToPlay -= OnServerReady;
        }

        void OnServerReady() {
            _server?.SendSidesToPlayers();
            print("Game started");
            GameplayUI.Init(this);
            _networkManager.Client.OnServerReadyToPlay -= OnServerReady;
        }

        void TryInitClient() {
            if ( (!_networkManager) || (!_networkManager.IsClient) ) {
                return;
            }
            Debug.Log("Register client handlers");
            _networkManager.Client.SetAsReadyToPlay();
        }

        void TryInitServer() {
            if ( (!_networkManager) || (!_networkManager.IsServer) ) {
                return;
            }
            Debug.Log("Register server handlers");
            _networkManager.Server.SetAsReadyToPlay();
        }
    }
}
