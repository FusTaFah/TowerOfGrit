using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//41.151.254.162

public class PlayerControls : MonoBehaviour {
    public float    m_floatMovementSpeed;
    public float    m_floatJumpHeight;
    float           m_floatXSpeed;
    GameObject      m_gameObjectAimLine;
    Vector3         m_vector3PlayerScale;
    const float     m_constFloatGroundDistanceQualifier = 0.03f;
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

        if (rch != null)
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
        Vector2 boundingBoxTopLeft = boundingBoxCenter - new Vector2(m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.x / 2.0f + m_constFloatGroundDistanceQualifier * 2.0f, -m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y / 2.0f);
        Vector2 boundingBoxTopRight = boundingBoxCenter - new Vector2(-m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.x / 2.0f - m_constFloatGroundDistanceQualifier * 2.0f, -m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y / 2.0f);
        RaycastHit2D slopeCheckLeft = Physics2D.Raycast(boundingBoxTopLeft, Vector2.down, m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y);
        RaycastHit2D slopeCheckRight = Physics2D.Raycast(boundingBoxTopRight, Vector2.down, m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y);
        Debug.DrawLine(boundingBoxTopLeft, boundingBoxTopLeft + Vector2.down * m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y, Color.green);
        Debug.DrawLine(boundingBoxTopRight, boundingBoxTopRight + Vector2.down * m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y, Color.green);

        if (m_floatXSpeed <= -m_constFloatDeadZone)
        {
            if(slopeCheckLeft.collider != null)
            {
                float heightFromFeet = m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y - slopeCheckLeft.distance;
                float angleBetween = Mathf.Atan(heightFromFeet / m_constFloatGroundDistanceQualifier) * Mathf.Rad2Deg;
                m_debugUtil.AppendDebugger(angleBetween.ToString());
                if (angleBetween < 45.0f)
                {
                    m_rigidBody2DplayerRig.velocity = new Vector2(m_floatXSpeed, m_rigidBody2DplayerRig.velocity.y);
                }
            }
            else
            {
                m_rigidBody2DplayerRig.velocity = new Vector2(m_floatXSpeed, m_rigidBody2DplayerRig.velocity.y);
            }
        }
        else if(m_floatXSpeed >= m_constFloatDeadZone)
        {
            if(slopeCheckRight.collider != null)
            {
                float heightFromFeet = m_rigidBody2DplayerRig.GetComponent<Collider2D>().bounds.size.y - slopeCheckRight.distance;
                float angleBetween = Mathf.Atan(heightFromFeet / m_constFloatGroundDistanceQualifier) * Mathf.Rad2Deg;
                m_debugUtil.AppendDebugger(angleBetween.ToString());
                if (angleBetween < 45.0f)
                {
                    m_rigidBody2DplayerRig.velocity = new Vector2(m_floatXSpeed, m_rigidBody2DplayerRig.velocity.y);
                }
            }
            else
            {
                m_rigidBody2DplayerRig.velocity = new Vector2(m_floatXSpeed, m_rigidBody2DplayerRig.velocity.y);
            }
        }
        

        float moveY = Input.GetAxis("Vertical");
        if (moveY < -0.5f)
        {
            m_boolIsCrouching = true;
        }
        else
        {
            m_boolIsCrouching = false;
        }
        m_debugUtil.AppendDebugger("is jumping? " + m_boolIsJumping);
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
