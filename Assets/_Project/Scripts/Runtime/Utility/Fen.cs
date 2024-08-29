using System.Linq;

    using System;
    using System.Collections.Generic;
    using UnityEngine;

namespace ChessGame
{
    public class Fen
    {
        public List<Piece> Pieces { get; set; }
        public PieceColor ActiveColor { get; set; } = PieceColor.White;
        public string CastlingAvailability { get; set; } = "-";
        public Vector2Int? EnPassantTargetSquare { get; set; } = null;
        public int HalfmoveClock { get; set; } = 0;
        public int FullmoveNumber { get; set; } = 0;

        private string _fenString;

        /// <summary>
        /// Create and decode a FEN string into a usable object
        /// </summary>
        /// <param name="fenstring"></param>
        public Fen(string fenstring)
        {
            _fenString = fenstring;
            Decode();
        }

        public static string Encode(Board board)
        {
            string piecePlacement = "";
            string activeColor;
            string castlingRights;
            string enpassant;

            for (int i = 7; i >= 0; i--)
            {
                string piecesInRow = "";
                int emptySquares = 0;

                for (int j = 0; j < 8; j++)
                {
                    if (board.TryGetPieceFromSquare(new Vector2Int(j,i), out Piece piece))
                    {
                        if (emptySquares > 0)
                        {
                            piecesInRow += emptySquares.ToString();
                            emptySquares = 0;
                        }

                        char type = GetCharFromType(piece.Type);
                        type = piece.PieceColor == PieceColor.Black ? char.ToLower(type) : type;
                        piecesInRow += type;
                    }
                    else
                    {
                        emptySquares++;
                    }
                }

                if (emptySquares > 0)
                    piecesInRow += emptySquares.ToString();

                piecePlacement += $"{piecesInRow}/";
            }

            if (piecePlacement.EndsWith("/"))
                piecePlacement = piecePlacement.TrimEnd('/');

            activeColor = GameController.Instance.CurrentTurn == PieceColor.White ? "w" : "b";

            //TODO implement en-passant and castling availability into the FEN
            
            return $"{piecePlacement} {activeColor} - - {board.HalfMoveClock} {board.FullMoveNumber}";
        }

        private static char GetCharFromType(PieceType type)
        {
            return type switch
            {
                PieceType.Pawn => 'P',
                PieceType.Knight => 'N',
                PieceType.Bishop => 'B',
                PieceType.Rook => 'R',
                PieceType.Queen => 'Q',
                PieceType.King => 'K',
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
        
        private void Decode()
        {
            string[] parts = _fenString.Split(' ');

            Pieces = GetPieces(parts[0]);

            // if no active piece colour exists in the Fen string, White always starts
            if (parts.Length == 1)
                ActiveColor = PieceColor.White;
            else
                ActiveColor = parts[1] == "w" ? PieceColor.White : PieceColor.Black;


            if (parts.Length > 2 && parts[2] != "-") CastlingAvailability = parts[2];

            if (parts.Length > 3 && parts[3] != "-")
                EnPassantTargetSquare = Utilities.FromAlgebraicNotation(parts[3]);

            if (parts.Length > 4 && int.TryParse(parts[4], out int halfmoveClock))
                HalfmoveClock = halfmoveClock;

            if (parts.Length > 5 && int.TryParse(parts[5], out int fullmoveNumber))
                FullmoveNumber = fullmoveNumber;
        }

        private List<Piece> GetPieces(string piecesString)
        {
            List<Piece> pieces = new List<Piece>();

            string[] rows = piecesString.Split("/");

            for (int y = 0; y < rows.Length; y++)
            {
                int x = 0;

                foreach (var c in rows[y])
                {
                    if (char.IsDigit(c))
                    {
                        x += (int)char.GetNumericValue(c);
                    }
                    else
                    {
                        PieceColor color = char.IsUpper(c) ? PieceColor.White : PieceColor.Black;
                        Piece piece = GetPieceTypeFromChar(c, new Vector2Int(x, 7 - y), color);
                        pieces.Add(piece);
                        x++;
                    }
                }
            }

            return pieces;
        }

        private Piece GetPieceTypeFromChar(char c, Vector2Int position, PieceColor color)
        {
            switch (char.ToLower(c))
            {
                case 'p': return new Pawn(position, color, null);
                case 'n': return new Knight(position, color, null);
                case 'b': return new Bishop(position, color, null);
                case 'r': return new Rook(position, color, null);
                case 'q': return new Queen(position, color, null);
                case 'k': return new King(position, color, null);
                default: throw new ArgumentException($"Invalid piece symbol: {c}");
            }
        }

        

    }

}