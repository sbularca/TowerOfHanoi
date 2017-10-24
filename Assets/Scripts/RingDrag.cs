using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Origin: Unity, from Standard Assets adapted to the current conditions
/// Drag class to be added on the dragged objects
/// </summary>
public class RingDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool dragOnSurfaces = false;
    private BoardController _mBoardController;
    private GameController _mGameController;
    private GameObject _mRingBackObject;
    private Dictionary<int, GameObject> _mDraggingIcons = new Dictionary<int, GameObject>();
    private Dictionary<int, RectTransform> _mDraggingPlanes = new Dictionary<int, RectTransform>();
    private GameObject _mPointerIdObject;
    private GameObject _mCanvas;
    private GameObject _mStartingPin;
    private Animator _mAnimator;
    private Animator _mBackAnimator;
    private Vector3 _mFrontInitialPosition;
    private Vector3 _mBackInitialPosition;
    private Transform _mInitialParent;
    private bool _mCanBePicked = false;


    void Start()
    {
        _mCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        _mAnimator = GetComponent<Animator>();
        _mRingBackObject = GetComponent<RingBackRefference>().RingBackObject;
        _mBackAnimator = _mRingBackObject.GetComponent<Animator>();
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        _mCanBePicked= GameController.Instance.CanBePicked(gameObject);

        if (_mCanBePicked)
        {
            _mAnimator.SetBool("onDrag", true);
            _mBackAnimator.SetBool("onDrag", true);            
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _mAnimator.SetBool("onDrag", false);
        _mBackAnimator.SetBool("onDrag", false);
    }


    public void OnBeginDrag(PointerEventData eventData)    {

        if (!GameController.Instance.OneRingIsDragged && _mCanBePicked)
        {
            if (_mCanvas == null)
                return;

            _mStartingPin = GameController.Instance.GetStartingPin(gameObject);
            _mFrontInitialPosition = gameObject.transform.position;
            _mInitialParent = transform.parent;
            _mDraggingIcons[eventData.pointerId] = gameObject;
            _mRingBackObject.transform.SetParent(gameObject.transform);
            _mBackInitialPosition = _mRingBackObject.transform.position;
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
            GameController.Instance.UpdateRingPosition(gameObject, null);
            GameController.Instance.OneRingIsDragged = true;
            GameController.Instance.PointerData = _mPointerIdObject;
            GameController.Instance.DisableRaycastOnRings();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameController.Instance.OneRingIsDragged)
        {
            _mPointerIdObject = _mDraggingIcons[eventData.pointerId];

            if (_mPointerIdObject != null)
            {
                SetDraggedPosition(eventData);
            }
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        StartCoroutine(UpdateRing());
    }

    /// <summary>
    /// Sets the position of the object while dragging
    /// </summary>
    /// <param name="eventData"></param>
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


    /// <summary>
    /// Sets the original position if the ring dragging is canceled 
    /// </summary>
    private void SetInitialPosition()
    {
        _mAnimator.SetBool("onDrag", false);
        _mBackAnimator.SetBool("onDrag", false);
        transform.position = _mFrontInitialPosition;
        transform.SetParent(_mInitialParent);
        _mRingBackObject.transform.position = _mBackInitialPosition;
        _mRingBackObject.transform.SetParent(transform.parent);
        _mRingBackObject.transform.SetAsFirstSibling();
        _mPointerIdObject = null;
        _mDraggingIcons.Clear();
        _mDraggingPlanes.Clear();
        Destroy(GetComponent<CanvasGroup>());
        EventsManager.Events.PostNotification("OnCancelDragObject", gameObject, _mStartingPin);
    }

    /// <summary>
    /// Clears the status of the ring if the ring has been positioned on a pin
    /// </summary>
    private void ConfirmPosition()
    {
        _mAnimator.SetBool("onDrag", false);
        _mBackAnimator.SetBool("onDrag", false);
        _mPointerIdObject = null;
        _mDraggingIcons.Clear();
        _mDraggingPlanes.Clear();
        Destroy(GetComponent<CanvasGroup>());
    }

    IEnumerator UpdateRing()
    {
        while (true)
        {
            if (GameController.Instance.OneRingIsDragged && !GameController.Instance.IsOverPin)
            {
                SetInitialPosition();
                yield return null;
            }
            else
            {
                ConfirmPosition();
                yield break;
            }
                
        }
    }

}
