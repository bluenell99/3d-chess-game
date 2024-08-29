using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using NUnit.Framework;
using UnityEngine;

namespace ChessGame.Tests
{
    public class PawnTests
    {

        #region TESTS
        
        [Test]
        public void Pawn_GetLegalMoves_ExpectedMovesWhenNoBlockingOrCapturingPieces()
        {
            Board board = new Board(8);
            Vector2Int startingPosition = new Vector2Int(4, 1);
            Pawn piece = new Pawn(startingPosition, PieceColor.White, board);
            

            board.AddPiece(piece);
            AddKings(board);

            HashSet<Vector2Int> expectedLegalMoves = new HashSet<Vector2Int>()
            {
                new(4, 2),
                new(4, 3)
            };

            HashSet<Move> legalMoves = piece.GetLegalMoves();

            Assert.IsNotEmpty(legalMoves);
            Assert.IsNotNull(legalMoves);
            Assert.That(legalMoves.Any(move=>move.Coordinate != startingPosition));
            Assert.That(expectedLegalMoves.Count == legalMoves.Count);

            foreach (var move in legalMoves)
            {
                Assert.IsTrue(expectedLegalMoves.Contains(move.Coordinate));
            }
        }
        
        [Test]
        public void Pawn_GetLegalMoves_ExpectedMovesWhenBlockingPieceExists()
        {
            Board board = new Board(8);
            Vector2Int startingPosition = new Vector2Int(4, 1);
           Pawn whitePawn = new Pawn(startingPosition, PieceColor.White, board);
           Pawn whitePiece = new Pawn(new Vector2Int(4,3), PieceColor.White, board);
           Pawn blackPiece = new Pawn(new Vector2Int(5, 2), PieceColor.Black, board);

            board.AddPiece(whitePawn);
            board.AddPiece(whitePiece);
            board.AddPiece(blackPiece);
            AddKings(board);

            HashSet<Vector2Int> expectedLegalMoves = new HashSet<Vector2Int>()
            {
                new(4, 2),
                blackPiece.Coordinate
            };

            HashSet<Move> legalMoves = whitePawn.GetLegalMoves();

            Assert.IsNotEmpty(legalMoves);
            Assert.IsNotNull(legalMoves);
            Assert.That(legalMoves.Any(move=>move.Coordinate != startingPosition));
            Assert.That(legalMoves.Any);
            Assert.That(expectedLegalMoves.Count == legalMoves.Count);

            foreach (var move in legalMoves)
            {
                Assert.IsTrue(expectedLegalMoves.Contains(move.Coordinate));
            }
        }

        [Test]  
        public void Pawn_OnPawnPromotionEvent_IsTriggered()
        {
            Board board = new Board(8);
            Pawn pawn = new Pawn(new Vector2Int(3, 6), PieceColor.White, board);
            bool onPawnPromotionEventTriggered = false;
            Vector2Int promotionCoordinate = new Vector2Int(3, 7);
            
            board.AddPiece(pawn);
            AddKings(board);
            pawn.onPawnPromotionAvailable += p => onPawnPromotionEventTriggered = p == pawn;

            pawn.SetPositionOnBoard(promotionCoordinate, false, false);

            Assert.True(pawn.Coordinate == promotionCoordinate);
            Assert.IsTrue(onPawnPromotionEventTriggered);
            Assert.That(!board.PiecesInPlay.Contains(pawn));
        }


        [Test]
        public void Pawn_EnPassantCapture_CaptureIsAvailable()
        {
            Board board = new Board(8);
            Pawn whitePawn = new Pawn(new Vector2Int(3, 4), PieceColor.White, board);
            Pawn blackPawn = new Pawn(new Vector2Int(4, 6), PieceColor.Black, board);

            Vector2Int enPassantCaptureSquare = new Vector2Int(4, 5);

            board.AddPiece(whitePawn);
            board.AddPiece(blackPawn);
            AddKings(board);
            
            // move the black pawn forward two squares
            blackPawn.SetPositionOnBoard(new Vector2Int(4,4), false, false);

            // get the white pawns moves
            HashSet<Move> whiteLegalMoves = whitePawn.GetLegalMoves();

            // was last moved piece the black pawn
            Assert.That(board.LastPieceMoved == blackPawn);
            // can the pawn get the en-passant target square (the square behind the black pawn)
            Assert.That(whiteLegalMoves.Any(move => move.Coordinate == enPassantCaptureSquare));

            // try and capture it
            whitePawn.SetPositionOnBoard(enPassantCaptureSquare, false, false);
            
            // verify capture
            Assert.That(!board.PiecesInPlay.Contains(blackPawn));

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