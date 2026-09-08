using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PieceSpawner : MonoBehaviourPunCallbacks
{
    private const string StartingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    [SerializeField] private GameObject _referenceTile;
    [SerializeField] private Transform _pieceContainer;
    [SerializeField] private PieceData[] _piecesData;

    private bool _spawned = false;

    IEnumerator Start()
    {
        PieceDataManager.Initialize();

        if (PhotonNetwork.IsMasterClient) PlayerManager.AssignRandomColors();
        yield return new WaitUntil(() =>
            PlayerManager.Instance != null &&
            PlayerManager.Instance.AreColorsAssigned());

        if (!PhotonNetwork.IsMasterClient) yield break;

        TrySpawnPieces();
    }

    void TrySpawnPieces()
    {
        if (_spawned)
        {
            Debug.Log("Boolean spawned is already true");
            return;
        }

        if (PieceDataManager.Instance == null)
        {
            Debug.LogError("PieceDataManager.Instance is null. Cannot spawn pieces");
            return;
        }

        SpawnPiecesFromFEN(StartingFen);
        _spawned = true;
    }

    private void SpawnPiecesFromFEN(string fen)
    {
        string[] parts = fen.Split(' ');
        string board = parts[0];
        char sideToMove = parts[1][0];
        string castling = parts[2];
        string enPassant = parts[3];
        int halfmove = int.Parse(parts[4]);
        int fullmove = int.Parse(parts[5]);

        int x = 0;
        int y = 7;

        foreach (char c in board)
        {
            if (c == '/')
            {
                y--;
                x = 0;
                continue;
            }

            if (char.IsDigit(c))
            {
                x += c - '0';
                continue;
            }

            var pieceData = FENParser.Instance.GetPieceData(c);

            PhotonNetwork.InstantiateRoomObject
                (
                    $"Prefabs/Pieces/{pieceData.name}",
                    Vector3.zero,
                    Quaternion.identity,
                    0,
                    new object[] { pieceData.name, x, y } // used in PieceSetup
                );
            x++;
        }

        OnPiecesSpawned();
    }

    private void OnPiecesSpawned()
    {
        GameManager.Instance.PiecesAreSpawned = true;
        GameManager.Instance.SetGameStateNetwork(GameManager.GameState.InGame);
        GameManager.Instance.AssignFirstTurnWhite();
        CalculateMoves.Instance.CalculateAllMoves();

        AudioManager.Instance.PlaySFX(AudioManager.Instance.GameStart);
    }
}
