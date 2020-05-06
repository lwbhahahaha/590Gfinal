using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandeler : MonoBehaviour
{ 
    public int num;
    public float time_step_h;
    public float ballBallCoefficientOfRestitution;
    public float ballBallCoefficientOfFriction;
    public float ballR;
    // Start is called before the first frame update
    public void getHitted(ball0Script smallS, Vector3 smallV, GameObject smallGO, ball0Script bigS, Vector3 bigV, GameObject bigGO)
    {
        //我是小
        //Debug.Log(num);
        Vector3 direction = (bigGO.transform.position - smallGO.transform.position).normalized;
        Vector3 myRelativeV = Vector3.Project(smallV, direction);
        Vector3 otherRelativeV = Vector3.Project(bigV, direction);
        Vector3 myRelativeW = Vector3.Project(smallS.wtemp, direction) * ballBallCoefficientOfRestitution;
        Vector3 opRelativeW = Vector3.Project(bigS.wtemp, direction) * ballBallCoefficientOfRestitution;
        smallS.v += -myRelativeV + otherRelativeV * ballBallCoefficientOfRestitution;//ballBallCoefficientOfRestitution
        bigS.v += myRelativeV * ballBallCoefficientOfRestitution - otherRelativeV;
        //angular:
        if (smallS.wtemp != Vector3.zero || bigS.wtemp != Vector3.zero)
        {
            smallS.wtemp -= myRelativeW * (1f - ballBallCoefficientOfRestitution);
            bigS.wtemp -= opRelativeW * (1f - ballBallCoefficientOfRestitution);
        }
        if (Mathf.Abs(smallS.spin.y + bigS.spin.y) > 0.1f)
        {
            float realistcFactor = time_step_h;
            smallS.v += Quaternion.Euler(0, 90f, 0) * direction * realistcFactor * ((smallS.spin.y + bigS.spin.y) * ballR - (myRelativeV.magnitude - otherRelativeV.magnitude) * ballBallCoefficientOfFriction);
            bigS.v += Quaternion.Euler(0, 90f, 0) * direction * realistcFactor * ((smallS.spin.y + bigS.spin.y) * ballR - (-myRelativeV.magnitude + otherRelativeV.magnitude) * ballBallCoefficientOfFriction);
        }
    }
}
