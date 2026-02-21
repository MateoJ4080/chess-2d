using System;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    public enum PlayerColor
    {
        White,
        Black
    }

    public enum GameState
    {
        MainMenu,
        Loading,
        InGame,
        GameOver
    }

    public PlayerColor currentTurn { get; private set; }
    public GameState state;

    private bool piecesAreSpawned = false;
    public bool PiecesAreSpawned
    {
        get => piecesAreSpawned;
        set => piecesAreSpawned = value;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (state)
        {
            case GameState.MainMenu:
                break;
            case GameState.Loading:
                break;
            case GameState.InGame:
                break;
            case GameState.GameOver:
                break;
        }
    }

    public override void OnCreatedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        var props = new ExitGames.Client.Photon.Hashtable {
            {"whiteCK", true},
            {"whiteCQ", true},
            {"blackCK", true},
            {"blackCQ", true}
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    public void OnPressPlay()
    {
        Instance.state = GameState.Loading;
    }

    public void OnPieceMovedBySelf(GameObject piece, Vector2Int from)
    {
        var data = piece.GetComponent<ChessPiece>().PieceData;

        if (data.PieceType == "King")
        {
            DisableCastling(data.IsWhite);
        }
        if (data.PieceType == "Rook")
        {
            if (data.IsWhite && from == new Vector2Int(7, 0)) DisableRookSide(PieceData.RookSide.King, data.IsWhite);
            if (data.IsWhite && from == new Vector2Int(0, 0)) DisableRookSide(PieceData.RookSide.Queen, data.IsWhite);

            if (!data.IsWhite && from == new Vector2Int(7, 0)) DisableRookSide(PieceData.RookSide.Queen, data.IsWhite);
            if (!data.IsWhite && from == new Vector2Int(0, 0)) DisableRookSide(PieceData.RookSide.King, data.IsWhite);
        }
    }

    void DisableCastling(bool isWhite)
    {
        var p = new ExitGames.Client.Photon.Hashtable();

        if (isWhite)
        {
            p["whiteCK"] = false;
            p["whiteCQ"] = false;
        }

        if (!isWhite)
        {
            p["blackCK"] = false;
            p["blackCQ"] = false;
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(p);
    }

    void DisableRookSide(PieceData.RookSide side, bool isWhite)
    {
        var p = new ExitGames.Client.Photon.Hashtable();

        if (isWhite) p[side == PieceData.RookSide.King ? "whiteCK" : "whiteCQ"] = false;
        else p[side == PieceData.RookSide.King ? "blackCK" : "blackCQ"] = false;

        PhotonNetwork.CurrentRoom.SetCustomProperties(p);
    }

    // Handle turns
    public bool ItsMyTurn()
    {
        return PhotonNetwork.CurrentRoom.CustomProperties["Turn"] as string == PhotonNetwork.LocalPlayer.CustomProperties["Color"] as string;
    }

    public void SwitchTurn()
    {
        currentTurn = PhotonNetwork.CurrentRoom.CustomProperties["Turn"] as string == PlayerColor.White.ToString() ? PlayerColor.Black : PlayerColor.White;
        Debug.Log($"<color=yellow>Switching turn to {currentTurn}");

        // Assign new turn to room properties
        ExitGames.Client.Photon.Hashtable turnProps = new() { { "Turn", currentTurn.ToString() } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);
    }

    public static void AssignFirstTurnWhite()
    {
        ExitGames.Client.Photon.Hashtable turnProps = new() { { "Turn", "White" } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);
    }

    public bool CanCastle(PieceData.RookSide side, GameObject pieceGO)
    {
        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        Vector2Int piecePos = Vector2Int.RoundToInt(pieceGO.transform.position);

        // These variables are turn to false when a king or a rook moves
        bool whiteCK = props.ContainsKey("whiteCK") && (bool)props["whiteCK"];
        bool whiteCQ = props.ContainsKey("whiteCQ") && (bool)props["whiteCQ"];
        bool blackCK = props.ContainsKey("blackCK") && (bool)props["blackCK"];
        bool blackCQ = props.ContainsKey("blackCQ") && (bool)props["blackCQ"];

        var data = pieceGO.GetComponent<ChessPiece>().PieceData;
        bool isWhite = data.IsWhite;

        if (isWhite)
        {
            if (side == PieceData.RookSide.King)
            {
                Vector2Int firstTile = piecePos + new Vector2Int(1, 0);
                Vector2Int secondTile = piecePos + new Vector2Int(2, 0);

                bool castlePathIsThreatened = BoardState.SquareIsThreatened(firstTile, pieceGO) && BoardState.SquareIsThreatened(secondTile, pieceGO);
                bool squaresAreEmpty = BoardUtils.SquareIsEmpty(firstTile) && BoardUtils.SquareIsEmpty(secondTile);

                return !castlePathIsThreatened && squaresAreEmpty && whiteCK;
            }
            if (side == PieceData.RookSide.Queen)
            {

                Vector2Int firstTile = piecePos + new Vector2Int(-1, 0);
                Vector2Int secondTile = piecePos + new Vector2Int(-2, 0);

                bool squaresAreThreatened = BoardState.SquareIsThreatened(firstTile, pieceGO) && BoardState.SquareIsThreatened(secondTile, pieceGO);
                bool squaresAreEmpty = BoardUtils.SquareIsEmpty(firstTile) && BoardUtils.SquareIsEmpty(secondTile);

                return !squaresAreThreatened && squaresAreEmpty && whiteCQ;
            }
        }

        if (!isWhite)
        {
            if (side == PieceData.RookSide.King)
            {
                Vector2Int firstTile = piecePos + new Vector2Int(-1, 0);
                Vector2Int secondTile = piecePos + new Vector2Int(-2, 0);

                bool squaresAreThreatened = BoardState.SquareIsThreatened(firstTile, pieceGO) && BoardState.SquareIsThreatened(secondTile, pieceGO);
                bool squaresAreEmpty = BoardUtils.SquareIsEmpty(firstTile) && BoardUtils.SquareIsEmpty(secondTile);

                return !squaresAreThreatened && squaresAreEmpty && blackCK;
            }

            if (side == PieceData.RookSide.Queen)
            {

                Vector2Int firstTile = piecePos + new Vector2Int(1, 0);
                Vector2Int secondTile = piecePos + new Vector2Int(2, 0);

                bool squaresAreThreatened = BoardState.SquareIsThreatened(firstTile, pieceGO) && BoardState.SquareIsThreatened(secondTile, pieceGO);
                bool squaresAreEmpty = BoardUtils.SquareIsEmpty(firstTile) && BoardUtils.SquareIsEmpty(secondTile);

                return !squaresAreThreatened && squaresAreEmpty && blackCQ;
            }
        }

        return false;
    }
}
