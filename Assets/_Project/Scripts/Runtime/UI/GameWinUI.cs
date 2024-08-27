using UnityEngine;
using UnityEngine.UI;


namespace ChessGame
{
    public class GameWinUI : MonoBehaviour
    {
        [SerializeField] private Text _winnerText;
        
        public void ShowWinner(PieceColor color)
        {
            _winnerText.text = $"{color} Wins!";
        }
    }
}

