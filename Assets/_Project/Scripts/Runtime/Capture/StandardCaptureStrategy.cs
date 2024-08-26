using UnityEngine;

namespace ChessGame
{
    public class StandardCaptureStrategy : CaptureStrategy
    {
        public override void TryCapture(Board board, Piece piece, Vector2Int position)
        {
            if (board.TryGetPieceFromSquare(position, out Piece occupyingPiece))
            {
                if (occupyingPiece.PieceColor != piece.PieceColor)
                {
                    // invoke the taken event on the opponents Piece
                    occupyingPiece.Take();
                }
            }
        }
    }
}