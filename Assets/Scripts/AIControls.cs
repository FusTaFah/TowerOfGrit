using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is an AI controller, the logic for the AI is implemented here.
//v0.2: walk towards the player and aim at it. if the player is higher than you, jump
public class AIControls : ActorControls {

    GameObject m_gameObjectPlayerReference;
    protected override void Initialise()
    {
        m_gameObjectPlayerReference = GameObject.FindGameObjectWithTag("Player");
    }
    
	protected override void ResolveMovementInputs()
    {
        //is the player to the left or to the right?
        Vector2 vectorToPlayer = m_gameObjectPlayerReference.GetComponent<Rigidbody2D>().position - gameObject.GetComponent<Rigidbody2D>().position;
        if(vectorToPlayer.x > 0.0f)
        {
            m_floatXMovementInput = 1.0f;
        }
        else
        {
            m_floatXMovementInput = -1.0f;
        }
        if (vectorToPlayer.y > 0.0f)
        {
            m_boolJumpInput = true;
        }
        else
        {
            m_boolJumpInput = false;
        }
        m_floatYMovementInput = 0.0f;
    }

    protected override void ResolveAimingInputs()
    {
        Vector2 vectorToPlayer = m_gameObjectPlayerReference.GetComponent<Rigidbody2D>().position - gameObject.GetComponent<Rigidbody2D>().position;
        vectorToPlayer.Normalize();
        m_floatXAimingInput = vectorToPlayer.x;
        m_floatYAimingInput = -vectorToPlayer.y;
    }
}
