using UnityEngine;

namespace ChessGame
{
    public class PieceController
    {
        /// <summary>
        /// Reference to the Piece model class
        /// </summary>
        private readonly Piece _piece;
        /// <summary>
        /// Reference the Piece view class
        /// </summary>
        private readonly PieceView _pieceView;
        
        /// <summary>
        /// The World Space square size
        /// </summary>
        private readonly float _squareSize;

        /// <summary>
        /// Get access to the Piece model
        /// </summary>
        public Piece Piece => _piece;

        /// <summary>
        /// Constructs a new PieceController with given Piece model, view, material, and square Size
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="view"></param>
        /// <param name="materials"></param>
        /// <param name="squareSize"></param>
        public PieceController(Piece piece, PieceView view, PieceMaterials materials, float squareSize)
        {
            _piece = piece;
            _pieceView = view;
            _squareSize = squareSize;

            // subscribe to required events
            _piece.onPositionChanged += OnPiecePositionChanged;
            _piece.onPieceTaken += OnPieceTaken;
            _piece.onPieceTurnEnd += OnPieceTurnEnd;

            // we also subscribe the OnPieceTaken function to the onPawnPromotion event as the Pawn needs to be removed from play
            if (_piece is Pawn pawn)
            {
                pawn.onPawnPromotionAvailable += OnPieceTaken;
            }

        }

        /// <summary>
        /// Callback to onPieceTurnEnd
        /// </summary>
        /// <param name="piece"></param>
        private void OnPieceTurnEnd(Piece piece)
        {
           GameController.Instance.NextTurn();
        }

        /// <summary>
        /// Callback to onPieceTaken
        /// </summary>
        /// <param name="piece"></param>
        private void OnPieceTaken(Piece piece)
        {
            _pieceView.Take(piece.PieceColor);
        }


        /// <summary>
        /// Callback to onPiecePositionChanged
        /// </summary>
        /// <param name="coordinate"></param>
        private void OnPiecePositionChanged(Vector2Int coordinate)
        {
            var offset = _squareSize / 2;
            var x = coordinate.x * _squareSize + offset;
            var y = coordinate.y * _squareSize + offset;

            var position = new Vector3(x, 0, y);

            _pieceView.SetPosition(position);
        }

        /// <summary>
        /// Places the Piece on the board for Initial setup
        /// </summary>
        /// <param name="newPosition"></param>
        public void PlacePieceOnBoard(Vector2Int newPosition)
        {
            _piece.SetPositionOnBoard(newPosition, true, false);
        }
    }
}