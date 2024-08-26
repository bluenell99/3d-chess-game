using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TestTools;

namespace ChessGame.Tests
{
    public class BoardTests
    {

        #region  Tests
        
        // Board gets initialized with a positive integer
        [Test]
        public void Initialization_BoardInitializesSquares()
        {
            int boardSize = 8;
            Board board = new Board(boardSize);
            Assert.NotNull(board);
            
            Assert.IsEmpty(board.PiecesInPlay);
            Assert.IsTrue(board.Squares.Count == boardSize * boardSize);
        }

        /// <summary>
        /// Board should thrown an exception if it's not given a positive integer size
        /// </summary>
        [Test]
        public void Initialization_BoardThrowsError_WhenInvalidSize()
        {
            Assert.Throws<Exception>(() => CreateBoard(-1));
        }

        [Test]
        public void SquareIsOnBoard_ReturnsTrue_WhenSquareIsFoundOnTheBoard()
        {
            Board board = new Board(8);
            Assert.IsTrue(board.IsSquareOnBoard(new Vector2Int(4,4)));
            Assert.IsFalse(board.IsSquareOnBoard(new Vector2Int(12,12)));
        }

        [Test]
        public void TryGetPieceFromSquare_ReturnsTrue_WhenPieceIsFound()
        {
            Board board = new Board(8);
            Piece expectedPiece = new Pawn(new Vector2Int(0, 1), PieceColor.White, board);
            board.AddPiece(expectedPiece);

            bool result = board.TryGetPieceFromSquare(new Vector2Int(0, 1), out Piece actualPiece);

            Assert.IsTrue(result);
            Assert.That(actualPiece, Is.EqualTo(expectedPiece));
        }
        
        [Test]
        public void TryGetPieceFromSquare_ReturnsFalse_WhenNoPieceIsFound()
        {
            Board board = new Board(8);
            Vector2Int coordinate = new Vector2Int(3, 3);

            bool result = board.TryGetPieceFromSquare(coordinate, out Piece actualPiece);

            Assert.IsFalse(result);
            Assert.IsNull(actualPiece);
        }

        [Test]
        public void IsSquareOccupied_ReturnsTrue_WhenSquareIsOccupied()
        {
            Board board = new Board(8);
            Piece piece = new Pawn(new Vector2Int(0, 1), PieceColor.White, board);
            board.AddPiece(piece);
            
            Assert.IsTrue(board.IsSquareOccupied(new Vector2Int(0,1)));
        }
        
        [Test]
        public void IsSquareOccupied_ReturnsFalse_WhenSquareIsUnoccupied()
        {
            Board board = new Board(8);
            Piece piece = new Pawn(new Vector2Int(0, 1), PieceColor.White, board);
            board.AddPiece(piece);
            
            Assert.IsFalse(board.IsSquareOccupied(new Vector2Int(3,3)));
        }
        
        
        /// <summary>
        /// Board can add pieces
        /// </summary>
        [Test]
        public void AddPiece_SuccessfullyAddPieceToBoard()
        {
            Board board = new Board(8);
            Piece piece = new Pawn(new Vector2Int(0, 1), PieceColor.White, board);

            Assert.NotNull(board);
            Assert.DoesNotThrow(()=> board.AddPiece(piece));
            Assert.IsTrue(board.PiecesInPlay.Contains(piece));
        }

        [Test]
        public void RemovePiece_SuccessfullyRemovePieceFromBoard()
        {
            // Setup board and add 8 pawns 
            Board board = new Board(8);
            List<Piece> pieces = new List<Piece>()
            {
                new Pawn(new Vector2Int(0, 1), PieceColor.White, board),
                new Pawn(new Vector2Int(1, 1), PieceColor.White, board),
                new Pawn(new Vector2Int(2, 1), PieceColor.White, board),
                new Pawn(new Vector2Int(3, 1), PieceColor.White, board),
                new Pawn(new Vector2Int(4, 1), PieceColor.White, board),
                new Pawn(new Vector2Int(5, 1), PieceColor.White, board),
                new Pawn(new Vector2Int(6, 1), PieceColor.White, board),
                new Pawn(new Vector2Int(7, 1), PieceColor.White, board),
            };

            // add the pawns to play
            foreach (var piece in pieces)
            {
                board.AddPiece(piece);
            }
            
            // make assertions
            Assert.IsNotNull(board != null);
            Assert.DoesNotThrow(()=> board.RemovePiece(pieces[0]));
            Assert.IsTrue(!board.PiecesInPlay.Contains(pieces[0]));
        }

