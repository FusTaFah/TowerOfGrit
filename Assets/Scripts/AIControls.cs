using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//this is an AI controller, the logic for the AI is implemented here.
//v0.3: walk towards the player and aim at it. 
//if the player is behind an obstruction, jump over it
//if the player is at a certain distance, aim opposite to where it is aiming
public class AIControls : ActorControls {

    public float        m_floatCombatDistance;
    GameObject          m_gameObjectPlayerReference;
    bool                m_boolCombatCondition;

    protected override void Start()
    {
        base.Start();
        m_gameObjectPlayerReference = GameObject.FindGameObjectWithTag("Player");
    }

    protected override void Update()
    {
        Vector2 vectorToPlayer = m_gameObjectPlayerReference.GetComponent<Rigidbody2D>().position - gameObject.GetComponent<Rigidbody2D>().position;
        m_boolCombatCondition = vectorToPlayer.magnitude <= 2.0f;
        base.Update();
    }

    protected override void ResolveMovementInputs()
    {
        

        if (m_boolCombatCondition)
        {
            CombatMovementBehaviour();
        }
        else
        {
            PathingMovementBehaviour();
        }
    }

    protected override void ResolveAimingInputs()
    {
        
        if (m_boolCombatCondition)
        {
            CombatAimingBehaviour();
        }
        else
        {
            MovementAimingBehaviour();
        }
    }

    private void CombatMovementBehaviour()
    {
        
        
    }

    private void PathingMovementBehaviour()
    {
        //is the player to the left or to the right?
        Vector2 vectorToPlayer = m_gameObjectPlayerReference.GetComponent<Rigidbody2D>().position - gameObject.GetComponent<Rigidbody2D>().position;
        if (vectorToPlayer.x > 0.0f)
        {
            m_floatXMovementInput = 1.0f;
        }
        else
        {
            m_floatXMovementInput = -1.0f;
        }
        //check for obstructions
        
        RaycastHit2D frontObsCheck = Physics2D.Raycast(m_rigidBody2DActorRig.position + (m_floatXMovementInput > 0.0f ? 1.0f : -1.0f) * new Vector2(m_rigidBody2DActorRig.GetComponent<Collider2D>().bounds.size.x * 0.5f + 0.05f, 0.0f), new Vector2(m_floatXMovementInput > 0.0f ? 1.0f : -1.0f, 0.0f), 2.0f);
        if (frontObsCheck.collider != null && frontObsCheck.collider.gameObject != null && frontObsCheck.collider.gameObject != m_gameObjectPlayerReference)
        {
            m_boolJumpInput = true;
        }
        else
        {
            m_boolJumpInput = false;
        }
        m_floatYMovementInput = 0.0f;
    }

    private void CombatAimingBehaviour()
    {
        //find out where the player is aiming
        float angle = m_gameObjectPlayerReference.GetComponent<PlayerControls>().GetActorAimLineDirection().eulerAngles.z;
        Vector2 directionOfAimLine = MathUtil.FromAngleToArgument(angle);
        m_floatXAimingInput = -directionOfAimLine.x;
        m_floatYAimingInput = directionOfAimLine.y;
    }

    private void MovementAimingBehaviour()
    {
        //Vector2 vectorToPlayer = m_gameObjectPlayerReference.GetComponent<Rigidbody2D>().position - gameObject.GetComponent<Rigidbody2D>().position;
        //vectorToPlayer.Normalize();
        //m_floatXAimingInput = vectorToPlayer.x;
        //m_floatYAimingInput = -vectorToPlayer.y;
        m_floatXAimingInput = 0.0f;
        m_floatYAimingInput = 0.0f;
    }

    protected enum AIState
    {
        IDLE,
        PATROLLING,
        COMBATCHASE,
        COMBATENGAGE,
    }
}
