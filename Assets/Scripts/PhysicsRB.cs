using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRB : MonoBehaviour
{
    public float fBodyMass = 1f;
    public float fBounciness = 1;
    public bool bObeysGravity = true;
    public Vector2 vGravity = new Vector2(0f, -9.8f);

    public Vector2 vCurrentVelocity;
    public Vector2 vMaxVelocity = new Vector2(10f, 10f);

    public bool bGrounded;

    [SerializeField]
    private bool bIsKinematic = false;

    private Vector2 vTotalForces;

    [SerializeField]
    private PhysicsEngineLite m_engine;

    public struct AABB
    {
        public Vector2 vBottomLeft;
        public Vector2 vTopRight;
    }
    public AABB aabb;

    private Vector2 vDebugBounds = Vector2.zero;
    private Vector2 vDebugBoundBotLeft;
    private Vector2 vDebugBoundTopRight;

    public void AddForce(Vector2 vForce)
    {
        vTotalForces += vForce;
    }

    public void Stop()
    {
        vCurrentVelocity = Vector2.zero;
        vTotalForces = Vector2.zero;
    }

    public bool IsGrounded()
    {
        bGrounded = m_engine.IsGrounded(this);
        return bGrounded;
    }

    void SetAABB()
    {
        Bounds bound = new Bounds(new Vector2(0f, 0f), new Vector2(1f, 1f));

        Renderer renderer = GetComponent<Renderer>();

        if (renderer)
        {
            bound = renderer.bounds;
        }

        aabb.vBottomLeft = new Vector2(bound.center.x - bound.extents.x, bound.center.y - bound.extents.y);
        aabb.vTopRight = new Vector2(bound.center.x + bound.extents.x, bound.center.y + bound.extents.y);

        vDebugBoundBotLeft = aabb.vBottomLeft;
        vDebugBoundTopRight = aabb.vTopRight;
        vDebugBounds = bound.center;
    }

    void Start()
    {
        SetAABB();
        m_engine.AddRigidBody(this);
    }

    public void Integrate(float deltaTime)
    {
        if (bIsKinematic)
        {
            return;
        }

        Vector2 acceleration = new Vector2();

        if (bObeysGravity && !IsGrounded())
        {
            acceleration = vGravity;
        }
        else
        {
            if (Mathf.Abs(vCurrentVelocity.y) < 0.05f)
            {
                vCurrentVelocity.y = 0;
            } 
        }

        if (fBodyMass == 0)
        {
            acceleration = Vector2.zero;
        }
		else
		{
			acceleration += vTotalForces / fBodyMass;
		}

        vCurrentVelocity += acceleration * deltaTime;
        vCurrentVelocity = Vector2.Min(vCurrentVelocity, vMaxVelocity);

        Vector2 vTempPos = transform.position;
        vTempPos += vCurrentVelocity * deltaTime;

        transform.position = vTempPos;

        SetAABB();

        vTotalForces = Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        if (vDebugBounds != Vector2.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(vDebugBounds, new Vector2(transform.lossyScale.x, transform.lossyScale.y));
            Gizmos.DrawWireSphere(vDebugBoundBotLeft, 0.1f);
            Gizmos.DrawWireSphere(vDebugBoundTopRight, 0.1f);
        }
    }
}
