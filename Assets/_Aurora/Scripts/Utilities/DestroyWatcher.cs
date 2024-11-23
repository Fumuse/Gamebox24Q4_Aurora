using System;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWatcher : MonoBehaviour
{
    public static List<IDestroyable> ListToDestroy = new();

    private void OnDestroy()
    {
        Debug.Log(ListToDestroy.Count);
        foreach (IDestroyable destroyObject in ListToDestroy)
        {
            Debug.Log(destroyObject);
            destroyObject.DestroyObject();
        }
    }
}