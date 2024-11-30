using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DestroyWatcher : MonoBehaviour
{
    public static List<IDestroyable> ListToDestroy = new();

    private void OnDisable()
    {
        Disable();
    }

    public static void Disable()
    {
        foreach (IDestroyable destroyObject in ListToDestroy)
        {
            destroyObject.DestroyObject();
        }
        
        ListToDestroy.Clear();
    }
}