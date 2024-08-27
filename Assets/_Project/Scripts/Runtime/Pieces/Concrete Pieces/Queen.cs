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
            Board = board;
        }

        /// <summary>
        /// Returns the legal moves this piece has
        /// </summary>
        public override HashSet<Move> GetLegalMoves()
        {
            // The Queen can any number of squares in any direction
            HashSet<Vector2Int> directions = new()
            {
                new Vector2Int(1, 0), new Vector2Int(-1, 0), // Horizontal
                new Vector2Int(0, 1), new Vector2Int(0, -1), // Vertical
                new Vector2Int(1, 1), new Vector2Int(1, -1), // Diagonal
                new Vector2Int(-1, 1), new Vector2Int(-1, -1)
            };

            // Create a new LineMoveStrategy and get the possible moves
            MoveStrategy strategy = new LineMoveStrategy();
            HashSet<Move> possibleMoves = strategy.GetPossibleMoves(directions, this, Board);

            // get the board to filter illegal moves
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