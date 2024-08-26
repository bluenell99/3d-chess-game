using UnityEngine;

namespace ChessGame
{
    public class PawnCaptureStrategy : CaptureStrategy
    {
        
        /// <summary>
        /// Trys to capture
        /// </summary>
        /// <param name="board"></param>
        /// <param name="piece"></param>
        /// <param name="position"></param>
        public override void TryCapture(Board board, Piece piece, Vector2Int position)
        {
            var pawn = piece as Pawn;
            
            // promotion
            int verticalEdge = piece.PieceColor == PieceColor.White ? 7 : 0;
            if (piece.Coordinate.y == verticalEdge)
                if (pawn != null)
                    pawn.Promote(pawn);

            // en-passant
            if (board.LastPieceMoved is Pawn lastMovedPawn)
            {
                int direction = piece.PieceColor == PieceColor.White ? 1 : -1;
                Vector2Int enpassantTarget = new Vector2Int(position.x, position.y - direction);

                if (lastMovedPawn.Coordinate == enpassantTarget && Mathf.Abs(piece.PreviousCoordinate.y - position.y) == 1 && piece.PreviousCoordinate.x != position.x)
                {
                    lastMovedPawn.Take();
                }
            }
            
            // normal take
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