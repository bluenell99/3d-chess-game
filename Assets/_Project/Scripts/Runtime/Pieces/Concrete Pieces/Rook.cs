using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChessGame
{
    public class Rook : Piece
    {
        public Rook(Vector2Int coordinate, PieceColor pieceColor, Board board) : base(PieceType.Rook, coordinate, pieceColor)
        {
            Board = board;
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
        /// <param name="isInitialSetup">Is this called from the Board initialisation</param>
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
            
            // try and get our King from the target position
            if (Board.TryGetPieceFromSquare(position, out Piece piece))
            {
                if (piece is King && piece.PieceColor == PieceColor)
                {
                    // if we succeed, this is a castle
                    position = Castle(piece, position);
                }
            }
            else
            {
                CaptureStrategy strategy = new RookCaptureStrategy();
                strategy.TryCapture(Board, this, position);
            }

            // finalise
            CompleteMove(position);
        }


        private Vector2Int Castle(Piece king, Vector2Int targetPosition)
        {
            Vector2Int newRookPosition;
            bool isKingside = Coordinate.x > king.Coordinate.x;

            if (isKingside)
            {
                // setup move for rook (+1 as move technically is the kings position, so we need to go right one square of where the king would be
                newRookPosition = new Vector2Int(targetPosition.x + 1, targetPosition.y);
                // trigger move for king (+2 takes the king right two squares
                Vector2Int kingPosition = new Vector2Int(king.Coordinate.x + 2, king.Coordinate.y);
                king.SetPositionOnBoard(kingPosition, false, true); // we bypass the turn order to ensure turn doesn't change after the king is moved

            }
            else
            {
                // setup move for rook (-1 as move technically is the kings position, so we need to go left one square of where the king would be
                newRookPosition = new Vector2Int(targetPosition.x - 1, targetPosition.y);

                // trigger move for king (-2 takes the king left two squares
                Vector2Int kingPosition = new Vector2Int(king.Coordinate.x - 2, king.Coordinate.y);
                king.SetPositionOnBoard(kingPosition, false, true);

            }

            return newRookPosition;
        }
    }
}
