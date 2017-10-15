using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Script origin is Unity, from Standard Assets adapted to the current conditions
/// Drop class to be added on the target object
/// </summary>
public class RingDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private Animator _mAnimator;
    private bool _mPointerHasData = false;
    private GameObject _mDraggedObject;
    private bool canBePositioned = true;

    private void OnEnable()
    {
        EventsManager.Events.AddListener("OnBeginDragObject", OnBeginDragObject);
    }

    private void Start()
    {
        _mAnimator = GetComponent<Animator>();
    }

    public void OnPointerDown(PointerEventData eventData) { }


    public void OnPointerEnter(PointerEventData eventData)    {

        if (_mPointerHasData)
        {
            canBePositioned = GameController.Instance.CanBePositioned(_mDraggedObject, gameObject);

            if (canBePositioned)
            {
                _mAnimator.SetBool("pinGreen", true);
                EventsManager.Events.PostNotification("IsOverPin", true);
            } else
            {
                _mAnimator.SetBool("pinRed", true);
            }

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _mAnimator.SetBool("pinGreen", false);
        _mAnimator.SetBool("pinRed", false);
        EventsManager.Events.PostNotification("IsOverPin", false);
        canBePositioned = true;
    }


    public void OnDrop(PointerEventData eventData)
    {
        GameController.Instance.UpdateRingPosition(_mDraggedObject, gameObject);
        PositionRing(_mDraggedObject);
        EventsManager.Events.PostNotification("CheckGameOver");
        _mDraggedObject = null;
        _mPointerHasData = false;
        _mAnimator.SetBool("pinGreen", false);
        _mAnimator.SetBool("pinRed", false);

    }

    /// <summary>
    /// Event method that sends the dragged object to the pins
    /// </summary>
    /// <param name="args"></param>
    private void OnBeginDragObject(params object[] args)
    {
        _mDraggedObject = args[0] as GameObject;
        if (_mDraggedObject != null)
        {
            _mPointerHasData = true;
        }
            
    }

    /// <summary>
    /// Positions the ring on this pin, if it can be positioned
    /// </summary>
    /// <param name="ring"></param>
    private void PositionRing(GameObject ring)
    {
        if (ring != null)
        {
            if (canBePositioned)
            {
                List<GameObject> ringsOnPin = new List<GameObject>();
                ringsOnPin.Clear();

                ring.transform.SetParent(BoardController.Instance.PinParent.transform);
                ringsOnPin = GameController.Instance.GetRingsOnPin(gameObject);
                RectTransform ringTransform = (RectTransform)ring.transform;
                RectTransform pinTransform = (RectTransform)gameObject.transform;

                float sizeY = ringTransform.rect.height - BoardController.Instance.RingHeightOffset;
                float positionY = (pinTransform.localPosition.y + (sizeY * (ringsOnPin.Count - 1))) - BoardController.Instance.RingHeightOffset + ringTransform.rect.height/2;
                Vector2 position = new Vector2(gameObject.transform.localPosition.x, positionY);
                ring.transform.localPosition = position;
                Transform back = ring.GetComponent<RingBackRefference>().RingBackObject.transform;
                back.SetParent(gameObject.transform.parent);
                back.SetAsFirstSibling();
            }
            else
            {
                EventsManager.Events.PostNotification("IsOverPin", false);
            }
        }
        

    }
}

