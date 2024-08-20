using System.Collections.Generic;
using UnityEngine;

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
            
            
                if (board.TryGetPieceFromSquare(potentialMove, out var occupyingPiece))
                {
                    if (occupyingPiece is King king)
                    {
                        piece.HasCheckOnKing = true;
                        king.IsInCheck = true;
                        availableMoves.Add(new Move(move, board, piece, true));
                        continue;
                        
                    }

                    if (occupyingPiece.PieceColor == piece.PieceColor)
                        continue;
                }
            

            // add the moves
            availableMoves.Add(new Move(potentialMove, board, piece, false));
        }

        return availableMoves;
    }
}