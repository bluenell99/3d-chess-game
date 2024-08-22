using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChessGame
{
    public class Move
    {
        public Vector2Int Coordinate { get; private set; }

        public MoveType MoveType { get; private set; }
        public bool IsDeliveringCheck { get; set; }
        
        private readonly Board _board;
        private readonly Piece _piece;
        

        public Move(Vector2Int coordinate, bool isDeliveringCheck, MoveType type)
        {
            Coordinate = coordinate;
            IsDeliveringCheck = isDeliveringCheck;
        }
    }

    public enum MoveType
    {
        Move,
        Capture,
        Castle,
        EnPassant,
        Check
    }
}

