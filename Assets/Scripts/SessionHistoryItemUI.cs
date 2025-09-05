using TMPro;
using UnityEngine;

public class SessionHistoryItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text userText;
    [SerializeField] private TMP_Text patternText;
    [SerializeField] private TMP_Text startText;
    [SerializeField] private TMP_Text endText;

    public void SetData(HistoryManager.SessionEntry entry)
    {
        userText.text = entry.userId;
        patternText.text = entry.pattern;
        startText.text = entry.startTime;
        endText.text = entry.endTime;
    }
}
