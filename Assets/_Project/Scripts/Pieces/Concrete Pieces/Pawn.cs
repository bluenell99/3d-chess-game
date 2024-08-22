using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    

    public class Pawn : Piece
    {
        public Pawn(Vector2Int coordinate, PieceColor pieceColor, Board board) : base(PieceType.Pawn, coordinate, pieceColor)
        {
        }

        public event Action<Pawn> onPawnPromotionAvailable;

        public override HashSet<Move> GetLegalMoves()
        {
            int direction = PieceColor == PieceColor.White ? 1 : 0;

            HashSet<Vector2Int> movement = new HashSet<Vector2Int>()
            {
                new(0, direction)
            };

            MoveStrategy strategy = new PawnMoveStrategy();
            HashSet<Move> possibleMoves = strategy.GetPossibleMoves(movement, this, Board);

            return Board.EvaluateMoveLegality(possibleMoves, this);
        }

        public override void SetPositionOnBoard(Vector2Int position, bool isIntialSetup, bool bypassTurnOrder)
        {
            base.SetPositionOnBoard(position, isIntialSetup, bypassTurnOrder);


            int verticalEdge = PieceColor == PieceColor.White ? 7 : 0;

            if (Coordinate.y == verticalEdge)
            {
                Debug.Log("Pawn at edge, ready for promotion");
                onPawnPromotionAvailable?.Invoke(this);
            }
        }
    }
}