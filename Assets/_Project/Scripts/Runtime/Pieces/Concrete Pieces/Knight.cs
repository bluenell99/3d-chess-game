using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    public class Knight : Piece
    {
        public Knight(Vector2Int coordinate, PieceColor pieceColor, Board board) : base(PieceType.Knight, coordinate,
            pieceColor)
        {
            Board = board;
        }

        /// <summary>
        /// Returns the legal moves this piece has
        /// </summary>
        public override HashSet<Move> GetLegalMoves()
        {
            // The Knight can move in an L-Shape
            HashSet<Vector2Int> knightMoves = new()
            {
                new(2, 1), new(2, -1),
                new(-2, 1), new(-2, -1),
                new(1, 2), new(1, -2),
                new(-1, 2), new(-1, -2),
            };

            // Create a new DirectMovementStrategy
            MoveStrategy strategy = new DirectMovementStrategy();
            HashSet<Move> possibleMoves = strategy.GetPossibleMoves(knightMoves, this, Board);

            // Ensure none of the moves are illegal
            return Board.EvaluateMoveLegality(possibleMoves, this);
        }
        
        
        /// <summary>
        /// Updates the Piece's position on the Board
        /// </summary>
        /// <param name="position">The new position</param>
        /// <param name="isInitialSetup">Is this called from the Board intialisation</param>
        /// <param name="bypassTurnOrder">Does this bypass the turn order system</param>
        public override void SetPositionOnBoard(Vector2Int position, bool isInitialSetup, bool bypassTurnOrder)
        {
            // update the previous coordinate to the current coordinate
            PreviousCoordinate = Coordinate;
            
            // check if this is being placed as part of Board initialisation
            if (isInitialSetup)
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