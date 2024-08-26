using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace ChessGame
{
    public class Queen : Piece
    {
        public Queen(Vector2Int coordinate, PieceColor pieceColor, Board board) : base(PieceType.Queen, coordinate, pieceColor)
        {
        }

        public override HashSet<Move> GetLegalMoves()
        {
            HashSet<Vector2Int> directions = new()
            {
                new Vector2Int(1, 0), new Vector2Int(-1, 0), // Horizontal
                new Vector2Int(0, 1), new Vector2Int(0, -1), // Vertical
                new Vector2Int(1, 1), new Vector2Int(1, -1), // Diagonal
                new Vector2Int(-1, 1), new Vector2Int(-1, -1)
            };

            MoveStrategy strategy = new LineMoveStrategy();
            HashSet<Move> possibleMoves = strategy.GetPossibleMoves(directions, this, Board);

            return Board.EvaluateMoveLegality(possibleMoves, this);

        }
        
        public override void SetPositionOnBoard(Vector2Int position, bool isIntialSetup, bool bypassTurnOrder)
        {
            
            PreviousCoordinate = Coordinate;
            
            if (isIntialSetup)
            {
                Coordinate = position;
                OnPositionChanged(position);
                return;
            }
            
            CaptureStrategy captureStrategy = new StandardCaptureStrategy();
            captureStrategy.TryCapture(Board, this, position);

            CompleteMove(this, position);
        }
    }
}