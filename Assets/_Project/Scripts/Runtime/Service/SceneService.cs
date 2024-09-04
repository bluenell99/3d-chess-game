using ChessGame;

using UnityEngine.SceneManagement;

public class SceneService : Service
{
    /// <summary>
    /// Reloads the current active scene
    /// </summary>
    public void ReloadScene()
    {
        Scene currentScene;
        currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("_Project/Scenes/Main");
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene("_Project/Scenes/Menu");
    }
}
