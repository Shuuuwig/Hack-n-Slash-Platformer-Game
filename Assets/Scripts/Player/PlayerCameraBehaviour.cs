using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCameraBehaviour : MonoBehaviour
{
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float parryZoomOutSpeed;
    [SerializeField] private float lookAheadDistance;
    [SerializeField] private float lookAheadSpeed;
    [SerializeField] private float yAxisOffset;
    private float smoothTime;
    private float maxSpeed = 5f;
    private float targetPosY;
    private float yDistance;

    [SerializeField] protected Camera mainCamera;
    [SerializeField] protected Transform viewPoint;
    private Vector3 targetPosition;
    private Vector3 updatePosition;
    private Vector3 velocityDamp = Vector3.zero;
    private Vector3 currentLookAhead;

    [SerializeField] protected Image topBar;
    [SerializeField] protected Image bottomBar;

    [SerializeField] protected PlayerCombat combat;
    [SerializeField] protected PlayerParryTrigger parryTrigger;

    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();

        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, viewPoint.position.y + yAxisOffset, mainCamera.transform.position.z);
    }

    void Update()
    {
        ParryZoom();

        yDistance = viewPoint.position.y - mainCamera.transform.position.y;

        if (yDistance > 0)
        {
            targetPosY = mainCamera.transform.position.y + yDistance;
        }
        else if (yDistance < -yAxisOffset)
        {
            targetPosY = viewPoint.position.y + yAxisOffset;
        }
        else
        {
            targetPosY = mainCamera.transform.position.y;
        }

        targetPosition = new Vector3(viewPoint.position.x, targetPosY, mainCamera.transform.position.z);
        currentLookAhead = Vector3.Lerp(currentLookAhead, new Vector3(Mathf.Sign(viewPoint.position.x - transform.position.x) * lookAheadDistance, 0, 0), lookAheadSpeed * Time.deltaTime);
        updatePosition = targetPosition + currentLookAhead;
        smoothTime = Mathf.Lerp(0.5f, 0.8f, Vector3.Distance(mainCamera.transform.position, targetPosition) / 5f);

        mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, updatePosition, ref velocityDamp, smoothTime, maxSpeed, cameraSpeed * Time.deltaTime);
    }

    void ParryZoom()
    {
        if (combat.ParriedAttack)
        {
            viewPoint = parryTrigger.collisionPoint;
            mainCamera.transform.position = new Vector3(viewPoint.transform.position.x, viewPoint.transform.position.y, mainCamera.transform.position.z);
            mainCamera.orthographicSize = Mathf.Lerp(2, 1.5f, Time.unscaledDeltaTime * 1);
            topBar.enabled = true;
            bottomBar.enabled = true;
        }
        else
        {
            if (mainCamera.orthographicSize < 3.5f)
            {
                if (viewPoint != null)
                {
                    viewPoint = null;
                    topBar.enabled = false;
                    bottomBar.enabled = false;
                    viewPoint = GameObject.Find("viewPoint").transform;
                }
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 3.5f, Time.deltaTime * parryZoomOutSpeed);
            }
            
        }
    }
}
