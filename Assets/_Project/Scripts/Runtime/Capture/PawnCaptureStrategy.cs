using UnityEngine;

namespace ChessGame
{
    public class PawnCaptureStrategy : CaptureStrategy
    {
        
        /// <summary>
        /// Try and capture a Piece on the Board.
        /// </summary>
        /// <param name="board">The Board in play</param>
        /// <param name="piece">The Piece attempting a capture</param>
        /// <param name="position">The target coordinate</param>
        /// <remarks>Pawn's can capture diagonally, and from an en-passant move. The Pawn can also be promoted, so we check for this too</remarks>
        public override void TryCapture(Board board, Piece piece, Vector2Int position)
        {
            // convert the given piece to a pawn
            var pawn = piece as Pawn;
            
            // PROMOTION
            // Calculate the rank required to promote the pawn based from it's colour
            int promotionRank = piece.PieceColor == PieceColor.White ? 7 : 0;
            
            // check the Pawns' position is it's promotion rank, and the Pawn isn't null
            if (position.y == promotionRank)
                if (pawn != null)
                {
                    pawn.Promote(pawn);
                    return;
                }

            // EN-PASSANT
            // check that the last Piece that moved was a pawn as en-passant must be acted immediatley
            if (board.LastPieceMoved is Pawn lastMovedPawn)
            {
                // get the direction of play for the Pawn
                int direction = piece.PieceColor == PieceColor.White ? 1 : -1;
                // get the target square for the en-passant move
                Vector2Int enpassantTarget = new Vector2Int(position.x, position.y - direction);

                // Check if the last moved pawn is at the en-passant target square and the current piece moved diagonally to the pawn's column.
                // If true, the pawn is captured via en-passant.
                if (lastMovedPawn.Coordinate == enpassantTarget && Mathf.Abs(piece.PreviousCoordinate.y - position.y) == 1 && piece.PreviousCoordinate.x != position.x)
                {
                    lastMovedPawn.Take();
                    return;
                }
            }
            
            // NORMAL DIAGONAL TAKE
            // if the target move position contains a piece
            if (board.TryGetPieceFromSquare(position, out Piece occupyingPiece))
            {
                // double check it's not our colour - this should always be true as the square should never show this square as a legal move
                if (occupyingPiece.PieceColor != piece.PieceColor)
                {
                    // invoke the taken event on the opponents Piece
                    occupyingPiece.Take();
                }
            }
        }
    }
}