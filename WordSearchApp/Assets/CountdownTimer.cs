using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public float initialTime = 120.0f; // Initial time in seconds
    private float currentTime;
    private static CountdownTimer instance;

    public TMP_Text timerText;

    public static CountdownTimer Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // The timer is initially stopped
    private bool isRunning = false;

    private void Update()
    {
        if (isRunning && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerText();
        }
        else if (currentTime <= 0)
        {
            isRunning = false;
            currentTime = 0;
            // Handle timer completion here (e.g., trigger an event or end the game).
        }
    }

    public void StartTimer(float duration)
    {
        currentTime = duration;
        UpdateTimerText();
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = initialTime;
        UpdateTimerText();
        isRunning = false;
    }

    private void UpdateTimerText()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
        timerText.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
    }
}
