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
        Debug.Log("collission " + collision.gameObject);
        if (collision.CompareTag("Player"))
        {
            Debug.Log(message: "loading next level");
            GameManager.Instance.ToNextLevel(nextSection);
        }
    }
}
