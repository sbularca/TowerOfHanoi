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
    private bool canBePositioned = true;

    private void Start()
    {
        _mAnimator = GetComponent<Animator>();
    }

    public void OnPointerDown(PointerEventData eventData) { }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (PointerHasData())
        {
            EventsManager.Events.AddListener("OnEndDragObject", OnEndDragObject);
            canBePositioned = GameController.Instance.CanBePositioned(GameController.Instance.PointerData, gameObject);

            if (canBePositioned)
            {
                _mAnimator.SetBool("pinGreen", true);
                GameController.Instance.IsOverPin = true;
            } else
            {
                _mAnimator.SetBool("pinRed", true);
                GameController.Instance.IsOverPin = false;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _mAnimator.SetBool("pinGreen", false);
        _mAnimator.SetBool("pinRed", false);
        GameController.Instance.IsOverPin = false;
        canBePositioned = true;
        EventsManager.Events.RemoveListener("OnEndDragObject", OnEndDragObject);
    }


    public void OnDrop(PointerEventData eventData)
    {
        if (GameController.Instance.IsOverPin && GameController.Instance.OneRingIsDragged)
        {
            GameController.Instance.UpdateRingPosition(GameController.Instance.PointerData, gameObject);
            PositionRing(GameController.Instance.PointerData);
            EventsManager.Events.PostNotification("CheckGameOver");
            _mAnimator.SetBool("pinGreen", false);
            _mAnimator.SetBool("pinRed", false);
            GameController.Instance.PointerData = null;
        }

    }

    private void OnEndDragObject(params object[]args)
    {
        GameController.Instance.IsOverPin = false;
        GameController.Instance.OneRingIsDragged = false;
    }

    private bool PointerHasData()
    {
        if (GameController.Instance.PointerData != null)
            return true;
        else
            return false;
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
                GameController.Instance.IsOverPin = false;
            }
        }
        

    }
}

