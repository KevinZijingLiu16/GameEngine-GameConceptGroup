using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class Goal
{
    public string Description { get; set; } // description of the goal. property.
    public bool Completed { get; set; } 

    public int CurrentAmount { get; set; } 

    public int RequiredAmount { get; set; } 

    public virtual void Initialize()
    {
        // default initialize function
    }

    public void Evaluate()
    {
        if (CurrentAmount >= RequiredAmount)
        {
            Complete();
        }
    }

    public void Complete()
    {
        Completed = true;
    }
}
