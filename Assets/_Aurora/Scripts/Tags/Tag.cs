using System;
using UnityEngine;

[Serializable]
public class Tag
{
    [SerializeField] private string tagName;
    public string Name => tagName;
}