using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerControls : MonoBehaviour {
    public float    m_floatMovementSpeed;
    GameObject[]    m_gameObjectGround;
    float           m_floatXSpeed;
    GameObject      m_gameObjectAimLine;
    bool            m_boolIsCrouching;
    Vector3         m_vector3PlayerScale;
    bool            m_boolIsJumping;
    const float     m_constFloatGroundDistanceQualifier = 0.01f;

	// Use this for initialization
	void Start () {
        m_gameObjectGround = GameObject.FindGameObjectsWithTag("Ground");
        m_floatXSpeed = 0.0f;
        m_gameObjectAimLine = GameObject.Find("AimLine");
        m_boolIsCrouching = false;
        m_vector3PlayerScale = gameObject.transform.localScale;
        m_boolIsJumping = false;
	}
	
	// Update is called once per frame
	void Update () {
        float moveX = Input.GetAxisRaw("Horizontal");
        
        Rigidbody2D playerRig = gameObject.GetComponent<Rigidbody2D>();

        m_floatXSpeed += moveX * m_floatMovementSpeed * Time.deltaTime;
        m_floatXSpeed = Mathf.Clamp(m_floatXSpeed, -3.0f, 3.0f);

        //ContactFilter2D movementCollisionFilter = new ContactFilter2D();
        //LayerMask movementMask = new LayerMask();
        //movementMask.value = GeneralUtil.LayerAlias.Movement;
        //movementCollisionFilter.layerMask = movementMask;
        Vector2 boundingBoxCenter = new Vector2(playerRig.GetComponent<Collider2D>().bounds.center.x, playerRig.GetComponent<Collider2D>().bounds.center.y);
        
        Vector2 rayOriginLeft = boundingBoxCenter - new Vector2(playerRig.GetComponent<Collider2D>().bounds.size.x / 2, playerRig.GetComponent<Collider2D>().bounds.size.y / 2.0f + m_constFloatGroundDistanceQualifier);
        Vector2 rayOriginMiddle = boundingBoxCenter - new Vector2(0, playerRig.GetComponent<Collider2D>().bounds.size.y / 2.0f + m_constFloatGroundDistanceQualifier);
        Vector2 rayOriginRight = boundingBoxCenter - new Vector2(-playerRig.GetComponent<Collider2D>().bounds.size.x / 2, playerRig.GetComponent<Collider2D>().bounds.size.y / 2.0f + m_constFloatGroundDistanceQualifier);

        Ray2D rayToGroundFromLeft = new Ray2D(rayOriginLeft, Vector2.down);
        Ray2D rayToGroundFromMiddle = new Ray2D(rayOriginMiddle, Vector2.down);
        Ray2D rayToGroundFromRight = new Ray2D(rayOriginRight, Vector2.down);

        RaycastHit2D raycastHit2DLeft = Physics2D.Raycast(rayToGroundFromLeft.origin, rayToGroundFromLeft.direction, m_constFloatGroundDistanceQualifier);
        RaycastHit2D raycastHit2DMiddle = Physics2D.Raycast(rayToGroundFromMiddle.origin, rayToGroundFromMiddle.direction, m_constFloatGroundDistanceQualifier);
        RaycastHit2D raycastHit2DRight = Physics2D.Raycast(rayToGroundFromRight.origin, rayToGroundFromRight.direction, m_constFloatGroundDistanceQualifier);

        Debug.DrawLine(rayToGroundFromLeft.origin, rayToGroundFromLeft.origin + rayToGroundFromLeft.direction * m_constFloatGroundDistanceQualifier, Color.green);
        Debug.DrawLine(rayToGroundFromMiddle.origin, rayToGroundFromMiddle.origin + rayToGroundFromMiddle.direction * m_constFloatGroundDistanceQualifier, Color.green);
        Debug.DrawLine(rayToGroundFromRight.origin, rayToGroundFromRight.origin + rayToGroundFromRight.direction * m_constFloatGroundDistanceQualifier, Color.green);

        //m_gameObjectGround.Count(n => playerRig.IsTouching(n.GetComponent<Collider2D>()/*, movementCollisionFilter*/)) > 0
        if (raycastHit2DLeft.collider != null || raycastHit2DMiddle.collider != null || raycastHit2DRight.collider != null)
        {

            if (moveX == 0.0f && !m_boolIsJumping)
            {
                playerRig.velocity = Vector2.zero;
                m_floatXSpeed = 0.0f;
            }
            else
            {
                playerRig.velocity = new Vector2(m_floatXSpeed, playerRig.velocity.y);
            }

            if (Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                m_boolIsJumping = true;
                playerRig.AddForce(new Vector2(0.0f, 400.0f));
            }
        }
        else
        {
            m_boolIsJumping = false;
        }
        

        float horizaAim = Input.GetAxis("HorizontalAim");
        float verticAim = Input.GetAxis("VerticalAim");

        m_gameObjectAimLine.transform.position = gameObject.transform.position;
        float angle = MathUtil.FromArgumentToAngle(horizaAim, -verticAim);
        m_gameObjectAimLine.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle * Mathf.Rad2Deg);

        float moveY = Input.GetAxis("Vertical");
        if(moveY < -0.5f)
        {
            m_boolIsCrouching = true;
        }
        else
        {
            m_boolIsCrouching = false;
        }

        if (m_boolIsCrouching)
        {
            gameObject.transform.localScale = new Vector3(m_vector3PlayerScale.x, m_vector3PlayerScale.y / 2.0f, m_vector3PlayerScale.z);
        }
        else
        {
            gameObject.transform.localScale = m_vector3PlayerScale;
        }

        if (m_boolIsJumping)
        {
            
        }

    }
}
