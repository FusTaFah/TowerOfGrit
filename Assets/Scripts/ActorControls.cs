﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//41.151.254.162

public class ActorControls : MonoBehaviour {
    public float            m_floatMovementSpeed;
    public float            m_floatJumpHeight;
    public float            m_floatMaxSlope;
    float                   m_floatXSpeed;
    GameObject              m_gameObjectAimLine;
    Vector3                 m_vector3PlayerScale;
    const float             m_constFloatGroundDistanceQualifier = 0.2f;
    const float             m_constFloatDeadZone = 0.1f;
    const float             m_constFloatHalfMinTerrainCheck = 0.05f;
    const float             m_constFloatColliderToGroundDistance = 0.0159f;
    bool                    m_boolFacingRight;
    bool                    m_boolIsJumping;
    bool                    m_boolIsCrouching;
    protected static DebugUtil     m_debugUtil;
    protected Rigidbody2D   m_rigidBody2DActorRig;
    protected float         m_floatXMovementInput;
    protected bool          m_boolJumpInput;
    protected float         m_floatYMovementInput;
    protected float         m_floatXAimingInput;
    protected float         m_floatYAimingInput;

    protected virtual void Start () {
        m_floatXSpeed = 0.0f;
        m_gameObjectAimLine = Instantiate(Resources.Load("Prefabs/AimLine")) as GameObject;
        m_boolIsCrouching = false;
        m_vector3PlayerScale = gameObject.transform.localScale;
        m_boolIsJumping = false;
        m_rigidBody2DActorRig = gameObject.GetComponent<Rigidbody2D>();
        m_boolFacingRight = true;
        m_debugUtil = GameObject.Find("DebugCanvasElementent").GetComponent<DebugUtil>();
    }

    protected virtual void FixedUpdate()
    {
        ResolveMovementInputs();
        ResolveMovement();
    }

    protected virtual void Update () {
        ResolveAimingInputs();
        ResolveAimLine();
        ResolveFacingDirection();
    }

    private void ResolveMovement()
    {
        float moveX = m_floatXMovementInput;
        if (moveX <= -m_constFloatDeadZone)
        {
            m_boolFacingRight = false;
        }
        else if(moveX >= m_constFloatDeadZone)
        {
            m_boolFacingRight = true;
        }

        m_floatXSpeed = moveX * m_floatMovementSpeed * Time.deltaTime;
        m_floatXSpeed = Mathf.Clamp(m_floatXSpeed, -3.0f, 3.0f);
        Vector2 boundingBoxCenter = new Vector2(m_rigidBody2DActorRig.GetComponent<Collider2D>().bounds.center.x, m_rigidBody2DActorRig.GetComponent<Collider2D>().bounds.center.y);
        SlopeCheckInfo slopeCheckInfo = CheckSlopeAndJump(boundingBoxCenter, m_rigidBody2DActorRig, m_boolFacingRight, m_floatMaxSlope);

        if (slopeCheckInfo.EnableJumping)
        {
            if (slopeCheckInfo.GroundCheckCollider != null || slopeCheckInfo.JumpingOverride)
            {
                if (moveX == 0.0f && !m_boolIsJumping)
                {
                    m_rigidBody2DActorRig.velocity = Vector2.zero;
                    m_floatXSpeed = 0.0f;
                }
                if (m_boolJumpInput)
                {
                    m_boolIsJumping = true;
                    m_rigidBody2DActorRig.velocity = Vector2.zero;
                    m_rigidBody2DActorRig.AddForce(new Vector2(0.0f, m_floatJumpHeight));
                }
            }
            else
            {
                m_boolIsJumping = false;
            }
        }
        if (slopeCheckInfo.EnableMovement)
        {
            m_rigidBody2DActorRig.velocity = new Vector2(m_floatXSpeed * (m_boolIsCrouching ? 0.5f : 1.0f), m_rigidBody2DActorRig.velocity.y);
        }

        float moveY = m_floatYMovementInput;
        if (moveY < -0.5f)
        {
            m_boolIsCrouching = true;
        }
        else
        {
            if (m_boolIsCrouching)
            {
                Vector2 centreOfCrouched = m_rigidBody2DActorRig.GetComponent<Collider2D>().bounds.center;
                float yOffsetToFullHeight = m_rigidBody2DActorRig.GetComponent<Collider2D>().bounds.size.y;
                yOffsetToFullHeight *= 0.5f;
                yOffsetToFullHeight *= 3.0f;
                Vector2 standUpHeight = new Vector2(centreOfCrouched.x, centreOfCrouched.y + yOffsetToFullHeight + 0.1f);
                Vector2 topLeft = standUpHeight - new Vector2(m_rigidBody2DActorRig.GetComponent<Collider2D>().bounds.size.x / 2.0f, 0.0f);
                Vector2 topRight = standUpHeight + new Vector2(m_rigidBody2DActorRig.GetComponent<Collider2D>().bounds.size.x / 2.0f, 0.0f);
                Collider2D crouchTopLine = Physics2D.OverlapArea(topLeft, topRight);
                Debug.DrawLine(topLeft, topRight, Color.blue);

                if (crouchTopLine == null)
                {
                    m_boolIsCrouching = false;
                }
                else
                {
                    m_boolIsCrouching = true;
                }
            }

        }
    }

