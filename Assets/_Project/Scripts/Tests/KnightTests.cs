using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace ChessGame.Tests
{
    public class KnightTests
    {
        #region TESTS

        [Test]
        public void Knight_GetLegalMoves_ExpectedMovesWhenNoBlockingOrCapturingPieces()
        {
            Board board = new Board(8);
            Vector2Int startingPosition = new Vector2Int(3, 4);
            Knight knight = new Knight(startingPosition, PieceColor.White, board);

            HashSet<Vector2Int> expectedLegalMoves = new HashSet<Vector2Int>()
            {
                new(4, 6),
                new(5, 5),
                new(5, 3),
                new(4, 2),
                new(2, 2),
                new(1, 3),
                new(1, 5),
                new(2, 6)
            };
            
            board.AddPiece(knight);
            AddKings(board);

            HashSet<Move> legalMoves = knight.GetLegalMoves();
            
            Assert.IsNotNull(legalMoves);
            Assert.IsNotEmpty(legalMoves);
            Assert.That(legalMoves.Any(move => move.Coordinate != startingPosition));
            Assert.That(expectedLegalMoves.Count == legalMoves.Count);

            foreach (var move in legalMoves)
            {
                Assert.IsTrue(expectedLegalMoves.Contains(move.Coordinate));
            }
        }

        [Test]
        public void Knight_GetLegalMoves_ExpectedMovesWhenBlockingPiecesExist()
        {
            Board board = new Board(8);
            Vector2Int startingPosition = new Vector2Int(3, 4);
            Knight knight = new Knight(startingPosition, PieceColor.White, board);
            Pawn whitePawn = new Pawn(new Vector2Int(2, 6), PieceColor.White, board);
            Pawn blackPawn = new Pawn(new Vector2Int(5, 5), PieceColor.Black, board);

            HashSet<Vector2Int> expectedLegalMoves = new HashSet<Vector2Int>()
            {
                new(4,6),
                new(5,3),
                new(4,2),
                new(2,2),
                new(1,3),
                new(1,5),
                blackPawn.Coordinate
            };
            
            board.AddPiece(knight);
            board.AddPiece(whitePawn);
            board.AddPiece(blackPawn);
            AddKings(board);

            HashSet<Move> legalMoves = knight.GetLegalMoves();
            
            Assert.IsNotNull(legalMoves);
            Assert.IsNotEmpty(legalMoves);
            Assert.That(legalMoves.Any(move => move.Coordinate != startingPosition));
            Assert.That(expectedLegalMoves.Count == legalMoves.Count);

            foreach (var move in legalMoves)
            {
                Assert.IsTrue(expectedLegalMoves.Contains(move.Coordinate));
            }
        }
        
        #endregion

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