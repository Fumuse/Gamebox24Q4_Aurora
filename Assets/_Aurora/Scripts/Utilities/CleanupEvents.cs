using System.Collections.Generic;
using UnityEngine;

public class CleanupEvents : MonoBehaviour
{
    private List<ISubscriberToCleanup> _subscribers;
    
    public void Init()
    {
        _subscribers = new();
    }

    public void Subscribe(ISubscriberToCleanup subscriber)
    {
        _subscribers.Add(subscriber);
    }

    public void Unsubscribe(ISubscriberToCleanup subscriber)
    {
        _subscribers.Remove(subscriber);
    }

    private void OnDestroy()
    {
        foreach (ISubscriberToCleanup subscriber in _subscribers)
        {
            subscriber.Cleanup();
        }
    }
}