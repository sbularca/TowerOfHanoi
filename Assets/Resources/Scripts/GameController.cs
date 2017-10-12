using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour {

    private BoardController _mBoardController;
    private GameObject[] _mPins;
    private GameObject[] _mRings;
    private int _mNumberOfRings;



    void Start()
    {
        _mBoardController = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardController>();
        _mNumberOfRings = _mBoardController.NumberOfRings;
        _mPins = new GameObject[_mBoardController.NumberOfPins];
        _mPins = _mBoardController.Pins;
        _mRings = new GameObject[_mNumberOfRings];
        _mRings = _mBoardController.Rings;
    }


    public int GetIndex(GameObject[] objArray, GameObject obj)
    {
        int index=100;

        if (objArray.Any(o => o.name == obj.name))
            index = Array.IndexOf(objArray, obj);

        return index;
    }
}
