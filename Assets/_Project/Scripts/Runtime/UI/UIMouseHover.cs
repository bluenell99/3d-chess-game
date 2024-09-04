using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;


namespace ChessGame.UI
{

    public class UIMouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        [SerializeField] private Vector3 _scaleFactor;
        [SerializeField] private float _duration;
        [SerializeField] private Ease _ease;
        
        private Vector3 _defaultScale;

        private void Awake()
        {
            _defaultScale = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOScale(_scaleFactor, _duration).SetEase(_ease);
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOScale(_defaultScale, _duration).SetEase(_ease);
        }
    }
}
