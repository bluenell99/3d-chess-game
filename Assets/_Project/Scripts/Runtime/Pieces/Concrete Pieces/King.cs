using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChessGame
{
    public class King : Piece
    {
        /// <summary>
        /// Is this King in Check
        /// </summary>
        public bool IsInCheck { get; set; }

        
        /// <summary>
        /// Is this King in Checkmate
        /// </summary>
        public bool IsCheckMate
        {
            get
            {
                // do we have no legal moves
                bool noLegalKingMoves = GetLegalMoves().Count == 0;

                // get all our pieces
                HashSet<Piece> pieces = Board.GetPieces(this);
                // collection for storing all pieces legal moves
                List<Move> totalLegalMoves = new List<Move>();

                // iterate through each peice
                foreach (var piece in pieces)
                {
                    // get their legal moves
                    HashSet<Move> pieceMoves = piece.GetLegalMoves();
                    // if moves have been found, we add them to the collection
                    if (pieceMoves != null)
                    {
                        totalLegalMoves.AddRange(pieceMoves);
                    }
                }

                // returns true if none of our pieces have legal moves
                bool noLegalPieceMoves = totalLegalMoves.Count == 0;

                // Checkmate occurs when the king cannot escape, and no pieces have any legal moves to protect it
                if (noLegalKingMoves && noLegalPieceMoves)
                {
                    // invoke the event
                    onKingInCheckMate?.Invoke(this);
                    return true;
                }

                return false;

            }
        }

        public event Action<King> onKingInCheckMate;

        public King(Vector2Int coordinate, PieceColor pieceColor, Board board) : base(PieceType.King, coordinate,
            pieceColor)
        {

            Board = board;
        }
        /// <summary>
        /// Returns the legal moves this piece has
        /// </summary>
        public override HashSet<Move> GetLegalMoves()
        {
            // The king can move to any adjacent square in all direction
            HashSet<Vector2Int> kingMoves = new()
            {
                new(1, 0), new(1, 1), new(0, 1),
                new(-1, 1), new(-1, 0), new(-1, -1),
                new(0, -1), new(1, -1)
            };

            // Create a new DirectMovementStrategy
            MoveStrategy strategy = new DirectMovementStrategy();
            HashSet<Move> possibleMoves = strategy.GetPossibleMoves(kingMoves, this, Board);

            // Create a new HashSet for safe moves
            HashSet<Move> safeMoves = new HashSet<Move>();

            // iterate all of our possible moves, and add moves that aren't attacked to the SafeMoves HashSet
            foreach (var move in possibleMoves.Where(move => !MoveAttacked(move)))
            {
                safeMoves.Add(move);
            }

            // return the SafeMoves that aren't attacked
            return safeMoves;
        }

        /// <summary>
        /// Evaluates if a potential move is attacked
        /// </summary>
        /// <param name="move">The move to test</param>
        /// <returns>True if the move is attacked</returns>
        /// <remarks>A King is not allowed to put himself into check, so we simulate a move and potential capture</remarks>
        private bool MoveAttacked(Move move)
        {
            // cache the move's coordinate
            Vector2Int targetCoordinate = move.Coordinate;

            // try and get a piece from this move, and temporarily remove it to simulate a capture
            Piece originalPiece;
            if (Board.TryGetPieceFromSquare(targetCoordinate, out originalPiece))
            {
                Board.RemovePiece(originalPiece);
            }

            // determine if the king would be under attack from this move
            bool isUnderAttack = IsKingInCheckAfterMove(move);

            // if there was a piece found from this move's coordinate, we add it back to the board
            if (originalPiece != null)
            {
                Board.AddPiece(originalPiece);
            }

            return isUnderAttack;
        }

        /// <summary>
        /// Checks if the King would be in check after a move
        /// </summary>
        /// <param name="move">The targetted move</param>
        /// <returns>True if this move would put the King in check</returns>
        private bool IsKingInCheckAfterMove(Move move)
        {
            // cache the new king's coordiante
            Vector2Int newKingCoordinate = move.Coordinate;

            // get all the opponents that aren't a King
            var opponents = Board.GetOpponentPieces(this).Where(p => p is not King);
            // iterate their moves, and check if any of them are the Kings new position
            foreach (var opponent in opponents)
            {
                // if so, the king would be under check here
                if (opponent.GetLegalMoves().Any(m => m.Coordinate == newKingCoordinate))
                    return true;
            }

            return false;
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