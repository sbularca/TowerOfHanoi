using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Fabric.Crashlytics;
using System.Diagnostics;

/// <summary>
/// Class that takes care of gameplay aspects
/// </summary>
public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [SerializeField] GameObject m_WiningPanel;

    private List<GameObject> _mRings = new List<GameObject>();
    private bool _mOneRingIsDragged = false;
    private Dictionary<GameObject, GameObject> _mRingsPosition = new Dictionary<GameObject, GameObject>();

    public bool OneRingIsDragged { get { return _mOneRingIsDragged; } set { _mOneRingIsDragged = value; } }

    #region Private methods

    private void Awake()
    {
        Instance = this;
        Input.multiTouchEnabled = false;
    }

    private void OnEnable()
    {
        EventsManager.Events.AddListener("OnBoardSetup", OnBoardSetup);
        EventsManager.Events.AddListener("OnCancelDragObject", OnCancelDragObject);
        EventsManager.Events.AddListener("CheckGameOver", CheckGameOver);
        EventsManager.Events.AddListener("OnBeginDragObject", OnBeginDragObject);
        m_WiningPanel.SetActive(false);
    }

    /// <summary>
    /// Event method for update/reset the board
    /// </summary>
    /// <param name="args"></param>
    private void OnBoardSetup(params object[] args)
    {
        GameObject pin = (GameObject)args[0];
        _mRings = BoardController.Instance.Rings.ToList();
        _mRingsPosition.Clear();
        m_WiningPanel.SetActive(false);

        foreach (GameObject ring in _mRings)
        {
            _mRingsPosition.Add(ring, pin);
        }

    }

    /// <summary>
    /// Event method to call when dragging and object was canceled
    /// </summary>
    /// <param name="args"></param>
    private void OnCancelDragObject(params object[] args)
    {
        GameObject ring = (GameObject)args[0];
        GameObject pin = (GameObject)args[1];

        UpdateRingPosition(ring, pin);

        foreach (KeyValuePair<GameObject, GameObject> kvp in _mRingsPosition)
        {
            kvp.Key.GetComponent<Image>().raycastTarget = true;
            kvp.Key.GetComponent<RingBackRefference>().GetComponent<Image>().raycastTarget = true;
        }
    }

    /// <summary>
    /// Event that checks for the wining conditions
    /// </summary>
    /// <param name="args"></param>
    private void CheckGameOver(params object[] args)
    {
        if (WiningCondition())
        {
            UnityEngine.Debug.Log("You Won!");
            m_WiningPanel.SetActive(true);
        }

        foreach (KeyValuePair<GameObject, GameObject> kvp in _mRingsPosition)
        {
            kvp.Key.GetComponent<Image>().raycastTarget = true;
            kvp.Key.GetComponent<RingBackRefference>().GetComponent<Image>().raycastTarget = true;
        }
    }

    /// <summary>
    /// Wining condition method
    /// </summary>
    /// <returns></returns>
    private bool WiningCondition()
    {
        bool result = false;
        BoardController boardController = BoardController.Instance;

        for (int i = 0; i < BoardController.Instance.NumberOfPins; i++)
        {
            if (GetRingsOnPin(boardController.Pins[i]).Count == boardController.NumberOfRings && boardController.Pins[i] != boardController.StartingPin)
                result = true;
        }
        return result;
    }

    /// <summary>
    /// Event for when an object starts being dragged
    /// </summary>
    /// <param name="args"></param>
    private void OnBeginDragObject(params object[] args)
    {
        GameObject ring = (GameObject)args[0];
        foreach (KeyValuePair<GameObject, GameObject> kvp in _mRingsPosition)
        {
            if (kvp.Key != ring)
            {
                kvp.Key.GetComponent<Image>().raycastTarget = false;
                kvp.Key.GetComponent<RingBackRefference>().GetComponent<Image>().raycastTarget = false;
            }
        }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Removes the ring from the associated pin list
    /// </summary>
    /// <param name="pin"></param>
    /// <param name="ring"></param>
    public void UpdateRingPosition(GameObject ring, GameObject newPin)
    {
        if (ring != null)
            _mRingsPosition[ring] = newPin;
    }

    /// <summary>
    /// Returns the coresponding pin value for the ring key
    /// </summary>
    /// <param name="ring"></param>
    /// <returns></returns>
    public GameObject GetStartingPin(GameObject ring)
    {
        if (ring != null)
            return _mRingsPosition[ring];
        else
            return null;
    }

    /// <summary>
    /// Returns all the rings on a pin
    /// </summary>
    /// <param name="pin"></param>
    /// <returns></returns>
    public List<GameObject> GetRingsOnPin(GameObject pin)
    {
        List<GameObject> rings = new List<GameObject>();
        rings.Clear();

        if (pin != null)
        {
            foreach (KeyValuePair<GameObject, GameObject> kvp in _mRingsPosition)
            {
                if (kvp.Value == pin)
                    rings.Add(kvp.Key);
            }
        }

        return rings;
    }

    /// <summary>
    /// Checks if a ring can actually be picked
    /// </summary>
    /// <param name="ringToCheck"></param>
    /// <returns></returns>
    public bool CanBePicked(GameObject ringToCheck)
    {
        bool result = false;

        if (ringToCheck != null)
        {
            if (!_mOneRingIsDragged)
            {
                RectTransform ringTransform = (RectTransform)ringToCheck.transform;
                float lengthToCheck = ringTransform.rect.width;
                List<GameObject> rings = new List<GameObject>();
                rings = GetRingsOnPin(_mRingsPosition[ringToCheck]);

                if (rings.Count > 1)
                {
                    for (int i = 0; i < rings.Count; i++)
                    {
                        RectTransform ringToCompare = (RectTransform)rings[i].transform;
                        float lengthToCompare = ringToCompare.rect.width;

                        if (lengthToCheck <= lengthToCompare)
                            result = true;
                        else
                        {
                            result = false;
                            break;
                        }

                    }
                }
                else
                    result = true;
            }
        }       

        return result;
    }

    /// <summary>
    /// Checks if a ring can be positioned
    /// </summary>
    /// <param name="ringToCheck"></param>
    /// <param name="pin"></param>
    /// <returns></returns>
    public bool CanBePositioned(GameObject ringToCheck, GameObject pin)
    {
        bool result = false;

        if (ringToCheck!=null && pin != null)
        {
            RectTransform ringTransform = (RectTransform)ringToCheck.transform;
            float lengthToCheck = ringTransform.rect.width;
            List<GameObject> rings = new List<GameObject>();
            rings = GetRingsOnPin(pin);


            if (rings.Count > 0)
            {
                for (int i = 0; i < rings.Count; i++)
                {
                    RectTransform ringToCompare = (RectTransform)rings[i].transform;
                    float lengthToCompare = ringToCompare.rect.width;

                    if (lengthToCheck <= lengthToCompare)
                        result = true;
                    else
                    {
                        result = false;
                        break;
                    }

                }
            }
            else
                return true;
        }

        return result;
    }


    /// <summary>
    /// Method that generates log entries and crashes on Android, which are logged on Fabric.io (using Crashlytics)
    /// </summary>
    public void CrashMe()
    {
        StackFrame fr = new StackFrame(1, true);
        StackTrace st = new StackTrace(fr);
        Crashlytics.ThrowNonFatal();
        Crashlytics.RecordCustomException("NotSoGood Exception", "I have no clue what just happened", st);
        Crashlytics.Log("Non Fatal Just Happened");
    }
    #endregion
}
