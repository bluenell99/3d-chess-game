using UnityEngine;

namespace ChessGame
{
    public class RookCaptureStrategy : CaptureStrategy
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
                else // this should only be called when castling opportunity
                {
                    if (occupyingPiece is King)
                    {
                        bool isKingside = piece.Coordinate.x > occupyingPiece.Coordinate.x;

                        if (isKingside)
                        {
                            // setup move for rook (+1 as move technically is the kings position, so we need to go right one square of where the king would be
                            position = new Vector2Int(position.x + 1, position.y);

                            // trigger move for king (+2 takes the king right two squares
                            Vector2Int kingPosition = new Vector2Int(occupyingPiece.Coordinate.x + 2, occupyingPiece.Coordinate.y);
                            occupyingPiece.SetPositionOnBoard(kingPosition, false, true); // we bypass the turn order to ensure turn doesn't change after the king is moved

                        }
                        else
                        {
                            // setup move for rook (-1 as move technically is the kings position, so we need to go left one square of where the king would be
                            position = new Vector2Int(position.x - 1, position.y);

                            // trigger move for king (-2 takes the king left two squares
                            Vector2Int kingPosition = new Vector2Int(occupyingPiece.Coordinate.x - 2, occupyingPiece.Coordinate.y);
                            occupyingPiece.SetPositionOnBoard(kingPosition, false, true);
                        }
                    }
                }
            }
        }
    }
}