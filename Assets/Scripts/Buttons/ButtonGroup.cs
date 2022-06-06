using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonGroup : MonoBehaviour
{
    [SerializeField] List<AbilityButton> tabButtons;

    [SerializeField] Color colorIdle;
    [SerializeField] Color colorHover;
    [SerializeField] Color colorActive;

    [SerializeField] AbilityButton selectedAbility;

    [SerializeField] List<GameObject> objectsToSwap;

    private void OnEnable()
    {
        selectedAbility = null;
        ResetButtons();
    }   

    public void Subscribe(AbilityButton button)
    {
        if (tabButtons == null)
            tabButtons = new List<AbilityButton>();

        tabButtons.Add(button);
    }

    public void OnAbilityButtonEnter(AbilityButton button)
    {
        ResetButtons();

        if (button != selectedAbility)
            button.background.color = colorHover;
    }

    public void OnAbilityButtonExit(AbilityButton button)
    {
        ResetButtons();
    }

    public void OnAbilityButtonSelected(AbilityButton button)
    {
        if (selectedAbility)
            selectedAbility.Deselect();

        selectedAbility = button;
        selectedAbility.Select();

        ResetButtons();
        button.background.color = colorActive;

        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
                objectsToSwap[i].SetActive(true);
            else
                objectsToSwap[i].SetActive(false);
        }
    }

    public void ResetButtons()
    {
        foreach (AbilityButton button in tabButtons)
        {
            if (selectedAbility && button == selectedAbility)
                continue;

            button.background.color = colorIdle;
        }
    }
}
