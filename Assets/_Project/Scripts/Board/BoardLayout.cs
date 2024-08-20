using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess/Board Layout")]
public class BoardLayout : SerializedScriptableObject
{
    public string Fen;
    public float SquareSize = 1.5f;

    public PieceColor StartingPieceColor { get; private set; }
    
    public List<Piece> GetPieces()
    {
        Fen fen = new Fen(Fen);

        StartingPieceColor = fen.ActiveColor;
        
        return fen.Pieces;
    }

   
}