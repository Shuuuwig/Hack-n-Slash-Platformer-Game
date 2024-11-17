using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputTally : MonoBehaviour
{
    private float upInputTally;
    private float downInputTally;
    private float leftInputTally;
    private float rightInputTally;

    public float UpInputTally { get { return upInputTally; } }
    public float DownInputTally { get { return downInputTally; } }
    public float LeftInputTally { get { return leftInputTally; } }
    public float RightInputTally { get { return rightInputTally; } }

    [SerializeField] private Cooldown buttonInputWindow;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            upInputTally++;
            Debug.Log($"Up tally:{upInputTally}");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            downInputTally++;
            Debug.Log($"Down tally:{downInputTally}");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            leftInputTally++;
            Debug.Log($"Left tally:{leftInputTally}");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            rightInputTally++;
            Debug.Log($"Right tally:{rightInputTally}");
        }

        if (buttonInputWindow.CurrentProgress is Cooldown.Progress.Ready)
        {
            if (upInputTally > 0 || downInputTally > 0 || leftInputTally > 0 || rightInputTally > 0)
            {
                buttonInputWindow.StartCooldown();
            }
            
        }

        if (buttonInputWindow.CurrentProgress is Cooldown.Progress.Finished)
        {
            upInputTally = 0;
            downInputTally = 0;
            leftInputTally = 0;
            rightInputTally = 0;
            
            buttonInputWindow.ResetCooldown();
        }
    }
}
