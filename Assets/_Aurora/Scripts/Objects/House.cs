using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    [SerializeField] private Room[] rooms;

    public IReadOnlyList<Room> Rooms => rooms;

    private void OnValidate()
    {
        rooms = FindObjectsByType<Room>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    }
}