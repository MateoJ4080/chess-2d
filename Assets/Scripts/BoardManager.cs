using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private bool boardIsInverted;

    public bool BoardIsInverted
    {
        get => boardIsInverted;
        set => boardIsInverted = value;
    }

    void Start()
    {
        // GenerateBoard();
    }
}
