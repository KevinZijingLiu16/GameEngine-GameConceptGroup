using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private List<DialogueString> dialogueStrings = new List<DialogueString>();

    [SerializeField] private Transform NPCTransform; // move the camera to the NPC
    private bool hasSpoken = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasSpoken)
        {
            other.gameObject.GetComponent<DialogueManager>().DialogueStart(dialogueStrings, NPCTransform);
            hasSpoken = true;
        }
    }



}

[System.Serializable]
public class DialogueString
{
    public string text; // text of NPC dialogue
    public bool isEnd; //is the end of the dialogue


    [Header("Branching Dialogue")]
    public bool isQustion;
    public string answer1;
    public string answer2;
    public int option1IndexJump;
    public int option2IndexJump;


    [Header("trigger Event")]
    public UnityEvent startDialogueEvent;
    public UnityEvent endDialogueEvent;


}   
