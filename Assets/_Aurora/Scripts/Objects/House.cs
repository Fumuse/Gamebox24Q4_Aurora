using System;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    [SerializeField] private Room[] rooms;

    public IReadOnlyList<Room> Rooms => rooms;

    private void OnValidate()
    {
        if (rooms.Length < 1)
        {
            rooms = FindObjectsByType<Room>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        }
    }
}