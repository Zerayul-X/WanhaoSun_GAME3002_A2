using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField]
    Transform spawnPoint1 = null;
    [SerializeField]
    Transform spawnPoint2 = null;
    [SerializeField]
    Transform spawnPoint3 = null;
    [SerializeField] 
    float acc = 100f;
    [SerializeField] 
    float jumpFactor = 100f;
    [SerializeField]
    private TextMeshProUGUI TimerText = null;
    [SerializeField]
    private TextMeshProUGUI Victory = null;
    [SerializeField]
    private TextMeshProUGUI Fail = null;

    private Rigidbody playerRB = null;
    private bool ifKey1 = false;
    private bool ifKey2 = false;
    private bool ifLevel1Clear = false;
    private bool ifLevel2Clear = false;
    private float slowAcc, fastAcc, stdAcc, stdJump, slowJump;
    private int jumpRemain = 2;
    private Vector3 movement = Vector3.zero;
    public float time = 120;
    private bool ifVictory = false;
    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        //diferent acceleration
        slowAcc = acc / 2;
        fastAcc = acc * 5;
        stdAcc = acc;
        //different jump factor
        stdJump = jumpFactor;
        slowJump = jumpFactor / 3;
        //set the victory and fail text to blank
        Victory.text = "";
        Fail.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        //setup the timer for the game
        if (!ifVictory)
        {
            if (time > 0)
            {
                time -= Time.deltaTime;
            }
            else
            {
                time = 0;
                Fail.text = "Failed...";
            }
            TimerText.text = "Time Remaining: " + time;
        }
        //jump movement
        if (Input.GetKeyDown(KeyCode.Space) && jumpRemain > 0)
        {
            if (jumpRemain > 0)
            {
                jumpRemain = jumpRemain - 1;
                playerRB.AddForce(Vector3.up * jumpFactor, ForceMode.Impulse);
            }
        }
    }

    void FixedUpdate()
    {
        //left and right movement
        movement.x = Input.GetAxis("Horizontal");
        if (movement.x != 0)
        {
            playerRB.AddForce(-movement * acc, ForceMode.Force);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //if player touch the spike, he will go back to where the level begin
        if (collision.gameObject.tag == "Spike")
        {
            if (!ifLevel1Clear)
            {
                transform.position = spawnPoint1.position;
            }
            else
            {
                if (!ifLevel2Clear)
                {
                    transform.position = spawnPoint2.position;
                }
                else
                {
                    transform.position = spawnPoint3.position;
                }
            }
        }
        //reset the jump when player is on the ground
        if (collision.contacts[0].normal.y >= 1.0f)
        {
            jumpRemain = 2;
        }
        //if player have the key, destroy the gate and set the level clear state to true
        if (collision.gameObject.tag == "Gate1")
        {
            if (ifKey1)
            {
                ifLevel1Clear = true;
                Destroy(collision.gameObject);
            }
        }
        if (collision.gameObject.tag == "Gate2")
        {
            if (ifKey2)
            {
                ifLevel2Clear = true;
                Destroy(collision.gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        //speed up zone, make the acceleration faster
        if (collision.gameObject.tag == "SpeedUp")
        {
            acc = fastAcc;
        }
        //slow down zone, player barely able to jump, and accelerating very slowly
        if (collision.gameObject.tag == "SlowDown")
        {
            acc = slowAcc;
            jumpFactor = slowJump;
        }
        //change the key state and destroy the object
        if (collision.gameObject.tag == "Key1")
        {
            ifKey1 = true; 
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "Key2")
        {
            ifKey2 = true;
            Destroy(collision.gameObject);
        }
        //win the game
        if (collision.gameObject.tag == "prize")
        {
            Victory.text = "Victory!";
            ifVictory = true;
            Destroy(collision.gameObject);
        }
    }

    void OnTriggerExit(Collider collision)
    {
        //if player exited the trigger(slow down and speed up), 
        //the acceleration and jump factor will be back to normal
        acc = stdAcc;
        jumpFactor = stdJump;
    }
}

