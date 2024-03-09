using UnityEngine;
using UnityEngine.EventSystems;

public class NPC1QuestHandler1 : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Vector3 moveDirection = new Vector3(1, 0, 0); // The direction NPC will move towards.
    public QuestManager questManager;
    public QuestInfoDisplay questInfoDisplay;
    public Quest questTemplateForNPC1; // Assign NPC1's quest template
    public Quest questTemplateForNPC2; // Assign NPC2's quest template

    private bool shouldMove = false;

    private void Update()
    {
        if (shouldMove)
        {
            MoveNPC();
        }
    }

    private void MoveNPC()
    {
        transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
    }

    public void AssignQuestButton()
    {

        if (gameObject.name == "NPC1")
        {
            AssignQuestToNPC(questTemplateForNPC1, gameObject.name);
        }
        else if (gameObject.name == "NPC2")
        {
            AssignQuestToNPC(questTemplateForNPC2, gameObject.name);
        }
    }

    public void AssignQuestToNPC(Quest quest, string npcName)
    {
        questManager.AssignQuest(quest, npcName);
        Debug.Log($"Assigned quest to collect: {quest.requiredAmount} woods for {npcName}");
        shouldMove = true;

        questInfoDisplay.UpdateAssignedNPCs(npcName, quest, true);
    }

}