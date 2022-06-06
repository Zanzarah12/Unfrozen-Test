using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hero", menuName = "Hero", order = 0)]
public class Unit : ScriptableObject
{
    [SerializeField] Fighter unitFighterPrefab;

    [Header("Unit Stats")]
    [SerializeField] int maxHealth;
    [Range(0f, 100f)]
    [SerializeField] int initiative;
    [Range(0f, 100f)]
    [SerializeField] int dodgeChance;

    [Header("Unit UI elements")]
    [SerializeField] UIFighterHUD unitHUDPrefab;
    [SerializeField] Ability[] abilities;

    public Fighter GetFighter() => unitFighterPrefab;
    public UIFighterHUD GetFighterHUD() => unitHUDPrefab;
    public Ability[] GetFighterAbilities() => abilities;

    public int GetMaxHealth() => maxHealth;
    public int GetInitiative() => initiative;
    public int GetDodgeChance() => dodgeChance;
}
