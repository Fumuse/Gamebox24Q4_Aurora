using System;
using System.Collections.Generic;

public class TagManager
{
    private static TagManager _instance;
    private List<Tag> _currentTags;

    public static Action OnTagAdded;
    public static Action OnTagRemoved;

    public TagManager()
    {
        _currentTags = new List<Tag>();
    }

    public void AddTag(Tag tag)
    {
        if (HasTag(tag)) return;
        _currentTags.Add(tag);
        
        OnTagAdded?.Invoke();
    }

    public void RemoveTag(Tag tag)
    {
        if (!HasTag(tag)) return;
        _currentTags.Remove(tag);
        
        OnTagRemoved?.Invoke();
    }

    public bool HasTag(Tag tag)
    {
        foreach (Tag inTags in _currentTags)
        {
            if (tag.Name == inTags.Name) return true;
        }

        return false;
    }
}