using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSequence : MonoBehaviour
{
    public event Action<Fighter> onNewFighterSelected;

    [SerializeField] List<Fighter> allFighters;
    [SerializeField] Fighter currentFighter;

    private void Start()
    {
        Invoke("StartFight", 1f);
    }

    public void StartFight()
    {
        if (currentFighter != null)
            return; // fight has already started

        allFighters.AddRange(FindObjectsOfType<Fighter>());
        allFighters.Shuffle();

        if (allFighters.Count == 0)
        {
            Debug.LogError("There are no fighters on the scene");
            return;
        }

        SetNextFighter();
    }

    public void SetNextFighter()
    {       
        int index = allFighters.IndexOf(currentFighter);
        
        if (index == allFighters.Count - 1 || index == -1)
            currentFighter = allFighters[0];
        else
            currentFighter = allFighters[index + 1];

        ActionController.currentAttacker = currentFighter;

        onNewFighterSelected(currentFighter);      
    }

    public void RemoveFighter(Fighter fighter)
    {
        allFighters.Remove(fighter);
    }

    int SortByInitiative(Fighter f1, Fighter f2)
    {
        return f1.GetInitiative().CompareTo(f2.GetInitiative());
    }
}
