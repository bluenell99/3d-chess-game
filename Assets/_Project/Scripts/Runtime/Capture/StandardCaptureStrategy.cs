using UnityEngine;

namespace ChessGame
{
    public class StandardCaptureStrategy : CaptureStrategy
    {
        /// <summary>
        /// Try and capture a Piece on the Board.
        /// </summary>
        /// <param name="board">The Board in play</param>
        /// <param name="piece">The Piece attempting a capture</param>
        /// <param name="position">The target coordinate</param>
        public override void TryCapture(Board board, Piece piece, Vector2Int position)
        {
            // try and get a piece from the target coordinate
            if (!board.TryGetPieceFromSquare(position, out Piece occupyingPiece)) return;
            
            // check if it's our colour
            if (occupyingPiece.PieceColor == piece.PieceColor) return;
            
            // invoke the taken event on the opponents Piece
            occupyingPiece.Take();
        }
    }
}