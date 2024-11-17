using System;
using UnityEngine;

[Serializable]
public class Tag
{
    [SerializeField] private TagEnum tagName;
    public TagEnum Name => tagName;

    public Tag(TagEnum tag)
    {
        tagName = tag;
    }
}