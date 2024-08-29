using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using NUnit.Framework;
using UnityEngine;

namespace ChessGame.Tests
{
    public class RookTests
    {
        [Test]
        public void Rook_GetLegalMoves_ExpectedMovesWhenNoBlockingOrCapturingPieces()
        {
            Board board = new Board(8);
            Vector2Int startingPosition = new Vector2Int(3, 2);
            Vector2Int movePosition = new Vector2Int(3, 3);
            HashSet<Vector2Int> expectedLegalMoves = new HashSet<Vector2Int>
            {
                new(0, 3),
                new(1, 3),
                new(2, 3),
                new(4, 3),
                new(5, 3),
                new(6, 3),
                new(7, 3),
                new(3, 0),
                new(3, 1),
                new(3, 2),
                new(3, 4),
                new(3, 5),
                new(3, 6),
                new(3, 7),
            };

            Piece piece = new Rook(startingPosition, PieceColor.White, board);
            board.AddPiece(piece);
            AddKings(board);

            piece.SetPositionOnBoard(movePosition, false, false);
            
            HashSet<Move> legalMoves = piece.GetLegalMoves();

            Assert.IsNotEmpty(legalMoves);
            Assert.IsNotNull(legalMoves);
            
            Assert.IsTrue(expectedLegalMoves.Count == legalMoves.Count);
            Assert.That(legalMoves.Any(move=>move.Coordinate != movePosition));

            foreach (var move in legalMoves)
            {
                Assert.IsTrue(expectedLegalMoves.Contains(move.Coordinate));
            }
        }
        
        [Test]
        public void Rook_GetLegalMoves_ExpectedMovesWhenBlockingPiecesExist()
        {
            Board board = new Board(8);
            Vector2Int startingPosition = new Vector2Int(3, 2);
            Vector2Int movePosition = new Vector2Int(3, 3);
            
            Piece rook = new Rook(startingPosition, PieceColor.White, board);
            Piece blackPiece = new Pawn(new Vector2Int(3, 5), PieceColor.Black, board);
            Piece whitePiece = new Pawn(new Vector2Int(5, 3), PieceColor.White, board);

            
            board.AddPiece(rook);
            board.AddPiece(blackPiece);
            board.AddPiece(whitePiece);
            AddKings(board);


            // need to move the rook otherwise it still shows the castling move as legal
            rook.SetPositionOnBoard(movePosition, false, false);
            
            HashSet<Vector2Int> expectedLegalMoves = new HashSet<Vector2Int>
            {
                blackPiece.Coordinate,
                new(0,3),
                new(1,3),
                new(2,3),
                new(3,4),
                new(4,3),
                new(3,2),
                new(3,1),
                new(3,0)
            };

            HashSet<Move> legalMoves = rook.GetLegalMoves();

            Assert.IsNotEmpty(legalMoves);
            Assert.IsNotNull(legalMoves);
            Assert.IsTrue(expectedLegalMoves.Count == legalMoves.Count);
            Assert.That(legalMoves.Any(move=>move.Coordinate != movePosition));

            foreach (var move in legalMoves)
            {
                Assert.IsTrue(expectedLegalMoves.Contains(move.Coordinate));
            }
        }

        [Test]  
        public void Rook_GetLegalMoves_CastlingOpportunityWhenExpected()
        {
            Board board = new Board(8);

            Vector2Int startingCoordinate = new Vector2Int(7, 0);
            Rook rook = new Rook(startingCoordinate, PieceColor.White, board);
            King whiteKing = new King(new Vector2Int(4, 0), PieceColor.White, board);
            King blackKing = new King(new Vector2Int(4, 7), PieceColor.Black, board);

            board.AddPiece(rook);
            board.AddPiece(whiteKing);
            board.AddPiece(blackKing);

            HashSet<Move> legalMoves = rook.GetLegalMoves();
            
            Assert.That(legalMoves.Any(m => m.Coordinate == whiteKing.Coordinate));
        }

        [Test]
        public void Rook_SetPositionOnBoard_Castling()
        {
            Board board = new Board(8);

            Vector2Int startingCoordinate = new Vector2Int(7, 0);
            Rook rook = new Rook(startingCoordinate, PieceColor.White, board);
            King whiteKing = new King(new Vector2Int(4, 0), PieceColor.White, board);
            King blackKing = new King(new Vector2Int(4, 7), PieceColor.Black, board);

            Vector2Int expectedRookCoordinate = new Vector2Int(5, 0);
            Vector2Int expectedKingCoordinate = new Vector2Int(6, 0);
            
            board.AddPiece(rook);
            board.AddPiece(whiteKing);
            board.AddPiece(blackKing);
            
            HashSet<Move> legalMoves = rook.GetLegalMoves();
            
            Assert.That(legalMoves.Any(move => move.Coordinate == whiteKing.Coordinate));

            rook.SetPositionOnBoard(whiteKing.Coordinate, false, false);
            
            Assert.IsTrue(rook.Coordinate==expectedRookCoordinate);
            Assert.IsTrue(whiteKing.Coordinate==expectedKingCoordinate);

        }
        
        #region HELPERS
        private void AddKings(Board board)
        {
            King whiteKing = new King(new Vector2Int(4, 0), PieceColor.White, board);
            King blackKing = new King(new Vector2Int(4, 7), PieceColor.Black, board);
           
            board.AddPiece(whiteKing);
            
            board.AddPiece(blackKing);
        }
        #endregion
    }
}