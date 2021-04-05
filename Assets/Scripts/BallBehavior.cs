using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BallBehavior : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ScoreText = null;

    [SerializeField]
    private TextMeshProUGUI LifeText = null;

    [SerializeField]
    Transform spawnPoint = null;

    private Rigidbody BallRb = null;

    private Vector3 newVelocity = Vector3.zero;
    private Vector3 oldVelocity = Vector3.zero; 
    
    public int life = 3;
    public int score = 0;
    // Start is called before the first frame update
    void Start()
    {
        BallRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //update the velocity
        oldVelocity = BallRb.velocity;
        //in case if the ball fly out without touching the bottom, player wont lose life for this
        if(BallRb.position.z >= 6)
        {
            transform.position = spawnPoint.position;
        }
        //update the score and life to UI
        ScoreText.text = "Score: " + score;
        LifeText.text = "Life: " + life;

    }

    private void OnCollisionEnter(Collision collision)
    {
        //reflect the velocity of the ball for different objects
        //update the score after each collsion
        if (collision.gameObject.tag == "ActiveBumper")
        {
            newVelocity = Vector3.Reflect(oldVelocity * 2, collision.GetContact(0).normal);
            BallRb.velocity = newVelocity;
            score = score + 200;
        }
        if (collision.gameObject.tag == "OverActive")
        {
            newVelocity = Vector3.Reflect(oldVelocity * 3, collision.GetContact(0).normal);
            BallRb.velocity = newVelocity;
            score = score + 500;
        }
        if (collision.gameObject.tag == "OuterWall")
        {
            newVelocity = Vector3.Reflect(oldVelocity / 2, collision.GetContact(0).normal);
            BallRb.velocity = newVelocity;
            score = score + 10;
        }
        if (collision.gameObject.tag == "InnerWall")
        {
            newVelocity = Vector3.Reflect(oldVelocity, collision.GetContact(0).normal);
            BallRb.velocity = newVelocity; 
            score = score + 50;
        }
        if (collision.gameObject.tag == "BashToy")
        {
            newVelocity = Vector3.Reflect(oldVelocity, collision.GetContact(0).normal);
            BallRb.velocity = newVelocity;
            score = score + 1000;
        }
        if (collision.gameObject.tag == "Bumper")
        {
            score = score + 100;
        }
        //respawn the ball if the ball hit the bottom of box
        if (collision.gameObject.tag == "OB")
        {
            if (life > 0)
            {
                transform.position = spawnPoint.position;
                life = life - 1;
            }
        }
    }
}
