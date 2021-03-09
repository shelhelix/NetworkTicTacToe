using System.IO;

using UnityEditor;
using UnityEditor.SceneManagement;

namespace NetworkTicTacToe.Editor {
	[InitializeOnLoad]
	public static class SimpleEditorUtils {
		static SimpleEditorUtils() {
			EditorApplication.playModeStateChanged += OnEnterPlayMode;
		}

		static void OnEnterPlayMode(PlayModeStateChange e) {
			switch ( e ) {
				case PlayModeStateChange.ExitingEditMode: {
					var oldScene = EditorSceneManager.GetActiveScene().path;
					File.WriteAllText("stashedPath.txt", oldScene);
					EditorSceneManager.SaveOpenScenes();
					EditorSceneManager.OpenScene("Assets/Scenes/StartScene.unity");
					break;
				}
				case PlayModeStateChange.EnteredEditMode: {
					var oldScene = File.ReadAllText("stashedPath.txt");
					EditorSceneManager.OpenScene(oldScene);
					break;
				}
			}
		}
	}
}