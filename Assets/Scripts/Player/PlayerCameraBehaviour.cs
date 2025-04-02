using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCameraBehaviour : MonoBehaviour
{
    [SerializeField] protected float cameraSpeed;
    [SerializeField] protected Camera mainCamera;
    [SerializeField] protected Transform viewPoint;
    private Vector3 updatePosition;

    [SerializeField] private float yAxisOffset; 

    private bool cameraLock;


    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
    }

    void Update()
    {
        updatePosition = new Vector3(viewPoint.position.x, transform.position.y + yAxisOffset, -10f);

        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, updatePosition, cameraSpeed * Time.deltaTime);
    }
}
