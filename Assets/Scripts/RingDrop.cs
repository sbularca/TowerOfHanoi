/*---------------------------------------------------------------------------------------------
 *  Origin: Unity, from Standard Assets adapted to the current conditions
 *  Description: Drop Script - to be added on the target object
 *--------------------------------------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RingDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private Animator _mAnimator;
    private bool _mPointerHasData=false;

    private void Start()
    {
        _mAnimator = GetComponent<Animator>();
        EventsManager.Events.AddListener("OnBeginDragObject", OnBeginDragObject);
        EventsManager.Events.AddListener("OnEndDragObject", OnEndDragObject);
    }

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnDrop(PointerEventData eventData) { }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_mPointerHasData)
            _mAnimator.SetBool("pinGreen", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _mAnimator.SetBool("pinGreen", false);
    }

    private void OnBeginDragObject(params object[] args)
    {
        GameObject obj = args[0] as GameObject;
        if (obj != null)
            _mPointerHasData = true;
    }

    private void OnEndDragObject(params object[] args)
    {
        _mPointerHasData = false;
        _mAnimator.SetBool("pinGreen", false);
    }
}
