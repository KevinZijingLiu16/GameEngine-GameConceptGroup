using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using AE0672;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button option1Button;
    [SerializeField] private Button option2Button;

    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float turnSpeed = 2f;

    private List<DialogueString> dialogueList;


    [Header("Player")]
    [SerializeField] private BasicMovement playerMovement;
    private Transform playerCamera;

    private int currentDialogueIndex = 0;



    private void Start()
    {
        dialogueParent.SetActive(false);
        playerCamera = Camera.main.transform;
    }

    public void DialogueStart(List<DialogueString> textToPrint, Transform NPC)
    {
        
        dialogueParent.SetActive(true);
        playerMovement.enabled = false;

        Cursor.lockState = CursorLockMode.None; // unlock the cursor
        Cursor.visible = true;

        StartCoroutine(TurnCameraTowardsNPC(NPC) );

        dialogueList = textToPrint;
        currentDialogueIndex = 0;

        DisableButtons();

        StartCoroutine(PrintDialogue() );


    }

    private void DisableButtons()
    {
        option1Button.interactable = false;
        option2Button.interactable = false;

        option1Button.GetComponentInChildren<TMP_Text>().text = "No Option";
        option2Button.GetComponentInChildren<TMP_Text>().text = "No Option";
    }

    private IEnumerator TurnCameraTowardsNPC(Transform NPC)
    {
        Quaternion originalRotation = playerCamera.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(NPC.position - playerCamera.position) ;

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            playerCamera.rotation = Quaternion.Slerp(originalRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime * turnSpeed;
            yield return null;
        }

        playerCamera.rotation = targetRotation;
    }

    private bool optionSelected = false;
    private IEnumerator PrintDialogue()
    {
        while(currentDialogueIndex < dialogueList.Count)
        {
            DialogueString line = dialogueList[currentDialogueIndex];

            line.startDialogueEvent?.Invoke();

           if( line.isQustion)
            {
                yield return StartCoroutine(TypeText(line.text));
                option1Button.interactable = true;
                option2Button.interactable = true;

                option1Button.GetComponentInChildren<TMP_Text>().text = line.answer1;
                option2Button.GetComponentInChildren<TMP_Text>().text = line.answer2;

                option1Button.onClick.AddListener( () => HandleOptionSelected(line.option1IndexJump) );
                option2Button.onClick.AddListener( () => HandleOptionSelected(line.option2IndexJump) );

                yield return new WaitUntil( () => optionSelected);
            }
           else
            {
               yield return StartCoroutine(TypeText(line.text) );
            }

           line.endDialogueEvent?.Invoke();

            optionSelected = false;
           
        }

        DialogueStop();

    }

    private void HandleOptionSelected(int indexJump)
    {
        optionSelected = true;
        DisableButtons();

        option1Button.onClick.RemoveListener(() => HandleOptionSelected(dialogueList[currentDialogueIndex].option1IndexJump));
        option2Button.onClick.RemoveListener(() => HandleOptionSelected(dialogueList[currentDialogueIndex].option2IndexJump));

        currentDialogueIndex = indexJump;
    }


    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // If it's not a question, wait for a mouse click to continue.
        if (!dialogueList[currentDialogueIndex].isQustion)
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        // Check if the current dialogue is the end before incrementing the index.
        if (dialogueList[currentDialogueIndex].isEnd)
        {
            DialogueStop();
            yield break; // Exit the coroutine
        }

        // Increment index only after all checks are done.
        currentDialogueIndex++;
    }


    private void DialogueStop()
    {
        StopAllCoroutines();
        dialogueText.text = "";
        dialogueParent.SetActive(false);
        playerMovement.enabled = true;

        // Reset the cursor state.
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        // Unregister the button listeners to prevent them from stacking up.
        option1Button.onClick.RemoveAllListeners();
        option2Button.onClick.RemoveAllListeners();
    }



}
