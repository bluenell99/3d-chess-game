using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Serialization;

namespace ChessGame
{
    public class GameController : Singleton<GameController>
    {

        [FormerlySerializedAs("_container")] [Header("Game Setup")] [SerializeField]
        private Transform _pieceContainer;

        [SerializeField, Required] private Transform _availableMovesContainer;
        [SerializeField, Required] private BoardLayout _layout;
        [SerializeField, Required] private PieceMaterials _materials;
        [SerializeField, Required] private SelectionView _selectionView;
        [SerializeField, Required] private TakenPieceContainer _takenPiecesContainer;
        [SerializeField, Required] private RectTransform _pawnPromotionUI;
        [SerializeField, Required] private GameWinUI _gameWinUI;
        
        [Header("Game Settings")]
        [SerializeField] private bool _turnsEnabled;

        [SerializeField] private bool _playAgainstBot;
        
        private BoardController _boardController;
        private InputService _inputService;
        private AudioService _audioService;
        private SceneService _sceneService;
        

        private Camera _mainCamera;
        private Piece _currentlySelectedPiece;
        private Pawn _pawnToBePromoted;
        private Stockfish _stockfish;

        public PieceColor CurrentTurn { get; private set; }
        public bool PawnPromotionInProgress { get; private set; }
        private bool _gameOver;

        public bool TurnsEnabled => _turnsEnabled;


        private void Start()
        {
            _mainCamera = Camera.main;
            _gameOver = false;

            _selectionView.Initialise(false);
            _selectionView.HideSelectionView();
            
            _gameWinUI.gameObject.SetActive(false);
            _pawnPromotionUI.gameObject.SetActive(false);
            
            _boardController = new BoardController(_pieceContainer, _layout, _materials);
            
            GetServices();
            StartStockfish();

            _inputService.InputReader.onSelectPressed += DetectSelection;

        }

        private void StartStockfish()
        {
            if (!DataManager.Instance.PlayingAgainstBot)
                return;
            
            _stockfish = new Stockfish(1);
            string fen = Fen.Encode(_boardController.Board);
            _stockfish.Start();
            _stockfish.SetFenPosition(fen);
        }

        private void GetServices()
        {
            _inputService = ServiceManager.GetService<InputService>();
            _audioService = ServiceManager.GetService<AudioService>();
            _sceneService = ServiceManager.GetService<SceneService>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                _sceneService.ReloadScene();
        }

