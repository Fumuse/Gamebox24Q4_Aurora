using System.Collections.Generic;

public class TagManager
{
    private static TagManager _instance;
    private List<Tag> _currentTags;

    public TagManager()
    {
        _currentTags = new List<Tag>();
    }

    public void AddTag(Tag tag)
    {
        _currentTags.Add(tag);
    }

    public void RemoveTag(Tag tag)
    {
        if (HasTag(tag))
        {
            _currentTags.Remove(tag);
        }
    }

    public bool HasTag(Tag tag)
    {
        return _currentTags.Contains(tag);
    }
}