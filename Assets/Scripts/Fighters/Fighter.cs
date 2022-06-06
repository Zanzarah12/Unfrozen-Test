using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public event Action onHealthUpdate;
    public event Action<bool> onMouseOver;
    public event Action<bool> onMouseExit;

    public SpineAnimationController spineAnimationController;

    [SerializeField] bool isEnemy;

    [Header("Unit stats (DEBUG)")]
    [SerializeField] int maxHealth;
    [SerializeField] int currentHealth;
    [SerializeField] int initiative;
    [SerializeField] int dodgeChance;

    [Header("Additional current stats (DEBUG)")]   
    [SerializeField] GameObject HUD;
    [SerializeField] GameObject abilities;
    [SerializeField] int lineUpIndex;

    UIFighterHUD uIFighterHUD;

    Animation animation;
    TurnSequence turnSequence;
    bool dodged = false;

    public int GetInitiative() => initiative;
    public bool IsEnemy() => isEnemy;
    public int GetLineUpIndex() => lineUpIndex;

    public float GetCurrentHealthPercentage()
    {
        return (float)currentHealth / (float)maxHealth;
    }

    public void SetHUD(GameObject itsHUD)
    {
        HUD = itsHUD;
        uIFighterHUD = HUD.GetComponent<UIFighterHUD>();
    }

    public void SetAbilitiesUIGameObject(GameObject itsAbilities)
    {
        abilities = itsAbilities;
    }

    public void SetEnemyStatus(bool enemy)
    {
        isEnemy = enemy;
    }

    public void SetLineUpIndex(int newIndex)
    {
        lineUpIndex = newIndex;
    }   

    private void Awake()
    {
        spineAnimationController = GetComponentInChildren<SpineAnimationController>();
        turnSequence = FindObjectOfType<TurnSequence>();
        animation = GetComponent<Animation>();
    }   

    private void Start()
    {
        currentHealth = maxHealth;
        ResetStateToIdle();
    }

    private void OnEnable()
    {
        turnSequence.onNewFighterSelected += NextTurn;
    }

    private void OnDisable()
    {
        turnSequence.onNewFighterSelected -= NextTurn;
    } 

    private void OnMouseOver()
    {
        if (ActionController.currentAttacker == this)
            return;

        if (ActionController.actionInProgress)
            return;

        if (isEnemy)
        {
            if (ActionController.currentAbility == null)
                return;

            if (IsAppropriateDistance())
            {
                onMouseOver?.Invoke(true);
            }
        }
    }

    private void OnMouseExit()
    {
        if (ActionController.currentAttacker == this)
            return;

        onMouseExit?.Invoke(false);
    }

    private void OnMouseDown()
    {
        if (ActionController.currentAbility == null)
            return;

        if (ActionController.actionInProgress)
            return;

        if (!isEnemy)
            return;

        if (!IsAppropriateDistance())
            return;

        ActionController.currentTarget = this;
        ActionController.actionInProgress = true;
        ActionController.AttackTarget();
    }

    public void ResetStateToIdle()
    {
        spineAnimationController.PlayIdle();
        dodged = false;
    }

    public void SetupFighterStats(Unit unit)
    {
        maxHealth = unit.GetMaxHealth();
        initiative = unit.GetInitiative();
        dodgeChance = unit.GetDodgeChance();
    }

    public IEnumerator TryAttack()
    {
        yield return spineAnimationController.PlayAnimation(ActionController.currentAbility.GetAnimationReference());
    }

	public void TryTakeDamage(int damageAmount)
	{
        dodged = TryDodge();

        if (dodged)
        {
            ActionController.ShowMissText();
            return;
        }

        if (ActionController.currentAbility.GetPushAmount() != 0)
        {
            spineAnimationController.PlayGrab();
        }
        else
            spineAnimationController.PlayTakeDamage();

        ActionController.ShowDamageText();

        currentHealth = (int)Mathf.Clamp(currentHealth - damageAmount, 0f, maxHealth);

        onHealthUpdate?.Invoke();
	}

    private bool TryDodge()
    {
        int shot = UnityEngine.Random.Range(1, 101); // percents
        return shot <= dodgeChance ? true : false;
    }

    public IEnumerator TryPush()
    {
        if (dodged)
            yield return null;
        else
            yield return BeingPulled(ActionController.currentAbility.GetPushAmount());
    }

    public IEnumerator CheckForDeath()
    {
        if (currentHealth > 0)
            yield break;

        Destroy(uIFighterHUD.gameObject);

        animation.Play("Death");
        yield return BeingPulled(10);

        yield return new WaitForSeconds(animation["Death"].length + 1f);

        turnSequence.RemoveFighter(this);
        FightersSpawner.RemoveFighter(this, isEnemy);      
        Destroy(gameObject);
    }

    private bool IsAppropriateDistance()
    {
        foreach (int possibleTargetIndex in ActionController.currentAbility.GetPossibleTargets())
        {
            if (lineUpIndex == possibleTargetIndex)
                return true;
        }

        return false;
    }

    private void NextTurn(Fighter nextFighter)
    {
        if (nextFighter == this)
        {
            uIFighterHUD.SetActiveSelection();

            if (isEnemy)
            {
                ActionController.currentAbility = GetRandomAbility();
                ActionController.currentTarget = FightersSpawner.GetRandomPlayerTarget(ActionController.currentAbility);
                ActionController.AttackTarget();
            }
            else
            {
                abilities.SetActive(true);
                onMouseOver?.Invoke(true);
            }
        }
        else
        {
            abilities.SetActive(false);
            onMouseOver?.Invoke(false);
        }
    }

    private IEnumerator BeingPulled(int amount)
    {
        List<Fighter> fightersTeam = isEnemy ? FightersSpawner.GetEnemyFighters() : FightersSpawner.GetPlayerFighters();

        int newIndex = lineUpIndex + amount;
        int oldIndex = lineUpIndex;
        newIndex = Mathf.Clamp(newIndex, 1, fightersTeam.Count);       

        Vector2 posToMove = new Vector2();

        if (oldIndex < newIndex)
        {
            for (int i = fightersTeam.Count - 1; i >= 0; i--)
            {
                if (fightersTeam[i].GetLineUpIndex() == newIndex)
                {
                    posToMove = fightersTeam[i].transform.position;
                    StartCoroutine(LerpPosition(fightersTeam[i].transform.parent, fightersTeam[i - 1].transform.position));
                    fightersTeam[i].SetLineUpIndex(fightersTeam[i - 1].GetLineUpIndex());
                }
                else if (fightersTeam[i].GetLineUpIndex() < newIndex && fightersTeam[i].GetLineUpIndex() > oldIndex)
                {
                    StartCoroutine(LerpPosition(fightersTeam[i].transform.parent, fightersTeam[i - 1].transform.position));
                    fightersTeam[i].SetLineUpIndex(fightersTeam[i - 1].GetLineUpIndex());
                }
                else if (fightersTeam[i].GetLineUpIndex() == oldIndex)
                {
                    StartCoroutine(LerpPosition(fightersTeam[i].transform.parent, posToMove));
                    fightersTeam[i].SetLineUpIndex(newIndex);
                    break;
                }
            }
        }
        else if (oldIndex > newIndex)
        {
            for (int i = 0; i < fightersTeam.Count; i++)
            {
                if (fightersTeam[i].GetLineUpIndex() == newIndex)
                {
                    posToMove = fightersTeam[i].transform.position;
                    StartCoroutine(LerpPosition(fightersTeam[i].transform.parent, fightersTeam[i + 1].transform.position));
                    fightersTeam[i].SetLineUpIndex(fightersTeam[i + 1].GetLineUpIndex());
                }
                else if (fightersTeam[i].GetLineUpIndex() > newIndex && fightersTeam[i].GetLineUpIndex() < oldIndex)
                {
                    StartCoroutine(LerpPosition(fightersTeam[i].transform.parent, fightersTeam[i + 1].transform.position));
                    fightersTeam[i].SetLineUpIndex(fightersTeam[i + 1].GetLineUpIndex());
                }
                else if (fightersTeam[i].GetLineUpIndex() == oldIndex)
                {
                    StartCoroutine(LerpPosition(fightersTeam[i].transform.parent, posToMove));
                    fightersTeam[i].SetLineUpIndex(newIndex);
                    break;
                }
            }
        }

        yield return null;
        FightersSpawner.SortTeamsByNewIndexes();
    } 

    private Ability GetRandomAbility()
    {
        UIAbility[] fighterAbilities = abilities.GetComponentsInChildren<UIAbility>();
        List<UIAbility> possibleToUseAbilities = new List<UIAbility>();
        List<Fighter> alivePlayerFighters = FightersSpawner.GetPlayerFighters();
        int[] possibleTargetsForAbility;

        foreach (UIAbility ab in fighterAbilities)
        {
            possibleTargetsForAbility = ab.GetAbility().GetPossibleTargets();

            foreach (Fighter f in alivePlayerFighters)
            {
                if (possibleTargetsForAbility.Contains(f.GetLineUpIndex()))
                {
                    possibleToUseAbilities.Add(ab);
                    break;
                }
            }

        }

        return fighterAbilities[UnityEngine.Random.Range(0, possibleToUseAbilities.Count)].GetAbility();
    }

    IEnumerator LerpPosition(Transform fighterTransform, Vector2 targetPosition)
    {
        float time = 0;
        Vector2 startPosition = fighterTransform.position;
        while (time < 0.5f)
        {
            fighterTransform.position = Vector2.Lerp(startPosition, targetPosition, time / 0.5f);
            time += Time.deltaTime;
            yield return null;
        }
        fighterTransform.position = targetPosition;
    }
}
