using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoBoard : MonoBehaviour
{
    bool uiIsOpen;

    [SerializeField] private Canvas infoUI;
   
    void Start()
    {
        //if (infoUI == null)
        //{
        //    Debug.Log("Missing Reference");
        //    return;
        //}
    }

    
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            uiIsOpen = true;
            infoUI.gameObject.SetActive(uiIsOpen);
            Debug.Log("PlayerNear");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            uiIsOpen = false;
            infoUI.gameObject.SetActive(uiIsOpen);
        }
    }
}
