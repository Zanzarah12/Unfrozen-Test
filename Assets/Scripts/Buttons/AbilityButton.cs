using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{    
    public Image background;

    public UnityEvent onAbilitySelected;
    public UnityEvent onAbilityDeselected;

    ButtonGroup buttonGroup;

    void Awake()
    {
        background = GetComponent<Image>();
        buttonGroup = GetComponentInParent<ButtonGroup>();
        buttonGroup.Subscribe(this);
    }

    public void Select()
    {
        if (onAbilitySelected != null)
            onAbilitySelected.Invoke();
    }

    public void Deselect()
    {
        if (onAbilityDeselected != null)
            onAbilityDeselected.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        buttonGroup.OnAbilityButtonSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonGroup.OnAbilityButtonEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonGroup.OnAbilityButtonExit(this);
    }
}
