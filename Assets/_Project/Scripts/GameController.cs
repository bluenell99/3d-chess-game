using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : Singleton<GameController>
{

    [Header("Game Setup")]
    [SerializeField] private float _squareSize = 1.5f;
    [SerializeField] private Transform _container;
    [SerializeField] private Transform _availableMovesContainer;
    [SerializeField] private BoardLayout _layout;
    [SerializeField] private PieceMaterials _materials;

    [SerializeField] private SelectionView _selectionView;
    [SerializeField] private bool _turnsEnabled;
    [SerializeField] private RectTransform _pawnPromotionUI;
    
    private BoardController _boardController;

    private Camera _mainCamera;
    private Piece _currentlySelectedPiece;
    private Pawn _pawnToBePromoted;
    

    public PieceColor CurrentTurn { get; set; }
    public bool PawnPromotionInProgress { get; private set; }
    public Piece LastPieceMoved { get; private set; }
    public bool TurnsEnabled => _turnsEnabled;
    
    
    private void Start()
    {
        _mainCamera = Camera.main;

        _selectionView.HideSelectionView();
        _pawnPromotionUI.gameObject.SetActive(false);
        _boardController = new BoardController(_container, _layout, _materials);
        
    }

    private void Update()
    {
        
        DetectSelection();
    }

    // TODO Optimise, refactor to a more generic "Selection" system
    private void DetectSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.TryGetComponent(out ISelectable selectable))
                {
                    selectable.OnSelect();
                }
            }
        }
    }

    public void SetSelectedPiece(PieceView view)
    {
        var piece = view.Controller.Piece;
        
        if (_currentlySelectedPiece != piece)
        {
            _currentlySelectedPiece = view.Controller.Piece;
            
            _selectionView.MoveSelectionView(view.transform.position);
            _selectionView.SetColor(Color.blue);
            
            ShowAvailableMoves(view);
            
        }
        else
        {
            _currentlySelectedPiece = null;
            _selectionView.HideSelectionView();
            ClearAvailableMovesView();
        }
        
    }

    private void ShowAvailableMoves(PieceView view)
    {
        ClearAvailableMovesView();
        
        Piece piece = view.Controller.Piece;
        HashSet<Move> moves = piece.GetLegalMoves().Where(m => m.IsDeliveringCheck == false).ToHashSet();

        
        foreach (var move in moves)
        {
            var offset = _squareSize / 2;
            var x = move.Coordinate.x * _squareSize + offset;
            var y = move.Coordinate.y * _squareSize + offset;
        
            var position = new Vector3(x, 0, y);

            var selection = Instantiate(_selectionView, _availableMovesContainer);
            selection.transform.localPosition = position;
            selection.Coordinate = move.Coordinate;
        }
        
    }

    private void ClearAvailableMovesView()
    {
        if (_availableMovesContainer.transform.childCount == 0)
            return; 
        
        for (int i = 0; i < _availableMovesContainer.transform.childCount; i++)
        {
            Destroy(_availableMovesContainer.transform.GetChild(i).gameObject);
        }
    }

    public void MoveSelected(Vector2Int coordinate)
    {
        _currentlySelectedPiece.SetPositionOnBoard(coordinate, false, false);
        
        _currentlySelectedPiece = null;
        _selectionView.HideSelectionView();
        
        ClearAvailableMovesView();
        
    }

    public void DisplayPromotionUI(Pawn pawn)
    {
        PawnPromotionInProgress = true;
        _pawnToBePromoted = pawn;
        _pawnPromotionUI.gameObject.SetActive(true);
    }

    public void PromotionSelected(int intType)
    {
        PieceType type = (PieceType)intType;

        PawnPromotionInProgress = false;
        
        _boardController.UpdatePiece(_pawnToBePromoted, type);
        _pawnToBePromoted = null;
        _pawnPromotionUI.gameObject.SetActive(false);
    }

    public void SetLastMovedPiece(Piece piece)
    {
        LastPieceMoved = piece;
    }
}