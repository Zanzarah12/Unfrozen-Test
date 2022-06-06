using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability", order = 0)]
public class Ability : ScriptableObject
{
    [SerializeField] int damage;
    [SerializeField] int[] possibleTargets;
    [SerializeField] int pushAmount = 0;
    [SerializeField] GameObject abilityUIPrefab;
    [SerializeField] AnimationReferenceAsset abilitySpineAnimationReferenceAsset;

    public int GetDamage() => damage;
    public int[] GetPossibleTargets() => possibleTargets;
    public int GetPushAmount() => pushAmount;
    public GameObject GetAbilityUIPrefab() => abilityUIPrefab;
    public AnimationReferenceAsset GetAnimationReference() => abilitySpineAnimationReferenceAsset;
}
