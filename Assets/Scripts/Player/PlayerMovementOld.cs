using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementOld : MonoBehaviour
{
    //==================== GRAPPLE ====================//
    //    private void Grapple()
    //    {
    //        if (isGrappling == true)
    //        {
    //            if (linkToGrapplePointTime.CurrentProgress is Cooldown.Progress.Ready) //Pause in position for grapple animation
    //            {
    //                linkToGrapplePointTime.StartCooldown();

    //                playerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    //            }
    //            else if (linkToGrapplePointTime.CurrentProgress is Cooldown.Progress.Finished) //Unpause after animation duration
    //            {
    //                storedPlayerMomentum = Vector2.zero;
    //                playerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX & RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

    //                Debug.Log("Start Grappling");

    //                if (targetedGrapplePoint == null)
    //                {
    //                    storedPlayerMomentum = playerRigidbody.velocity;
    //                    return;
    //                }

    //                Vector2 grappleDirection = (transform.position - targetedGrapplePoint.position).normalized;
    //                playerRigidbody.velocity = -grappleDirection * grapplePower;
    //                storedPlayerMomentum = playerRigidbody.velocity;
    //            }
    //        }

    //        //Maintain momentum after colliding with grapple point (Refer to "PLAYER GENERAL COLLISION CHECK > ONTRIGGERENTER2D")
    //        if (keepGrappleMomentum == true)
    //        {
    //            playerRigidbody.velocity = new Vector2(storedPlayerMomentum.x / 1.5f, playerRigidbody.velocity.y);
    //            Debug.Log("Let go of grapple point");
    //        }

    //        if ((isGrounded == true || isClimbingWall == true) && keepGrappleMomentum == true)
    //        {
    //            linkToGrapplePointTime.ResetCooldown();
    //            keepGrappleMomentum = false;
    //        }
    //    }

    //    private void GrappleCheck()
    //    {
    //        if (isDashingForward == true || isDashingBackward == true)
    //            return;

    //        grappleOverlapCircle = Physics2D.OverlapCircle(grappleDetector.position, grappleRadius, grappleLayer);
    //        grappleInvalidCircle = Physics2D.OverlapCircle(grappleDetector.position, grappleInvalidRadius, grappleLayer);

    //        if (grappleOverlapCircle == false) //Detection radius for grapple
    //        {
    //            targetedGrapplePoint = null;
    //            return;
    //        }
    //        else if (grappleInvalidCircle) //Detection radius to check if player is too close to a grapple point
    //        {
    //            tooCloseToGrapplePoint = true;
    //        }
    //        else
    //        {
    //            tooCloseToGrapplePoint = false;
    //            Debug.Log("Near grapple point");
    //        }

    //        grappleRaycastDistance = grappleRadius;
    //        grappleRaycastDirection = grappleOverlapCircle.transform.position - grappleDetector.position;
    //        grappleRaycast = Physics2D.Raycast(grappleDetector.position, grappleRaycastDirection, grappleRaycastDistance, grappleObstacleLayers);


    //        if (grappleRaycast) //Vision line in detection radius to check for obstacles
    //        {
    //            Debug.Log("Grapple point blocked");
    //            targetedGrapplePoint = null;
    //            return;
    //        }
    //        else
    //        {
    //            Debug.Log("Can grapple to point");
    //            if (targetedGrapplePoint == null)
    //            {
    //                targetedGrapplePoint = grappleOverlapCircle.transform;
    //            }

    //            if (Input.GetKey(KeyCode.L) && tooCloseToGrapplePoint == false) //Grapple when not too close to grapple point
    //            {
    //                if (keepGrappleMomentum == false && isGrappling == false)
    //                {
    //                    Debug.Log("Input to grapple");
    //                    isGrappling = true;
    //                }
    //            }
    //        }
    //    }

    //    //==================== POGO ====================//
    //    private void Pogo()
    //    {
    //        if (playerCombat.HitObstacle == false || isGrappling == true)
    //            return;

    //        //Y velocity power set to opposite direction of attack contact point
    //        if (playerCombat.AirOverheadAttack == true)
    //        {
    //            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, -pogoPower);
    //        }
    //        else if (playerCombat.AirLowAttack == true)
    //        {
    //            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, pogoPower);
    //            Debug.Log("Pogo Up");
    //        }
    //        else if (playerCombat.NeutralAttack == true)
    //        {
    //            if (transform.localScale.x > 0)
    //            {
    //                Debug.Log("Attack");
    //                playerRigidbody.velocity = new Vector2(-pogoPower, playerRigidbody.velocity.y);
    //            }
    //            else if (transform.localScale.x < 0)
    //            {
    //                playerRigidbody.velocity = new Vector2(pogoPower, playerRigidbody.velocity.y);
    //            }
    //        }
    //    }
}
