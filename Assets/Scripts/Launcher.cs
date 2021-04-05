using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField]
    Transform reset = null;

    private void OnCollisionEnter(Collision collision)
    {
        //if the laucher hit the stopper, tansform it back to the original position
        if (collision.gameObject.tag == "Stopper")
        {
            transform.position = reset.position;
        }
    }
}