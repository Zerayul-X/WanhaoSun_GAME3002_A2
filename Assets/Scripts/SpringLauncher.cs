using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringLauncher : MonoBehaviour
{
    //attach the spring to the spring launcher
    [SerializeField]
    private Rigidbody launcher = null;

    [SerializeField]
    private float SpringFactor;

    private Vector3 launchPosition;
    private Vector3 acc;

    private bool ifLaunching = false;

    // Start is called before the first frame update
    void Start()
    {
        launchPosition = transform.position - launcher.position;
    }


    // Update is called once per frame
    void Update()
    {
        ////if key is pressed, bring the launcher down
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    ifLaunching = false;
        //    launcher.velocity = -launchPosition.normalized * 2;
        //}

        ////if key is released, realase the launcher
        //if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    launcher.velocity = Vector3.zero;
        //    ifLaunching = true;
        //}
    }
    void FixedUpdate()
    {
        acc = SpringFactor * (transform.position - launcher.transform.position);
        launcher.AddForce(acc, ForceMode.Acceleration);
        //add the force to the launcher, launch the ball
        //if (ifLaunching)
        //{
        //    acc = SpringFactor * (transform.position - launcher.transform.position);
        //    launcher.AddForce(acc, ForceMode.Acceleration);
        //}
    }
}

