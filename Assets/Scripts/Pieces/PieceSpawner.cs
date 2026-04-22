using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PieceSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _referenceTile;
    [SerializeField] private Transform _pieceContainer;
    [SerializeField] private PieceData[] _piecesData; // Array containing the scriptable object of each piece. Set in the inspector
    [SerializeField] private TextMeshProUGUI _currentTurnTMP;

    private bool spawned = false;

    IEnumerator Start()
    {
        var turnObj = GameObject.Find("TurnInfoText");
        if (turnObj != null) _currentTurnTMP = turnObj.GetComponent<TextMeshProUGUI>();

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

    // Note: Move out of Update asap
    void Update()
    {
        var props = PhotonNetwork.CurrentRoom?.CustomProperties;
        if (_currentTurnTMP != null && props != null && props.ContainsKey("Turn") && props["Turn"] != null)
        {
            _currentTurnTMP.text = $"Turn: {props["Turn"]}";
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
        GameManager.Instance.SetGameStateNetwork(GameManager.GameState.InGame);
        ExitGames.Client.Photon.Hashtable props = new()
        {
            { "GameState", GameManager.GameState.InGame.ToString() }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        GameManager.Instance.PiecesAreSpawned = true;

        CalculateMoves.Instance.CalculateAllMoves();
        GameManager.AssignFirstTurnWhite();
    }
}
