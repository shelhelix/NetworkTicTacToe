using System.Collections.Generic;
using NetworkTicTacToe.Utils;
using UnityEngine;

namespace NetworkTicTacToe {
	public class PlayerController : Singleton<PlayerController> {
		// Converting player id => player side
		readonly Dictionary<int, PlayerSide> _allPlayers = new Dictionary<int, PlayerSide>();
		
		public bool TryAddPlayer(int playerId) {
			if ( _allPlayers.Count == 2 ) {
				Debug.LogError("Can't add player - room is full");
				return false;
			}
			_allPlayers.Add(playerId, (_allPlayers.Count == 0) ? PlayerSide.Circle : PlayerSide.Cross);
			return true;
		}

		public void RemovePlayer(int playerId) {
			_allPlayers.Remove(playerId);
		}

		public PlayerSide GetPlayerSide(int playerId) {
			if ( !_allPlayers.ContainsKey(playerId) ) {
				Debug.LogError($"can't get player side. Id isn't registered {playerId}");
				return PlayerSide.Invalid;
			}
			return _allPlayers[playerId];
		}
	}
}