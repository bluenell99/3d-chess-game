using System.Collections.Generic;
using UnityEngine;

public abstract class MoveStrategy
{
    public abstract HashSet<Move> GetPossibleMoves(HashSet<Vector2Int> movement, Piece piece, Board board);
}