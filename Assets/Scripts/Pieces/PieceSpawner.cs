using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PieceSpawner : MonoBehaviourPunCallbacks
{
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

        SpawnPiecesDefault();
        _spawned = true;
    }

    private void SpawnPiecesDefault()
    {
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
