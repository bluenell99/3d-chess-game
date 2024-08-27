using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChessGame
{
    public class Move
    {
        
        /// <summary>
        /// The coordinate of the move
        /// </summary>
        public Vector2Int Coordinate { get; }

        // TODO implement logic to read this value 
        /// <summary>
        /// The type of move
        /// </summary>
        public MoveType MoveType { get; private set; } 
        
        /// <summary>
        /// Is this move delivering check
        /// </summary>
        public bool IsDeliveringCheck { get; set; }
        
        /// <summary>
        /// Create a new Move
        /// </summary>
        /// <param name="coordinate">The coordinate of this move</param>
        /// <param name="isDeliveringCheck">Is the Move delivering check</param>
        /// <param name="type">The type of Move</param>
        
        public Move(Vector2Int coordinate, bool isDeliveringCheck, MoveType type)
        {
            Coordinate = coordinate;
            IsDeliveringCheck = isDeliveringCheck;
        }

        // TODO Implement this fully to stop pieces from moving to squares that would reveal a check. This scenario is considered an illegal move
        public bool RevealsCheck(Piece piece, Board board)
        {
            if (board.IsEvaluatingCheck)
            {
                return false;
            }

            board.IsEvaluatingCheck = true;
            King king = board.GetKing(piece);

            try
            {
                // temporarily remove our piece from play to simulate the move
                board.RemovePiece(piece);

                // get opponent moves
                HashSet<Piece> opponentPieces = board.GetOpponentPieces(piece).Where(p => p is not King).ToHashSet();
                HashSet<Move> opponentMoves = new HashSet<Move>();

                foreach (var move in opponentPieces.SelectMany(opponentPiece => opponentPiece.GetLegalMoves()))
                {
                    opponentMoves.Add(move);
                }

                board.AddPiece(piece);
                return opponentMoves.Any(m => m.Coordinate == king.Coordinate);
            }
            finally
            {
                board.IsEvaluatingCheck = false;
                king.IsInCheck = false;
                
            }
            
        }
    }

    public enum MoveType
    {
        Move,
        Capture,
        Castle,
        EnPassant,
        Check
    }
}

