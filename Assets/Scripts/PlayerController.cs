using UnityEngine;

using System.Collections.Generic;

using NetworkTicTacToe.Utils;

namespace NetworkTicTacToe {
	public class PlayerController : Singleton<PlayerController> {
		public const int NeededPlayers = 2;
		
		// Converting player id => player side
		readonly Dictionary<int, PlayerSide> _allPlayers = new Dictionary<int, PlayerSide>();

		public int ReadyPlayers => _allPlayers.Count;
		
		public bool TryAddPlayer(int playerId) {
			if ( _allPlayers.Count == NeededPlayers ) {
				Debug.LogError("Can't add player - room is full");
				return false;
			}
			if ( _allPlayers.ContainsKey(playerId) ) {
				Debug.LogError($"Can't add player - player with id {playerId} already connected");
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