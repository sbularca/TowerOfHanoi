using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// Class that takes care of the setup and the reseting of the board
/// </summary>
public class BoardController : MonoBehaviour
{

    [Header("Board Variables")]
    [SerializeField]
    float m_PinDistanceX; //distance between pins
    [SerializeField] float m_PinPositionY; //base Y position on the pins
    [SerializeField] float m_RingsHeightOffset; //offset for the distance between pins pivots
    [SerializeField] int m_NumberOfRings; //in case less discs are used which the current setup supports
    [SerializeField] int m_StartingPin = 0; //the starting pin can be changed

    [Header("Board Elements")]
    [SerializeField] GameObject m_PinPrefab;
    [SerializeField] GameObject m_RingPrefab;
    [SerializeField] GameObject m_PinParent;

    [Header("Ring images for back and front")]
    [SerializeField] RingsImages[] m_RingsImages;

    public static BoardController Instance;

    private int _mNumberOfPins = 3; //nr of pins is predefined
    private GameObject _mCurrentPin;
    private GameObject[] _mPins;
    private GameObject[] _mRings;

    public GameObject[] Rings { get { return _mRings; } }
    public GameObject[] Pins { get { return _mPins; } }
    public int NumberOfPins { get { return _mNumberOfPins; } }
    public int NumberOfRings { get { return m_NumberOfRings; } }
    public float RingHeightOffset { get { return m_RingsHeightOffset; } }
    public GameObject PinParent { get { return m_PinParent; } }
    public GameObject StartingPin { get { return _mPins[m_StartingPin]; } }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _mPins = new GameObject[NumberOfPins];
        _mRings = new GameObject[m_NumberOfRings];
        _mPins = PinInstantions();
        _mRings = InstantiateRings();
        _mCurrentPin = _mPins[m_StartingPin];

        SetPinsPosition(_mPins, m_PinDistanceX, m_PinPositionY);
        SetInitialRingsPosition(_mCurrentPin);

        EventsManager.Events.PostNotification("OnBoardSetup", _mCurrentPin);
    }

    /// <summary>
    /// Instantiates the board pins
    /// </summary>
    /// <returns>The array of pins</returns>
    private GameObject[] PinInstantions()
    {
        GameObject[] pins = new GameObject[NumberOfPins];

        if (m_PinPrefab != null && m_PinParent != null)
        {
            for (int i = 0; i < NumberOfPins; i++)
            {
                pins[i] = Instantiate(m_PinPrefab, m_PinParent.transform);
                pins[i].name = string.Format("P{0}", i);
            }
        }
        else
            Debug.LogError("The are references missing for pin prefab and/or pin parrent");

        return pins;
    }

    /// <summary>
    /// Positions the pins on the board. Since the number of pins is predifined, the method is hardcoding the position.
    /// It can be easily generalized
    /// </summary>
    /// <param name="pins">The pins array</param>
    /// <param name="distBetweenPins">The distance between pins</param>
    /// <param name="positionY">Position on Y axis</param>
    private void SetPinsPosition(GameObject[] pins, float distBetweenPins, float positionY)
    {
        Vector2 pin0 = new Vector3(-distBetweenPins, positionY);
        Vector2 pin1 = new Vector3(0, positionY);
        Vector2 pin2 = new Vector3(distBetweenPins, positionY);

        if (pins.Any(p => p != null))
        {
            pins[0].transform.localPosition = pin0;
            pins[1].transform.localPosition = pin1;
            pins[2].transform.localPosition = pin2;
        }
        else
            Debug.LogError("Not all pins have been instantiated");
    }

    /// <summary>
    /// Instantiates the rings
    /// </summary>
    /// <returns>The instantiated rings</returns>
    private GameObject[] InstantiateRings()
    {
        GameObject[] rings = new GameObject[m_NumberOfRings];

        if (m_RingPrefab != null && m_PinParent != null)
        {
            for (int i = m_NumberOfRings - 1; i > -1; i--)
            {
                rings[i] = Instantiate(m_RingPrefab, m_PinParent.transform);
                rings[i].name = string.Format("R{0}", i);
                rings[i].GetComponent<RingBackRefference>().RingBackObject.name = string.Format("RB{0}", i);
                rings[i].transform.SetAsLastSibling();
                Image imageFront = rings[i].GetComponent<Image>();
                Image imageBack = rings[i].transform.GetChild(0).GetComponent<Image>();

                imageFront.sprite = m_RingsImages[i].m_Front;
                imageBack.sprite = m_RingsImages[i].m_Back;

                imageFront.SetNativeSize();
                imageBack.SetNativeSize();
            }
        }
        else
            Debug.LogError("There are references missing for the rings prefab and/or rings parent");

        return rings;
    }

    /// <summary>
    /// Sets the starting positions for the rings
    /// </summary>
    /// <param name="pin">The pin for the starting position</param>
    private void SetInitialRingsPosition(GameObject pin)
    {
        RectTransform pinTransform = (RectTransform)pin.transform;

        for (int i = 0; i < m_NumberOfRings; i++)
        {
            _mRings[i].transform.SetParent(PinParent.transform);
            RectTransform ringTransform = (RectTransform)_mRings[i].transform;
            float sizeY = ringTransform.rect.height - m_RingsHeightOffset;
            float positionY = (pinTransform.localPosition.y + (sizeY * (m_NumberOfRings - 1 - i))) - m_RingsHeightOffset + ringTransform.rect.height / 2;
            Vector2 position = new Vector2(pinTransform.localPosition.x, positionY);
            _mRings[i].transform.localPosition = position;
            Transform back = _mRings[i].GetComponent<RingBackRefference>().RingBackObject.transform;
            back.SetParent(m_PinParent.transform);
            back.SetAsFirstSibling();
        }
    }

    /// <summary>
    /// UI accessed method to reset the board
    /// </summary>
    public void ResetBoard()
    {
        foreach (GameObject ring in _mRings)
        {
            if (ring != null)
            {
                Destroy(ring);
                Destroy(ring.GetComponent<RingBackRefference>().RingBackObject);
            }

        }

        _mRings = InstantiateRings();

        SetInitialRingsPosition(_mCurrentPin);

        EventsManager.Events.PostNotification("OnBoardSetup", _mCurrentPin);
    }
}

/// <summary>
/// Struct to hold the sprites for each ring
/// </summary>
[System.Serializable]
public struct RingsImages
{
    public Sprite m_Front;
    public Sprite m_Back;
}
