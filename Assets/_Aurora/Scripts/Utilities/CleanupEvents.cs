using System.Collections.Generic;

public class CleanupEvents : Singleton<CleanupEvents>
{
    private List<ISubscriberToCleanup> _subscribers;
    
    public void Init()
    {
        if (Instance == null)
        {
            this.Awake();
        }

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