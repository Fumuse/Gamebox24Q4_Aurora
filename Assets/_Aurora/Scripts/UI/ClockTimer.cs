using DG.Tweening;
using UnityEngine;

public class ClockTimer : MonoBehaviour
{
    [SerializeField] private Transform smallArrow;
    [SerializeField] private Transform longArrow;

    private float _minuteArrowStep = 360f / 60f;
    private float _hourArrowStep = 360f / 12f / 60f;
    
    private void OnEnable()
    {
        Timer.OnTimeChanged += OnTimeChanged;
    }

    private void OnDisable()
    {
        Timer.OnTimeChanged -= OnTimeChanged;
    }

    private void OnTimeChanged()
    {
        MinutesChanged();
        HoursChanged();
    }

    private void MinutesChanged()
    {
        float degrees = _minuteArrowStep * GameManager.Instance.Timer.TimePasses;
        Quaternion minutes = Quaternion.Euler(0, 0, -degrees);
        
        longArrow.DORotateQuaternion(minutes, 3f);
    }

    private void HoursChanged()
    {
        float degrees = _hourArrowStep * GameManager.Instance.Timer.TimePasses;
        Quaternion minutes = Quaternion.Euler(0, 0, -degrees);
        
        smallArrow.DORotateQuaternion(minutes, 1f);
    }
}