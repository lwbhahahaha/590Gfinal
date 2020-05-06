/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class physics : MonoBehaviour
{
    private float percentOfPReductionAfterHittingWall;
    private float percentOfPReductionAfterHittingBall;
    private float angularDrag;
    private float drag;
    private Vector3 v;
    private Vector3 w;
    private static float time_step_h;
    //
    public Balls ballsScript;
    public ball0Script otherScript0;
    public ball1Script otherScript1;
    public ball2Script otherScript2;
    public ball3Script otherScript3;
    public ball4Script otherScript4;
    public ball5Script otherScript5;
    public ball6Script otherScript6;
    public ball7Script otherScript7;
    public ball8Script otherScript8;
    public ball9Script otherScript9;
    public ball10Script otherScript10;
    public ball11Script otherScript11;
    public ball12Script otherScript12;
    public ball13Script otherScript13;
    public ball14Script otherScript14;
    public ball15Script otherScript15;
    public Rigidbody rb;
    //

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        percentOfPReductionAfterHittingWall = ballsScript.pReduction;
        percentOfPReductionAfterHittingBall = ballsScript.bReduction;
        angularDrag = ballsScript.k_fv;
        drag = ballsScript.k_fw;
        time_step_h = ballsScript.fxtime;
        Time.fixedDeltaTime = time_step_h;
        //v = 0f;
        //a = 0f;
        //w = 0f;
        //wa = 0f;
    }

    void hitWall(Vector3 hitpoint)
    {
        Vector3 inNormal = (hitpoint - transform.position).normalized;
        Vector3 rslt = Vector3.Reflect(v, inNormal) * (1 - percentOfPReductionAfterHittingWall);
        v = rslt;
    }

    void OnCollisionEnter(Collision collision)
    {
        string name = collision.gameObject.name;
        if (name.Contains("table"))
        {
            hitWall(collision.contacts[0].point);
            Debug.Log("table");
        }
        else if (name.Contains("white"))
        {
            v += otherScript0.getHitted(transform.position, v);
        }
        else if (name.Contains("1"))
        {
            v += otherScript1.getHitted(transform.position, v);
        }
        else if (name.Contains("2"))
        {
            v += otherScript2.getHitted(transform.position, v);
        }
        else if (name.Contains("3"))
        {
            v += otherScript3.getHitted(transform.position, v);
        }
        else if (name.Contains("4"))
        {
            v += otherScript4.getHitted(transform.position, v);
        }
        else if (name.Contains("5"))
        {
            v += otherScript5.getHitted(transform.position, v);
        }
        else if (name.Contains("6"))
        {
            v += otherScript6.getHitted(transform.position, v);
        }
        else if (name.Contains("7"))
        {
            v += otherScript7.getHitted(transform.position, v);
        }
        else if (name.Contains("black"))
        {
            v += otherScript8.getHitted(transform.position, v);
        }
        else if (name.Contains("9"))
        {
            v += otherScript9.getHitted(transform.position, v);
        }
        else if (name.Contains("10"))
        {
            v += otherScript10.getHitted(transform.position, v);
        }
        else if (name.Contains("11"))
        {
            v += otherScript11.getHitted(transform.position, v);
        }
        else if (name.Contains("12"))
        {
            v += otherScript12.getHitted(transform.position, v);
        }
        else if (name.Contains("13"))
        {
            v += otherScript13.getHitted(transform.position, v);
        }
        else if (name.Contains("14"))
        {
            v += otherScript14.getHitted(transform.position, v);
        }
        else if (name.Contains("15"))
        {
            v += otherScript15.getHitted(transform.position, v);
        }
    }

    public Vector3 getHitted(Vector3 oppocenter, Vector3 oppoV)
    {
        Vector3 v3n = (transform.position - oppocenter).normalized;
        Vector3 v2n = v.normalized;
        float a = Vector3.Angle(v2n, v3n);
        if (a > 90)
            a = a - 90;
        Vector3 v3 = v * Mathf.Cos(a);

        Vector3 V5n = oppoV.normalized;
        Vector3 V4n = v3n;
        float b = Vector3.Angle(V4n, V5n);
        Vector3 v4 = v * Mathf.Cos(b);
        Vector3 v3t = v4 * (1f - percentOfPReductionAfterHittingBall);
        Vector3 v4t = v3 * (1f - percentOfPReductionAfterHittingBall);
        v += -v3 + v3t;
        return -v4 + v4t;

    }

    //for white ball only
    public void hittedByCue(Vector3 P, Vector3 hitPoint)
    {
         
    }

    void FixedUpdate()
    {
        /*
        if (v.magnitude < 0)
            v = new Vector3(0,0,0);
        if (w.magnitude < 0)
            w = new Vector3(0, 0, 0);
        float va = v + 0.5f * ballsScript.distance * w;
        if (v.magnitude > 0)
        {
            transform.position += v * time_step_h;
            v += time_step_h * a;
        }
        if (w.magnitude > 0)
        {
            transform.eulerAngles+=w * time_step_h;
            w += time_step_h * wa;
        }
        Vector3 va = v + 0.5f * ballsScript.distance * w;
        rb.velocity = va;
        rb.angularVelocity = w;
        rb.angularDrag = angularDrag;
        rb.drag = drag;
    }
}
 */