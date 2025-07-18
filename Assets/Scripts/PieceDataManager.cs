using UnityEngine;

[CreateAssetMenu(fileName = "PieceDataManager", menuName = "Chess/PieceDataManager")]
public class PieceDataManager : ScriptableObject
{
    public PieceData[] allPieceData;

    private static PieceDataManager _instance;
    public static PieceDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Load from resources because it's not referenced
                _instance = Resources.Load<PieceDataManager>("PieceDataManager");
                if (_instance == null)
                {
                    Debug.LogError("PieceDataManager asset not found in Resources! Make sure it's in a 'Resources' folder.");
                }
            }
            return _instance;
        }
    }

    public PieceData GetPieceDataByName(string name)
    {
        if (allPieceData == null)
        {
            Debug.LogError("allPieceData is null in PieceDataManager!");
            return null;
        }

        foreach (var pd in allPieceData)
        {
            if (pd != null && pd.name == name)
                return pd;
        }

        Debug.LogWarning($"PieceData with name '{name}' not found!");
        return null;
    }

    // Method to manually initialize if necessary
    public static void Initialize()
    {
        if (_instance == null)
        {
            var _ = Instance;
        }
    }
}