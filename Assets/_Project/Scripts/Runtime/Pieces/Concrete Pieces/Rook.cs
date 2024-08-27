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
            // The Rook can move Up, Down, Left, and Right
            HashSet<Vector2Int> directions = new()
            {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, 1), new Vector2Int(0, -1)
            };

            // Create a new LineMoveStrategy
            MoveStrategy strategy = new LineMoveStrategy();
            HashSet<Move> possibleMoves = strategy.GetPossibleMoves(directions, this, Board);


            // CASTLING
            // Get our King
            King king = Board.GetKing(this);

            // if this piece hasn't moved, and the King hasn't moved
            if (!king.HasMoved && !HasMoved)
            {
                // determine if we're Kingside or not
                bool isKingside = Coordinate.x > king.Coordinate.x;

                // determine if we're able to Castle
                if (CanCastle(isKingside, king))
                {
                    // add this as a possible move
                    possibleMoves.Add(new Move(king.Coordinate, false, MoveType.Castle));
                }
            }
            
            // get the board to filter illegal moves
            return Board.EvaluateMoveLegality(possibleMoves, this);

        }

        /// <summary>
        /// Determines if the Rook can castle
        /// </summary>
        /// <param name="isKingside">Are we Kingside</param>
        /// <param name="king">Our King</param>
        /// <returns>True if the required castling squares are unoccupied</returns>
        private bool CanCastle(bool isKingside, King king)
        {
            // get the castling squares from the Board
            var castlingSquares = Board.GetCastlingSquares(king, isKingside);

            // return if all the castling squares are unoccupied
            return castlingSquares.All(sq => !Board.IsSquareOccupied(sq));
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
            CaptureStrategy strategy = new RookCaptureStrategy();
            strategy.TryCapture(Board, this, position);

            // finalise
            CompleteMove(position);
        }



    }
}
