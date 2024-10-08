﻿using UnityEngine;

namespace ChessGame
{
    public class SelectionView : MonoBehaviour, ISelectable
    {

        [SerializeField] private GameObject _gfx;

        private MeshRenderer[] _renderers;

        private bool isSelectable;
        
        public void Initialise(bool selectable)
        {
            isSelectable = selectable;
        }
        
        public void MoveSelectionView(Vector3 position)
        {
            _gfx.SetActive(true);
            transform.position = position;
        }

        public void HideSelectionView()
        {
            _gfx.SetActive(false);
        }

        public void SetColor(Color color)
        {
            if (_renderers == null)
                _renderers = _gfx.GetComponentsInChildren<MeshRenderer>();

            foreach (var renderer in _renderers)
            {
                renderer.material.color = color;
            }
        }

        public Vector2Int Coordinate { get; set; }


        public void OnSelect()
        {
            if (!isSelectable) return;
            
            GameController.Instance.MoveSelected(Coordinate);
        }
    }
}