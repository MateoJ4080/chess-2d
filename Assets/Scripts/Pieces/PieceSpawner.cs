using System.Collections;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PieceSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _referenceTile;
    [SerializeField] private Transform _pieceContainer;
    [SerializeField] BoardManager _boardManager;
    [SerializeField] private PieceData[] _piecesData; // Array containing the scriptable object of each piece. Set in the inspector
    [SerializeField] private TextMeshProUGUI currentTurnTMP;

    private bool spawned = false;

    IEnumerator Start()
    {
        currentTurnTMP = GameObject.FindGameObjectWithTag("TurnInfoText").GetComponent<TextMeshProUGUI>(); ;

        PieceDataManager.Initialize();

        // Avoid both clients assigning colors
        if (PhotonNetwork.IsMasterClient)
        {
            PlayerManager.AssignRandomColors();
        }

        yield return new WaitUntil(() =>
            PlayerManager.Instance != null &&
            PlayerManager.Instance.CheckColorsAssigned());

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Not master client, waiting for pieces to be spawned by master client...");
            yield break; // Non-master clients will wait for the master client to spawn pieces
        }
        TrySpawnPieces();
    }

    void Update()
    {
        if (currentTurnTMP == null)
        {
            Debug.LogError("currentTurnTMP is null");
            return;
        }

        var props = PhotonNetwork.CurrentRoom?.CustomProperties;
        if (props != null && props.ContainsKey("Turn") && props["Turn"] != null)
        {
            currentTurnTMP.text = $"Turn: {props["Turn"]}";
        }
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
        GameManager.Instance.PiecesAreSpawned = true;
        GameManager.Instance.UpdateGameState(GameManager.GameState.InGame);

        CalculateMoves.Instance.CalculateAllMoves();
        GameManager.AssignFirstTurnWhite();
    }
}
