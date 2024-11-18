using System;
using UnityEngine;

[Serializable]
public class InteractableObjectStateVisionPair
{
    [SerializeField] public InteractableStateVisionEnum visionKey;
    [SerializeField] public InteractableObjectState interactableObjectState;
}