using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    

    public class PawnMoveStrategy : MoveStrategy
    {
        
        /// <summary>
        /// Gets all possible moves for a given Piece
        /// </summary>
        /// <param name="movement">The directions to move in</param>
        /// <param name="piece">The Piece we want check</param>
        /// <param name="board">Reference to the board</param>
        /// <returns></returns>
        /// <remarks>Pawns have different ways to move: They can perform a normal single square move in their direction,
        /// a double move if they've not moved yet, and a diagonal capture</remarks>
        public override HashSet<Move> GetPossibleMoves(HashSet<Vector2Int> movement, Piece piece, Board board)
        {
            // Initialise an empty HashSet of Moves
            HashSet<Move> availableMoves = new HashSet<Move>();
            // Get it's movement direction
            int direction = piece.PieceColor == PieceColor.White ? 1 : -1;
            // Get it's forward move
            Vector2Int forwardMove = new Vector2Int(piece.Coordinate.x, piece.Coordinate.y + direction);
            piece.HasCheckOnKing = false;

            // FORWARD MOVE
            // check if the square is on the board, and it's not occupied
            if (board.IsSquareOnBoard(forwardMove) && !board.IsSquareOccupied(forwardMove))
            {
                // add the move 
                availableMoves.Add(new Move(forwardMove,false, MoveType.Move));

                // if we've yet to move
                if (!piece.HasMoved)
                {
                    // get the double forward coordinate
                    Vector2Int doubleForwardMove = new Vector2Int(piece.Coordinate.x, piece.Coordinate.y + 2 * direction);
                    // check if that's on the board
                    if (!board.IsSquareOccupied(doubleForwardMove))
                    {
                        // add the move
                        availableMoves.Add(new Move(doubleForwardMove, false, MoveType.Move));
                    }
                }
            }

            // DIAGONAL MOVES
            // Create a temporary array of coordinates for the diagonal squares
            Vector2Int[] diagonalMoves =
            {
                new(piece.Coordinate.x + 1, piece.Coordinate.y + direction),
                new(piece.Coordinate.x - 1, piece.Coordinate.y + direction)
            };

            // iterate through these moves
            foreach (var move in diagonalMoves)
            {
                // if it's not on the board, try the next move
                if (!board.IsSquareOnBoard(move)) continue;

                // try and get a Piece from the square
                if (board.TryGetPieceFromSquare(move, out var targetPiece))
                {
                    // if it's a King
                    if (targetPiece is King king)
                    {
                        // mark the flags as providing check, and move to the next move
                        piece.HasCheckOnKing = true;
                        king.IsInCheck = true;
                        availableMoves.Add(new Move(move,true, MoveType.Check));
                        continue;
                    }

                    // if the piece is our colour, we ignore and try the next move
                    if (targetPiece.PieceColor == piece.PieceColor)
                        continue;

                    // add the move
                    availableMoves.Add(new Move(move, false, MoveType.Capture));
                }
                else
                {
                    AddEnPassantMove(availableMoves, piece, move, board, direction);
                }
            }

            return availableMoves;

        }

        /// <summary>
        /// Adds the En-Passant move to the collection of available moves
        /// </summary>
        /// <param name="availableMoves"></param>
        /// <param name="piece"></param>
        /// <param name="target"></param>
        /// <param name="board"></param>
        /// <param name="dir"></param>
        private void AddEnPassantMove(HashSet<Move> availableMoves, Piece piece, Vector2Int target, Board board, int dir)
        {
            // get the last piece that moved
            Piece lastPieceMoved = board.LastPieceMoved;

            // if it's not a pawn, we don't care and exit early
            if (lastPieceMoved is not Pawn)
                return;

            // calculate if the pawn performed a double move by calculating the difference in Rank from it's previous coordinate
            int rankDiff = Mathf.Abs(lastPieceMoved.PreviousCoordinate.y - lastPieceMoved.Coordinate.y);

            // if the rank difference isn't 2, it didn't performed the double move, and we can't add this as a potential move
            if (rankDiff != 2) return;
            
            // We check that the Pawn moved next to our pawn
            if (lastPieceMoved.Coordinate.y != piece.Coordinate.y || Mathf.Abs(lastPieceMoved.Coordinate.x - piece.Coordinate.x) != 1) return;
            
            // get the square behind the last moved pawn
            Vector2Int enpassantCaptureSquare = new Vector2Int(lastPieceMoved.Coordinate.x, piece.Coordinate.y + dir);
            // check that our move is this sqyare 
            if (target == enpassantCaptureSquare)
            {
                // add as an available move
                availableMoves.Add(new Move(enpassantCaptureSquare, false, MoveType.EnPassant));
            }
        }
    }
}