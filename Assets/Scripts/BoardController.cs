﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BoardController : MonoBehaviour
{

    [Header("Board Variables")]
    [SerializeField] float m_PinDistanceX; //distance between pins
    [SerializeField] float m_PinPositionY;
    [SerializeField] float m_BaseRingPosY;
    [SerializeField] float m_RingsHeightOffset;
    [SerializeField] int m_NumberOfRings; //in case less discs are used

    [Header("Board Elements")]
    [SerializeField] GameObject m_PinPrefab;
    [SerializeField] GameObject m_RingPrefab;
    [SerializeField] GameObject m_PinParent;

    [Header("Ring images for back and front")]
    [SerializeField] RingsImages[] m_RingsImages;

    const int NumberOfPins = 3; //nr of pins does not change

    private GameObject[] m_Pins;
    private GameObject[] m_Rings;

    public GameObject[] Pins { get { return m_Pins; } }
    public GameObject[] Rings { get { return m_Rings; } }

    private void Start()
    {
        m_Pins = new GameObject[NumberOfPins];
        m_Rings = new GameObject[m_NumberOfRings];

        m_Pins = PinInstantions();
        SetPinsPosition(m_Pins, m_PinDistanceX, m_PinPositionY);

        m_Rings = InstantiateRings();
        SetInitialRingsPosition(0);
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
            }
        }
        else
            Debug.LogError("The are references missing for pin prefab and/or pin parrent");

        return pins;
    }
    
    /// <summary>
    /// Positions pins to their respective positions
    /// </summary>
    /// <param name="pins">The pins array</param>
    /// <param name="distBetweenPins">The distance between pins</param>
    /// <param name="positionY">Position on Y axis</param>
    private void SetPinsPosition(GameObject [] pins, float distBetweenPins, float positionY)
    {
        Vector2 pin0 = new Vector3(-distBetweenPins, positionY);
        Vector2 pin1 = new Vector3(0, positionY);
        Vector2 pin2 = new Vector3(distBetweenPins, positionY);

        if (pins.Any(p => p != null))
        {
            pins[0].transform.localPosition = pin0;
            pins[1].transform.localPosition = pin1;
            pins[2].transform.localPosition = pin2;
        } else
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
            for (int i = m_NumberOfRings-1; i > -1; i--)
            {
                rings[i] = Instantiate(m_RingPrefab, m_PinParent.transform);
                rings[i].name = string.Format("Ring{0}", i);
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
    /// <param name="pinIndex">The pin for the starting position</param>
    private void SetInitialRingsPosition(int pinIndex)
    {
        for (int i=0; i<m_NumberOfRings; i++)
        {
            RectTransform ringTransform = (RectTransform) m_Rings[i].transform;
            float sizeY = ringTransform.rect.height - m_RingsHeightOffset;
            float positionY = m_BaseRingPosY + (sizeY * (m_NumberOfRings-1-i));
            Vector2 position = new Vector2(m_Pins[pinIndex].transform.localPosition.x, positionY);
            m_Rings[i].transform.localPosition = position;
        }

        Transform back = m_Rings[0].transform.GetChild(0);
        back.transform.SetParent(m_PinParent.transform);
        back.transform.SetAsFirstSibling();
    }
}

/// <summary>
/// Class to hold the sprites for each ring
/// </summary>
[System.Serializable]
public class RingsImages
{
    public Sprite m_Front;
    public Sprite m_Back;
}
