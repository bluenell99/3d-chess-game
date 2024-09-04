using ChessGame;
using UnityEditor;
using UnityEngine;

namespace ChessGame
{
    public class GameLoader : Singleton<GameLoader>
    {

        public void LoadGame(bool bot)
        {
            DataManager.Instance.PlayingAgainstBot = bot;
            ServiceManager.GetService<SceneService>().LoadGameScene();

        }

        public void Quit()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                EditorApplication.isPlaying = false;
                return;
            }
#endif

            Application.Quit();
        }
    }
}