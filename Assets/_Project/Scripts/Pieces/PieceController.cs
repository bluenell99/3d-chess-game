using UnityEngine;

namespace ChessGame
{
    

    public class PieceController
    {
        private readonly Piece _piece;
        private readonly PieceView _pieceView;
        private readonly float _squareSize;

        public Piece Piece => _piece;

        public PieceController(Piece piece, PieceView view, PieceMaterials materials, float squareSize)
        {
            _piece = piece;
            _pieceView = view;
            _squareSize = squareSize;

            _piece.onPositionChanged += OnPiecePositionChanged;
            _piece.onPieceTaken += OnPieceTaken;

            if (_piece is Pawn pawn)
            {
                pawn.onPawnPromotionAvailable += OnPieceTaken;
            }

        }

        private void OnPieceTaken(Piece piece)
        {
            _pieceView.gameObject.SetActive(false);
        }


        private void OnPiecePositionChanged(Vector2Int coordinate)
        {
            var offset = _squareSize / 2;
            var x = coordinate.x * _squareSize + offset;
            var y = coordinate.y * _squareSize + offset;

            var position = new Vector3(x, 0, y);

            _pieceView.SetPosition(position);
        }

        public void PlacePieceOnBoard(Vector2Int newPosition)
        {
            _piece.SetPositionOnBoard(newPosition, true, false);
        }
    }
}