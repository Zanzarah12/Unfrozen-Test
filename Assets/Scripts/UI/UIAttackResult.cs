using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIAttackResult : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float scaleTime = 1f;
    RectTransform rectTransform;
    TextMeshProUGUI textMeshPro;

    public void SetText(string newText)
    {
        textMeshPro.text = newText;
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        StartCoroutine(ScaleDown());
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        rectTransform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }

    IEnumerator ScaleDown()
    {
        float time = 0;
        Vector3 startScale = transform.localScale;

        while (time < scaleTime)
        {           
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, time / scaleTime);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.zero;
    } 
}
