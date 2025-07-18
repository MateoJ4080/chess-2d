using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PieceSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _referenceTile;
    [SerializeField] private Transform _pieceContainer;
    [SerializeField] BoardManager _boardManager;
    [SerializeField] private PieceData[] _piecesData;      // Array containing the scriptable object of each piece. Set in the inspector

    private bool spawned = false;

    IEnumerator Start()
    {
        PieceDataManager.Initialize();

        if (PhotonNetwork.IsMasterClient) // Avoids both clients assigning colors
            PlayerManager.AssignRandomColors();

        yield return new WaitUntil(() =>
            PlayerManager.Instance != null &&
            PlayerManager.Instance.ColorsAreAssigned);

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Not master client, waiting for pieces to be spawned by master client...");
            yield break; // Non-master clients will wait for the master client to spawn pieces
        }
        TrySpawnPieces();
    }

    void TrySpawnPieces()
    {
        if (spawned)
        {
            Debug.Log("Boolean spawned is already in true");
            return;
        }

        if (!PlayerManager.Instance.ColorsAreAssigned)
        {
            Debug.LogError("Player colors are not assigned yet! Cannot spawn pieces.");
            return;
        }

        if (PieceDataManager.Instance == null)
        {
            Debug.LogError("PieceDataManager.Instance is null! Cannot spawn pieces");
            return;
        }

        SpawnPieces();
        spawned = true;
    }

    public void SpawnPieces()
    {
        Debug.Log("Spawning pieces...");
        foreach (var pieceData in _piecesData)
        {
            foreach (var pos in pieceData.InitialPositions)
            {
                PhotonNetwork.Instantiate
                (
                    $"Prefabs/Pieces/{pieceData.name}",
                    Vector3.zero,
                    Quaternion.identity,
                    0,
                    new object[] { pieceData.name, pos.x, pos.y } // used in PieceSetup
                );
            }
        }
    }
}
