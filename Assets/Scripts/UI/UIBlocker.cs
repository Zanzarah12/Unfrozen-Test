using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBlocker : MonoBehaviour
{
    [SerializeField] GameObject[] blockers;

    private void Start()
    {
        if (blockers.Length == 0)
            enabled = false;
    }

    void Update()
    {
        if (ActionController.actionInProgress && !blockers[0].activeInHierarchy)
            SwitchBlockers(true);
        else if (!ActionController.actionInProgress && blockers[0].activeInHierarchy)
            SwitchBlockers(false);
    }

    private void SwitchBlockers(bool value)
    {
        foreach (var blocker in blockers)
        {
            blocker.transform.SetAsLastSibling();
            blocker.SetActive(value);
        }
    }
}
