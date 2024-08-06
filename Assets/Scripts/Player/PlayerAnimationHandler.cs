using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private bool isFacingRight;
    private Vector3 currentScale = Vector3.one;

    //Player Component Reference
    [SerializeField] private PlayerMovement playerMovement;

    private void Update()
    {
        HandleFlip();
        HandleAnimation();
    }

    void HandleFlip()
    {
        if (playerMovement.IsMovingRight == true && !isFacingRight)
        {
            FlipCheck();
        }
        if (playerMovement.IsMovingRight == false && isFacingRight)
        {
            FlipCheck();
        }
    }

    void FlipCheck()
    {
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }

    private void HandleAnimation()
    {
        
    }
}
