using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChessGame
{
    public class Rook : Piece
    {
        public Rook(Vector2Int coordinate, PieceColor pieceColor, Board board) : base(PieceType.Rook, coordinate, pieceColor)
        {
        }

        public override HashSet<Move> GetLegalMoves()
        {
            HashSet<Vector2Int> directions = new()
            {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, 1), new Vector2Int(0, -1)
            };

            MoveStrategy strategy = new LineMoveStrategy();
            HashSet<Move> possibleMoves = strategy.GetPossibleMoves(directions, this, Board);


            King king = Board.GetKing(this);

            // if this piece hasn't moved, and the knight hasn't moved
            if (!king.HasMoved && !HasMoved)
            {

                bool isKingside = Coordinate.x > king.Coordinate.x;

                if (CanCastle(isKingside, king))
                {
                    possibleMoves.Add(new Move(king.Coordinate, false, MoveType.Check));
                }

            }


            return Board.EvaluateMoveLegality(possibleMoves, this);

        }

        private bool CanCastle(bool isKingside, King king)
        {
            var castlingSquares = Board.GetCastlingSquares(king, isKingside);

            return castlingSquares.All(sq => !Board.IsSquareOccupied(sq));
        }




    }
}
