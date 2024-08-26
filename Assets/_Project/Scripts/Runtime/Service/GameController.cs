using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Unity.UI;
using UnityEngine.UI;

namespace ChessGame
{
    public class GameController : Singleton<GameController>
    {

        [Header("Game Setup")] [SerializeField]
        private Transform _container;

        [SerializeField, Required] private Transform _availableMovesContainer;
        [SerializeField, Required] private BoardLayout _layout;
        [SerializeField, Required] private PieceMaterials _materials;
        [SerializeField, Required] private SelectionView _selectionView;
        [SerializeField, Required] private TakenPieceContainer _takenPiecesContainer;
        [SerializeField, Required] private RectTransform _pawnPromotionUI;
        [SerializeField, Required] private Text _winText;
        
        [Header("Game Settings")]
        [SerializeField] private bool _turnsEnabled;
        
        private BoardController _boardController;

        private Camera _mainCamera;
        private Piece _currentlySelectedPiece;
        private Pawn _pawnToBePromoted;


        public PieceColor CurrentTurn { get; set; }
        public bool PawnPromotionInProgress { get; private set; }

        public bool TurnsEnabled => _turnsEnabled;


        private void Start()
        {
            _mainCamera = Camera.main;

            _selectionView.Initialise(false);
            _selectionView.HideSelectionView();
            
            _pawnPromotionUI.gameObject.SetActive(false);
            _boardController = new BoardController(_container, _layout, _materials);
            _winText.gameObject.SetActive(false);

        }

        private void Update()
        {
            DetectSelection();
        }

        // TODO Optimise, refactor to a more generic "Selection" system
        private void DetectSelection()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit))
                {
                    if (hit.transform.TryGetComponent(out ISelectable selectable))
                    {
                        selectable.OnSelect();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _boardController.ResetBoard();
            }
        }

        public void DisplayWinText(PieceColor color)
        {
            _winText.gameObject.SetActive(true);
            _winText.text = $"Checkmate! {color} wins!";
        }
        
        public void SetSelectedPiece(PieceView view)
        {
            var piece = view.Controller.Piece;

            if (_currentlySelectedPiece != piece)
            {
                _currentlySelectedPiece = view.Controller.Piece;

                _selectionView.MoveSelectionView(view.transform.position);
                _selectionView.SetColor(Color.blue);

                ShowAvailableMoves(view);

            }
            else
            {
                _currentlySelectedPiece = null;
                _selectionView.HideSelectionView();
                ClearAvailableMovesView();
            }

        }

        private void ShowAvailableMoves(PieceView view)
        {
            ClearAvailableMovesView();

            Piece piece = view.Controller.Piece;
            HashSet<Move> moves = piece.GetLegalMoves().Where(m => m.IsDeliveringCheck == false).ToHashSet();


            foreach (var move in moves)
            {
                var offset = _layout.SquareSize / 2;
                var x = move.Coordinate.x * _layout.SquareSize + offset;
                var y = move.Coordinate.y * _layout.SquareSize + offset;

                var position = new Vector3(x, 0, y);

                var selection = Instantiate(_selectionView, _availableMovesContainer);
                selection.Initialise(true);
                
                selection.transform.localPosition = position;
                selection.Coordinate = move.Coordinate;
            }

            ServiceManager.GetService<AudioService>().OnPieceMove();

        }

        private void ClearAvailableMovesView()
        {
            if (_availableMovesContainer.transform.childCount == 0)
                return;

            for (int i = 0; i < _availableMovesContainer.transform.childCount; i++)
            {
                Destroy(_availableMovesContainer.transform.GetChild(i).gameObject);
            }
        }

        public void MoveSelected(Vector2Int coordinate)
        {
            _currentlySelectedPiece.SetPositionOnBoard(coordinate, false, false
            );

            _currentlySelectedPiece = null;
            _selectionView.HideSelectionView();

            ServiceManager.GetService<AudioService>().OnPieceMove();

            ClearAvailableMovesView();

        }

        public void DisplayPromotionUI(Pawn pawn)
        {
            PawnPromotionInProgress = true;
            _pawnToBePromoted = pawn;
            _pawnPromotionUI.gameObject.SetActive(true);
        }

        public void PromotionSelected(int intType)
        {
            PieceType type = (PieceType)intType;

            PawnPromotionInProgress = false;

            _boardController.UpdatePiece(_pawnToBePromoted, type);
            _pawnToBePromoted = null;
            _pawnPromotionUI.gameObject.SetActive(false);
        }

        public Transform GetTakenPieceContainer(PieceColor color)
        {
            return color == PieceColor.White ? _takenPiecesContainer.White : _takenPiecesContainer.Black;
        }
    }
}