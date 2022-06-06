using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomViewDuel : MonoBehaviour
{
    [SerializeField] float zoomTime;
    [SerializeField] float zoomCameraSize;
    [SerializeField] float scaleSize = 1.1f;
    [SerializeField] int duelSortingOrder = 10;
    [SerializeField] Camera cam;
    [SerializeField] GameObject background;

    [SerializeField] Transform toGo1, toGo2;

    int defaultSortingOrder;
    float camDefaultSize, f1ScaleStartValue, f1ScaleEndValue, f2ScaleStartValue, f2ScaleEndValue;
    Transform f1, f2;
    Vector2 f1DefPos, f2DefPos;
    Vector3 startScalef1, startScalef2;
    MeshRenderer f1MR, f2MR;

    private void Start()
    {
        camDefaultSize = cam.orthographicSize;
    }

    public IEnumerator StartDuel()
    {
        background.SetActive(true);

        f1MR = ActionController.currentAttacker.GetComponentInChildren<SpineAnimationController>().GetComponent<MeshRenderer>();
        f2MR = ActionController.currentTarget.GetComponentInChildren<SpineAnimationController>().GetComponent<MeshRenderer>();

        if ((ActionController.playersTurn && ActionController.playerIsOnTheLeftSide) || (!ActionController.playersTurn && !ActionController.playerIsOnTheLeftSide))
        {
            f1 = f1MR.transform;
            f2 = f2MR.transform;
        }
        else
        {
            f2 = f1MR.transform;
            f1 = f2MR.transform;
        }

        defaultSortingOrder = f1MR.sortingOrder;

        f1DefPos = f1.position;
        f2DefPos = f2.position;

        startScalef1 = f1.localScale;
        f1ScaleStartValue = startScalef1.x;
        f1ScaleEndValue = f1ScaleStartValue * scaleSize;
        startScalef2 = f2.localScale;
        f2ScaleStartValue = startScalef2.x;
        f2ScaleEndValue = f2ScaleStartValue * scaleSize;

        f1MR.sortingOrder = duelSortingOrder;
        f2MR.sortingOrder = duelSortingOrder;

        yield return StartCoroutine(ZoomInLerp());

    }

    public IEnumerator FinishDuel()
    {
        background.SetActive(false);
        yield return StartCoroutine(ZoomOutLerp());

        f1MR.sortingOrder = defaultSortingOrder;
        f2MR.sortingOrder = defaultSortingOrder;
    }

    IEnumerator ZoomInLerp()
    {        
        float time = 0f;

        float scaleModifier;      

        while (time < zoomTime)
        {
            cam.orthographicSize = Mathf.Lerp(camDefaultSize, zoomCameraSize, time / zoomTime);

            f1.position = Vector2.Lerp(f1DefPos, toGo1.position, time / zoomTime);
            scaleModifier = Mathf.Lerp(f1ScaleStartValue, f1ScaleEndValue, time / zoomTime);
            f1.localScale = startScalef1 * scaleModifier;

            f2.position = Vector2.Lerp(f2DefPos, toGo2.position, time / zoomTime);
            scaleModifier = Mathf.Lerp(f2ScaleStartValue, f2ScaleEndValue, time / zoomTime);
            f2.localScale = startScalef2 * scaleModifier;

            time += Time.deltaTime;
            yield return null;
        }

        cam.orthographicSize = zoomCameraSize;

        f1.position = toGo1.position;       
        f1.localScale = startScalef1 * scaleSize;
        f2.position = toGo2.position;
        f2.localScale = startScalef2 * scaleSize;
    }

    IEnumerator ZoomOutLerp()
    {
        float time = 0f;

        float scaleModifier;

        while (time < zoomTime)
        {
            cam.orthographicSize = Mathf.Lerp(zoomCameraSize, camDefaultSize, time / zoomTime);

            f1.position = Vector2.Lerp(toGo1.position, f1DefPos, time / zoomTime);
            scaleModifier = Mathf.Lerp(f1ScaleEndValue, f1ScaleStartValue, time / zoomTime);
            f1.localScale = startScalef1 * scaleModifier;

            f2.position = Vector2.Lerp(toGo2.position, f2DefPos, time / zoomTime);
            scaleModifier = Mathf.Lerp(f2ScaleEndValue, f2ScaleStartValue, time / zoomTime);
            f2.localScale = startScalef2 * scaleModifier;

            time += Time.deltaTime;
            yield return null;
        }

        cam.orthographicSize = camDefaultSize;

        f1.position = f1DefPos;
        f1.localScale = startScalef1;
        f2.position = f2DefPos;
        f2.localScale = startScalef2;
    }   
}
