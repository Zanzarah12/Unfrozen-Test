using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetup : MonoBehaviour
{
    //FighterUI[] allFightersUI = new FighterUI[8];
    //List<FighterHUD> allFightersUI = new List<FighterHUD>();

    [SerializeField] Transform fightersHUDContainer;
    [SerializeField] Transform fightersAbilitiesContainer;

    [SerializeField] GameObject abilitiesContainerPrefab;

    //public GameObject SetupFighterHUD(Fighter fighter)
    //{
    //    if (allFightersUI.Count == 0)
    //        FindUI();

    //    FighterUI newFighterUI = allFightersUI[0];        
    //    newFighterUI.Initialize(fighter);
    //    newFighterUI.gameObject.SetActive(true);

    //    allFightersUI.RemoveAt(0);
    //    return newFighterUI.gameObject;
    //}

    public GameObject SetupFighterHUD(Fighter fighter, UIFighterHUD fighterHUDPrefab)
    {
        UIFighterHUD newFighterHUD = Instantiate(fighterHUDPrefab, fightersHUDContainer.transform);
        newFighterHUD.Initialize(fighter);
        return newFighterHUD.gameObject;
    }

    public GameObject SetupAbilities(Fighter fighter, Ability[] abilities)
    {
        //GameObject abilitiesContainer = new GameObject();
        //RectTransform rectTransform = abilitiesContainer.AddComponent<RectTransform>();        
        //abilitiesContainer.transform.SetParent(fightersAbilitiesContainer);
        //rectTransform.sizeDelta = new Vector2(0f, 0f);

        GameObject abilitiesContainer = Instantiate(abilitiesContainerPrefab, fightersAbilitiesContainer);

        foreach (Ability abil in abilities)
        {
            GameObject newAbilityButton = Instantiate(abil.GetAbilityUIPrefab(), abilitiesContainer.transform);
            UIAbility itsUI = newAbilityButton.GetComponent<UIAbility>();
            if (itsUI)
                itsUI.SetAbility(abil);
        }

        abilitiesContainer.SetActive(false);

        return abilitiesContainer;

        //GameObject abilitiesGO = Instantiate(gameObject.em, fightersAbilitiesContainer.transform);
    }

    //private void FindUI()
    //{
    //    //allFightersUI.AddRange(GetComponentsInChildren<FighterUI>());

    //    foreach (Transform item in transform)
    //    {
    //        FighterHUD fui = item.GetComponent<FighterHUD>();
    //        if (fui != null)
    //            allFightersUI.Add(fui);
    //    }       

    //    if (allFightersUI.Count == 0)
    //        Debug.LogError("No UI found.");
    //}  
}
