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

        /// <summary>
        /// Returns the legal moves this piece has
        /// </summary>
        public override HashSet<Move> GetLegalMoves()
        {
            // Bishop moves diagonally
            HashSet<Vector2Int> directions = new HashSet<Vector2Int>
            {
                new(1, 1), new(1, -1),
                new(-1, 1), new(-1, -1)
            };

            // Create a new LineMoveStrategy
            MoveStrategy strategy = new LineMoveStrategy();
            // get the possible moves
            HashSet<Move> possibleMoves = strategy.GetPossibleMoves(directions, this, Board);

            // Get the board to filter illegal moves
            return Board.EvaluateMoveLegality(possibleMoves, this);
        }

        /// <summary>
        /// Updates the Piece's position on the Board
        /// </summary>
        /// <param name="position">The new position</param>
        /// <param name="isIntialSetup">Is this called from the Board intialisation</param>
        /// <param name="bypassTurnOrder">Does this bypass the turn order system</param>
        public override void SetPositionOnBoard(Vector2Int position, bool isIntialSetup, bool bypassTurnOrder)
        {
            // update the previous coordinate to the current coordinate
            PreviousCoordinate = Coordinate;
            
            // check if this is being placed as part of Board initialisation
            if (isIntialSetup)
            {
                // update our current position
                Coordinate = position;
                OnPositionChanged(position);
                return;
            }

            // Try and capture a piece on this position
            CaptureStrategy captureStrategy = new StandardCaptureStrategy();
            captureStrategy.TryCapture(Board, this, position);

            // finalise
            CompleteMove(position);
        }
    }
}