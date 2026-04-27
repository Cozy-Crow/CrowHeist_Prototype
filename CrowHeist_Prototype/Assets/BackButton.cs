using UnityEngine;

public class BackButton : MonoBehaviour
{
    public void OnBackButtonPressed()
    {
        gameObject.SetActive(false);
    }
}