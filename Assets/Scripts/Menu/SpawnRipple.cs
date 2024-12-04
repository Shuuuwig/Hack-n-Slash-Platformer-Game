using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Vector3 mouseposition;
    public ParticleSystem ripple;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseposition.z = 0f;
            ParticleSystem rippleclone = Instantiate(ripple, mouseposition, Quaternion.identity);
            Destroy(rippleclone.gameObject, 1f);
        }
        
    }
}
