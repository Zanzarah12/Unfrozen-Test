using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TurnSequence))]
[RequireComponent(typeof(ZoomViewDuel))]
public class ActionController : MonoBehaviour
{
    public static Ability currentAbility;
    public static Fighter currentTarget;
    public static Fighter currentAttacker;

    public static bool playerIsOnTheLeftSide;
    public static bool playersTurn;
    public static bool actionInProgress;

    static ActionController instance;

    [SerializeField] Transform resultTextParent;
    [SerializeField] UIAttackResult damageTextPrefab;
    [SerializeField] UIAttackResult missTextPrefab;

    TurnSequence turnSequence;
    ZoomViewDuel zoomViewDuel;

    private void Awake()
    {
        turnSequence = GetComponent<TurnSequence>();
        zoomViewDuel = FindObjectOfType<ZoomViewDuel>();
        instance = this;
    }

    public static void AttackTarget()
    {
        if (currentAbility && currentTarget && currentAttacker)
        {
            instance.StartAction();
        }    
    }

    public static void ShowDamageText()
    {
        instance.SpawnDamageText();
    }

    public static void ShowMissText()
    {
        instance.SpawnMissText();
    }

    private void StartAction()
    {
        playersTurn = currentTarget.IsEnemy() ? true : false;
        StartCoroutine(PerformAction());
    }

    IEnumerator PerformAction()
    {
        yield return zoomViewDuel.StartDuel();

        yield return currentAttacker.TryAttack();

        yield return new WaitForSeconds(0.5f);

        yield return zoomViewDuel.FinishDuel(); 
        
        yield return currentTarget.CheckForDeath();

        if (currentAbility.GetPushAmount() != 0 && currentTarget.GetCurrentHealthPercentage() != 0)
            yield return currentTarget.TryPush();

        yield return new WaitForSeconds(1f);
       
        EndAttack();
        turnSequence.SetNextFighter();
    }

    private void EndAttack()
    {
        actionInProgress = false;

        currentAttacker.ResetStateToIdle();
        currentTarget.ResetStateToIdle();

        currentAbility = null;
        currentTarget = null;       
    }  

    void SpawnDamageText()
    {
        UIAttackResult newText = Instantiate(damageTextPrefab, resultTextParent);
        newText.SetText(currentAbility.GetDamage().ToString());
        newText.gameObject.SetActive(true);
    }   

    void SpawnMissText()
    {
        UIAttackResult newText = Instantiate(missTextPrefab, resultTextParent);
        newText.gameObject.SetActive(true);
    }
}