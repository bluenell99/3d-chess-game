using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChessGame
{
    public class Move
    {
        public Vector2Int Coordinate { get; private set; }

        public MoveType MoveType { get; private set; }
        public bool IsDeliveringCheck { get; set; }
        
        
        public Move(Vector2Int coordinate, bool isDeliveringCheck, MoveType type)
        {
            Coordinate = coordinate;
            IsDeliveringCheck = isDeliveringCheck;
        }

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

