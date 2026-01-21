using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AltWinCoinsUI : MonoBehaviour
{
    [SerializeField] private List<Sprite> coinsSprites = new List<Sprite>();

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateScoreUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScoreUI()
    {
        if (GameManager.AltCoinsScore > coinsSprites.Count)
        {
            Debug.Log("Not enough score sprites");
            return;
        }
        
        image.sprite = coinsSprites[GameManager.AltCoinsScore];
    }
}
