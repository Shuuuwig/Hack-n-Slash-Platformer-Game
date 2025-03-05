using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputTracker : MonoBehaviour
{
    private int inputTally;
    [SerializeField] private Cooldown InputTallyWindow;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            inputTally++;
            Debug.Log(inputTally);
        }

        if (InputTallyWindow.CurrentProgress is Cooldown.Progress.Ready && inputTally > 0)
        {
            InputTallyWindow.StartCooldown();
        }

        if (InputTallyWindow.CurrentProgress is Cooldown.Progress.Finished)
        {
            inputTally = 0;
            InputTallyWindow.ResetCooldown();
        }
    }
}
