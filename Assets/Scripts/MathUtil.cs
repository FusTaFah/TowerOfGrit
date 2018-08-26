using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtil {

	public static float FromArgumentToAngle(float x, float y)
    {
        if(x == 0 && y == 0)
        {
            return 0.0f;
        }
        else if(x >= 0.0f && y >= 0.0f)
        {
            return Mathf.Atan(y / x);
        }
        else if(x < 0.0f && y >= 0.0f)
        {
            return Mathf.PI + Mathf.Atan(y / x);
        }
        else if(x < 0.0f && y < 0.0f)
        {
            return -Mathf.PI + Mathf.Atan(y / x);
        }
        else if(x >= 0.0f && y < 0.0f)
        {
            return Mathf.Atan(y / x);
        }
        return 0.0f;
    }

    public static Vector2 FromAngleToArgument(float theta)
    {
        float angle = (theta % 360.0f) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
}
