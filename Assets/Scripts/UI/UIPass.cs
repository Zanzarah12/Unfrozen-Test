using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPass : MonoBehaviour
{
    TurnSequence turnSequence;

    private void Awake()
    {
        turnSequence = FindObjectOfType<TurnSequence>();
    }

    public void OnClick()
    {
        turnSequence.SetNextFighter();
    }
}