        [Test]
        public void GetKing_ReturnsKingOfGivenPiecesColour()
        {
            Board board = new Board(8);
            Piece givenPiece = new Pawn(new Vector2Int(0, 1), PieceColor.White, board);
            King expectedKing = new King(new Vector2Int(6, 0), PieceColor.White, board);
            
            board.AddPiece(givenPiece);
            board.AddPiece(expectedKing);

            Assert.That(expectedKing, Is.EqualTo(board.GetKing(givenPiece)));
        }

        [Test]
        public void GetOpponentsKing_ReturnsOpponentsKingOfGivenPiecesColour()
        {
            Board board = new Board(8);
            Piece givenPiece = new Pawn(new Vector2Int(0, 1), PieceColor.White, board);
            King expectedKing = new King(new Vector2Int(6, 7), PieceColor.Black, board);
            
            board.AddPiece(givenPiece);
            board.AddPiece(expectedKing);

            Assert.That(expectedKing, Is.EqualTo(board.GetOpponentsKing(givenPiece)));
        }

        [Test]
        public void GetPieces_ReturnsAllPiecesOfGivenPiecesColour()
        {
            Board board = new Board(8);

            List<Piece> ourPieces = new List<Piece>()
            {
                new Pawn(new Vector2Int(0, 1), PieceColor.White, board),
                new Pawn(new Vector2Int(1, 1), PieceColor.White, board),
                new Pawn(new Vector2Int(2, 1), PieceColor.White, board),
                new Pawn(new Vector2Int(3, 1), PieceColor.White, board),
                new Pawn(new Vector2Int(4, 1), PieceColor.White, board),
                new Pawn(new Vector2Int(5, 1), PieceColor.White, board),
                new Pawn(new Vector2Int(6, 1), PieceColor.White, board),
                new Pawn(new Vector2Int(7, 1), PieceColor.White, board),
            };

            foreach (var piece in ourPieces)
            {
                board.AddPiece(piece);
            }

            List<Piece> actualPieces = board.GetPieces(ourPieces[0]).ToList();
            
            Assert.IsNotNull(actualPieces);
            Assert.IsNotEmpty(actualPieces);
            Assert.That(actualPieces, Is.EquivalentTo(ourPieces));
        }
        
        [Test]
        public void GetOpponentPieces_ReturnsAllOpponentPiecesOfGivenPiecesColour()
        {
            Board board = new Board(8);

            Piece ourPiece = new Pawn(new Vector2Int(0, 1), PieceColor.White, board);
            List<Piece> opponentPieces = new List<Piece>()
            {
                new Pawn(new Vector2Int(0, 6), PieceColor.Black, board),
                new Pawn(new Vector2Int(1, 6), PieceColor.Black, board),
                new Pawn(new Vector2Int(2, 6), PieceColor.Black, board),
                new Pawn(new Vector2Int(3, 6), PieceColor.Black, board),
                new Pawn(new Vector2Int(4, 6), PieceColor.Black, board),
                new Pawn(new Vector2Int(5, 6), PieceColor.Black, board),
                new Pawn(new Vector2Int(6, 6), PieceColor.Black, board),
                new Pawn(new Vector2Int(7, 6), PieceColor.Black, board),
            };

            board.AddPiece(ourPiece);

            foreach (var piece in opponentPieces)
            {
                board.AddPiece(piece);
            }
            
            List<Piece> actualPieces = board.GetOpponentPieces(ourPiece).ToList();
            Assert.That(actualPieces, Is.EquivalentTo(opponentPieces));
        }
        
        
        
        #endregion

        #region Helpers

        private void CreateBoard(int size)
        {
            Board board = new Board(size);
        }
        
        #endregion
        
    }
}
