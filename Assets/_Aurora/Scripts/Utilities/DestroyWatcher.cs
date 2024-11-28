using System.Collections.Generic;
using UnityEngine;

public class DestroyWatcher : MonoBehaviour
{
    public static List<IDestroyable> ListToDestroy = new();

    private void OnDisable()
    {
        foreach (IDestroyable destroyObject in ListToDestroy)
        {
            destroyObject.DestroyObject();
        }
        
        ListToDestroy.Clear();
    }
}