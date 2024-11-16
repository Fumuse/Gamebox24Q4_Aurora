using TMPro;
using UnityEngine;

public class DialogInteractiveView : MonoBehaviour
{
    [SerializeField] private Canvas _interactableView;
    [SerializeField] private TMP_Text _messageTMP;

    public void Inizialize(string message)
    {
        _messageTMP.text = message;
    }

    public void SetVisible(bool visible) => _interactableView.enabled = visible;

}
