﻿using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public Knight(Vector2Int coordinate, PieceColor pieceColor, Board board) : base(PieceType.Knight, coordinate, pieceColor)
    {
    }

    public override HashSet<Move> GetLegalMoves()
    {
        HashSet<Vector2Int> knightMoves = new()
        {
            new(2, 1), new(2, -1),
            new(-2, 1), new(-2, -1),
            new(1, 2), new(1, -2),
            new(-1, 2), new(-1, -2),
        };

        MoveStrategy strategy = new DirectMovementStrategy();

        HashSet<Move> possibleMoves = strategy.GetPossibleMoves(knightMoves, this, Board);


        

        return Board.EvaluateMoveLegality(possibleMoves, this);
    }

    
}