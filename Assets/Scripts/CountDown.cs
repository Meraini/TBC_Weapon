using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    [SerializeField] private Image _timerImg;

    private float _currentTime;

    [SerializeField] private float _duration;

    private bool _isTimerRunning = false;

    // Event to notify when the timer ends
    public delegate void TimerEnded();
    public event TimerEnded OnTimerEnded;

    void Start()
    {
        StartTimer();
    }

    public void StartTimer()
    {
        _currentTime = _duration;
        _isTimerRunning = true;
        StartCoroutine(UpdateTime());
    }

    public void AddTime(float timeToAdd)
    {
        _currentTime += timeToAdd;
        if (_currentTime > _duration)
        {
            _currentTime = _duration;
        }
    }

    public void StopTimer()
    {
        _isTimerRunning = false;
    }

    private IEnumerator UpdateTime()
    {
        while (_currentTime >= 0 && _isTimerRunning)
        {
            _timerImg.fillAmount = Mathf.InverseLerp(0, _duration, _currentTime);
            yield return new WaitForSeconds(1f);
            _currentTime--;

            // Ensure the timer image updates after decrementing
            _timerImg.fillAmount = Mathf.InverseLerp(0, _duration, _currentTime);
        }

        _timerImg.fillAmount = 0;
        _isTimerRunning = false;

        // Invoke the event when the timer ends
        if (OnTimerEnded != null)
        {
            OnTimerEnded.Invoke();
        }
    }
}
