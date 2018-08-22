using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//41.151.254.162

public class PlayerControls : MonoBehaviour {
    public float    m_floatMovementSpeed;
    public float    m_floatJumpHeight;
    public float    m_floatMaxSlope;
    float           m_floatXSpeed;
    GameObject      m_gameObjectAimLine;
    Vector3         m_vector3PlayerScale;
    const float     m_constFloatGroundDistanceQualifier = 0.2f;
    const float     m_constFloatDeadZone = 0.1f;
    Rigidbody2D     m_rigidBody2DplayerRig;
    bool            m_boolFacingRight;
    bool            m_boolIsJumping;
    bool            m_boolIsCrouching;
    DebugUtil       m_debugUtil;

    void Start () {
        m_floatXSpeed = 0.0f;
        m_gameObjectAimLine = GameObject.Find("AimLine");
        m_boolIsCrouching = false;
        m_vector3PlayerScale = gameObject.transform.localScale;
        m_boolIsJumping = false;
        m_rigidBody2DplayerRig = gameObject.GetComponent<Rigidbody2D>();
        m_boolFacingRight = true;
        m_debugUtil = GameObject.Find("DebugCanvasElementent").GetComponent<DebugUtil>();
    }
	
    void FixedUpdate()
    {
        ResolveMovement();
    }

    void Update () {
        ResolveAimLine();
        ResolveFacingDirection();
    }

    private void ResolveMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
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
        Vector2 boundingBoxCenter = new Vector2(m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.center.x, m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.center.y);
        Vector2 boundingBoxBottomLeft = boundingBoxCenter - new Vector2(m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.x / 2.0f - m_constFloatGroundDistanceQualifier, m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y / 2.0f + m_constFloatGroundDistanceQualifier);
        Vector2 boundingBoxBottomRight = boundingBoxCenter - new Vector2(-m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.x / 2.0f + m_constFloatGroundDistanceQualifier, m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y / 2.0f + m_constFloatGroundDistanceQualifier);
        Collider2D rch = Physics2D.OverlapArea(boundingBoxBottomLeft, boundingBoxBottomRight);

        
        Vector2 boundingBoxTopLeft = boundingBoxCenter - new Vector2(m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.x / 2.0f + m_constFloatGroundDistanceQualifier * 1.0f, -m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y / 2.0f);
        Vector2 boundingBoxTopRight = boundingBoxCenter - new Vector2(-m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.x / 2.0f - m_constFloatGroundDistanceQualifier * 1.0f, -m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y / 2.0f);
        RaycastHit2D slopeCheckLeft = Physics2D.Raycast(boundingBoxTopLeft, Vector2.down, m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y);
        RaycastHit2D slopeCheckRight = Physics2D.Raycast(boundingBoxTopRight, Vector2.down, m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y);
        Debug.DrawLine(boundingBoxTopLeft, boundingBoxTopLeft + Vector2.down * m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y, Color.green);
        Debug.DrawLine(boundingBoxTopRight, boundingBoxTopRight + Vector2.down * m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y, Color.red);

        bool enableJumping = true;
        bool enableMovement = true;
        //ONLY if the player gets stuck
        bool jumpingOverride = false;

