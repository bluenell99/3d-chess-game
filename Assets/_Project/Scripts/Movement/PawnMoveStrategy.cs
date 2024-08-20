using System.Collections.Generic;
using UnityEngine;

public class PawnMoveStrategy : MoveStrategy
{
    public override HashSet<Move> GetPossibleMoves(HashSet<Vector2Int> movement, Piece piece, Board board)
    {
        HashSet<Move> availableMoves = new HashSet<Move>();
        int direction = piece.PieceColor == PieceColor.White ? 1 : -1;
        Vector2Int forwardMove = new Vector2Int(piece.Coordinate.x, piece.Coordinate.y + direction);
        piece.HasCheckOnKing = false;
        
        // Forward move
        if (board.IsSquareOnBoard(forwardMove) && !board.IsSquareOccupied(forwardMove))
        {
            availableMoves.Add(new Move(forwardMove, board, piece, false));

            if (!piece.HasMoved)
            {
                Vector2Int doubleForwardMove = new Vector2Int(piece.Coordinate.x, piece.Coordinate.y + 2 * direction);
                if (!board.IsSquareOccupied(doubleForwardMove))
                {
                    availableMoves.Add(new Move(doubleForwardMove, board, piece, false));
                }
            }
        }

        // Diagonal moves
        Vector2Int[] diagonalMoves = {
            new(piece.Coordinate.x + 1, piece.Coordinate.y + direction),
            new(piece.Coordinate.x - 1, piece.Coordinate.y + direction)
        };

        foreach (var move in diagonalMoves)
        {
            if (!board.IsSquareOnBoard(move)) continue;

            if (board.TryGetPieceFromSquare(move, out var targetPiece))
            {
                if (targetPiece is King king)
                {
                    piece.HasCheckOnKing = true;
                    king.IsInCheck = true;
                    availableMoves.Add(new Move(move, board, piece, true));
                    continue;
                }

                if (targetPiece.PieceColor == piece.PieceColor)
                    continue;

                availableMoves.Add(new Move(move, board, piece, false));
            }
            else
            {
                AddEnPassantMove(availableMoves, piece, move, board, direction);
            }
        }

        return availableMoves;

    }

    private void AddEnPassantMove(HashSet<Move> availableMoves, Piece piece, Vector2Int target, Board board, int dir)
    {
        Piece lastPieceMoved = GameController.Instance.LastPieceMoved;

        if (lastPieceMoved is not Pawn)
            return;

        int rankDiff = Mathf.Abs(lastPieceMoved.PreviousCoordinate.y - lastPieceMoved.Coordinate.y);

        if (rankDiff == 2)
        {
            // check if pawns are next to share the same Y position, and are adjacent
            if (lastPieceMoved.Coordinate.y == piece.Coordinate.y && Mathf.Abs(lastPieceMoved.Coordinate.x - piece.Coordinate.x) == 1)
            {
                Vector2Int enpassantCaptureSquare = new Vector2Int(lastPieceMoved.Coordinate.x, piece.Coordinate.y + dir);
                if (target == enpassantCaptureSquare)
                {
                    availableMoves.Add(new Move(enpassantCaptureSquare, board, piece, false));
                }
            }
        }
    }
}