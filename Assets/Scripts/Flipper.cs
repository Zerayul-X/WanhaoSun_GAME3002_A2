using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper : MonoBehaviour
{
    //user input
    [SerializeField]
    KeyCode key = 0;

    private int BaseAcc = 10000;

    //create the rigitbody and hinge joint
    private Rigidbody FlipRb = null;
    private HingeJoint FlipHinge = null;

    // Start is called before the first frame update
    void Start()
    {
        //initialize the rigitbody and hinge joint
        FlipRb = GetComponent<Rigidbody>();
        FlipHinge = GetComponent<HingeJoint>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(key))
        {
            //accelerating until the maximum angle
            if (FlipHinge.angle < FlipHinge.limits.max)
            {
                Vector3 acc = BaseAcc * FlipHinge.axis * Time.deltaTime;
                FlipRb.AddTorque(acc, ForceMode.Acceleration);
            }
        }
        else
        {
            //accelerating to the opposide until the maximum angle
            if (FlipHinge.angle > FlipHinge.limits.min)
            {
                Vector3 acc = -BaseAcc * FlipHinge.axis * Time.deltaTime;
                FlipRb.AddTorque(acc, ForceMode.Acceleration);
            }
        }
    }
}
