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

        public PieceController CreatePiece(Piece piece, Transform container, Board board)
        {
            board.AddPiece(piece);
            piece.Board = board;

            var pieceView = Object.Instantiate(Resources.Load<PieceView>($"Pieces/Piece_{piece.Type}"), container);
            var pieceMaterial = piece.PieceColor == PieceColor.White
                ? _materials._whiteMaterial
                : _materials._blackMaterial;

            var controller = new PieceController(piece, pieceView, _materials, _squareSize);
            pieceView.Initialise(controller, pieceMaterial, piece.PieceColor, piece.Type);
            return controller;

        }
    }
}