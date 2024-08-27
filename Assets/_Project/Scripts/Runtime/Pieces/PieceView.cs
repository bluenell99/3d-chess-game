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

        /// <summary>
        /// Reference to the PieceController
        /// </summary>
        public PieceController Controller { get; private set; }
        /// <summary>
        /// Reference to the GameController
        /// </summary>
        private GameController _gameController;

        /// <summary>
        /// Sets this PieceView as selectable
        /// </summary>
        /// <param name="selecatable"></param>
        private void SetSelectable(bool selecatable)
        {
            _isSelectable = selecatable;
        }

        /// <summary>
        /// Initialises this PieceView
        /// </summary>
        /// <param name="controller">The container Transform</param>
        /// <param name="material">The Mesh material</param>
        /// <param name="color">It's Piece colour</param>
        /// <param name="type">It's Piece Type</param>
        public void Initialise(PieceController controller, Material material, PieceColor color, PieceType type)
        {
            Controller = controller;
            _isSelectable = true;
            _transform = transform;
            _gameController = GameController.Instance;

            SetMaterial(material);
            SetRotation(color, type);
        }

        /// <summary>
        /// Sets the rotation of the piece view
        /// </summary>
        /// <param name="color">The colour of the piece</param>
        /// <param name="type">The type of the Piece</param>
        /// <remarks>Most pieces have symmetry, but the Knight needs to face it's teams forward direction</remarks>
        private void SetRotation(PieceColor color, PieceType type)
        {
            // if it's not a Knight, we can exit early as all other Pieces have symmetry
            if (type != PieceType.Knight)
                return;

            // update the views rotation based on colour
            float rotationAmount = color == PieceColor.White ? 180f : 0;
            Quaternion rotation = Quaternion.Euler(0, rotationAmount, 0);
            transform.rotation = rotation;
        }

        /// <summary>
        /// Sets the View's World Space position
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(Vector3 position)
        {
            transform.localPosition = position;
        }

        
        /// <summary>
        /// Removes this PieceView from play
        /// </summary>
        /// <param name="color"></param>
        /// <remarks>When Pieces are taken, they appear outside of the board. The Board doesn't consider it a piece in play, and is no longer selectable
        /// but it's nice to still show the view to the player, like how a real game of chess works</remarks>
        public void Take(PieceColor color)
        {
            // set as unselectable
            SetSelectable(false);
            
            // create a new Placement stratgegy
            PlacementStrategy placementStrategy = new RandomPointInCircle(3);
            Transform container = GameController.Instance.GetTakenPieceContainer(color);
            
            // set it's position 
            Vector3 position = placementStrategy.GetPosition(container.transform.position);
            SetPosition(position);
        }

        /// <summary>
        /// Sets the MeshRenderer's material 
        /// </summary>
        /// <param name="material"></param>
        private void SetMaterial(Material material)
        {
            if (_meshRenderer == null)
                _meshRenderer = GetComponentInChildren<MeshRenderer>();

            _meshRenderer.material = material;
        }

        /// <summary>
        /// When the PieceView is selected
        /// </summary>
        /// <remarks>PieceView implements ISelectable</remarks>
        public void OnSelect()
        {
            // if the Piece isn't considered selectable, we exit early
            if (!_isSelectable)
                return;
            
            // if turns are enabled AND it's not our turn. OR if PawnPromotion is in progress, we exit early
            if ((_gameController.TurnsEnabled && Controller.Piece.PieceColor != _gameController.CurrentTurn) 
                ||_gameController.PawnPromotionInProgress)
                return;
            
            // update the selected Piece in the game controller
            _gameController.SetSelectedPiece(this);


        }

        // TODO implement a CheckIndicator GameObject to visually show when the King is under check
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