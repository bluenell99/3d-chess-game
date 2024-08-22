using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChessGame
{
    public class Bishop : Piece
    {
        public Bishop(Vector2Int coordinate, PieceColor pieceColor, Board board) : base(PieceType.Bishop, coordinate, pieceColor)
        {
        }

        public override HashSet<Move> GetLegalMoves()
        {
            // Bishop moves diagonally
            HashSet<Vector2Int> directions = new HashSet<Vector2Int>
            {
                new(1, 1), new(1, -1),
                new(-1, 1), new(-1, -1)
            };


            MoveStrategy strategy = new LineMoveStrategy();
            HashSet<Move> possibleMoves = strategy.GetPossibleMoves(directions, this, Board);

            return Board.EvaluateMoveLegality(possibleMoves, this);
        }


    }
}