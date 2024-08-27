using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    

    public class Pawn : Piece
    {
        public Pawn(Vector2Int coordinate, PieceColor pieceColor, Board board) : base(PieceType.Pawn, coordinate, pieceColor)
        {
            Board = board;
        }

        /// <summary>
        /// Invoked when this Pawn can be promoted
        /// </summary>
        public event Action<Pawn> onPawnPromotionAvailable;


        /// <summary>
        /// Triggers the Promotion event
        /// </summary>
        /// <param name="pawn"></param>
        public void Promote(Pawn pawn)
        {
            onPawnPromotionAvailable?.Invoke(pawn);
        }
        
        
        /// <summary>
        /// Returns the legal moves this piece has
        /// </summary>
        public override HashSet<Move> GetLegalMoves()
        {
            // Determine direction this Pawn moves in
            int direction = PieceColor == PieceColor.White ? 1 : 0;

            // We need to pass a movement HashSet into GetPossibleMoves, so just create one for it's normal move
            HashSet<Vector2Int> movement = new HashSet<Vector2Int>()
            {
                new(0, direction)
            };

            // Create a new PawnMovementStrategy and get it's moves
            MoveStrategy strategy = new PawnMoveStrategy();
            HashSet<Move> possibleMoves = strategy.GetPossibleMoves(movement, this, Board);

            // Get the board to filter illegal moves
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
            
            // Try and capture a Piece on this position
            CaptureStrategy captureStrategy = new PawnCaptureStrategy();
            captureStrategy.TryCapture(Board, this, position);

            // finalise
            CompleteMove(position);
        }
    }
}