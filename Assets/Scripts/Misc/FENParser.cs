using System;
using System.Collections.Generic;
using UnityEngine;

public class FENParser : MonoBehaviour
{
    public static FENParser Instance { get; private set; }

    [SerializeField] private PieceData[] _piecesData;

    private Dictionary<char, PieceData> _pieceMap = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (var piece in _piecesData)
        {
            char key = GetFENCharacter(piece);
            _pieceMap.Add(key, piece);
        }
    }

    private char GetFENCharacter(PieceData piece)
    {
        char character = piece.PieceType switch
        {
            PieceType.Pawn => 'p',
            PieceType.Knight => 'n',
            PieceType.Bishop => 'b',
            PieceType.Rook => 'r',
            PieceType.Queen => 'q',
            PieceType.King => 'k',
            _ => throw new ArgumentOutOfRangeException()
        };

        return piece.Color == PlayerColor.White
        ? char.ToUpper(character)
        : character;
    }

    public PieceData GetPieceData(char c)
    {
        return _pieceMap[c];
    }

    private PlayerColor GetTurn(char c)
    {
        return c switch
        {
            'w' => PlayerColor.White,
            'b' => PlayerColor.Black,
            _ => throw new ArgumentException($"Invalid FEN turn character: {c}")

        };
    }

    public void ApplyFENState(char sideToMove, string castling, string enPassant, int halfMove, int fullMove)
    {
        GameManager.Instance.AssignFirstTurn(GetTurn(sideToMove));
        GameManager.Instance.SetCastlingRights(castling.Contains('K'), castling.Contains('Q'), castling.Contains('k'), castling.Contains('q'));
    }
}
