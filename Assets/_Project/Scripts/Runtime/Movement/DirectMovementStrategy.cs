using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    public class DirectMovementStrategy : MoveStrategy
    {

        /// <summary>
        /// Gets all possible moves for a given Piece
        /// </summary>
        /// <param name="movement">The directions to move in</param>
        /// <param name="piece">The Piece we want check</param>
        /// <param name="board">Reference to the board</param>
        /// <returns></returns>
        /// <remarks>Direct Move is a straight "teleport" to a square, and is used by the Knight and the King</remarks>
        public override HashSet<Move> GetPossibleMoves(HashSet<Vector2Int> movement, Piece piece, Board board)
        {
            // Initialise empty HashSet of Moves,
            HashSet<Move> availableMoves = new HashSet<Move>();
            // Intialise the Piece as not having check on the opponent's King
            piece.HasCheckOnKing = false;

            // for each move in given movement pattern
            foreach (var move in movement)
            {
                // get coordinate of move
                Vector2Int potentialMove = piece.Coordinate + move;

                // if the square isn't on the board, try the next move
                if (!board.IsSquareOnBoard(potentialMove)) continue;

                // if theres a piece in this coordinate
                if (board.TryGetPieceFromSquare(potentialMove, out var occupyingPiece))
                {
                    // if it's ours, we ignore and try the next move
                    if (occupyingPiece.PieceColor == piece.PieceColor)
                        continue;
                    
                    // if it's a king, we mark the move as delivering check, and flag both the king and our piece, and go to the next move
                    if (occupyingPiece is King king)
                    {
                        piece.HasCheckOnKing = true;
                        king.IsInCheck = true;
                        availableMoves.Add(new Move(move, true, MoveType.Check));
                        continue;
                    }
                    
                    // if there's a piece, but not the king, we mark the move as a capture
                    availableMoves.Add(new Move(potentialMove, false, MoveType.Capture));
                    continue;
                }
                
                // this is just a normal move
                availableMoves.Add(new Move(potentialMove, false, MoveType.Move));
            }

            return availableMoves;
        }
    }
}