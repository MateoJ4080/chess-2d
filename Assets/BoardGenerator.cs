using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    public GameObject squarePrefab;           // Square sprite with SpriteRenderer
    public Transform boardContainer;          // Empty GameObject named "Board"
    public Color lightColor = Color.white;
    public Color darkColor = Color.gray;

    public int boardWidth = 8;
    public int boardHeight = 8;

    void Start()
    {
        GenerateBoard();
    }

    void GenerateBoard()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                GameObject square = Instantiate(squarePrefab, boardContainer);
                square.name = $"Square_{x}_{y}";
                square.transform.localPosition = new Vector3(x, y, 0);

                var sr = square.GetComponent<SpriteRenderer>();
                bool isLight = (x + y) % 2 == 0;
                sr.color = isLight ? lightColor : darkColor;
            }
        }
        // "0.5f" as offset to center the board, because the tiles are positioned by their left-bottom corner
        boardContainer.transform.position = new Vector2(0.5f + -boardWidth / 2, 0.5f + -boardHeight / 2);
    }
}
