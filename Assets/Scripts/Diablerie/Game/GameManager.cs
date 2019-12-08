using UnityEditor;
using UnityEngine;

namespace Diablerie.Game
{
    public class GameManager
    {
        public static void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        }
    }
}