using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
public class QuestInfoDisplay : MonoBehaviour
{
    public GameObject questInfoPanel;
    public TMP_Text npcTextTemplate;
    public Button showQuestInfoButton;

    // change from a List to a Dictionary to include quest details
    private Dictionary<string, Quest> npcsWithQuest = new Dictionary<string, Quest>();

    private void Start()
    {
        questInfoPanel.SetActive(false); // hide the panel initially
        showQuestInfoButton.onClick.AddListener(ToggleQuestInfoPanel);
    }

    public void ToggleQuestInfoPanel()
    {
        questInfoPanel.SetActive(!questInfoPanel.activeSelf);
        UpdateQuestInfoPanel();
    }

    public void UpdateQuestInfoPanel()
    {
        
        foreach (Transform child in npcTextTemplate.transform.parent)
        {
            if (child != npcTextTemplate.transform) 
                Destroy(child.gameObject);
        }

        
        foreach (var entry in npcsWithQuest)
        {
            var npcText = Instantiate(npcTextTemplate, npcTextTemplate.transform.parent);
            npcText.text = $"{entry.Key} - {entry.Value.description} ({entry.Value.currentAmount}/{entry.Value.requiredAmount})";
            npcText.gameObject.SetActive(true);
        }
    }


    public void UpdateAssignedNPCs(string npcName, Quest quest, bool assigned)
    {
        if (assigned)
        {
            if (npcsWithQuest.ContainsKey(npcName))
            {
                npcsWithQuest[npcName] = quest;
            }
            else
            {
                npcsWithQuest.Add(npcName, quest);
            }
        }
        else
        {
            npcsWithQuest.Remove(npcName);
        }

        if (questInfoPanel.activeSelf) // Update the panel if it's currently visible
            UpdateQuestInfoPanel();
    }
}


