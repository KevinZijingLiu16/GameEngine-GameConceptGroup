using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestButtonTrigger : MonoBehaviour
{
    public GameObject questButton; 

    private void Start()
    {
        questButton.SetActive(false); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            questButton.SetActive(true); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            questButton.SetActive(false); 
        }
    }
}
