using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Author unknown. Script improved by me to support multiple number and types of arguments
/// Events manager Singleton that uses Unity embeded EventSystem to post and receive events
/// </summary>
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
            //Debug.Log("Listener to event " + eventName + " added");
        }
        else
        {
            thisEvent = new UnityEvent1();
            thisEvent.AddListener(listener);
            Listeners.Add(eventName, thisEvent);
            //Debug.Log("The new event " + eventName + " added");
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
                //Debug.Log("Event " + eventName + " invoked");
            }
        }
        else
        {
            //Debug.Log("Event " + eventName + " was null or empty");
        }

    }
  
}

public class UnityEvent1 : UnityEvent<object[]> { }