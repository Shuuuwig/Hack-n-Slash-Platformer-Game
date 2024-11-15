using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoBoard : MonoBehaviour
{
    bool uiIsOpen;

    [SerializeField] private Canvas nearToInfo;
    [SerializeField] private Canvas infoUI;
    [SerializeField] private Cooldown interactCooldown;
   
    void Start()
    {
        if (nearToInfo == null || infoUI == null)
        {
            Debug.Log("Missing Reference");
            return;
        }
    }

    
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        ShowInfo();
    }

    private void ShowInfo()
    {
        if (nearToInfo.gameObject.activeSelf == false)
            return;

        if (Input.GetKey(KeyCode.E) && interactCooldown.CurrentProgress is Cooldown.Progress.Ready)
        {
            uiIsOpen = !uiIsOpen;
            infoUI.gameObject.SetActive(uiIsOpen);
            interactCooldown.StartCooldown();
        }
        
        if (interactCooldown.CurrentProgress is Cooldown.Progress.Finished)
        {
            interactCooldown.ResetCooldown();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            nearToInfo.gameObject.SetActive(true);
            Debug.Log("PlayerNear");
        }
        else
        {
            nearToInfo.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            nearToInfo.gameObject.SetActive(false);
            uiIsOpen = false;
            infoUI.gameObject.SetActive(uiIsOpen);
        }
    }
}
