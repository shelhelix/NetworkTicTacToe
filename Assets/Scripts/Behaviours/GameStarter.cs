using UnityEngine;

using NetworkTicTacToe.Gameplay;
using NetworkTicTacToe.Gameplay.Players;

using Mirror;
using NetworkTicTacToe.Utils.Network;

namespace NetworkTicTacToe.Behaviours {
    public class GameStarter : MonoBehaviour {
        // Exists only on host
        Server _server;
        
        public GameplayUI GameplayUI;

        public GameplayController GameplayController;
        public ClientPlayer             ClientPlayer;

        BaseNetworkManager _networkManager;

        void Start() {
            _networkManager    =  NetworkManager.singleton as BaseNetworkManager;
            GameplayController =  new GameplayController();
            ClientPlayer       = new ClientPlayer(_networkManager, GameplayController);
            if ( _networkManager.IsServer ) {
                _server = new Server();
                _server.Init(_networkManager);
            }
            if ( !_networkManager.IsReady ) {
                _networkManager.OnServerReadyToPlay += OnServerReady;
            } else {
                OnServerReady();
            }
            TryInitServer();
            TryInitClient();
        }

        void OnDestroy() {
            ClientPlayer?.Deinit();
            _server?.Deinit();
            _networkManager.OnServerReadyToPlay -= OnServerReady;
        }

        void OnServerReady() {
            _server?.SendSidesToPlayers();
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
