using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ChessGame
{
    public class PieceFactory
    {
        private readonly float _squareSize;
        private readonly PieceMaterials _materials;

        public PieceFactory(float squareSize, PieceMaterials materials)
        {
            _squareSize = squareSize;
            _materials = materials;
        }

        /// <summary>
        /// Constructs a new Piece with given Piece model. Instantiates a PieceView and initialises it
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="container"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        /// <remarks>This function takes a Piece as a param because Pieces are created when reading the BoardLayout</remarks>
        public PieceController CreatePiece(Piece piece, Transform container, Board board)
        {
            // add the Piece to the board
            board.AddPiece(piece);
            // assign the Board reference to the Piece
            piece.Board = board;

            // Instantiate a new PieceView GameObject
            var pieceView = Object.Instantiate(Resources.Load<PieceView>($"Pieces/Piece_{piece.Type}"), container);
            var pieceMaterial = piece.PieceColor == PieceColor.White
                ? _materials._whiteMaterial
                : _materials._blackMaterial;

            // Create a new PieceController
            var controller = new PieceController(piece, pieceView, _materials, _squareSize);
            pieceView.Initialise(controller, pieceMaterial, piece.PieceColor, piece.Type);
            return controller;

        }
    }
}