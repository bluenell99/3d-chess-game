using NUnit.Framework;
using UnityEngine;

namespace ChessGame.Tests
{
    public class PieceTests
    {
        #region TESTS

        [Test]
        public void Piece_Initialisation_PropertiesSetCorrectly()
        {
            PieceType type = PieceType.Pawn;
            Vector2Int coordinate = new Vector2Int(0, 1);
            PieceColor color = PieceColor.White;

            Board board = new Board(8);

            Piece piece = new Pawn(coordinate, PieceColor.White, board);
            board.AddPiece(piece);

            Assert.AreEqual(type, piece.Type);
            Assert.NotNull(piece.Board);
            Assert.AreEqual(coordinate, piece.Coordinate);
            Assert.AreEqual(coordinate, piece.StartingCoordinate);
            Assert.AreEqual(color, piece.PieceColor);
            Assert.AreEqual(GameConstants.MaterialCostsDictionary[type], piece.MaterialCost);
            Assert.IsFalse(piece.HasMoved);
        }

        [Test]
        public void Piece_OnPositionChanged_EventTriggered()
        {
            Board board = new Board(8);
            Piece piece = new Pawn(new Vector2Int(0, 1), PieceColor.White, board);
            Vector2Int newPosition = new Vector2Int(0, 2);
            bool positionChangedEventTriggered = false;
            bool turnEndEventtriggered = false;
            
            AddKings(board);
            board.AddPiece(piece);

            piece.onPositionChanged += (pos) => positionChangedEventTriggered = pos == newPosition;
            piece.onPieceTurnEnd += p => turnEndEventtriggered = p == piece;

            piece.SetPositionOnBoard(newPosition, false, false);

            Assert.AreEqual(newPosition, piece.Coordinate);
            Assert.IsTrue(positionChangedEventTriggered);
            Assert.IsTrue(turnEndEventtriggered);
            
        }

        [Test]
        public void Piece_Take_EventTriggered()
        {
            Board board = new Board(8);
            Piece piece = new Pawn(new Vector2Int(0, 1), PieceColor.White, board);
            board.AddPiece(piece);
            
            bool eventTriggered = false;
            piece.onPieceTaken += p => eventTriggered = p == piece;
            
            piece.Take();

            Assert.IsTrue(eventTriggered);
        }

       

        [Test]
        public void Piece_ResetPiece_ReturnsToStartingPosition()
        {
            Board board = new Board(8);
            Vector2Int startingCoordinate = new Vector2Int(0, 1);
            Piece piece = new Pawn(startingCoordinate, PieceColor.White, board);
            PieceColor nextTurnColour = PieceColor.Black; 

            
            board.AddPiece(piece);
            AddKings(board);

            piece.SetPositionOnBoard(new Vector2Int(0, 3), false, false);
            piece.ResetPiece();

            Assert.AreEqual(startingCoordinate, piece.Coordinate);
            Assert.IsFalse(piece.HasMoved);
            

        }

        [Test]
        public void Piece_SetPieceOnBoard_OpponentIsTakenOnMatchingSquare()
        {
            Board board = new Board(8);

            Piece whitePawn = new Pawn(new Vector2Int(1, 1), PieceColor.White, board);
            Piece blackPawn = new Pawn(new Vector2Int(1, 2), PieceColor.Black, board);
            bool blackPieceOnTakeEventTriggered = false;

            
            board.AddPiece(whitePawn);
            board.AddPiece(blackPawn);
            AddKings(board);

            blackPawn.onPieceTaken += p => blackPieceOnTakeEventTriggered = p == blackPawn;
            
            whitePawn.SetPositionOnBoard(blackPawn.Coordinate, false, false);
            
            Assert.IsTrue(!board.PiecesInPlay.Contains(blackPawn));
            Assert.IsTrue(whitePawn.Coordinate == blackPawn.Coordinate);
            Assert.IsTrue(whitePawn.HasMoved);
            Assert.IsTrue(blackPieceOnTakeEventTriggered);
        }
        
        
       #endregion
       
       #region HELPERS

       /// <summary>
       /// Need to add kings to the board as the board evaluates legal movement and this requires the Kings
       /// </summary>
       /// <param name="board"></param>
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