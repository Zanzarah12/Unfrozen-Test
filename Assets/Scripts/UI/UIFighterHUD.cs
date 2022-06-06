using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFighterHUD : MonoBehaviour
{   
    [SerializeField] Image healthBar;
    [SerializeField] GameObject allyHighlight;
    [SerializeField] GameObject enemyHighlight;
    [SerializeField] Vector2 uIOffset;  

    Fighter connectedFighter;
    Transform followTransform;

    Camera mainCam;
    RectTransform rectTransform, canvasRT;
    Vector2 viewportPosition, worldObjectScreenPosition;
    GameObject currentHighlight;

    public void Initialize(Fighter fighter)
    {
        connectedFighter = fighter;
        followTransform = fighter.transform.parent;

        connectedFighter.onHealthUpdate += UpdateHealthBar;
        connectedFighter.onMouseOver += ShowHideUI;
        connectedFighter.onMouseExit += ShowHideUI;
    }

    private void Awake()
    {        
        rectTransform = GetComponent<RectTransform>();     
        canvasRT = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    private void Start()
    {
        mainCam = Camera.main;

        if (connectedFighter.IsEnemy())
        {
            currentHighlight = enemyHighlight;
        }
        else
        {
            currentHighlight = allyHighlight;
        }
    }

    private void Update()
    {   
        viewportPosition = mainCam.WorldToViewportPoint(followTransform.position);

        worldObjectScreenPosition = new Vector2(
        (viewportPosition.x * canvasRT.sizeDelta.x) - (canvasRT.sizeDelta.x * 0.5f),
        (viewportPosition.y * canvasRT.sizeDelta.y) - (canvasRT.sizeDelta.y * 0.5f));

        rectTransform.anchoredPosition = worldObjectScreenPosition + uIOffset;
    }

    private void UpdateHealthBar()
    {
        healthBar.rectTransform.localScale = new Vector3(connectedFighter.GetCurrentHealthPercentage(), 1f, 1f);
    }

    private void ShowHideUI(bool value)
    {
        if (connectedFighter.IsEnemy())
        {
            if(currentHighlight)
                currentHighlight.SetActive(value);
        }
        else
            currentHighlight.SetActive(value);
    }  

    internal void SetActiveSelection()
    {
        if (!connectedFighter.IsEnemy())
        {
            currentHighlight.SetActive(true);
        }
    }
}
