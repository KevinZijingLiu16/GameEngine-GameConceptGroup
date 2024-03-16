using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    // Static instance to make the QuestManager accessible from anywhere
    public static QuestManager Instance { get; private set; }

    // Use a dictionary to store quests for each NPC
    private Dictionary<string, List<Quest>> npcQuests = new Dictionary<string, List<Quest>>();

    private void Awake()
    {
        // Set the static instance to this script when it's created
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // If an instance already exists, destroy this duplicate
            Destroy(gameObject);
        }
    }

    public void AssignQuest(Quest quest, string npcName)
    {
        // If the NPC is not in the dictionary, add an entry
        if (!npcQuests.ContainsKey(npcName))
        {
            npcQuests[npcName] = new List<Quest>();
        }

        // Add the quest to the NPC's list
        npcQuests[npcName].Add(quest);
        Debug.Log($"Quest assigned to {npcName}: {quest.description}");
    }

    public void UpdateQuest(string npcName, string questDescription, int amount)
    {
        // Check if the NPC has a list of quests
        if (npcQuests.TryGetValue(npcName, out List<Quest> npcQuestList))
        {
            // Iterate through the quests for the specific NPC
            foreach (var quest in npcQuestList)
            {
                if (quest.description == questDescription && !quest.isComplete)
                {
                    quest.currentAmount += amount;

                    if (quest.isComplete)
                    {
                        Debug.Log($"Quest completed for {npcName}: {quest.description}");
                    }

                    break;
                }
            }
        }
    }
}

