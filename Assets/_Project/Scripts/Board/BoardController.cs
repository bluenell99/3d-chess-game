using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardController
{
    public Board Board { get; private set; }
    private readonly PieceFactory _pieceFactory;
    private Transform _container;
    private BoardLayout _layout;
    

    public BoardController(Transform container, BoardLayout layout, PieceMaterials materials)
    {
        Board = new Board(GameConstants.BOARD_SIZE);
        _container = container;
        _pieceFactory = new PieceFactory(layout.SquareSize, materials);
        _layout = layout;
        
        InitialiseBoard();
    }

    private void InitialiseBoard()
    {
        // Init pieces, place them on board
        foreach (var piece in _layout.GetPieces())
        {
            var pieceController = _pieceFactory.CreatePiece(piece, _container, Board);
            pieceController.PlacePieceOnBoard(piece.Coordinate);
            
            piece.onPieceTaken += OnPieceTaken;

            if (piece is Pawn pawn)
            {
                pawn.onPawnPromotionAvailable += OnPawnPromotionAvailable;
            }
            
        }

        // update turn order based on starting layout
        GameController.Instance.CurrentTurn = _layout.StartingPieceColor;
        
    }

    public void UpdatePiece(Piece piece, PieceType type)
    {
        
        // add new based on type
        switch (type)
        {
            case PieceType.Queen :
                Queen queen = new Queen(piece.Coordinate, piece.PieceColor, Board);
                var queenController = _pieceFactory.CreatePiece(queen, _container, Board);
                queenController.PlacePieceOnBoard(piece.Coordinate);
                
                queen.onPieceTaken += OnPieceTaken;
                break;
            
            case PieceType.Bishop :
                Bishop bishop = new Bishop(piece.Coordinate, piece.PieceColor, Board);
                var bishopController = _pieceFactory.CreatePiece(bishop, _container, Board);
                bishopController.PlacePieceOnBoard(piece.Coordinate);
                
                bishop.onPieceTaken += OnPieceTaken;
                break;
            
            case PieceType.Rook : 
                Rook rook = new Rook(piece.Coordinate, piece.PieceColor, Board);
                var rookController = _pieceFactory.CreatePiece(rook, _container, Board);
                rookController.PlacePieceOnBoard(piece.Coordinate);
                
                rook.onPieceTaken += OnPieceTaken;
                break;
            
            case PieceType.Knight : 
                Knight knight = new Knight(piece.Coordinate, piece.PieceColor, Board);
                
                var knightController = _pieceFactory.CreatePiece(knight, _container, Board);
                knightController.PlacePieceOnBoard(piece.Coordinate);
                knight.onPieceTaken += OnPieceTaken;
                break;
        }
        
        
    }
    private void OnPawnPromotionAvailable(Pawn pawn)
    {
        GameController.Instance.DisplayPromotionUI(pawn);
    }

    private void OnPieceTaken(Piece piece)
    {
        Board.RemovePiece(piece);
    }
}
