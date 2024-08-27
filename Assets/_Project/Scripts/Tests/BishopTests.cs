using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using NUnit.Framework;
using UnityEngine;

namespace ChessGame.Tests
{
    public class BishopTests
    {
        [Test]
        public void Bishop_GetLegalMoves_ExpectedMovesWhenNoBlockingOrCapturingPieces()
        {
            Board board = new Board(8);
            Vector2Int startingPosition = new Vector2Int(3, 3);
            HashSet<Vector2Int> expectedLegalMoves = new HashSet<Vector2Int>()
            {
                // left down
                new(0, 0),
                new(1, 1),
                new(2, 2),
                new(4, 4),
                new(5, 5),
                new(6, 6),
                new(7, 7),
                new(4, 2),
                new(5, 1),
                new(6, 0),
                new(2, 4),
                new(1, 5),
                new(0, 6),            
            };
            

            Piece piece = new Bishop(startingPosition, PieceColor.White, board);
            board.AddPiece(piece);
            AddKings(board);

            HashSet<Move> legalMoves = piece.GetLegalMoves();
            
            Assert.IsNotNull(legalMoves);
            Assert.IsNotEmpty(legalMoves);
            
            foreach (var move in legalMoves)
            {
                Assert.IsTrue(expectedLegalMoves.Contains(move.Coordinate));
            }
            
        }
        
        [Test]
        public void Bishop_GetLegalMoves_ExpectedMovesWhenBlockingPiecesExist()
        {
            Board board = new Board(8);
            Vector2Int startingPosition = new Vector2Int(3, 3);

            Piece whitePawn = new Pawn(new Vector2Int(5, 5), PieceColor.White, board);
            Piece blackPawn = new Pawn(new Vector2Int(1, 5), PieceColor.Black, board);
            
            HashSet<Vector2Int> expectedLegalMoves = new HashSet<Vector2Int>()
            {
                blackPawn.Coordinate,
                new(2,4),
                new(2,2),
                new(4,4),
                new(2,2),
                new(1,1),
                new(0,0),
                new (4,2),
                new (5,1),
                new (6,0)
,           };
            

            Piece piece = new Bishop(startingPosition, PieceColor.White, board);
            board.AddPiece(piece);
            board.AddPiece(whitePawn);
            board.AddPiece(blackPawn);
            AddKings(board);

            HashSet<Move> legalMoves = piece.GetLegalMoves();

            
            Assert.IsNotNull(legalMoves);
            Assert.IsNotEmpty(legalMoves);
            
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