    private void ResolveAimLine()
    {
        float horizaAim = m_floatXAimingInput;
        float verticAim = m_floatYAimingInput;
        float playerHeight = m_rigidBody2DActorRig.GetComponent<Collider2D>().bounds.size.y;
        Vector2 shoulderOffset = new Vector2(0.0f, playerHeight * m_constFloatDeadZone);
        Vector2 playerMiddlePosition = m_rigidBody2DActorRig.GetComponent<Collider2D>().bounds.center;

        m_gameObjectAimLine.transform.position = playerMiddlePosition + shoulderOffset;
        float angle = MathUtil.FromArgumentToAngle(horizaAim, -verticAim);
        float zeroOffset = gameObject.transform.localScale.x >= 0 ? 0.0f : 180.0f;
        zeroOffset = Mathf.Abs(horizaAim) <= m_constFloatDeadZone && Mathf.Abs(verticAim) <= m_constFloatDeadZone ? zeroOffset : 0.0f;
        if(horizaAim >= m_constFloatDeadZone)
        {
            m_boolFacingRight = true;
        }
        else if(horizaAim <= -m_constFloatDeadZone)
        {
            m_boolFacingRight = false;
        }
        m_gameObjectAimLine.transform.rotation = Quaternion.Euler(0.0f, 0.0f, zeroOffset + angle * Mathf.Rad2Deg);
    }

    public Quaternion GetActorAimLineDirection()
    {
        return m_gameObjectAimLine.transform.rotation;
    }

    private void ResolveFacingDirection()
    {
        m_vector3PlayerScale = new Vector3((m_boolFacingRight ? 1 : -1) * Mathf.Abs(m_vector3PlayerScale.x), m_vector3PlayerScale.y, m_vector3PlayerScale.z);
        
        gameObject.transform.localScale = new Vector3(m_vector3PlayerScale.x, m_vector3PlayerScale.y / (m_boolIsCrouching ? 2.0f : 1.0f), m_vector3PlayerScale.z);
    }

