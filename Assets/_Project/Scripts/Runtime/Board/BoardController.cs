using System;
using UnityEngine;

namespace ChessGame
{
    public class BoardController
    {
        public Board Board { get; private set; }
        private readonly PieceFactory _pieceFactory;
        private Transform _pieceContainer;
        private BoardLayout _layout;

        private GameController _gameController;

        /// <summary>
        /// Creates a new Board controller
        /// </summary>
        /// <param name="pieceContainer">The Transform that will contain all the PieceView objects</param>
        /// <param name="layout">The Layout that this Board will start with</param>
        /// <param name="materials">The Materials that the Pieces will use</param>
        public BoardController(Transform pieceContainer, BoardLayout layout, PieceMaterials materials)
        {
            Board = new Board(GameConstants.BOARD_SIZE);
            _pieceContainer = pieceContainer;
            _pieceFactory = new PieceFactory(layout.SquareSize, materials);
            _layout = layout;
            _gameController = GameController.Instance;

            AddPiecesToBoard();
            
            Debug.Log(Fen.Encode(Board));
        }

        /// <summary>
        /// Spawns all the Pieces on the board
        /// </summary>
        private void AddPiecesToBoard()
        {
            // Iterate all the Pieces from the layout
            foreach (var piece in _layout.GetPieces())
            {
                // Instruct the PieceFactory to create a PieceView object
                var pieceController = _pieceFactory.CreatePiece(piece, _pieceContainer, Board);
                pieceController.PlacePieceOnBoard(piece.Coordinate);

                // Subscribe to required events 
                switch (piece)
                {
                    case King king:
                        king.onKingInCheckMate += OnKingCheckmate;
                        break;
                    case Pawn pawn:
                        pawn.onPawnPromotionAvailable += OnPawnPromotionAvailable;
                        break;
                }
            }

            // update turn order based on starting layout
            _gameController.SetTurn(_layout.StartingPieceColor);
            Board.onBoardReset += OnBoardReset;

        }

        /// <summary>
        /// Callback from <c>onKingCheckMate</c>
        /// </summary>
        /// <param name="king"></param>
        private void OnKingCheckmate(King king)
        {
            // get this Checkmated King's colour
            PieceColor color = king.PieceColor;
            
            // The winner is the opposite colour
            PieceColor winColour = color == PieceColor.White ? PieceColor.Black : PieceColor.White;
            
            // End the game
            _gameController.EndGame(winColour);
        }

        /// <summary>
        /// Callback from <c>onBoardReset</c>
        /// </summary>
        private void OnBoardReset()
        {
            // Set the turn based on the starting Layout
            _gameController.SetTurn(_layout.StartingPieceColor);
        }

        /// <summary>
        /// Updates a Pawn's type to a new PieceType
        /// </summary>
        /// <param name="pawn">The Pawn that is being promoted</param>
        /// <param name="type">The selected PieceType</param>
        public void UpdatePiece(Pawn pawn, PieceType type)
        {

            // add new based on type
            switch (type)
            {
                case PieceType.Queen:
                    
                    // Create new Piece, PieceView, and set it on the board where the Pawn was
                    Queen queen = new Queen(pawn.Coordinate, pawn.PieceColor, Board);
                    var queenController = _pieceFactory.CreatePiece(queen, _pieceContainer, Board);
                    queenController.PlacePieceOnBoard(pawn.Coordinate);

                    break;

                case PieceType.Bishop:
                    Bishop bishop = new Bishop(pawn.Coordinate, pawn.PieceColor, Board);
                    var bishopController = _pieceFactory.CreatePiece(bishop, _pieceContainer, Board);
                    bishopController.PlacePieceOnBoard(pawn.Coordinate);

                  
                    break;

                case PieceType.Rook:
                    Rook rook = new Rook(pawn.Coordinate, pawn.PieceColor, Board);
                    var rookController = _pieceFactory.CreatePiece(rook, _pieceContainer, Board);
                    rookController.PlacePieceOnBoard(pawn.Coordinate);

                    
                    break;

                case PieceType.Knight:
                    Knight knight = new Knight(pawn.Coordinate, pawn.PieceColor, Board);

                    var knightController = _pieceFactory.CreatePiece(knight, _pieceContainer, Board);
                    knightController.PlacePieceOnBoard(pawn.Coordinate);
                    
                    break;
            }


        }

        /// <summary>
        /// Callback function from <c>onPawnPromtionAvailable</c>
        /// </summary>
        /// <param name="pawn">The pawn that will be promoted</param>
        private void OnPawnPromotionAvailable(Pawn pawn)
        {
            _gameController.DisplayPromotionUI(pawn);
        }

        /// <summary>
        /// Callback function from <c>onPieceTaken</c>
        /// </summary>
        /// <param name="piece">The piece that has been taken</param>
        private void OnPieceTaken(Piece piece)
        {
            Board.RemovePiece(piece);
        }

        /// <summary>
        /// Resets the board
        /// </summary>
        public void ResetBoard()
        {
            Board.ResetBoard();
        }
    }

}