        private void DetectSelection()
        {
            if (_gameOver) return;
            
            Ray ray = _mainCamera.ScreenPointToRay(_inputService.InputReader.MousePointer);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.TryGetComponent(out ISelectable selectable))
                {
                    selectable.OnSelect();
                }
            }
        }

        /// <summary>
        /// Sets the games current turn
        /// </summary>
        /// <param name="color">The given turns colour </param>
        public void SetTurn(PieceColor colour) => CurrentTurn = colour;


        /// <summary>
        /// Advance to the next turn
        /// </summary>
        public void NextTurn()
        {
            CurrentTurn = CurrentTurn == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;

            if (!DataManager.Instance.PlayingAgainstBot) return;

            if (CurrentTurn != PieceColor.Black) return;
            
            ProcessBotMove();
        }

        /// <summary>
        /// Processes a move with the Stockfish API
        /// </summary>
        private void ProcessBotMove()
        {
            // get the current board layout in Fen notation
            string fen = Fen.Encode(_boardController.Board);
            
            // send to stockfish API
            _stockfish.SetFenPosition(fen);
            string bestMove =  _stockfish.GetBestMove();
            
            // convert response to usable Vector2Int coordinates (response gives piece to move position, and where to position)
            Vector2Int[] bestMoveConverted = Utilities.AlgebraicMoveToVector2IntArray(bestMove);
            
            // get the piece
            Piece fromPiece = _boardController.Board.GetPieceFromSquare(bestMoveConverted[0]);
            // move the piece
            fromPiece.SetPositionOnBoard(bestMoveConverted[1], false, false);
            
            // wait half a second so it's not instant
            StartCoroutine(WaitForSeconds(0.5f));
        }

        private IEnumerator WaitForSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
        }
        
        /// <summary>
        /// Ends the game
        /// </summary>
        /// <param name="color">The winning piece colour</param>
        public void EndGame(PieceColor color)
        {
            _gameWinUI.gameObject.SetActive(true);
            _gameWinUI.ShowWinner(color);
            _gameOver = true;
        }
        
        /// <summary>
        /// Sets the currently selected piece
        /// </summary>
        /// <param name="view">The selected PieceView object</param>
        public void SetSelectedPiece(PieceView view)
        {
            // cache the piece
            var piece = view.Controller.Piece;

            // if the given piece is not the currently selected
            if (_currentlySelectedPiece != piece)
            {
                // set the currently selected as the given piece and update the selection views
                _currentlySelectedPiece = piece;
                _selectionView.MoveSelectionView(view.transform.position);
                _selectionView.SetColor(Color.blue);

                ShowAvailableMoves(view);

            }
            else // otherwise, clear the selection views as we've unselected our piece
            {
                _currentlySelectedPiece = null;
                _selectionView.HideSelectionView();
                ClearAvailableMovesView();
            }

        }

        /// <summary>
        /// Shows a <c>SelectionView</c> object for each available move this piece has
        /// </summary>
        /// <param name="view"></param>
        private void ShowAvailableMoves(PieceView view)
        {
            // Clear out any current views
            ClearAvailableMovesView();

            // Get reference to the actual piece, and it's moves
            Piece piece = view.Controller.Piece;
            HashSet<Move> moves = piece.GetLegalMoves().Where(m => m.IsDeliveringCheck == false).ToHashSet();

            // iterate through each of it's moves
            foreach (var move in moves)
            {
                // calculate the world space X & Z positions for the piece
                var offset = _layout.SquareSize / 2;
                var x = move.Coordinate.x * _layout.SquareSize + offset;
                var z = move.Coordinate.y * _layout.SquareSize + offset;
                var position = new Vector3(x, 0, z);

                // instantiate and initialise the SelectionView
                var selection = Instantiate(_selectionView, _availableMovesContainer);
                selection.Initialise(true);
                
                // Update it's position and reference coordinate
                selection.transform.localPosition = position;
                selection.Coordinate = move.Coordinate;
            }

            // notify the AudioService
            _audioService.OnPieceMove();

        }

        /// <summary>
        /// Clears all the <c>SelectionView</c> objects from the screen
        /// </summary>
        private void ClearAvailableMovesView()
        {
            // if there are no SelectionView objects, exit early
            if (_availableMovesContainer.transform.childCount == 0)
                return;

            // iterate through all the SelectionView objects under the containg Transform and destroy it
            for (int i = 0; i < _availableMovesContainer.transform.childCount; i++)
            {
                Destroy(_availableMovesContainer.transform.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Moves the currently selected <c>Piece</c> to a new board coordinate
        /// </summary>
        /// <param name="coordinate"></param>
        public void MoveSelected(Vector2Int coordinate)
        {
            // move the Piece
            _currentlySelectedPiece.SetPositionOnBoard(coordinate, false, false);

            // Update the currently selected Piece and it's SelectionView
            _currentlySelectedPiece = null;
            _selectionView.HideSelectionView();

            // notify the AudioService
            _audioService.OnPieceMove();

            // Clear it's available moves SelectionView's
            ClearAvailableMovesView();

        }

        /// <summary>
        /// Displays the PawnPromotion UI object
        /// </summary>
        /// <param name="pawn">The Pawn that will be promoted</param>
        public void DisplayPromotionUI(Pawn pawn)
        {
            PawnPromotionInProgress = true;
            _pawnToBePromoted = pawn;
            _pawnPromotionUI.gameObject.SetActive(true);
        }

        /// <summary>
        /// UI button callback when user selects their desired promotion Piece type
        /// </summary>
        /// <param name="intType">The enum index for the PieceType</param>
        /// <remarks>Enums cannot be used as button event parameters, so we use the integer index of the enum value instead</remarks>
        public void PromotionSelected(int intType)
        {
            // Cast the integer value as a usable PieceType
            PieceType type = (PieceType)intType;
            
            // Update flags, and update the Board with the new Piece
            PawnPromotionInProgress = false;
            _boardController.UpdatePiece(_pawnToBePromoted, type);
            
            // Cleanup
            _pawnToBePromoted = null;
            _pawnPromotionUI.gameObject.SetActive(false);
        }

        /// <summary>
        /// Returns the Transform of the given PieceColor's "Taken Piece Container"
        /// </summary>
        /// <param name="color">The taken Piece's colour</param>
        /// <returns></returns>
        /// <remarks>This is where Pieces spawn when they have been captured</remarks>
        public Transform GetTakenPieceContainer(PieceColor color)
        {
            return color == PieceColor.White ? _takenPiecesContainer.White : _takenPiecesContainer.Black;
        }

        private void OnApplicationQuit()
        {
            if (_stockfish!= null)
            {
                _stockfish.StopStockfish();
            }
        }
    }
}