using UnityEngine;
using UnityEngine.UI;

public class ControlsMenu : MonoBehaviour
{
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private Button backButton;

    private bool _openedBeforeStart = false;

    public bool IsOpen { get; private set; }

    private void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(CloseControls);

        if (controlsPanel != null && !_openedBeforeStart)
            controlsPanel.SetActive(false);
    }

    public void OpenControls()
    {
        _openedBeforeStart = true;
        if (controlsPanel != null)
            controlsPanel.SetActive(true);
        IsOpen = true;
    }

    public void CloseControls()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(false);
        IsOpen = false;
    }
}
