using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ChessGame.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _singleplayerButton;
        [SerializeField] private Button _multiplayerButton;
        [SerializeField] private Button _quitButton;

        private void Awake()
        {
            GameLoader loader = GameLoader.Instance;

            _singleplayerButton.onClick.AddListener(() => loader.LoadGame(true));
            _multiplayerButton.onClick.AddListener(() => loader.LoadGame(false));
            _quitButton.onClick.AddListener(() => loader.Quit());
        }


}
    
    
}