using UnityEngine;
using UnityEngine.UI;

public class TutorialPopup : MonoBehaviour
{
    [SerializeField] private Button closeButton;

    private CanvasGroup panelCanvasGroup;

    private void Awake()
    {
        Debug.Log("TutorialMenuParent active: " + gameObject.activeSelf);

        CanvasGroup[] all = GetComponentsInChildren<CanvasGroup>(true);
        Debug.Log("Total CanvasGroups found including inactive: " + all.Length);
        foreach (CanvasGroup cg in all)
            Debug.Log("Found CanvasGroup on: " + cg.gameObject.name);

        panelCanvasGroup = GetComponentInChildren<CanvasGroup>(true);

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
        }
        else
        {
            Debug.LogError("TutorialPopup: No CanvasGroup found in children!");
        }

        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);
    }

    public void Show()
    {
        Debug.Log("TutorialPopup.Show called");
        
        if (panelCanvasGroup == null)
        {
            CanvasGroup[] all = FindObjectsOfType<CanvasGroup>(true);
            foreach (CanvasGroup cg in all)
            {
                Debug.Log("Found CanvasGroup on: " + cg.gameObject.name);
                if (cg.gameObject.name == "TutorialMenu")
                {
                    panelCanvasGroup = cg;
                    break;
                }
            }
        }

        if (panelCanvasGroup == null)
        {
            Debug.LogError("Still null!");
            return;
        }

        panelCanvasGroup.alpha = 1f;
        panelCanvasGroup.interactable = true;
        panelCanvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        if (panelCanvasGroup == null) return;

        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;
    }
}