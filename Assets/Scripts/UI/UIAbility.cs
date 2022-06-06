using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAbility : MonoBehaviour
{
    Ability ability;

    public Ability GetAbility() => ability;

    public void SetAbility(Ability newAbility)
    {
        ability = newAbility;
    }

    public void OnClick()
    {
        ActionController.currentAbility = ability;
    }   
}
