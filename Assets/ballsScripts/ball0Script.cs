using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball0Script : MonoBehaviour
{
    public AudioSource wall1;
    public AudioSource wall2;
    public AudioSource bag;
    public AudioSource ballball;
    public AudioSource cue;



    private float ballR;
    //private Vector3 w;
    public Vector3 wtemp;
    public Vector3 spin;
    public int num;
    public bool isfreeball = false;
    public Vector3 v;
    private static float time_step_h;
    private Vector3 hit;
    private GameObject billiard;
    private float scale;
    public bool inbag=false;
    public bool isBusy;
    private int ct;
    //
    public Balls ballsScript;
    public CollisionHandeler[] CollisionHandeler;
    public bool gEnabled;
    //
    //coefficients
    private float slidingFriction;// ball-cloth coefficient of sliding friction                    //scale^0
    private float spinDecelerationRate;//ball-cloth spin deceleration rate                               //scale^0
    private float rollingResistance;//ball-cloth coefficient of rolling resistance                    //scale^0
    private float ballRailCoefficientOfRestitution;//ball-rail coefficient of restitution    //scale^0
    private float ballTableCoefficientOfRestitution;//ball-table coefficient of restitution   //scale^0
    private float ballBallCoefficientOfRestitution;//ball-ball coefficient of restitution    //scale^0
    public float ballBallCoefficientOfFriction;                                              //scale^0
    private float mass;         //scale^1                                                            done
    private float g;            //sacle^1  
    public float height;
    private float dropHeight;
    private GameObject[] walls;
    //
    // Start is called before the first frame update
    void Start()
    {
        wall1=GameObject.Find("wal1S").GetComponent<AudioSource>();
        wall2 = GameObject.Find("wal2S").GetComponent<AudioSource>();
        bag = GameObject.Find("bgS").GetComponent<AudioSource>();
        ballball = GameObject.Find("bbS").GetComponent<AudioSource>();
        cue = GameObject.Find("qS").GetComponent<AudioSource>();

        CollisionHandeler = new CollisionHandeler[120];
        walls = new GameObject[6];
        isBusy = false;
        ct = 0;
        inbag = false;
        spin = new Vector3(0f, 0f, 0f);
        ballR = ballsScript.ballR;
        //Debug.Log("ini ballR:" + ballR);
        gEnabled = false;
        billiard = ballsScript.billiard;
        dropHeight = billiard.transform.TransformPoint(new Vector3(0, 0.8117664f, 0) * scale).y - ballR;
        //height = billiard.transform.TransformPoint(new Vector3(0, 0.8117664f, 0) * scale).y;
        //Time.timeScale = 0.1f;
        mass = ballsScript.mass;
        //Debug.Log("mass:" + mass);
        scale = ballsScript.scale;
        g = ballsScript.g;
        //Debug.Log("g:" + g);
        ballRailCoefficientOfRestitution = ballsScript.ballRailCoefficientOfRestitution;
        ballBallCoefficientOfRestitution = ballsScript.ballBallCoefficientOfRestitution;
        ballBallCoefficientOfFriction = ballsScript.ballBallCoefficientOfFriction;
        slidingFriction = ballsScript.slideDrag;
        spinDecelerationRate = ballsScript.spinDrag;
        rollingResistance = ballsScript.rollDrag;
        time_step_h = ballsScript.fxtime;
        Time.fixedDeltaTime = time_step_h;
        v = new Vector3(0f, 0f, 0f);
        wtemp = new Vector3(0f, 0f, 0f);
        //w = new Vector3(0f, 0f, 0f);
        //Debug.Log("2: "+ scale);
        iniCollisonHandeler(); iniWalls();

    }

    void playSound(int i, float vol)
    {
        vol /= scale;
        switch (i)
        {
            case 1:
                if (Random.Range(0, 2) == 0)
                    //wall1.Play(0);
                    wall1.PlayOneShot(wall1.clip, vol/2f);
                else
                    wall2.PlayOneShot(wall2.clip, vol/2f);
                break;
            case 2:
                cue.PlayOneShot(cue.clip, Mathf.Min(vol,3f));
                break;
            case 3:
                bag.PlayOneShot(bag.clip, vol/7f);
                break;
            case 4:
                ballball.PlayOneShot(ballball.clip, Mathf.Min(vol, 3f));
                break;
        }
    }

    void iniCollisonHandeler()
    {
        GameObject[] go = FindObjectsOfType<GameObject>();

        for (int i = 0; i < go.Length; i++)
        {
            if (go[i].name.Contains("CollisionHandeler"))
            {
                string currName = go[i].name;
                //Debug.Log(currName);
                
                int pos = currName.IndexOf("(");
                currName = currName.Substring(pos + 1, currName.Length - 2 - pos);
                //Debug.Log(currName);
                CollisionHandeler[int.Parse(currName) - 1] = go[i].GetComponent<CollisionHandeler>();
                CollisionHandeler[int.Parse(currName) - 1].time_step_h= time_step_h;
                CollisionHandeler[int.Parse(currName) - 1].ballBallCoefficientOfRestitution = ballBallCoefficientOfRestitution;
                CollisionHandeler[int.Parse(currName) - 1].ballBallCoefficientOfFriction= ballBallCoefficientOfFriction;
                CollisionHandeler[int.Parse(currName) - 1].ballR= ballR;
                CollisionHandeler[int.Parse(currName) - 1].num = int.Parse(currName);
            }
        }
    }

    void iniWalls()
    {
        GameObject[] go = FindObjectsOfType<GameObject>();

        for (int i = 0; i < go.Length; i++)
        {
            if (go[i].name.Contains("wall"))
            {
                string currName = go[i].name;
                //Debug.Log(currName);

                int pos = currName.IndexOf("(");
                currName = currName.Substring(pos + 1, currName.Length - 2 - pos);
                //Debug.Log(currName);
                walls[int.Parse(currName) - 1] = go[i];
            }
        }
    }

    void hitWall()
    {

        playSound(1,v.magnitude);
        v.y = 0;
        Vector3 p0 = transform.position;
        Debug.DrawRay(p0, v.normalized * ballR, Color.gray, 150f);
        //Debug.DrawRay(p0, wtemp, Color.yellow, 150f);
        Vector3 p1 = p0;
        Vector3 p2 = p0;
        RaycastHit Ray;
        float pos = -ballR;
        Physics.Raycast(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, v.normalized, out Ray, 100 * scale);


        bool isInside = false;
        Vector3 currPos = p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos;
        for (int i=0;i<6;i++)
        {
            if (walls[i].GetComponent<Collider>().bounds.Contains(currPos))
            {
                isInside = true;
            }
        }
        if (isInside)
        {
            p1 = p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos;
        }
        else
        {
            if (Physics.Raycast(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, v.normalized, out Ray, 100 * scale))
            {
                if (Ray.collider.name.Contains("wall") || Ray.collider.name.Contains("corner"))
                {
                    p1 = Ray.point;
                }
            }
        }
  









        /*
        if (Physics.Raycast(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, v.normalized, out Ray, 100 * scale))
        {
            if (Ray.collider.name.Contains("wall"))
            {
                Debug.DrawLine(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, Ray.point, Color.blue, 150f);
                p1 = Ray.point;
            }
            else if (Physics.Raycast(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, -v.normalized, out Ray, 100 * scale))
            {
                if (Ray.collider.name.Contains("wall"))
                {
                    Debug.DrawLine(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, Ray.point, Color.blue, 150f);
                    p1 = Ray.point;
                }
                else if (Physics.Raycast(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, v.normalized, out Ray, 100 * scale))
                {
                    if (Ray.collider.name.Contains("corner"))
                    {
                        Debug.DrawLine(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, Ray.point, Color.blue, 150f);
                        p1 = Ray.point;
                    }
                    else if (Physics.Raycast(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, -v.normalized, out Ray, 100 * scale))
                    {
                        if (Ray.collider.name.Contains("corner"))
                        {
                            Debug.DrawLine(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, Ray.point, Color.blue, 150f);
                            p1 = Ray.point;
                        }
                        else
                        {
                            p1 = p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos;
                        }
                    }
                }
            }
        }*/
        Debug.DrawLine(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, p1, Color.magenta, 150f);

        pos = ballR;

        isInside = false;
        currPos = p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos;
        for (int i = 0; i < 6; i++)
        {
            if (walls[i].GetComponent<Collider>().bounds.Contains(currPos))
            {
                isInside = true;
            }
        }
        if (isInside)
        {
            p2 = p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos;
        }
        else
        {
            if (Physics.Raycast(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, v.normalized, out Ray, 100 * scale))
            {
                if (Ray.collider.name.Contains("wall") || Ray.collider.name.Contains("corner"))
                {
                    p2 = Ray.point;
                }
            }
        }

        Debug.DrawLine(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, p2, Color.cyan, 150f);

        //Ray.point = p0;

        /*
        if (Physics.Raycast(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, v.normalized, out Ray, 100 * scale))
        {
            if (Ray.collider.name.Contains("wall"))
            {
                Debug.DrawLine(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, Ray.point, Color.blue, 150f);
                p2 = Ray.point;
            }
            else if (Physics.Raycast(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, -v.normalized, out Ray, 100 * scale))
            {
                if (Ray.collider.name.Contains("wall"))
                {
                    Debug.DrawLine(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, Ray.point, Color.blue, 150f);
                    p2 = Ray.point;
                }
                else if (Physics.Raycast(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, v.normalized, out Ray, 100 * scale))
                {
                    if (Ray.collider.name.Contains("corner"))
                    {
                        Debug.DrawLine(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, Ray.point, Color.blue, 150f);
                        p2 = Ray.point;
                    }
                    else if (Physics.Raycast(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, -v.normalized, out Ray, 100 * scale))
                    {
                        if (Ray.collider.name.Contains("corner"))
                        {
                            Debug.DrawLine(p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos, Ray.point, Color.blue, 150f);
                            p2 = Ray.point;
                        }
                        else
                        {
                            p2 = p0 + Quaternion.Euler(0, 90f, 0) * v.normalized * pos;
                        }
                    }
                }
            }
        }*/
        //////// method 2:
        float accuracy = 0.1f;
        float numberOfRays = 15f;
        float currAngle = -Vector3.Angle(p1 - p0, v);
        //float maxAngle = currAngle;
        float angleD = Vector3.Angle(p1 - p0, p2 - p0) / numberOfRays;
        //Debug.Log(currAngle);
        //Debug.Log(Vector3.Angle(p2 - p0, v));
        float length = 100f;
        float prevLength = 100f;
        Ray.point = p0 + Vector3.forward * ballR * 2f;
        float ct = 0;
        while ((Ray.point - p0).magnitude > ballR * 1.5f && ct <= 10f)
        {
            while (Mathf.Abs(angleD) > accuracy)
            {
                while (length <= prevLength)
                {
                    prevLength = length;
                    if (Physics.Raycast(p0, Quaternion.Euler(0, currAngle, 0) * v.normalized, out Ray, 100 * scale))
                    {
                        //Debug.Log(Ray.point);
                        Debug.DrawLine(p0, Ray.point, Color.blue, 150f);
                        length = (Ray.point - p0).magnitude;
                    }
                    currAngle += angleD;
                }
                Debug.DrawLine(p0, Ray.point, Color.red, 150f);
                currAngle -= 2f * angleD;
                length = prevLength;
                angleD /= numberOfRays;
                currAngle += angleD;
            }
            currAngle = Vector3.Angle(p2 - p0, v);
            angleD = -Vector3.Angle(p1 - p0, p2 - p0) / numberOfRays;
            length = 100f;
            prevLength = 100f;
            ct++;
            //Time.timeScale = 0f;
        }
        Vector3 inNormal = (Ray.point - p0).normalized;
        Debug.DrawRay(p0, inNormal, Color.white, 150f);
        v = Vector3.Reflect(v, inNormal) * ballRailCoefficientOfRestitution;
        Debug.DrawRay(p0, wtemp, Color.yellow, 150f);
        wtemp = Vector3.Reflect(wtemp, inNormal) * ballRailCoefficientOfRestitution; //Time.timeScale = 0f;
        Debug.DrawRay(p0, wtemp, Color.yellow, 150f);

        //spin:
        wallSpin();
    }
    
    public Vector3[] wallSpinSimulate(Vector3 p0, Vector3 vv, Vector3 spinn)
    {
        float a = 1.47763814840382f;
        float b = -0.942600487272892f;
        float c = 18.6318980880424f;
        float d = 0.492916748029808f;
        float left = (a - d) / (1f + Mathf.Pow((vv.magnitude / scale / c), b)) + d;
        float factor = 0.3f;
        float giveToV = left * factor;
        left *= (1f - factor);
        float realistcFactor = 1f;
        Vector3[] rslt = new Vector3[2];
        //Debug.Log(billiard.transform.InverseTransformPoint(p0));
        if (billiard.transform.InverseTransformPoint(p0).x < -1.22f * scale)
        {
            //up
            vv += spinn.normalized.y * Vector3.back * realistcFactor *
                Mathf.Abs(spinn.y) * ballR * giveToV;
        }
        else if (billiard.transform.InverseTransformPoint(p0).z < -0.6f * scale)
        {
            //left
            vv += spinn.normalized.y * Vector3.right * realistcFactor *
                Mathf.Abs(spinn.y) * ballR * giveToV;
        }
        else if (billiard.transform.InverseTransformPoint(p0).z > 0.6f * scale)
        {
            //right
            vv += spinn.normalized.y * Vector3.left * realistcFactor *
                Mathf.Abs(spinn.y) * ballR * giveToV;
        }
        else
        {
            //down
            vv += spinn.normalized.y * Vector3.forward * realistcFactor *
                Mathf.Abs(spinn.y) * ballR * giveToV;
        }
        rslt[0] = vv;
        rslt[1] = spinn * left;
        return rslt;
        //Debug.Log((a - d) / (1f + Mathf.Pow((vv.magnitude / scale/ c), b)) + d); 
        //wtemp = vv / ballR;
    }

    void wallSpin()
    {
        float a = 1.47763814840382f;
        float b = -0.942600487272892f;
        float c = 18.6318980880424f;
        float d = 0.492916748029808f;
        float left = (a - d) / (1f + Mathf.Pow((v.magnitude / scale / c), b)) + d;
        float factor = 0.3f;
        float giveToV = left * factor;
        left *= (1f - factor);
        float realistcFactor = 1f;
        Vector3 p0 = transform.position;
        //Debug.Log(billiard.transform.InverseTransformPoint(p0));
        if (billiard.transform.InverseTransformPoint(p0).x < -1.22f * scale)
        {
            //up
            v += spin.normalized.y * Vector3.back * realistcFactor *
                Mathf.Abs(spin.y) * ballR * giveToV;
        }
        else if (billiard.transform.InverseTransformPoint(p0).z < -0.6f * scale)
        {
            //left
            v += spin.normalized.y * Vector3.right * realistcFactor *
                Mathf.Abs(spin.y) * ballR * giveToV;
        }
        else if (billiard.transform.InverseTransformPoint(p0).z > 0.6f * scale)
        {
            //right
            v += spin.normalized.y * Vector3.left * realistcFactor *
                Mathf.Abs(spin.y) * ballR * giveToV;
        }
        else
        {
            //down
            v += spin.normalized.y * Vector3.forward * realistcFactor *
                Mathf.Abs(spin.y) * ballR * giveToV;
        }
        spin *= left;
        //Debug.Log((a - d) / (1f + Mathf.Pow((vv.magnitude / scale/ c), b)) + d); 
        //wtemp = vv / ballR;
    }

    /*
    private void OnTriggerExit(Collider other)
    {
        
        //pause to debug
        //Time.timeScale = 0f;
        GameObject otherGO = other.gameObject;
        ball0Script otherS = otherGO.GetComponent<ball0Script>();
        string name = otherGO.name;
        //Debug.Log(name);
        if (name.Contains("table"))
        {
            gEnabled = true;
            Debug.Log("Gravity Enabled.");
        }
    }
    void beingHitting(Vector3 oppocenter, Vector3 oppoV, GameObject otherGO)
    {
        Vector3 directionV = (oppocenter - transform.position).normalized;
        Vector3 directionH = Quaternion.Euler(0, 90, 0) * directionV;

        Vector3 myRelativeVH = Vector3.Project(v, directionH);
        Vector3 otherRelativeVH = Vector3.Project(oppoV, directionH);

        Vector3 RelativeH = otherRelativeVH - myRelativeVH;

        v -= RelativeH.normalized * ballBallCoefficientOfFriction * g;
        //wtemp = v / (ballR);
    }*/
    /*
    void OnTriggerStay(Collider other)
    {
        Debug.Log(other.name);
    }*/

    private void OnTriggerEnter(Collider other)
    {
        //pause to debug
        //Time.timeScale = 0f;
        GameObject otherGO = other.gameObject;
        ball0Script otherS = otherGO.GetComponent<ball0Script>();
        string name = other.name;
        //Debug.Log(name);
        if (name.Contains("ball"))
        {
            getHittedBeta(otherS, otherS.v, otherGO);
            if (name.Contains("white"))
                ballsScript.whiteTouched.Add(num);
        }
        else if (name.Contains("wall"))
        {
            hitWall();
            //Debug.Log("table");
        }
        else if (name.Contains("corner") || name.Contains("bag"))
        {
            playSound(3, v.magnitude);
            v = v.normalized * 60f;
            wtemp *= 0;
            gEnabled = true;
            inbag = true;
            ballsScript.newBallsInBag.Add(num);
            StartCoroutine(waitToDestory());
        }
        else 
        {
            Debug.Log(name);
        }
    }

    IEnumerator waitToDestory()
    {
        if (!isfreeball)
        {
                yield return new WaitForSeconds(0.08f);
                isfreeball = true;
                v *= 0;
                wtemp *= 0;
                spin *= 0;
                gEnabled = false;
        }
    }

    public Vector3[] getHittedSimulate(Vector3 oppoV, Vector3 oppoW, Vector3 oppoSpin, Vector3 oppoPos)
    {
        //Debug.Log(oppoW);
        Vector3[] rslt = new Vector3[4];//0:my v; 1: my w; 2: op v; 3: opw.
        //Debug.Log((otherGO.transform.position- ballsScript.temp).magnitude);
        Vector3 direction = (oppoPos - transform.position).normalized;
        Vector3 myRelativeV = Vector3.Project(v, direction);
        Vector3 otherRelativeV = Vector3.Project(oppoV, direction);
        Vector3 myRelativeW = Vector3.Project(wtemp, direction) * ballBallCoefficientOfRestitution;
        Vector3 opRelativeW = Vector3.Project(oppoW, direction) * ballBallCoefficientOfRestitution;
        rslt[0] = v + -myRelativeV * ballBallCoefficientOfRestitution + otherRelativeV;//ballBallCoefficientOfRestitution
        rslt[2] = oppoV + myRelativeV - otherRelativeV * ballBallCoefficientOfRestitution;
        //angular:
        rslt[1] = wtemp - myRelativeW * (1f - ballBallCoefficientOfRestitution);
        rslt[3] = oppoW - opRelativeW * (1f - ballBallCoefficientOfRestitution);
        if (Mathf.Abs(spin.y + oppoSpin.y) > 0.1f)
        {
            float realistcFactor = time_step_h;
            rslt[0] += Quaternion.Euler(0, 90f, 0) * direction * realistcFactor * ((spin.y + oppoSpin.y) * ballR - (myRelativeV.magnitude - otherRelativeV.magnitude) * ballBallCoefficientOfFriction);
            rslt[2] += Quaternion.Euler(0, 90f, 0) * direction * realistcFactor * ((oppoSpin.y + spin.y) * ballR - (-myRelativeV.magnitude + otherRelativeV.magnitude) * ballBallCoefficientOfFriction);
        }
        return rslt;
    }

    void getHitted(ball0Script otherS, Vector3 oppoV, GameObject otherGO)
    {
        //obj with larger v handles collision, return v to obj with smaller v
        if (num > otherS.num || otherS.isBusy)
        {
            isBusy = true;
            //Debug.Log(name + "\thit\t" + otherGO.name);
            Vector3 direction = (otherGO.transform.position - transform.position).normalized;
            Vector3 myRelativeV = Vector3.Project(v, direction);
            Vector3 otherRelativeV = Vector3.Project(oppoV, direction);
            Vector3 myRelativeW = Vector3.Project(wtemp, direction) * ballBallCoefficientOfRestitution;
            Vector3 opRelativeW = Vector3.Project(otherS.wtemp, direction) * ballBallCoefficientOfRestitution;
            v += -myRelativeV + otherRelativeV * ballBallCoefficientOfRestitution;//ballBallCoefficientOfRestitution
            otherS.v += myRelativeV * ballBallCoefficientOfRestitution - otherRelativeV;
            //angular:
            if (wtemp != Vector3.zero || otherS.wtemp != Vector3.zero)
            {
                wtemp -= myRelativeW * (1f - ballBallCoefficientOfRestitution);
                otherS.wtemp -= opRelativeW * (1f - ballBallCoefficientOfRestitution);
            }
            if (Mathf.Abs(spin.y + otherS.spin.y) > 0.1f)
            {
                float realistcFactor = time_step_h;
                v += Quaternion.Euler(0, 90f, 0) * direction * realistcFactor * ((spin.y + otherS.spin.y) * ballR - (myRelativeV.magnitude - otherRelativeV.magnitude) * ballBallCoefficientOfFriction);
                otherS.v += Quaternion.Euler(0, 90f, 0) * direction * realistcFactor * ((otherS.spin.y + spin.y) * ballR - (-myRelativeV.magnitude + otherRelativeV.magnitude) * ballBallCoefficientOfFriction);
            }
            isBusy = false;
        }
    }

    void getHittedBeta(ball0Script otherS, Vector3 oppoV, GameObject otherGO)
    {
        if (num > otherS.num)
        {

            playSound(4,(v+ oppoV).magnitude);
            int idx = 0;
            for (int i=15;i>=16- otherS.num; i--)
            {
                idx += i;
            }
            CollisionHandeler[idx+num- otherS.num-1].getHitted(otherS, oppoV, otherGO, this, v, gameObject);
        }
    }

    //for white ball only
    public void hittedByCue(Vector3 P, Vector3 hitPoint, float s)
    {
        playSound(2,(P/mass).magnitude);
        scale = s;
        Vector3 direction = P.normalized;
        //get v0
        v = P / mass;
        //get w0
        Vector3 r = hitPoint - transform.position;
        Vector3 L = Vector3.Cross(r, P);
        float J = 0.4f * mass * (ballR) * (ballR);//ball mass moment of inertia
        Vector3 w = L / J;
        wtemp.x = -w.z;
        wtemp.y = v.y;
        wtemp.z = w.x;
        //spin
        spin.y = w.y;
    }

    public Vector3[] hittedByCueSimulate(Vector3 P, Vector3 hitPoint, float s)
    {
        Vector3[] rslt = new Vector3[3];//0:v; 1:w; 2:spin.
        scale = s;
        Vector3 direction = P.normalized;
        //get v0
        rslt[0] = P / mass;
        //get w0
        Vector3 r = hitPoint - transform.position;
        Vector3 L = Vector3.Cross(r, P);
        float J = 0.4f * mass * (ballR) * (ballR);//ball mass moment of inertia
        Vector3 w = L / J;
        rslt[1].x = -w.z;
        rslt[1].y = v.y;
        rslt[1].z = w.x;
        //spin
        rslt[2].y = w.y;
        return rslt;
    }

    public bool isStopped()
    {
        if (inbag || v.magnitude < 1f && wtemp.magnitude < 0.2f)
        {
            //v *= 0;
            //wtemp *= 0;
            return true;
        }
        //Debug.Log("v: " + v.magnitude+"\tw: " + wtemp.magnitude+"\t"+(inbag || v.magnitude < 0.065f && wtemp.magnitude < 0.2f));
        //Debug.Log("w: "+ wtemp.magnitude);
        return false;
    }

    void Update()
    {
        

        /*
        if (v.magnitude < 0.005f && wtemp.magnitude < 0.005f)
        {
            IsStopped = true;
        }
        else
        {
            //Debug.Log("moving");
            IsStopped = false;
        }*/
        //Debug.DrawRay(transform.position, v, Color.white, time_step_h);
        //Debug.DrawRay(transform.position, Vector3.up, Color.red, 15f);
        /*
        if (name.Contains("white") && v.magnitude > 0.1f)
        {
            //Debug.Log(v.magnitude/scale);

            Debug.DrawRay(transform.position, Vector3.up, Color.red, 15f);
            //Debug.DrawRay(Quaternion.Euler(0, -90, 0) * v.normalized * ballR + transform.position, Vector3.up * wtemp.magnitude*ballR, Color.blue, 15f);
            //Debug.DrawRay(Quaternion.Euler(0, 90, 0) * v.normalized * ballR + transform.position, Quaternion.AngleAxis(-90f, Quaternion.Euler(0, 90, 0) * v)*v, Color.white, 15f);
            //Debug.DrawRay(Quaternion.Euler(0, -90, 0) * v.normalized * ballR + transform.position, Quaternion.AngleAxis(-90f, Quaternion.Euler(0, 90, 0) * v) *(- v.normalized) * rollingResistance * g, Color.red, 15f);
            //Debug.DrawRay(transform.position, spin, Color.red, 15f);
            //Debug.DrawRay(transform.position, wtemp, Color.white, 15f);
        }*/

        //Debug.DrawRay(transform.position, Quaternion.AngleAxis(90f, Quaternion.Euler(0, 90, 0)*v) * wtemp/100f, Color.blue, time_step_h);
    }

    void FixedUpdate()
    {
        if (!isfreeball)
        {
            //height = billiard.transform.TransformPoint(new Vector3(0, 0.8117664f, 0) * scale).y;
            //Debug.Log(height);
            if (transform.position.y > height || gEnabled)
            {
                transform.position -= new Vector3(0, g / 10f * time_step_h, 0);
            }
            if (transform.position.y < height && !gEnabled)
            {
                transform.position = new Vector3(transform.position.x, height, transform.position.z);
            }

            transform.RotateAround(transform.position, Vector3.up, spin.y * Mathf.Rad2Deg * time_step_h);//spin
            transform.position += v * time_step_h;
            transform.RotateAround(transform.position, Quaternion.Euler(0, 90, 0) * v, (v / ballR + wtemp).magnitude * Mathf.Rad2Deg * time_step_h);

            v += (-v.normalized) * rollingResistance * g * time_step_h;
            v += wtemp.normalized * slidingFriction * g * time_step_h;
            wtemp += -wtemp.normalized * slidingFriction * g * time_step_h / ballR;
            spin -= spin.normalized * spinDecelerationRate * time_step_h;
        }
    }
}