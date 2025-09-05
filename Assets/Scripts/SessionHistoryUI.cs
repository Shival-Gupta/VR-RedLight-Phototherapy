using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionHistoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform contentContainer;
    [SerializeField] private GameObject historyItemPrefab;

    private void Start()
    {
        PopulateHistory();
    }
    public void LoadMenuScene()
    {
        string menuScene = GlobalConfig.SceneMainMenu;
        if (SceneManager.GetActiveScene().name != menuScene)
        {
            SceneController.LoadScene(menuScene);
        }
    }
    private void PopulateHistory()
    {
        if (HistoryManager.Instance == null)
        {
            Debug.Log("[SessionHistoryUI] No History Manager found.");
            return;
        }
        if (HistoryManager.Instance.GetHistory().Count == 0)
        {
            Debug.Log("[SessionHistoryUI] No session history found. count is 0.");
            return;
        }

        List<HistoryManager.SessionEntry> history = HistoryManager.Instance.GetHistory();

        foreach (var entry in history)
        {
            GameObject item = Instantiate(historyItemPrefab, contentContainer);
            SessionHistoryItemUI ui = item.GetComponent<SessionHistoryItemUI>();
            if (ui != null)
            {
                ui.SetData(entry);
            }
        }
    }
}
