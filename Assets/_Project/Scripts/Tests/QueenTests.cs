using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace ChessGame.Tests
{
    public class QueenTests
    {
        [Test]
        public void Queen_GetLegalMoves_ExpectedMovesWhenNoBlockingOrCapturingPieces()
        {
            Board board = new Board(8);
            Vector2Int startingPosition = new Vector2Int(3, 4);
            HashSet<Vector2Int> expectedLegalMoves = new HashSet<Vector2Int>()
            {
                new(0,1),     
                new(1,2),     
                new(2,3),     
                new(0,4),     
                new(1,4),
                new(2,4),
                new(2,5),     
                new(1,6),     
                new(0,7),     
                new(3,5),     
                new(3,6),     
                new(3,7),     
                new(4,5),     
                new(5,6),     
                new(6,7),     
                new(4,4),     
                new(5,4),     
                new(6,4),     
                new(7,4),     
                new(4,3),     
                new(5,2),     
                new(6,1),     
                new(7,0),     
                new(3,3),
                new(3,2),
                new(3,1),
                new(3,0)
            };
            

            Piece piece = new Queen(startingPosition, PieceColor.White, board);
            board.AddPiece(piece);
            AddKings(board);

            HashSet<Move> legalMoves = piece.GetLegalMoves();
            
            Assert.IsNotNull(legalMoves);
            Assert.IsNotEmpty(legalMoves);
            Assert.That(legalMoves.Any(move => move.Coordinate != startingPosition));
            
            foreach (var move in legalMoves)
            {
                Assert.IsTrue(expectedLegalMoves.Contains(move.Coordinate));
            }
            
        }
        
        [Test]
        public void Queen_GetLegalMoves_ExpectedMovesWhenBlockingPiecesExist()
        {
            Board board = new Board(8);
            Vector2Int startingPosition = new Vector2Int(3, 4);

            Piece whitePawn = new Pawn(new Vector2Int(3, 2), PieceColor.White, board);
            Piece blackPawn = new Pawn(new Vector2Int(1, 4), PieceColor.Black, board);
            Piece blackPawn2 = new Pawn(new Vector2Int(5, 2), PieceColor.Black, board);
            
            HashSet<Vector2Int> expectedLegalMoves = new HashSet<Vector2Int>()
            {
                blackPawn.Coordinate,
                blackPawn2.Coordinate,
                new(0,1),
                new(1,2),
                new(2,3),
                new(2,4),
                new(2,5),
                new(1,6),
                new(0,7),
                new(3,5),
                new(3,6),
                new(3,7),
                new(4,5),
                new(5,6),
                new(6,7),
                new(4,4),
                new(5,4),
                new(6,4),
                new(7,4),
                new(4,3),
                new(5,2),
                new(3,3),
           };
            

            Piece piece = new Queen(startingPosition, PieceColor.White, board);
            board.AddPiece(piece);
            board.AddPiece(whitePawn);
            board.AddPiece(blackPawn);
            board.AddPiece(blackPawn2);
            AddKings(board);

            HashSet<Move> legalMoves = piece.GetLegalMoves();
            
            Assert.IsNotNull(legalMoves);
            Assert.IsNotEmpty(legalMoves);
            Assert.That(legalMoves.Any(move => move.Coordinate != startingPosition));
            
            
            foreach (var move in legalMoves)
            {
                Assert.IsTrue(expectedLegalMoves.Contains(move.Coordinate));
            }
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