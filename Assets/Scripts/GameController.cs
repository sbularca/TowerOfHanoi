using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Fabric.Crashlytics;
using System.Diagnostics;

public class GameController : MonoBehaviour
{
    private List <Sets> _mBoardSetsHolder = new List<Sets>();
    private BoardController _mBoardController;
    private GameObject[] _mPins;
    private GameObject[] _mRings;
    private int _mNumberOfRings;

    public List<Sets> BoardSetsHolder
    {
        get { return _mBoardSetsHolder; }
        set { _mBoardSetsHolder = value; }
    }

    void Start()
    {
        _mBoardController = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardController>();
        _mNumberOfRings = _mBoardController.NumberOfRings;
        _mPins = new GameObject[_mBoardController.NumberOfPins];
        _mPins = _mBoardController.Pins;
        _mRings = new GameObject[_mNumberOfRings];
        _mRings = _mBoardController.Rings;

        EventsManager.Events.AddListener("OnBoardInitialSetup", OnBoardInitialSetup);
    }

    private void OnBoardInitialSetup(params object[]args)
    {
        BoardSetsHolder.Add(new Sets{m_Pin = _mPins[0], m_Rings = _mRings.ToList()});

        UnityEngine.Debug.Log(string.Format("The pin {0} contains a number of {1} rings", BoardSetsHolder[0].m_Pin.name, BoardSetsHolder[0].m_Rings.Count())); 
    }

    /// <summary>
    /// Returns the index of an item in the array
    /// </summary>
    /// <returns>The index.</returns>
    /// <param name="objArray">Object array.</param>
    /// <param name="obj">Object.</param>
    public int GetIndex(GameObject[] objArray, GameObject obj)
    {
        int index = -1;

        if (objArray.Any(o => o.name == obj.name))
            index = Array.IndexOf(objArray, obj);
        else
            UnityEngine.Debug.Log("There is no element in the array with the name" + obj.name);

        return index;
    }

    public void CrashMe()
    {
       StackFrame fr = new StackFrame(1, true);
        StackTrace st = new StackTrace(fr);
        Crashlytics.ThrowNonFatal();
        Crashlytics.RecordCustomException("NotSoGood Exception", "I have no clue what just happened", st);
        Crashlytics.Log("Non Fatal Just Happened");
    }
}

/// <summary>
/// Holds the amount of rings for each Pin
/// </summary>
public struct Sets
{
    public GameObject m_Pin;
    public List<GameObject> m_Rings;
}
