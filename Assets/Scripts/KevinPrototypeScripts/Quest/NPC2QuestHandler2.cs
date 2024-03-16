using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC2QuestHandler2 : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public float flySpeed = 5f;

    private bool shouldRotate = false;
    private bool shouldFly = false;

    private void Update()
    {
        if (shouldRotate)
        {
            RotateNPC();
        }

        if (shouldFly)
        {
            FlyNPC();
        }
    }

    private void RotateNPC()
    {
        // Rotate the NPC around its up axis
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void FlyNPC()
    {
        // Move the NPC upward
        transform.Translate(Vector3.up * flySpeed * Time.deltaTime);
    }

    public void AssignQuestButton()
    {
        
        shouldRotate = true;
        shouldFly = true;

        // You can also assign the "Find Water" quest here if needed
        QuestManager.Instance.AssignQuest(new Quest()
        {
            description = "Find Water",
            requiredAmount = 1
        }, gameObject.name);
    }
}