        if(slopeCheckLeft.collider != null || slopeCheckRight.collider != null)
        {
            float heightFromFeetLeft = (slopeCheckLeft.collider == null ? 0.0f : m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y - slopeCheckLeft.distance);
            float heightFromFeetRight = (slopeCheckRight.collider == null ? 0.0f : m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y - slopeCheckRight.distance);

            //as of 22/08/2018 it seems the angle is off by approximately 3.3 degrees if m_constFloatGroundDistanceQualifier is 2.0f
            float angleBetweenLeft = (Mathf.Atan(heightFromFeetLeft / m_constFloatGroundDistanceQualifier) * Mathf.Rad2Deg) % 80.0f + 3.3f;
            float angleBetweenRight = (Mathf.Atan(heightFromFeetRight / m_constFloatGroundDistanceQualifier) * Mathf.Rad2Deg) % 80.0f + 3.3f;
            //if (angleBetweenLeft >= 80.0f || angleBetweenRight >= 80.0f) jumpingOverride = true;
            m_debugUtil.AppendDebugger(string.Format("angle between left = {0}\nangle between right = {1}", angleBetweenLeft, angleBetweenRight));
            if (!m_boolFacingRight && angleBetweenLeft > m_floatMaxSlope && angleBetweenRight <= m_floatMaxSlope)
            {
                enableJumping = false;
                enableMovement = false;
                m_debugUtil.AppendDebugger("00");
            }
            else if (!m_boolFacingRight && angleBetweenLeft > m_floatMaxSlope && angleBetweenRight > m_floatMaxSlope)
            {
                enableJumping = true;
                jumpingOverride = true;
                enableMovement = false;
                m_debugUtil.AppendDebugger("01");
            }
            else if (!m_boolFacingRight && angleBetweenLeft <= m_floatMaxSlope && angleBetweenRight > m_floatMaxSlope)
            {
                enableJumping = false;
                enableMovement = true;
                m_debugUtil.AppendDebugger("02");
            }
            else if (m_boolFacingRight && angleBetweenLeft > m_floatMaxSlope && angleBetweenRight > m_floatMaxSlope)
            {
                enableJumping = true;
                jumpingOverride = true;
                enableMovement = false;
                m_debugUtil.AppendDebugger("03");
            }
            else if (m_boolFacingRight && angleBetweenLeft <= m_floatMaxSlope && angleBetweenRight > m_floatMaxSlope)
            {
                enableJumping = false;
                enableMovement = false;
                m_debugUtil.AppendDebugger("04");
            }

        }

        if (enableJumping)
        {
            if (rch != null || jumpingOverride)
            {
                if (moveX == 0.0f && !m_boolIsJumping)
                {
                    m_rigidBody2DplayerRig.velocity = Vector2.zero;
                    m_floatXSpeed = 0.0f;
                }
                if (Input.GetKey(KeyCode.Joystick1Button0))
                {
                    m_boolIsJumping = true;
                    m_rigidBody2DplayerRig.velocity = Vector2.zero;
                    m_rigidBody2DplayerRig.AddForce(new Vector2(0.0f, m_floatJumpHeight));
                }
            }
            else
            {
                m_boolIsJumping = false;
            }
        }
        if (enableMovement)
        {
            m_rigidBody2DplayerRig.velocity = new Vector2(m_floatXSpeed * (m_boolIsCrouching ? 0.5f : 1.0f), m_rigidBody2DplayerRig.velocity.y);
        }

        float moveY = Input.GetAxis("Vertical");
        if (moveY < -0.5f)
        {
            m_boolIsCrouching = true;
        }
        else
        {
            if (m_boolIsCrouching)
            {
                Vector2 centreOfCrouched = m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.center;
                float yOffsetToFullHeight = m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y;
                yOffsetToFullHeight *= 0.5f;
                yOffsetToFullHeight *= 3.0f;
                Vector2 standUpHeight = new Vector2(centreOfCrouched.x, centreOfCrouched.y + yOffsetToFullHeight + 0.1f);
                Vector2 topLeft = standUpHeight - new Vector2(m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.x / 2.0f, 0.0f);
                Vector2 topRight = standUpHeight + new Vector2(m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.x / 2.0f, 0.0f);
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
        float horizaAim = Input.GetAxis("HorizontalAim");
        float verticAim = Input.GetAxis("VerticalAim");
        float playerHeight = m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y;
        Vector2 shoulderOffset = new Vector2(0.0f, playerHeight * m_constFloatDeadZone);
        Vector2 playerMiddlePosition = m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.center;

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

    private void ResolveFacingDirection()
    {
        m_vector3PlayerScale = new Vector3((m_boolFacingRight ? 1 : -1) * Mathf.Abs(m_vector3PlayerScale.x), m_vector3PlayerScale.y, m_vector3PlayerScale.z);
        
        gameObject.transform.localScale = new Vector3(m_vector3PlayerScale.x, m_vector3PlayerScale.y / (m_boolIsCrouching ? 2.0f : 1.0f), m_vector3PlayerScale.z);
    }
}
