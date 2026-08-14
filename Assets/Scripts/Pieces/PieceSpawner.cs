using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PieceSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _referenceTile;
    [SerializeField] private Transform _pieceContainer;
    [SerializeField] private PieceData[] _piecesData;

    private bool spawned = false;

    IEnumerator Start()
    {
        PieceDataManager.Initialize();

        if (PhotonNetwork.IsMasterClient) PlayerManager.AssignRandomColors();

        yield return new WaitUntil(() =>
            PlayerManager.Instance != null &&
            PlayerManager.Instance.CheckColorsAssigned());

        if (!PhotonNetwork.IsMasterClient) yield break;

        TrySpawnPieces();
    }

    void TrySpawnPieces()
    {
        if (DebugManager.IsDebugMode) Debug.Log("TrySpawnPieces() called");

        if (spawned)
        {
            Debug.Log("Boolean spawned is already true");
            return;
        }

        if (!PlayerManager.Instance.ColorsAreAssigned)
        {
            Debug.LogError("Player colors are not assigned yet. Cannot spawn pieces.");
            return;
        }

        if (PieceDataManager.Instance == null)
        {
            Debug.LogError("PieceDataManager.Instance is null. Cannot spawn pieces");
            return;
        }

        SpawnPieces();
        spawned = true;
    }

    public void SpawnPieces()
    {
        if (DebugManager.IsDebugMode) Debug.Log("SpawnPieces() called");

        foreach (var pieceData in _piecesData)
        {
            foreach (var pos in pieceData.InitialPositions)
            {
                PhotonNetwork.InstantiateRoomObject
                (
                    $"Prefabs/Pieces/{pieceData.name}",
                    Vector3.zero,
                    Quaternion.identity,
                    0,
                    new object[] { pieceData.name, pos.x, pos.y } // used in PieceSetup
                );
            }
        }
        GameManager.Instance.SetGameStateNetwork(GameManager.GameState.InGame);
        ExitGames.Client.Photon.Hashtable props = new()
        {
            { "GameState", GameManager.GameState.InGame.ToString() }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        GameManager.Instance.PiecesAreSpawned = true;

        CalculateMoves.Instance.CalculateAllMoves();
        GameManager.Instance.AssignFirstTurnWhite();
    }
}
