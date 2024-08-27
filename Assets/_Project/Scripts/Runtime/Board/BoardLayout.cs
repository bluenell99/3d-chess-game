using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ChessGame
{
    [CreateAssetMenu(menuName = "Chess/Board Layout")]

    public class BoardLayout : SerializedScriptableObject
    {
        /// <summary>
        /// The FEN notation of the starting layout
        /// </summary>
        public string Fen;

        /// <summary>
        /// The World Space size of a Board Square
        /// </summary>
        public float SquareSize;

        /// <summary>
        /// The colour the starts based on the layout
        /// </summary>
        public PieceColor StartingPieceColor { get; private set; }

        /// <summary>
        /// Gets a list of Pieces from the FEN string
        /// </summary>
        /// <returns></returns>
        public List<Piece> GetPieces()
        {
            // Create a new FEN
            Fen fen = new Fen(Fen);

            // Get the Pieces and starting colour
            StartingPieceColor = fen.ActiveColor;
            return fen.Pieces;
        }
    }
}