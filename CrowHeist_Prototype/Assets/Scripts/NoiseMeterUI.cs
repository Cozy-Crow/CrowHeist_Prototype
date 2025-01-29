using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoiseMeterUI : MonoBehaviour
{
    [SerializeField] private List<Sprite> _sound = new List<Sprite>();

    private int _noiseLevel = 0;
    private Image _noiseMeterImage;
    private void Awake()
    {
        _noiseMeterImage = GetComponent<Image>();
    }
    private void Start()
    {
        UpdateNoise(0);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) // Press the Up Arrow key to increase noise
        {
            _noiseLevel++;
            UpdateNoise(_noiseLevel);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) // Press the Down Arrow key to decrease noise
        {
            _noiseLevel = _noiseLevel - 1;
            UpdateNoise(_noiseLevel);
        }
    }

    public void UpdateNoise(int noise)
    {
        if (noise > _sound.Count)
        {
            Debug.LogError("Not enough noise sprites in the list");
            return;
        }

        _noiseMeterImage.sprite = _sound[_noiseLevel];
    }
}
