using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChessGame
{
    public class King : Piece
    {
        public bool IsInCheck { get; set; }

        public bool IsCheckMate
        {
            get
            {
                bool noLegalKingMoves = GetLegalMoves().Count == 0;

                HashSet<Piece> pieces = Board.GetPieces(this);
                List<Move> totalLegalMoves = new List<Move>();

                foreach (var piece in pieces)
                {
                    HashSet<Move> pieceMoves = piece.GetLegalMoves();
                    if (pieceMoves != null)
                    {
                        totalLegalMoves.AddRange(pieceMoves);
                    }

                }

                bool noLegalPieceMoves = totalLegalMoves.Count == 0;

                return noLegalKingMoves && noLegalPieceMoves;

            }
        }

        public King(Vector2Int coordinate, PieceColor pieceColor, Board board) : base(PieceType.King, coordinate,
            pieceColor)
        {

        }

        public override HashSet<Move> GetLegalMoves()
        {
            HashSet<Vector2Int> kingMoves = new()
            {
                new(1, 0), new(1, 1), new(0, 1),
                new(-1, 1), new(-1, 0), new(-1, -1),
                new(0, -1), new(1, -1)
            };

            MoveStrategy strategy = new DirectMovementStrategy();
            HashSet<Move> possibleMoves = strategy.GetPossibleMoves(kingMoves, this, Board);

            HashSet<Move> safeMoves = new HashSet<Move>();

            foreach (var move in possibleMoves.Where(move => !MoveAttacked(move)))
            {
                safeMoves.Add(move);
            }

            return safeMoves;
        }

        private bool MoveAttacked(Move move)
        {
            Vector2Int targetCoordinate = move.Coordinate;

            Piece originalPiece;

            if (Board.TryGetPieceFromSquare(targetCoordinate, out originalPiece))
            {
                Board.RemovePiece(originalPiece);
            }

            bool isUnderAttack = IsKingInCheckAfterMove(move);

            if (originalPiece != null)
            {
                Board.AddPiece(originalPiece);
            }

            return isUnderAttack;
        }

        private bool IsKingInCheckAfterMove(Move move)
        {
            Vector2Int newKingCoordinate = move.Coordinate;

            var opponents = Board.GetOpponentPieces(this).Where(p => p is not King);
            foreach (var opponent in opponents)
            {
                if (opponent.GetLegalMoves().Any(m => m.Coordinate == newKingCoordinate))
                    return true;
            }

            return false;
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