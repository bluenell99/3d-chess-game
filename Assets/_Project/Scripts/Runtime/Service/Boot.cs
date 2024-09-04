using System;
using UnityEditor;
using UnityEngine;

namespace ChessGame
{
    public class Boot : MonoBehaviour
    {
        private void Awake()
        {
            ServiceManager.GetService<SceneService>().LoadMenuScene();
        }
    }
}