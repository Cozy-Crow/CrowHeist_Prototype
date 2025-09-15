using Unity.VisualScripting;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private GameObject hanoiTable;
    [SerializeField] private GameObject largeDisc;
    [SerializeField] private GameObject mediumDisc;
    [SerializeField] private GameObject smallDisc;
    [SerializeField] private GameObject coin;

    private bool puzzleCompleted = false;

    void Update()
    {
        if (!puzzleCompleted && IsDiscsStackedCorrectly())
        {
            puzzleCompleted = true;
            Debug.Log("Puzzle completed true");
            OnPuzzleComplete();
        }
    }

    bool IsDiscsStackedCorrectly()
    {
        Collider largeCol = largeDisc.GetComponent<Collider>();
        Collider mediumCol = mediumDisc.GetComponent<Collider>();
        Collider smallCol = smallDisc.GetComponent<Collider>();
        Collider tableCol = hanoiTable.GetComponent<Collider>();

        if (largeCol == null || mediumCol == null || smallCol == null || tableCol == null)
        {
            Debug.LogWarning("Missing Collider");
            return false;
        }

        bool isLargeOnTable = largeCol.bounds.Intersects(tableCol.bounds);
        bool isMediumOnLarge = mediumCol.bounds.Intersects(largeCol.bounds);
        bool isSmallOnMedium = smallCol.bounds.Intersects(mediumCol.bounds);

        return isLargeOnTable && isMediumOnLarge && isSmallOnMedium;
    }

    void OnPuzzleComplete()
    {
        Transform coinSpawnpoint = smallDisc.GetComponent<Transform>();
        coinSpawnpoint.position = new Vector3(coinSpawnpoint.position.x, coinSpawnpoint.position.y+10, coinSpawnpoint.position.z);
        
        Debug.Log("Spawn Called");
        Instantiate(coin, coinSpawnpoint);
        

    }
}
