using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace ChessGame.Tests
{
    public class KingTests
    {
        #region TESTS


        [Test]
        public void King_GetLegalMoves_ExpectedMovesWhenNoBlockingOrCapturingPieces()
        {
            Board board = new Board(8);
            Vector2Int startingPosition = new Vector2Int(4, 1);

            King king = new King(startingPosition, PieceColor.White, board);
            King blackKing = new King(new Vector2Int(4, 7), PieceColor.Black, board);
            
            HashSet<Vector2Int> expectedLegalMoves = new HashSet<Vector2Int>()
            {
                new(3, 2),
                new(4, 2),
                new(5, 2),
                new(5, 1),
                new(5, 0),
                new(4, 0),
                new(3, 0),
                new(3, 1),
            };

            board.AddPiece(king);
            board.AddPiece(blackKing);
            
            HashSet<Move> legalMoves = king.GetLegalMoves();
            
            Assert.IsNotNull(legalMoves);
            Assert.IsNotEmpty(legalMoves);
            Assert.That(legalMoves.Any(move => move.Coordinate != startingPosition));
            Assert.IsTrue(expectedLegalMoves.Count == legalMoves.Count);

            foreach (var move in legalMoves)
            {
                Assert.IsTrue(expectedLegalMoves.Contains(move.Coordinate));
            }
        }
        
        [Test]
        public void King_GetLegalMoves_ExpectedMovesWhenBlockingPiecesExist()
        {
            Board board = new Board(8);
            Vector2Int startingPosition = new Vector2Int(3, 2);

            King king = new King(startingPosition, PieceColor.White, board);
            King blackKing = new King(new Vector2Int(4, 7), PieceColor.Black, board);

            Pawn whitePawn = new Pawn(new Vector2Int(4,3), PieceColor.White, board);
            Pawn blackPawn = new Pawn(new Vector2Int(3,3), PieceColor.Black, board);
            
            HashSet<Vector2Int> expectedLegalMoves = new HashSet<Vector2Int>()
            {
                blackPawn.Coordinate,
                new (4,2),
                new(4,1),
                new(3,1),
                new(2,1),
                new(2,2),
                new(2,3)
            };

            board.AddPiece(king);
            board.AddPiece(blackKing);
            board.AddPiece(whitePawn);
            board.AddPiece(blackPawn);
            
            HashSet<Move> legalMoves = king.GetLegalMoves();
            
            Assert.IsNotNull(legalMoves);
            Assert.IsNotEmpty(legalMoves);
            Assert.That(legalMoves.Any(move => move.Coordinate != startingPosition));
            Assert.IsTrue(expectedLegalMoves.Count == legalMoves.Count);

            foreach (var move in legalMoves)
            {
                Assert.IsTrue(expectedLegalMoves.Contains(move.Coordinate));
            }
        }

        [Test]
        public void King_GetLegalMoves_KingCannotPutItselfIntoCheck()
        {
            Board board = new Board(8);
            Vector2Int startingPosition = new Vector2Int(3, 1);

            King whiteKing = new King(startingPosition, PieceColor.White, board);
            King blackKing = new King(new Vector2Int(4, 7), PieceColor.Black, board);
            Queen blackQueen = new Queen(new Vector2Int(2, 7), PieceColor.Black, board);
            Rook blackRook = new Rook(new Vector2Int(7, 2), PieceColor.Black, board);
            
            board.AddPiece(whiteKing);
            board.AddPiece(blackKing);
            board.AddPiece(blackQueen);
            board.AddPiece(blackRook);

            HashSet<Vector2Int> expectedLegalMoves = new HashSet<Vector2Int>
            {
                new(4, 1),
                new(4, 0),
                new(3, 0)
            };

            HashSet<Move> legalMoves = whiteKing.GetLegalMoves();

            Assert.IsNotEmpty(legalMoves);
            Assert.IsNotNull(legalMoves);
            Assert.That(legalMoves.Any(m=>m.Coordinate != startingPosition));
            Assert.IsTrue(expectedLegalMoves.Count == legalMoves.Count);
            
            foreach (var move in legalMoves)        
            {
                Assert.IsTrue(expectedLegalMoves.Contains(move.Coordinate));    
            }
        }
        
        #endregion
    }
}