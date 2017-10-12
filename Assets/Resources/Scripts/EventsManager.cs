/*---------------------------------------------------------------------------------------------
 *  Author: Unknown. Script improved by me to support multiple number and types of arguments
 *	Description: Events manager using the Unity embeded EventSystem
 *--------------------------------------------------------------------------------------------*/
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class EventsManager : MonoBehaviour
{
    public static EventsManager Events = null;

    private Dictionary<string, UnityEvent1> Listeners = new Dictionary<string, UnityEvent1>();

    public void Awake()
    {
        if (Events == null)
            Events = this;
        else if (Events != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Ads the eventName to a list of listeners for this event
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    public void AddListener(string eventName, UnityAction<object[]> listener)
    {
        UnityEvent1 thisEvent = null;
        if (Listeners.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent1();
            thisEvent.AddListener(listener);
            Listeners.Add(eventName, thisEvent);
        }
    }

    /// <summary>
    /// Removes the eventName from the list of listeners for this event
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    public void RemoveListener(string eventName, UnityAction<object[]> listener)
    {
        UnityEvent1 thisEvent = null;
        if (Listeners.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    /// <summary>
    /// Posts and event which will be triggered by all listeners
    /// </summary>
    /// <param name="eventName"></param>
    public void PostNotification(string eventName, params object[] args)
    {
        UnityEvent1 thisEvent = null;

        if (!string.IsNullOrEmpty(eventName))
        {
            if (Listeners.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke(args);
            }
        }
        else
        {
           // Debug.LogAssertion("The EventManager 'eventname' was null or empty");
        }

    }
  
}

public class UnityEvent1 : UnityEvent<object[]> { }