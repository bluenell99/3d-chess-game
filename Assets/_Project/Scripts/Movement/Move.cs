using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Move
{
    public Vector2Int Coordinate { get; private set; }

    private readonly Board _board;
    private readonly Piece _piece;

    public Move(Vector2Int coordinate, Board board, Piece piece, bool isDeliveringCheck)
    {
        Coordinate = coordinate;
        _board = board;
        _piece = piece;
        IsDeliveringCheck = isDeliveringCheck;
    }

    public bool IsDeliveringCheck { get; private set; }
    
    
    
    
}

