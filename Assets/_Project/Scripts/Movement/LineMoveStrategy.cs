using System.Collections.Generic;
using UnityEngine;

public class LineMoveStrategy : MoveStrategy
{

    
    /// <summary>
    /// Traverses the board in each given direction until it reaches the edge of the board, is blocked by a piece, or can capture the piece
    /// </summary>
    /// <param name="movement">The directions it can move in</param>
    /// <param name="piece">The piece</param>
    /// <param name="board">Reference to the board/param>
    /// <returns></returns>
    public override HashSet<Move> GetPossibleMoves(HashSet<Vector2Int> movement, Piece piece, Board board)
    {
        var availableMoves = new HashSet<Move>();
        piece.HasCheckOnKing = false;
        
        // for every movement in the pieces movements
        foreach (var direction in movement)
        {
            Vector2Int current = piece.Coordinate + direction;
            
            while (board.IsSquareOnBoard(current))
            {
                if (board.TryGetPieceFromSquare(current, out var occupyingPiece))
                {
                    if (occupyingPiece.PieceColor == piece.PieceColor)
                        break;

                    if (occupyingPiece is King king)
                    {
                        piece.HasCheckOnKing = true;
                        king.IsInCheck = true;
                        availableMoves.Add(new Move(current, board, piece, true));
                        break;
                    }

                    availableMoves.Add(new Move(current, board, piece, false));
                    break;
                }
                
                availableMoves.Add(new Move(current, board, piece, false));
                current += direction;
            }
        }

        return availableMoves;
    }
}