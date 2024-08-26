using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    public abstract class MoveStrategy
    {
        public abstract HashSet<Move> GetPossibleMoves(HashSet<Vector2Int> movement, Piece piece, Board board);
    }
}