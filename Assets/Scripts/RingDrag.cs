/*---------------------------------------------------------------------------------------------
 *  Origin: Unity, from Standard Assets adapted to the current conditions
 *  Description: Drag Script - to be added on the dragged objects
 *--------------------------------------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RingDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool dragOnSurfaces = false;

    private BoardController _mBoardController;
    private GameController _mGameController;
    private GameObject _mRingBackObject;
    private Dictionary<int, GameObject> _mDraggingIcons = new Dictionary<int, GameObject>();
    private Dictionary<int, RectTransform> _mDraggingPlanes = new Dictionary<int, RectTransform>();
    private GameObject _mPointerIdObject;
    private GameObject _mCanvas;
    private Animator _mAnimator;
    private Animator _mBackAnimator;
    private Vector3 _mFrontInitialPosition;
    private Vector3 _mBackInitialPosition;
    private Transform _mInitialParent;
    private int index;

    void Start()
    {
        _mBoardController = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardController>();
        _mCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        _mRingBackObject = GetComponent<RingBackRefference>().RingBackObject;
        _mAnimator = GetComponent<Animator>();
        _mBackAnimator = _mRingBackObject.GetComponent<Animator>();
        
    }

    public void OnPointerEnter(PointerEventData eventData) { }
	
    public void OnPointerExit(PointerEventData eventData) { }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_mCanvas == null)
            return;
        _mFrontInitialPosition = transform.position;
        _mBackInitialPosition = _mRingBackObject.transform.position;
        _mInitialParent = transform.parent;
        _mDraggingIcons[eventData.pointerId] = this.gameObject;
        _mRingBackObject.transform.SetParent(this.gameObject.transform);
        _mDraggingIcons[eventData.pointerId].transform.SetParent(_mCanvas.transform, false);
        _mDraggingIcons[eventData.pointerId].transform.SetAsLastSibling();
        _mAnimator.SetBool("onDrag", true);
        _mBackAnimator.SetBool("onDrag", true);

        var group = _mDraggingIcons[eventData.pointerId].AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;

        if (dragOnSurfaces)
            _mDraggingPlanes[eventData.pointerId] = transform as RectTransform;
        else
            _mDraggingPlanes[eventData.pointerId] = _mCanvas.transform as RectTransform;

        SetDraggedPosition(eventData);
        EventsManager.Events.PostNotification("OnBeginDragObject", _mPointerIdObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _mPointerIdObject = _mDraggingIcons[eventData.pointerId];

        if (_mPointerIdObject != null)
        {
            SetDraggedPosition(eventData);
        }
    }

    public void OnEndDrag (PointerEventData eventData)
    {
        _mAnimator.SetBool("onDrag", false);
        _mBackAnimator.SetBool("onDrag", false);
        transform.position = _mFrontInitialPosition;
        transform.SetParent(_mInitialParent);
        _mRingBackObject.transform.position = _mBackInitialPosition;

        _mRingBackObject.transform.SetParent(transform.parent);
        _mRingBackObject.transform.SetAsFirstSibling();

        _mDraggingIcons.Clear();
        _mDraggingPlanes.Clear();
        Destroy(GetComponent<CanvasGroup>());

        EventsManager.Events.PostNotification("OnEndDragObject");
        _mPointerIdObject = null;        
    }


    private void SetDraggedPosition(PointerEventData eventData)
    {
        if (dragOnSurfaces && eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null)
        {
            _mDraggingPlanes[eventData.pointerId] = eventData.pointerEnter.transform as RectTransform;
        }

        var rt = _mDraggingIcons[eventData.pointerId].GetComponent<RectTransform>();

        Vector3 globalMousePos;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_mDraggingPlanes[eventData.pointerId], eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = _mDraggingPlanes[eventData.pointerId].rotation;
        }

        _mPointerIdObject = _mDraggingIcons[eventData.pointerId];
    }

}
