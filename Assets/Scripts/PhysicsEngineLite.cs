using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsEngineLite : MonoBehaviour
{
    public float fGroundedTolerance = 0.1f;

    public struct CollisionPair
    {
        public PhysicsRB rigidBodyA;
        public PhysicsRB rigidBodyB;
    }

    public struct CollisionInfo
    {
        public Vector2 vCollisionNormal;
        public float fPenetration;
    }

    private Dictionary<CollisionPair, CollisionInfo> collisions = new Dictionary<CollisionPair, CollisionInfo>();
    private List<PhysicsRB> rigidBodiesList = new List<PhysicsRB>();

    public void AddRigidBody(PhysicsRB rigidBody)
    {
        rigidBodiesList.Add(rigidBody);
    }

    void IntegrateBodies(float dT)
    {
        foreach (PhysicsRB rb in rigidBodiesList)
        {
            rb.Integrate(dT);
        }
    }

    public bool IsGrounded(PhysicsRB rigidBody)
    {
        foreach (PhysicsRB rb in rigidBodiesList)
        {
            if (rb != rigidBody)
            {
                if (rigidBody.aabb.vBottomLeft.x < rb.aabb.vTopRight.x 
                    && rigidBody.aabb.vTopRight.x > rb.aabb.vBottomLeft.x
                    && Mathf.Abs(rigidBody.aabb.vBottomLeft.y - rb.aabb.vTopRight.y) <= fGroundedTolerance)
                {
                    if (Mathf.Abs(rigidBody.vCurrentVelocity.y) < fGroundedTolerance)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    void CheckCollisions()
    {
        foreach (PhysicsRB bodyA in rigidBodiesList.GetRange(0, rigidBodiesList.Count - 1))
        {
            foreach (PhysicsRB bodyB in rigidBodiesList.GetRange(rigidBodiesList.IndexOf(bodyA), rigidBodiesList.Count - rigidBodiesList.IndexOf(bodyA)))
            {
                if (bodyA != bodyB)
                {
                    CollisionPair pair = new CollisionPair();
                    CollisionInfo colInfo = new CollisionInfo();

                    pair.rigidBodyA = bodyA;
                    pair.rigidBodyB = bodyB;

                    Vector2 vDistance = bodyB.transform.position - bodyA.transform.position;

                    Vector2 vHalfSizeA = (bodyA.aabb.vTopRight - bodyA.aabb.vBottomLeft) / 2;
                    Vector2 vHalfSizeB = (bodyB.aabb.vTopRight - bodyB.aabb.vBottomLeft) / 2;

                    Vector2 vGap = new Vector2(Mathf.Abs(vDistance.x), Mathf.Abs(vDistance.y)) - (vHalfSizeA + vHalfSizeB);

                    // Separating-Axis Theorem test
                    if (vGap.x < 0 && vGap.y < 0)
                    {
                        Debug.Log("Collided!!!");

                        if (collisions.ContainsKey(pair))
                        {
                            collisions.Remove(pair);
                        }

                        if (vGap.x > vGap.y)
                        {
                            if (vDistance.x > 0)
                            {
                                colInfo.vCollisionNormal.x = 1;
                            }
                            else
                            {
                                colInfo.vCollisionNormal.x = -1;
                            }

                            colInfo.fPenetration = vGap.x;
                        }
                        else
                        {
                            if (vDistance.y > 0)
                            {
                                colInfo.vCollisionNormal.y = 1;
                            }
                            else
                            {
                                colInfo.vCollisionNormal.y = -1;
                            }

                            colInfo.fPenetration = vGap.y;
                        }

                        collisions.Add(pair, colInfo);
                    }
                    else if (collisions.ContainsKey(pair))
                    {
                        Debug.Log("Removed from pair!");
                        collisions.Remove(pair);
                    }
                }
            }
        }
    }

    void ResolveCollisions()
    {
        foreach (CollisionPair pair in collisions.Keys)
        {
            float fMinBounce = Mathf.Min(pair.rigidBodyA.fBounciness, pair.rigidBodyB.fBounciness);

            float fVelAlongNormal = Vector2.Dot(pair.rigidBodyB.vCurrentVelocity -
                pair.rigidBodyA.vCurrentVelocity, collisions[pair].vCollisionNormal);

            if (fVelAlongNormal > 0)
            {
                continue;
            } 
           
            float fImpForce = -(1 + fMinBounce) * fVelAlongNormal;
            float fInvMassA, fInvMassB;

            if (pair.rigidBodyA.fBodyMass == 0)
            {
                fInvMassA = 0;
            }
            else
            {
                fInvMassA = 1 / pair.rigidBodyA.fBodyMass;
            }

            if (pair.rigidBodyB.fBodyMass == 0)
            {
                fInvMassB = 0;
            }
            else
            {
                fInvMassB = 1 / pair.rigidBodyB.fBodyMass;
            }

            fImpForce /= fInvMassA + fInvMassB;

            Vector2 vImpulse = fImpForce * collisions[pair].vCollisionNormal;

            Vector2 vAccel1 = vImpulse / Time.fixedDeltaTime;
            Vector2 vAccel2 = vImpulse / Time.fixedDeltaTime;

            pair.rigidBodyA.AddForce(vAccel1 * -1);
            pair.rigidBodyB.AddForce(vAccel2);

            if (Mathf.Abs(collisions[pair].fPenetration) > 0.001f)
            {
                PositionalCorrection(pair);
            }
        }
    }

    void PositionalCorrection(CollisionPair collPair)
    {
        const float fPercent = 0.2f;

        float fInvMassA, fInvMassB;
        if (collPair.rigidBodyA.fBodyMass == 0)
        {
            fInvMassA = 0;
        }
        else
        {
            fInvMassA = 1 / collPair.rigidBodyA.fBodyMass;
        }

        if (collPair.rigidBodyB.fBodyMass == 0)
        {
            fInvMassB = 0;
        }
        else
        {
            fInvMassB = 1 / collPair.rigidBodyB.fBodyMass;
        }

        Vector2 vCorrection = ((collisions[collPair].fPenetration / (fInvMassA + fInvMassB)) * fPercent) * -collisions[collPair].vCollisionNormal;

        Vector2 vTemp = collPair.rigidBodyA.transform.position;
        vTemp -= fInvMassA * vCorrection;
        collPair.rigidBodyA.transform.position = vTemp;

        vTemp = collPair.rigidBodyB.transform.position;
        vTemp += fInvMassB * vCorrection;
        collPair.rigidBodyB.transform.position = vTemp;
    }

    void UpdatePhysics()
    {
        IntegrateBodies(Time.fixedDeltaTime);
        CheckCollisions();
        ResolveCollisions();
    }

    void FixedUpdate()
    {
        UpdatePhysics();
    }
}
