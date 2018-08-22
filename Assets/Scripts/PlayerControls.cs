using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : ActorControls {

    protected override void ResolveAimingInputs()
    {
        m_floatXAimingInput = Input.GetAxis("HorizontalAim");
        m_floatYAimingInput = Input.GetAxis("VerticalAim");
    }

    protected override void ResolveMovementInputs()
    {
        m_floatYMovementInput = Input.GetAxis("Vertical");
        m_floatXMovementInput = Input.GetAxis("Horizontal");
        m_boolJumpInput = Input.GetKey(KeyCode.Joystick1Button0);
    }
}
