using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Cooldown
{
    //Cooldown States List
    public enum Progress { Ready, InProgress, Finished }
    public Progress CurrentProgress = Progress.Ready;

    //Input Value
    public float Duration;

    //Variables
    private float currentDuration = 0f;
    private bool isOnCooldown = false;

    public float TimeLeft { get { return currentDuration; } }
    public bool IsOnCooldown { get { return isOnCooldown; } }

    private Coroutine _coroutine;

    public void StartCooldown()
    {
        if (CurrentProgress is Progress.InProgress)
            return;

        _coroutine = CoroutineHost.Instance.StartCoroutine(DoCooldown());
    }

    public void StartCooldownRealtime()
    {
        if (CurrentProgress is Progress.InProgress)
            return;

        _coroutine = CoroutineHost.Instance.StartCoroutine(DoCooldownRealtime());
    }

    public void ResetCooldown()
    {
        if (_coroutine != null)
            CoroutineHost.Instance.StopCoroutine(_coroutine);

        currentDuration = 0f;
        isOnCooldown = false;
        CurrentProgress = Progress.Ready;
    }

    IEnumerator DoCooldown()
    {
        currentDuration = Duration;
        isOnCooldown = true;

        while (currentDuration > 0f)
        {
            currentDuration -= Time.deltaTime;
            CurrentProgress = Progress.InProgress;

            yield return null;
        }

        currentDuration = 0f;
        isOnCooldown = false;

        CurrentProgress = Progress.Finished;
    }

    IEnumerator DoCooldownRealtime()
    {
        currentDuration = Duration;
        isOnCooldown = true;

        while (currentDuration > 0f)
        {
            currentDuration -= Time.unscaledDeltaTime;
            CurrentProgress = Progress.InProgress;
            Debug.Log(currentDuration);

            yield return null;
        }

        currentDuration = 0f;
        isOnCooldown = false;

        CurrentProgress = Progress.Finished;
    }
}
