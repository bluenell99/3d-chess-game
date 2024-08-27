using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    public class LineMoveStrategy : MoveStrategy
    {


        /// <summary>
        /// Gets all possible moves for a given Piece
        /// </summary>
        /// <param name="movement">The directions to move in</param>
        /// <param name="piece">The Piece we want check</param>
        /// <param name="board">Reference to the board</param>
        /// <returns></returns>
        /// <remarks>Line Movement traverses the whole board in the given directions until it's blocked, can capture,
        /// or the target square is no longer on the board</remarks>
        public override HashSet<Move> GetPossibleMoves(HashSet<Vector2Int> movement, Piece piece, Board board)
        {
            // Initialise an empty HashSet of Moves, and mark the Piece as not having check
            var availableMoves = new HashSet<Move>();
            piece.HasCheckOnKing = false;

            // for each direction this Piece can go in
            foreach (var direction in movement)
            {
                // cache the current square
                Vector2Int current = piece.Coordinate + direction;
                
                // create a temporary list of moves in this direction
                var movesInDirection = new List<Move>();
                // create a temporary flag if this direction is delivering check
                bool isDeliveringCheck = false;

                // while the current square is on the board
                while (board.IsSquareOnBoard(current))
                {
                    // if we get a piece from our square
                    if (board.TryGetPieceFromSquare(current, out var occupyingPiece))
                    {
                        // ... and it's our colour, we can't take this piece and break out of the loop for this direction
                        if (occupyingPiece.PieceColor == piece.PieceColor)
                            break;

                        // ... if it's a King, we're providing check against it
                        if (occupyingPiece is King king)
                        {
                            // mark the flags for our piece, their king, and our temporary flag
                            piece.HasCheckOnKing = true;
                            king.IsInCheck = true;
                            isDeliveringCheck = true;

                            // retroactivley mark all previous moves as delivering check too, as all moves in this direction will be also delivering check
                            foreach (var move in movesInDirection)
                            {
                                move.IsDeliveringCheck = true;
                            }
                        }

                        // add these moves to the temporary list, and go to the next direction
                        movesInDirection.Add(new Move(current, isDeliveringCheck, isDeliveringCheck? MoveType.Check : MoveType.Move));
                        break;
                    }
                    
                    // We've not found a piece so add the move in this direction to the temporary list, and try the next square in the direction
                    movesInDirection.Add(new Move(current, false, MoveType.Move));
                    current += direction;
                }

                // We've reached the end of the board in this direction, so add all the moves in this direction to the AvailableMoves HashSet
                foreach (var move in movesInDirection)
                {
                    availableMoves.Add(move);
                }
            }

            return availableMoves;
        }
    }
}