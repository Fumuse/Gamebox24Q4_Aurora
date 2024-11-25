using System.Collections.Generic;
using UnityEngine;

public class DestroyWatcher : MonoBehaviour
{
    public static List<IDestroyable> ListToDestroy = new();

    private void OnDestroy()
    {
        foreach (IDestroyable destroyObject in ListToDestroy)
        {
            destroyObject.DestroyObject();
        }
    }
}