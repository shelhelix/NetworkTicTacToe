using UnityEngine;

using Mirror;

namespace NetworkTicTacToe.Behaviours {
    public class GameStarter : MonoBehaviour {
        GameplayNetworkManager _networkManager;

        void Start() {
            _networkManager = NetworkManager.singleton as GameplayNetworkManager;
            print("Game started");
            TryInitServer();
            TryInitClient();
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
