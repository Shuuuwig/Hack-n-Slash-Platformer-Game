using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntingBladeAnimationHandler : MonoBehaviour
{
    private string currentAnimation;

    [SerializeField] private HauntingBlade HauntingBlade;
    [SerializeField] private Animator hauntingBladeAnimator;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleAnimation();
    }

    private void ChangeAnimation(string animation)
    {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            hauntingBladeAnimator.Play(animation);
        }
    }

    private void HandleAnimation()
    {
        if (HauntingBlade.NeutralSlash == true)
        {
            ChangeAnimation("hauntingBladeSlash");
        }

        else
        {
            ChangeAnimation("hauntingBladeIdle");
        }
    }
}