    protected static SlopeCheckInfo CheckSlopeAndJump(Vector2 boundingBoxCenter, Rigidbody2D actorRig, bool facingRight, float maxSlope)
    {

        Vector2 boundingBoxBottomLeft = boundingBoxCenter + new Vector2(-actorRig.GetComponent<Collider2D>().bounds.size.x / 2.0f + m_constFloatGroundDistanceQualifier, -actorRig.GetComponent<Collider2D>().bounds.size.y / 2.0f - m_constFloatGroundDistanceQualifier);
        Vector2 boundingBoxBottomRight = boundingBoxCenter + new Vector2(actorRig.GetComponent<Collider2D>().bounds.size.x / 2.0f - m_constFloatGroundDistanceQualifier, -actorRig.GetComponent<Collider2D>().bounds.size.y / 2.0f - m_constFloatGroundDistanceQualifier);
        Collider2D bottomAreaBoxOverlap = Physics2D.OverlapArea(boundingBoxBottomLeft, boundingBoxBottomRight);


        //Vector2 boundingBoxTopLeft = boundingBoxCenter - new Vector2(actorRig.GetComponent<Collider2D>().bounds.size.x / 2.0f + m_constFloatGroundDistanceQualifier * 1.0f, -actorRig.GetComponent<Collider2D>().bounds.size.y / 2.0f);
        //Vector2 boundingBoxTopRight = boundingBoxCenter - new Vector2(-actorRig.GetComponent<Collider2D>().bounds.size.x / 2.0f - m_constFloatGroundDistanceQualifier * 1.0f, -actorRig.GetComponent<Collider2D>().bounds.size.y / 2.0f);
        //RaycastHit2D slopeCheckLeft = Physics2D.Raycast(boundingBoxTopLeft, Vector2.down, actorRig.GetComponent<Collider2D>().bounds.size.y);
        //RaycastHit2D slopeCheckRight = Physics2D.Raycast(boundingBoxTopRight, Vector2.down, actorRig.GetComponent<Collider2D>().bounds.size.y);
        //Debug.DrawLine(boundingBoxTopLeft, boundingBoxTopLeft + Vector2.down * actorRig.GetComponent<Collider2D>().bounds.size.y, Color.green);
        //Debug.DrawLine(boundingBoxTopRight, boundingBoxTopRight + Vector2.down * actorRig.GetComponent<Collider2D>().bounds.size.y, Color.red);



        //bool enableJumping = true;
        //bool enableMovement = true;
        ////ONLY if the player gets stuck
        //bool jumpingOverride = false;

        //if (slopeCheckLeft.collider != null || slopeCheckRight.collider != null)
        //{
        //    float heightFromFeetLeft = (slopeCheckLeft.collider == null ? 0.0f : actorRig.GetComponent<Collider2D>().bounds.size.y - slopeCheckLeft.distance);
        //    float heightFromFeetRight = (slopeCheckRight.collider == null ? 0.0f : actorRig.GetComponent<Collider2D>().bounds.size.y - slopeCheckRight.distance);

        //    //as of 22/08/2018 it seems the angle is off by approximately 3.3 degrees if m_constFloatGroundDistanceQualifier is 2.0f
        //    float angleBetweenLeft = (Mathf.Atan(heightFromFeetLeft / m_constFloatGroundDistanceQualifier) * Mathf.Rad2Deg) % 80.0f + 3.3f;
        //    float angleBetweenRight = (Mathf.Atan(heightFromFeetRight / m_constFloatGroundDistanceQualifier) * Mathf.Rad2Deg) % 80.0f + 3.3f;
        //    //if (angleBetweenLeft >= 80.0f || angleBetweenRight >= 80.0f) jumpingOverride = true;
        //    if (!facingRight && angleBetweenLeft > maxSlope && angleBetweenRight <= maxSlope)
        //    {
        //        enableJumping = false;
        //        enableMovement = false;
        //    }
        //    else if (!facingRight && angleBetweenLeft > maxSlope && angleBetweenRight > maxSlope)
        //    {
        //        enableJumping = true;
        //        jumpingOverride = true;
        //        enableMovement = false;
        //    }
        //    else if (!facingRight && angleBetweenLeft <= maxSlope && angleBetweenRight > maxSlope)
        //    {
        //        enableJumping = false;
        //        enableMovement = true;
        //    }
        //    else if (facingRight && angleBetweenLeft > maxSlope && angleBetweenRight > maxSlope)
        //    {
        //        enableJumping = true;
        //        jumpingOverride = true;
        //        enableMovement = false;
        //    }
        //    else if (facingRight && angleBetweenLeft <= maxSlope && angleBetweenRight > maxSlope)
        //    {
        //        enableJumping = false;
        //        enableMovement = false;
        //    }

        //}



        //describe the box
        Vector2 boundingBoxBottomLeftSlopeCheck = boundingBoxCenter + new Vector2(-actorRig.GetComponent<Collider2D>().bounds.size.x / 2.0f, -actorRig.GetComponent<Collider2D>().bounds.size.y / 2.0f);
        Vector2 boundingBoxBottomRighSlopeCheck = boundingBoxCenter + new Vector2(+actorRig.GetComponent<Collider2D>().bounds.size.x / 2.0f, -actorRig.GetComponent<Collider2D>().bounds.size.y / 2.0f);
        //OFFSET BOTTOM WITH ANGLED ARG
        //m_constFloatHalfMinTerrainCheck;
        Vector2 argLeft = MathUtil.FromAngleToArgument(maxSlope); argLeft.x = -argLeft.x;
        Vector2 argRigh = MathUtil.FromAngleToArgument(maxSlope);
        Vector2 argLeftGroundEpsilon = MathUtil.FromAngleToArgument(90.0f - maxSlope); argLeftGroundEpsilon.x = -argLeftGroundEpsilon.x; argLeftGroundEpsilon.y = -argLeftGroundEpsilon.y;
        Vector2 argRighGroundEpsilon = MathUtil.FromAngleToArgument(90.0f - maxSlope); argRighGroundEpsilon.y = -argRighGroundEpsilon.y;
        Vector2 resultantLeftCheckPosition = boundingBoxBottomLeftSlopeCheck + argLeft * m_constFloatHalfMinTerrainCheck + argLeftGroundEpsilon * m_constFloatColliderToGroundDistance;
        Vector2 resultantRighCheckPosition = boundingBoxBottomRighSlopeCheck + argRigh * m_constFloatHalfMinTerrainCheck + argRighGroundEpsilon * m_constFloatColliderToGroundDistance;
        m_debugUtil.MarkDebugLocation2D(resultantLeftCheckPosition, Color.green);
        m_debugUtil.MarkDebugLocation2D(resultantRighCheckPosition, Color.red);
        Collider2D leftSlopePointCheck = Physics2D.OverlapPoint(resultantLeftCheckPosition);
        Collider2D righSlopePointCheck = Physics2D.OverlapPoint(resultantRighCheckPosition);

        bool enableJumping = true;
        bool enableMovement = true;
        //ONLY if the actor gets stuck
        bool jumpingOverride = false;
        
        bool isLeftTraversable = leftSlopePointCheck == null;
        bool isRighTraversable = righSlopePointCheck == null;
        m_debugUtil.AppendDebugger(isLeftTraversable.ToString());
        Debug.Log(isLeftTraversable);
        m_debugUtil.AppendDebugger(isRighTraversable.ToString());
        if (!facingRight && !isLeftTraversable && isRighTraversable)
        {
            enableJumping = false;
            enableMovement = false;
        }
        else if (!facingRight && !isLeftTraversable && !isRighTraversable)
        {
            enableJumping = true;
            jumpingOverride = true;
            enableMovement = false;
        }
        else if (!facingRight && isLeftTraversable && !isRighTraversable)
        {
            enableJumping = false;
            enableMovement = true;
        }
        else if (facingRight && !isLeftTraversable && !isRighTraversable)
        {
            enableJumping = true;
            jumpingOverride = true;
            enableMovement = false;
        }
        else if (facingRight && isLeftTraversable && !isRighTraversable)
        {
            enableJumping = false;
            enableMovement = false;
        }

        return new SlopeCheckInfo
        {
            EnableJumping = enableJumping,
            EnableMovement = enableMovement,
            JumpingOverride = jumpingOverride,
            GroundCheckCollider = bottomAreaBoxOverlap
        };
    }

    protected virtual void ResolveAimingInputs()
    {
    }

    protected virtual void ResolveMovementInputs()
    {
    }

    protected struct SlopeCheckInfo
    {
        public bool EnableJumping;
        public bool EnableMovement;
        public bool JumpingOverride;
        public Collider2D GroundCheckCollider;
    }
}
