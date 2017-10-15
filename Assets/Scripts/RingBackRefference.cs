using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Refference in the prefab for the back ring object
/// </summary>
public class RingBackRefference : MonoBehaviour {

    public GameObject m_Refference;

    public GameObject RingBackObject { get { return m_Refference; } }

}
