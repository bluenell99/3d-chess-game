using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    public class DirectMovementStrategy : MoveStrategy
    {

        public override HashSet<Move> GetPossibleMoves(HashSet<Vector2Int> movement, Piece piece, Board board)
        {
            HashSet<Move> availableMoves = new HashSet<Move>();
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