using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextSection : MonoBehaviour
{
    [SerializeField] private int nextSection;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.ToNextLevel(nextSection);
        }
    }
}
