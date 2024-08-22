using UnityEngine;
using UnityEngine.Serialization;

namespace ChessGame
{
    public class PieceView : MonoBehaviour, ISelectable
    {

        [FormerlySerializedAs("checkDetector")] [SerializeField]
        private GameObject checkIndicator;

        private MeshRenderer _meshRenderer;
        private bool _isSelectable;
        private Transform _transform;

        
        public PieceController Controller { get; private set; }

        private void SetSelectable(bool selecatable)
        {
            _isSelectable = selecatable;
        }

        public void Initialise(PieceController controller, Material material, PieceColor color, PieceType type)
        {
            Controller = controller;
            _isSelectable = true;
            _transform = transform;

            SetMaterial(material);
            SetRotation(color, type);
        }

        private void SetRotation(PieceColor color, PieceType type)
        {
            if (type != PieceType.Knight)
                return;

            float rotationAmount = color == PieceColor.White ? 180f : 0;
            Quaternion rotation = Quaternion.Euler(0, rotationAmount, 0);
            transform.rotation = rotation;
        }

        public void SetPosition(Vector3 position)
        {
            transform.localPosition = position;
        }

        public void Take(PieceColor color)
        {
            SetSelectable(false);
            
            PlacementStrategy placementStrategy = new RandomPointInCircle(3);
            Transform container = GameController.Instance.GetTakenPieceContainer(color);
            
            _transform.SetParent(container);
            _transform.position = Vector3.zero;
            Vector3 position = placementStrategy.GetPosition(_transform.position);
            
            SetPosition(position);
        }

        private void SetMaterial(Material material)
        {
            if (_meshRenderer == null)
                _meshRenderer = GetComponentInChildren<MeshRenderer>();

            _meshRenderer.material = material;
        }

        public void OnSelect()
        {
            if (!_isSelectable)
                return;
            
            if ((GameController.Instance.TurnsEnabled &&
                 Controller.Piece.PieceColor != GameController.Instance.CurrentTurn) ||
                GameController.Instance.PawnPromotionInProgress)
                return;


            GameController.Instance.SetSelectedPiece(this);


        }

        public void EnableCheckIndicator(bool enable)
        {
            if (checkIndicator != null)
            {
                checkIndicator.SetActive(enable);
            }
        }

    }

    public interface ISelectable
    {
        public void OnSelect();
    }
}