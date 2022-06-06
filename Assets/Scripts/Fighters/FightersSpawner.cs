using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FightersSpawner : MonoBehaviour
{
    [SerializeField] bool playerIsOnTheLeftSide = true;

    [SerializeField] Transform[] leftSpawnPoints = new Transform[4];
    [SerializeField] Transform[] rightSpawnPoints = new Transform[4];
    [SerializeField] Unit[] leftGuysToSpawn = new Unit[4];
    [SerializeField] Unit[] rightGuysToSpawn = new Unit[4];

    static List<Fighter> playerFighters = new List<Fighter>();
    static List<Fighter> enemyFighters = new List<Fighter>();

    UISetup uISetup;

    public static List<Fighter> GetPlayerFighters() => playerFighters;
    public static List<Fighter> GetEnemyFighters() => enemyFighters;

    public static void RemoveFighter(Fighter fighter, bool isEnemy)
    {
        if (isEnemy)
            enemyFighters.Remove(fighter);
        else
            playerFighters.Remove(fighter);
    }

    public static Fighter GetRandomPlayerTarget(Ability currentAbility)
    {
        List<Fighter> possibleTargets = new List<Fighter>();

        foreach (var playerFighter in playerFighters)
        {
            if (currentAbility.GetPossibleTargets().Contains(playerFighter.GetLineUpIndex()))
                possibleTargets.Add(playerFighter);
        }

        if (possibleTargets.Count == 0)
            return null;

        return possibleTargets[UnityEngine.Random.Range(0, possibleTargets.Count)];
    }

    public static void SortTeamsByNewIndexes()
    {
        SortByLineUpIndex(playerFighters);
        SortByLineUpIndex(enemyFighters);
    }

    private static void SortByLineUpIndex(List<Fighter> team)
    {
        Fighter tempFighter;

        for (int i = 0; i < team.Count; i++)
        {
            for (int j = 0; j < team.Count - 1 - i; j++)
            {
                if (team[j].GetLineUpIndex() > team[j + 1].GetLineUpIndex())
                {
                    tempFighter = team[j];
                    team[j] = team[j + 1];
                    team[j + 1] = tempFighter;
                }
            }
        }
    }
    void OnValidate()
    {
        Array.Resize(ref leftSpawnPoints, 4);
        Array.Resize(ref rightSpawnPoints, 4);

        if (leftGuysToSpawn.Length > 4)
        {
            Debug.LogWarning("Player has too many fighters! Max is 4.");
            Array.Resize(ref leftGuysToSpawn, 4);
        }
        else if (leftGuysToSpawn.Length == 0)
            Array.Resize(ref leftGuysToSpawn, 1);

        if (rightGuysToSpawn.Length > 4)
        {
            Debug.LogWarning("Enemy has too many fighters! Max is 4.");
            Array.Resize(ref rightGuysToSpawn, 4);
        }
        else if (rightGuysToSpawn.Length == 0)
            Array.Resize(ref rightGuysToSpawn, 1);
    }

    private void Awake()
    {
        CheckArray(leftSpawnPoints);
        CheckArray(rightSpawnPoints);
        CheckArray(leftGuysToSpawn);
        CheckArray(rightGuysToSpawn);

        uISetup = FindObjectOfType<UISetup>();
        ActionController.playerIsOnTheLeftSide = playerIsOnTheLeftSide;

        SetupScene();
    }

    private void SetupScene()
    {
        SetupTeam(leftGuysToSpawn, leftSpawnPoints, true);
        SetupTeam(rightGuysToSpawn, rightSpawnPoints, false);
    }

    private void SetupTeam(Unit[] team, Transform[] teamSpawnPoints, bool teamSideLeft)
    {
        int lineUpIndex = 0;

        for (int i = 0; i < team.Length; i++)
        {
            lineUpIndex++;

            Fighter newFighter = Instantiate(team[i].GetFighter(), teamSpawnPoints[i].position, Quaternion.identity, teamSpawnPoints[i]);

            GameObject itsHUD = uISetup.SetupFighterHUD(newFighter, team[i].GetFighterHUD());
            GameObject itsAbilities = uISetup.SetupAbilities(newFighter, team[i].GetFighterAbilities());

            newFighter.SetupFighterStats(team[i]);

            newFighter.SetHUD(itsHUD);
            newFighter.SetAbilitiesUIGameObject(itsAbilities);
            newFighter.SetLineUpIndex(lineUpIndex);

            if ((playerIsOnTheLeftSide && teamSideLeft) || (!playerIsOnTheLeftSide && !teamSideLeft))
            {
                newFighter.SetEnemyStatus(false);
                playerFighters.Add(newFighter);
            }
            else
            {
                newFighter.SetEnemyStatus(true);
                enemyFighters.Add(newFighter);
            }
        }
    }

    private void CheckArray(IEnumerable arr)
    {
        foreach (var item in arr)
        {
            if (item == null)
            {
                Debug.LogError("Make sure that you filled in all default values!");
                EditorApplication.isPlaying = false;
            }
        }
    } 
}
