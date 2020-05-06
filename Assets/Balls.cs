using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balls : MonoBehaviour
{
    public GameObject[] balls;
    public GameObject billiard;
    public GameObject white;
    public GameObject ball1;
    public GameObject ball2;
    public GameObject ball3;
    public GameObject ball4;
    public GameObject ball5;
    public GameObject ball6;
    public GameObject ball7;
    public GameObject black;
    public GameObject ball9;
    public GameObject ball10;
    public GameObject ball11;
    public GameObject ball12;
    public GameObject ball13;
    public GameObject ball14;
    public GameObject ball15;
    public GameObject eyes;
    public GameObject cue;
    public GameObject target;
    public GameObject cueHead;
    public GameObject redDot;
    public GameObject helperBall;
    public GameObject powerBar;
    public GameObject txt;
    public float distance;// ball d=2r
    public float fxtime = 0.01f;
    public float scale;
    public float ballR;
    public LineRenderer line1;
    public LineRenderer line2;
    public LineRenderer line3;
    public Vector3 temp;
    public GameObject invisiableBall;
    //coefficients
    public float slideDrag;// ball-cloth coefficient of sliding friction                    //scale^0
    public float spinDrag;//ball-cloth spin deceleration rate                               //scale^0
    public float ballBallCoefficientOfFriction;                                              //scale^0
    public float rollDrag;//ball-cloth coefficient of rolling resistance                    //scale^0
    public float ballRailCoefficientOfRestitution;//ball-rail coefficient of restitution    //scale^0
    public float ballTableCoefficientOfRestitution;//ball-table coefficient of restitution   //scale^0
    public float ballBallCoefficientOfRestitution;//ball-ball coefficient of restitution    //scale^0
    // in order to make physics realistic, we need:
    // total moving length = p^2/(m^2*μ*g) ---->to be scale^1
    // total moving time = P/(m*μ*g); ---->to be sacle^0
    //scaled vars
    public float power;         //scale^2       slow:0.45m/s                                        done
    public float mass;         //scale^1                                                            done
    public float g;            //sacle^1                                                            done
    public AI oppo;
    /*for power:
    soft touch = < 0.45 m/s                 I <= 0.07654365 kg·m /s
    slow = 0.45-0.89 m/s                    I = 0.07654365 kg·m /s - 0.15138633 kg·m /s
    medium = 0.89-1.9 m/s                   I = 0.15138633 kg·m /s - 0.3231843 kg·m /s
    fast = 1.9-3.2 m/s                      I = 0.3231843 kg·m /s - 0.5443104 kg·m /s
    power shot = 3.2-4.5 m/s                I = 0.5443104 kg·m /s - 0.7654365 kg·m /s
    powerful break = 11-13 m/s              I = 1.871067 kg·m /s - 2.211261 kg·m /s
    ridiculously powerful break = 16 m/s    I = 2.721552 kg·m /s
    */

    //public Camera eyes;

    private float eyeDistance;
    private float eyeheight;
    private float cueDistance;
    private float cueheight;
    private bool[] picked;
    private float W;
    private float H;
    private bool helperReset = true;
    private float cueMoveSpeed = 30f;
    private bool runOnlyOnce = true;
    private bool runOnlyOnce2 = true;
    private float xMin;
    private float xMax;
    private float zMin;
    private float zMax;

    public List<int> prevBallsInBag;
    public List<int> newBallsInBag;
    public List<int> whiteTouched;

    private int myColor;//0: not clear yet, 1: solid, 2:strip
    private int oppoColor;//0: not clear yet,1: solid, 2:strip

    public int status;//0: My turn, setting things up; 1: I'm ready to shoot;  
                       //2: oppo turn, setting things up 3: oppo is ready to shoot
                       //4:I shoot and wait until ball stops
                       //5:oppo shoot and wait until ball stops
                       //6:My freeball
                       //7:oppo freeball

    // Start is called before the first frame update
    void Start()
    {
        powerBar.GetComponent<Healthbar>().SetHealth(50f);
        float a = 36.4489496258712f;
        float b = -2.98046118445402f;
        float c = 234.532924037123f;
        float d = 0.0399968601588961f;
        power = (a - d) / (1f + Mathf.Pow((50f / c), b)) + d;
        //Time.timeScale = 1f;
        scale = cue.transform.localScale.x * 100f;
        mass *= scale;
        //Debug.Log("mass scaled:" + mass);
        g *= scale;
        //Debug.Log("g scaled:" + g);
        //Debug.Log("1: "+scale);
        eyeDistance = 0.9f * scale;
        eyeheight = 0.9f * scale;
        cueDistance = .80f * scale;
        cueheight = .15f * scale;

        //Time.timeScale = 0.2f;
        distance = 0.028575f * scale;
        ballR = distance;
        status = 0;
        myColor = 0;
        oppoColor = 0;
        W = 0f;
        H = 0f;
        iniBalls();
        int[] startPos = randomStart();
        putBalls(startPos);
        //Debug.DrawLine(white.transform.position, white.transform.position+new Vector3(distance,0,0), Color.white, 15f);//normal
        //cueLength = Vector3.Distance(cueHead.transform.position, cueTail.transform.position);
        //eyes = camera.GetComponent<Camera>();
        oppo.Start();
        xMin = billiard.transform.TransformPoint(new Vector3(-1.24f, 0f, 0f) * scale).x;
        xMax = billiard.transform.TransformPoint(new Vector3(1.24f, 0f, 0f) * scale).x;
        zMin = billiard.transform.TransformPoint(new Vector3(0f, 0f, -0.6f) * scale).z;
        zMax = billiard.transform.TransformPoint(new Vector3(0f, 0f, 0.6f) * scale).z;
    }

    void putBalls(int[] pos)
    {
        float distance = this.distance * 2f / scale * 1.01f;
        float h = Mathf.Sqrt(3.0f) / 2.0f * distance * 1.01f;

        white.transform.position = billiard.transform.TransformPoint(new Vector3(-0.635f, 0.8117664f, 0f) * scale);
        balls[pos[0]].transform.position = billiard.transform.TransformPoint(new Vector3(0.635f, 0.8117664f, 0f) * scale);

        balls[pos[1]].transform.position = billiard.transform.TransformPoint(new Vector3(0.635f + h, 0.8117664f, -distance / 2.0f) * scale);
        balls[pos[2]].transform.position = billiard.transform.TransformPoint(new Vector3(0.635f + h, 0.8117664f, distance / 2.0f) * scale);
        balls[pos[3]].transform.position = billiard.transform.TransformPoint(new Vector3(0.635f + 2.0f * h, 0.8117664f, -distance) * scale);
        balls[pos[4]].transform.position = billiard.transform.TransformPoint(new Vector3(0.635f + 2.0f * h, 0.8117664f, 0f) * scale);
        balls[pos[5]].transform.position = billiard.transform.TransformPoint(new Vector3(0.635f + 2.0f * h, 0.8117664f, distance) * scale);
        balls[pos[6]].transform.position = billiard.transform.TransformPoint(new Vector3(0.635f + 3.0f * h, 0.8117664f, -1.5f * distance) * scale);
        balls[pos[7]].transform.position = billiard.transform.TransformPoint(new Vector3(0.635f + 3.0f * h, 0.8117664f, -distance / 2.0f) * scale);

        balls[pos[8]].transform.position = billiard.transform.TransformPoint(new Vector3(0.635f + 3.0f * h, 0.8117664f, distance / 2.0f) * scale);
        balls[pos[9]].transform.position = billiard.transform.TransformPoint(new Vector3(0.635f + 3.0f * h, 0.8117664f, 1.5f * distance) * scale);


        balls[pos[10]].transform.position = billiard.transform.TransformPoint(new Vector3(0.635f + 4.0f * h, 0.8117664f, -2.0f * distance) * scale);
        balls[pos[11]].transform.position = billiard.transform.TransformPoint(new Vector3(0.635f + 4.0f * h, 0.8117664f, -distance) * scale);
        balls[pos[12]].transform.position = billiard.transform.TransformPoint(new Vector3(0.635f + 4.0f * h, 0.8117664f, 0f) * scale);
        balls[pos[13]].transform.position = billiard.transform.TransformPoint(new Vector3(0.635f + 4.0f * h, 0.8117664f, distance) * scale);
        balls[pos[14]].transform.position = billiard.transform.TransformPoint(new Vector3(0.635f + 4.0f * h, 0.8117664f, 2.0f * distance) * scale);

    }

    void iniBalls()
    {
        balls = new GameObject[16];
        balls[0] = white;
        balls[1] = ball1;
        balls[2] = ball2;
        balls[3] = ball3;
        balls[4] = ball4;
        balls[5] = ball5;
        balls[6] = ball6;
        balls[7] = ball7;
        balls[8] = black;
        balls[9] = ball9;
        balls[10] = ball10;
        balls[11] = ball11;
        balls[12] = ball12;
        balls[13] = ball13;
        balls[14] = ball14;
        balls[15] = ball15;
        picked = new bool[15];
        for (int i = 0; i < picked.Length; i++)
        {
            picked[i] = true;
            balls[i].SetActive(true);
            balls[i].GetComponent<ball0Script>().enabled = true;
            balls[i].GetComponent<ball0Script>().num = i;
            balls[i].GetComponent<ball0Script>().height = billiard.transform.TransformPoint(new Vector3(0, 0.8117664f, 0) * scale).y;
        }
        balls[15].SetActive(true);
        balls[15].GetComponent<ball0Script>().enabled = true;
        balls[15].GetComponent<ball0Script>().height = billiard.transform.TransformPoint(new Vector3(0, 0.8117664f, 0) * scale).y;
        balls[15].GetComponent<ball0Script>().num = 15;
    }

    int[] randomStart()
    {
        int[] rslt = new int[15];
        picked[7] = false;
        rslt[4] = 8;
        //corner
        if (random0or1() == 0)
        {
            rslt[10] = randomPickStrip();
            rslt[14] = randomPickSolid();
        }
        else
        {
            rslt[10] = randomPickSolid();
            rslt[14] = randomPickStrip();
        }
        picked[rslt[10] - 1] = false;
        picked[rslt[14] - 1] = false;
        for (int i = 0; i < 14; i++)
        {
            if (i != 4 && i != 10)
            {
                int curr = Random.Range(1, 15);
                while (!picked[curr - 1])
                {
                    curr = Random.Range(1, 16);
                }
                rslt[i] = curr;
                picked[curr - 1] = false;
            }
        }

        return rslt;
    }

    int randomPickStrip()
    {
        return Random.Range(9, 16);
    }

    int randomPickSolid()
    {
        return Random.Range(1, 8);
    }

    int random0or1()
    {
        return Random.Range(0, 2);
    }


    GameObject findNearestBall(Vector3 whitepos, int color = 0)
    {
        if (color==0)
        {
            float minD = 10000f;
            GameObject rslt = white;
            for (int i = 1; i < 16; i++)
            {
                if (i == 8)
                    continue;
                GameObject currBall = balls[i];
                float tempDist = Vector3.Distance(whitepos, balls[i].transform.position);
                if (tempDist < minD)
                {
                    minD = tempDist;
                    rslt = currBall;
                }
            }
            return rslt;
        }

        float minDD = 10000f;
        GameObject rsltt = black;//may have a bug here. check later
        for (int i = 1; i < 16; i++)
        {
            if (!balls[i].activeSelf || i == 8 || ballNumToColor(i)!=color)
                continue;
            GameObject currBall = balls[i];
            float tempDist = Vector3.Distance(whitepos, balls[i].transform.position);
            if (tempDist < minDD)
            {
                minDD = tempDist;
                rsltt = currBall;
            }
        }
        Debug.Log("下一个是：" + rsltt.name) ;
        return rsltt;
    }

    GameObject findNextPossibleBall(Vector3 whitepos)
    {
        if (myColor == 0)
        {
            //color not clear yet
            return findNearestBall(whitepos);
        }
        else if (status == 0)
        {
            //me shoot
            return findNearestBall(whitepos, myColor);
        }
        return findNearestBall(whitepos, oppoColor);
    }

    void LocateWhiteAndPlaceCue()
    {
        //white ball
        Vector3 whitePos = white.transform.position;
        //find next possible ball
        GameObject targetBall = findNextPossibleBall(whitePos);
        //adjustCamera
        eyes.transform.position = white.transform.position;
        Vector3 Direction = (whitePos - targetBall.transform.position).normalized;
        eyes.transform.position += Direction * eyeDistance;
        eyes.transform.position += new Vector3(0, eyeheight, 0);
        eyes.transform.LookAt(targetBall.transform);
        //place cue
        cue.SetActive(true);
        cue.transform.position = white.transform.position;
        cue.transform.position += Direction * cueDistance;
        cue.transform.position += new Vector3(0, cueheight, 0);
        cue.transform.LookAt(white.transform);
        float x = cue.transform.rotation.eulerAngles.x;
        float y = cue.transform.rotation.eulerAngles.y;
        cue.transform.localEulerAngles = new Vector3(0, y - 90f, -x);
        //set target

        target.transform.position = white.transform.position;
        target.transform.position -= Direction * scale / 10f;
    }

    void moveTarget(bool left, bool accurate = false)
    {
        float factor = 2f;
        if (accurate)
        {
            factor = 0.1f;
        }
        Vector3 whitePos = white.transform.position;
        if (left)
        {
            target.transform.RotateAround(whitePos, new Vector3(0, 1, 0), cueMoveSpeed * factor * Time.deltaTime * 1f / Time.timeScale);
        }
        else
        {
            target.transform.RotateAround(whitePos, new Vector3(0, 1, 0), -cueMoveSpeed * factor * Time.deltaTime * 1f / Time.timeScale);
        }
        //place cue
        Vector3 Direction = (whitePos - target.transform.position).normalized;
        cue.transform.position = white.transform.position;
        cue.transform.position += Direction * cueDistance;
        cue.transform.position += new Vector3(0, cueheight, 0);
        cue.transform.LookAt(white.transform);
        float x = cue.transform.rotation.eulerAngles.x;
        float y = cue.transform.rotation.eulerAngles.y;
        cue.transform.localEulerAngles = new Vector3(0, y - 90f, -x);
        //camera
        eyes.transform.position = white.transform.position;
        eyes.transform.position += Direction * eyeDistance;
        eyes.transform.position += new Vector3(0, eyeheight, 0);
        eyes.transform.LookAt(target.transform);
    }

    void helperLineAndBall()
    {
        //destination ball
        Vector3 pos = HelperBallCenter();
        helperBall.SetActive(true);
        helperBall.transform.position = pos;
        //lines

        //get line1 from white ball calculation

        //get line2 from ball calculation
    }

    Vector3 HelperBallCenter()
    {
        Vector3 whitePos = white.transform.position;
        Vector3 targetPos = target.transform.position;
        Vector3 Direction = (targetPos - whitePos).normalized;
        RaycastHit hit;
        if (white.GetComponent<Rigidbody>().SweepTest(Direction, out hit, 100 * scale))
        {
            Vector3 inNormal = hit.normal;
            inNormal.y = 0;
            Vector3 spinV = Quaternion.Euler(0, -W * 45f / 80f, 0) * Quaternion.AngleAxis(H * 45f / 80f, Quaternion.Euler(0, 90, 0) * Direction) * (-Direction) * ballR;
            //
            Vector3[] hitRslt = white.GetComponent<ball0Script>().hittedByCueSimulate(Direction * power * scale * scale, whitePos + spinV, scale);
            Vector3 v0 = hitRslt[0];
            Vector3 w = hitRslt[1];
            Vector3 s = hitRslt[2];

            float t1 = Mathf.Abs(w.magnitude * ballR / slideDrag / g);
            Vector3 a1 = (-v0).normalized * rollDrag * g + (w).normalized * slideDrag * g;
            Vector3 v1 = v0 + (-v0).normalized * rollDrag * g * t1 + (w).normalized * slideDrag * g * t1;
            float L1 = Mathf.Abs((v1.magnitude * v1.magnitude - v0.magnitude * v0.magnitude) / 2 / a1.magnitude);

            Vector3 a2 = (-v0).normalized * rollDrag * g;
            float L2 = Mathf.Abs((v1.magnitude * v1.magnitude) / 2 / a2.magnitude);

            float Lmax = L1 + L2;

            float Ltarget = (hit.point + inNormal * ballR - whitePos).magnitude;
            if (Ltarget > Lmax)
            {
                //too far away
                line1.SetVertexCount(2);
                line1.SetPosition(0, whitePos);
                line1.SetPosition(1, whitePos + Direction * Lmax);
                line1.SetWidth(0.5f, 0.5f);
                return whitePos + Direction * Lmax;
            }
            else
            {
                //line1
                line1.SetVertexCount(2);
                line1.SetPosition(0, whitePos);
                line1.SetPosition(1, hit.point + inNormal * ballR);
                line1.SetWidth(0.5f, 0.5f);

                float v2 = Mathf.Sqrt(v0.magnitude * v0.magnitude + 2f * a1.magnitude * Ltarget);
                float w2 = Mathf.Sqrt(w.magnitude * w.magnitude - 2f * slideDrag * g / ballR * Ltarget);
                Vector3 vHit = Direction * v2;
                Vector3 wHit = w.normalized * w2;

                float t2 = Mathf.Abs((v2 - v0.magnitude) / a1.magnitude);

                float s2 = Mathf.Min((s + (-s.normalized) * spinDrag * t2).y, 0);
                if (s.y >= 0)
                {
                    s2 = Mathf.Max((s + (-s.normalized) * spinDrag * t2).y, 0);
                }
                Vector3 sHit = s.normalized * Mathf.Abs(s2);

                if (Ltarget <= Lmax && Ltarget > L1)
                {
                    float L3 = Ltarget - L1;
                    v2 = Mathf.Sqrt(v1.magnitude * v1.magnitude - 2f * rollDrag * g * L3);
                    vHit = Direction * v2;
                    wHit *= 0;
                    t2 = t1 + Mathf.Abs((v2 - v1.magnitude) / (rollDrag * g));

                    s2 = Mathf.Min((s + (-s.normalized) * spinDrag * t2).y, 0);
                    if (s.y >= 0)
                    {
                        s2 = Mathf.Max((s + (-s.normalized) * spinDrag * t2).y, 0);
                    }
                    sHit = s.normalized * Mathf.Abs(s2);

                }
                if (hit.collider.gameObject.name.Contains("ball"))
                {
                    //0:other v; 1: other w; 2: white v; 3: white w.
                    Vector3[] rslt = hit.collider.gameObject.GetComponent<ball0Script>().getHittedSimulate(vHit, wHit, sHit, hit.point + inNormal * ballR + Direction * 0.4f);
                    //line2
                    line2.SetVertexCount(2);
                    line2.SetPosition(0, hit.point + inNormal * ballR);
                    line2.SetPosition(1, hit.point + inNormal * ballR + rslt[2].normalized * 6 * ballR);
                    line2.SetWidth(0.5f, 0.5f);
                    //line3
                    line3.SetVertexCount(2);
                    line3.SetPosition(0, hit.collider.gameObject.transform.position);
                    line3.SetPosition(1, hit.collider.gameObject.transform.position + rslt[0].normalized * 6 * ballR);
                    line3.SetWidth(0.5f, 0.5f);
                }
                else
                {
                    //Debug.Log(inNormal);
                    Vector3 v3 = Vector3.Reflect(vHit, inNormal) * ballRailCoefficientOfRestitution;
                    //v3.y = 0;
                    Vector3 w3 = Vector3.Reflect(wHit, inNormal) * ballRailCoefficientOfRestitution;
                    //Debug.Log(sHit.y);
                    Vector3[] v3ands3 = white.GetComponent<ball0Script>().wallSpinSimulate(hit.point + inNormal * ballR, v3, sHit);
                    v3 = v3ands3[0];
                    Vector3 s3 = v3ands3[1];
                    line3.SetVertexCount(0);
                    line2.SetVertexCount(0);
                    line1.SetVertexCount(3);
                    //line1.SetPosition(0, whitePos);
                    //line1.SetPosition(1, hit.point + inNormal * ballR);
                    line1.SetPosition(2, hit.point + inNormal * ballR + v3.normalized * 6 * ballR);
                    //line1.SetWidth(0.5f, 0.5f);
                    //Debug.DrawRay(hit.point + inNormal * ballR, rslt, Color.blue, 150f);

                    //Debug.Log(HelperLine(hit.point + inNormal * ballR, v3.normalized, v3, w3, s3));
                }
            }
            return hit.point + inNormal * ballR;
        }
        return new Vector3(0, 0, 0);
    }

    void HitBall()
    {
        helperBall.SetActive(false);
        cue.SetActive(false);
        Vector3 targetPos = target.transform.position;
        Vector3 whitePos = white.transform.position;
        Vector3 Direction = (targetPos - whitePos).normalized;
        //Vector3 spinV = Quaternion.Euler(0, -W*170f/80f, H * 170f / 80f) * (-Direction)*ballR;
        //helper.transform.position = whitePos + spinV;
        //white.GetComponent<Rigidbody>().AddForceAtPosition(Direction*10f, whitePos + spinV,ForceMode.Impulse);
        //Debug.Log("hitted");
        //Debug.Log("***********");
        //Debug.DrawRay(whitePos, spinV, Color.white, 150f);
        //Debug.DrawRay(whitePos+ spinV, Direction * power * scale, Color.red, 150f);
        //

        Vector3 spinV = Quaternion.Euler(0, -W * 45f / 80f, 0) * Quaternion.AngleAxis(H * 45f / 80f, Quaternion.Euler(0, 90, 0) * Direction) * (-Direction) * ballR;
        //
        white.GetComponent<ball0Script>().hittedByCue(Direction * power * scale * scale, whitePos + spinV, scale);
        line1.SetVertexCount(0);
        line2.SetVertexCount(0);
        line3.SetVertexCount(0);
    }

    bool redDotisDistance(RectTransform m_RectTransform)
    {
        float rslt = m_RectTransform.anchoredPosition.x * m_RectTransform.anchoredPosition.x + m_RectTransform.anchoredPosition.y * m_RectTransform.anchoredPosition.y;
        return rslt < 1600f;
    }

    void spinAdjust(int direction)//1: up; 2: down; 3: left; 4: right 5:back to og
    {
        RectTransform m_RectTransform = redDot.GetComponent<RectTransform>();
        switch (direction)
        {
            case 1:
                if (redDotisDistance(m_RectTransform) || m_RectTransform.anchoredPosition.y <= 0)
                    m_RectTransform.anchoredPosition += new Vector2(0f, cueMoveSpeed * Time.deltaTime * 1f / Time.timeScale);
                break;
            case 2:
                if (redDotisDistance(m_RectTransform) || m_RectTransform.anchoredPosition.y >= 0)
                    m_RectTransform.anchoredPosition += new Vector2(0f, -cueMoveSpeed * Time.deltaTime * 1f / Time.timeScale);
                break;
            case 3:
                if (redDotisDistance(m_RectTransform) || m_RectTransform.anchoredPosition.x >= 0)
                    m_RectTransform.anchoredPosition -= new Vector2(cueMoveSpeed * Time.deltaTime * 1f / Time.timeScale, 0f);
                break;
            case 4:
                if (redDotisDistance(m_RectTransform) || m_RectTransform.anchoredPosition.x <= 0)
                    m_RectTransform.anchoredPosition += new Vector2(cueMoveSpeed * Time.deltaTime * 1f / Time.timeScale, 0f);
                break;
            case 5:
                m_RectTransform.anchoredPosition = new Vector2(0f, 0f);
                break;
            default:
                break;
        }
        W = m_RectTransform.anchoredPosition.x;
        H = m_RectTransform.anchoredPosition.y;
        //
    }

    bool noBallIsMoving()
    {
        for (int i = 0; i < 16; i++)
        {
            if (balls[i] != null && !balls[i].GetComponent<ball0Script>().isStopped())
            {
                //Debug.Log(balls[i] != null);
                //Debug.Log(balls[i].name); 
                //Debug.Log(balls[0].GetComponent<ball0Script>().v.magnitude);
                //Debug.Log(balls[0].GetComponent<ball0Script>().wtemp.magnitude);
                return false;
            }
        }
        return true;
    }

    bool whiteHittedRightColor(int s)
    {
        if (s == 5)
        {
            return ballNumToColor(whiteTouched[0]) == oppoColor;
        }
        return ballNumToColor(whiteTouched[0]) == myColor;
    }

    bool isTimeForBlackBall(int s)
    {
        int ct = 0;
        if (s == 5)
        {
            for (int i = 0; i < prevBallsInBag.Count; i++)
            {
                if (ballNumToColor(prevBallsInBag[i]) == oppoColor)
                    ct++;
            }
            return ct == 7;
        }
        for (int i = 0; i < prevBallsInBag.Count; i++)
        {
            if (ballNumToColor(prevBallsInBag[i]) == myColor)
                ct++;
        }
        return ct == 7;
        //刚刚击球的人应该打黑八？
    }

    bool collideWtihAnotherBallOnPos(Vector3 pos)
    {
        for (int i = 1; i < 16; i++)
        {
            if ((balls[i].transform.position - pos).magnitude <= 2f * ballR)
                return true;
        }
        return false;
    }

    void freeBall(int s)
    {
        //Debug.Log(Input.mousePosition);
        eyes.transform.position = billiard.transform.TransformPoint(new Vector3(0, 230f, 0));
        eyes.transform.LookAt(billiard.transform);
        //刚刚击球的人自由球
        if (s == 6)
        {
        }
        else
        {
            //then AI
            oppo.Start();
            oppo.AIThinking(true);
            status = -1;
        }
    }

    int ballNumToColor(int ball)
    {
        if (ball <= 7)
            return 1;
        return 2;
    }

    void win(int s)
    {
        if (s == 5)
            txt.GetComponent<UnityEngine.UI.Text>().text = "You lose!";
        else
            txt.GetComponent<UnityEngine.UI.Text>().text = "You win!";
        //刚刚击球的人胜利
    }

    void lose(int s)
    {
        //刚刚击球的人失败
        if (s == 4)
            txt.GetComponent<UnityEngine.UI.Text>().text = "You lose!";
        else
            txt.GetComponent<UnityEngine.UI.Text>().text = "You win!";
    }

    bool hasValidColor(int s)
    {
        if (s == 5)
        {
            return ballNumToColor(newBallsInBag[0]) == oppoColor;
        }
        return ballNumToColor(newBallsInBag[0]) == myColor;
        //袋里有刚刚击球的人的花色？
    }

    void changePlayer(int s)
    {
        status = s * -2 + 10;
    }
    void keepGoing(int s)
    {
        status = s * 2 - 8;
    }

    void gameLogic(int s)
    {
        //#1
        Debug.Log("game logic:\t#1");
        if (newBallsInBag.Contains(0))
        {
            //#3
            Debug.Log("game logic:\t#3");
            if (s == 5)
                status = 6;
            else
                status = 7;
        }
        else
        {
            //#2
            Debug.Log("game logic:\t#2");
            if (whiteTouched.Count > 0)
            {
                //#4
                Debug.Log("game logic:\t#4");
                if (myColor == 0)
                {
                    //#7
                    Debug.Log("game logic:\t#7");
                    if (newBallsInBag.Count > 0)
                    {
                        //#8
                        Debug.Log("game logic:\t#8");
                        if (newBallsInBag.Contains(8))
                        {
                            //#10
                            Debug.Log("game logic:\t#10");
                            lose(s);
                        }
                        else
                        {
                            //#9
                            Debug.Log("game logic:\t#9");
                            if (s == 5)
                            {
                                oppoColor = ballNumToColor(newBallsInBag[0]);
                                myColor = 3 - oppoColor;
                                oppo.AIColor = oppoColor;
                                Debug.Log("确定AI的花色是: " + ((oppoColor == 1) ? "solid" : "strip"));
                            }
                            else
                            {
                                myColor = ballNumToColor(newBallsInBag[0]);
                                oppoColor = 3 - myColor;
                                oppo.AIColor = oppoColor;
                                Debug.Log("确定我的花色是: "+ ((myColor==1)?"solid":"strip"));
                            }
                            //#11
                            Debug.Log("game logic:\t#11");
                            keepGoing(s);
                        }
                    }
                    else
                    {
                        //#12
                        Debug.Log("game logic:\t#12");
                        Debug.Log("开局无进球");
                        changePlayer(s);
                    }
                }
                else
                {
                    //#6
                    Debug.Log("game logic:\t#6");
                    if (whiteHittedRightColor(s))
                    {
                        //#13
                        Debug.Log("game logic:\t#13");
                        if (newBallsInBag.Count > 0)
                        {
                            //#16
                            Debug.Log("game logic:\t#16");
                            if (newBallsInBag.Contains(8))
                            {
                                //#17
                                Debug.Log("game logic:\t#17");
                                if (isTimeForBlackBall(s))
                                {
                                    //#21
                                    Debug.Log("game logic:\t#21");
                                    win(s);
                                }
                                else
                                {
                                    //#22
                                    Debug.Log("game logic:\t#22");
                                    lose(s);
                                }
                            }
                            else
                            {
                                //#18
                                Debug.Log("game logic:\t#18");
                                if (hasValidColor(s))
                                {
                                    //#19
                                    Debug.Log("game logic:\t#19");
                                    keepGoing(s);
                                }
                                else
                                {
                                    //#20
                                    Debug.Log("game logic:\t#20");
                                    changePlayer(s);
                                }
                            }
                        }
                        else
                        {
                            //#15
                            Debug.Log("game logic:\t#15");
                            Debug.Log("无进球");
                            changePlayer(s);
                        }
                    }
                    else
                    {
                        //#14
                        Debug.Log("game logic:\t#14");
                        Debug.Log("白球先打到："+ whiteTouched[0]);
                        if (s == 5)
                            status = 6;
                        else
                            status = 7;
                    }
                }
            }
            else
            {
                //#5
                Debug.Log("game logic:\t#5");
                Debug.Log("白球没有碰到任何球");
                if (s == 5)
                    status = 6;
                else
                    status = 7;
            }
        }
        prevBallsInBag.AddRange(newBallsInBag);
        newBallsInBag.Clear();
        whiteTouched.Clear();
    }

    IEnumerator wait(int s)
    {
        yield return new WaitForSeconds(1.5f);
        gameLogic(s);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(newBallsInBag.Count);
        if (status == 0)
        {
            //UI
            if (myColor==0)
            {
                txt.GetComponent<UnityEngine.UI.Text>().text = "You can aim at any ball";
            }
            else
            {
                txt.GetComponent<UnityEngine.UI.Text>().text = "You can only aim at "+((myColor==1)?"solid":"strip")+ " ball";
            }
            //
            powerBar.GetComponent<Healthbar>().SetHealth(50f);
            float a = 36.4489496258712f;
            float b = -2.98046118445402f;
            float c = 234.532924037123f;
            float d = 0.0399968601588961f;
            power = (a - d) / (1f + Mathf.Pow((50f / c), b)) + d;
            W = 0;
            H = 0;
            LocateWhiteAndPlaceCue();
            helperLineAndBall();
            status = 1;
        }
        if (status == 1)
        {
            runOnlyOnce = true;
            if (helperReset)
            {
                helperReset = false;
                helperLineAndBall();
            }
            if (Input.GetKey("w"))
            {
                powerBar.GetComponent<Healthbar>().GainHealth(1);
                float a = 36.4489496258712f;
                float b = -2.98046118445402f;
                float c = 234.532924037123f;
                float d = 0.0399968601588961f;
                power = (a - d) / (1f + Mathf.Pow(((float)powerBar.GetComponent<Healthbar>().healthPercentage / c), b)) + d;
                helperLineAndBall();
            }
            if (Input.GetKey("s"))
            {
                powerBar.GetComponent<Healthbar>().TakeDamage(1);
                float a = 36.4489496258712f;
                float b = -2.98046118445402f;
                float c = 234.532924037123f;
                float d = 0.0399968601588961f;
                power = (a - d) / (1f + Mathf.Pow(((float)powerBar.GetComponent<Healthbar>().healthPercentage / c), b)) + d;
                helperLineAndBall();
            }
            if (Input.GetKey("q"))
            {
                moveTarget(false, true);
                helperLineAndBall();
            }
            if (Input.GetKey("e"))
            {
                moveTarget(true, true);
                helperLineAndBall();
            }
            if (Input.GetKey("a"))
            {
                moveTarget(false);
                helperLineAndBall();
            }
            if (Input.GetKey("d"))
            {
                moveTarget(true);
                helperLineAndBall();
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                spinAdjust(1);
                helperLineAndBall();
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                spinAdjust(2);
                helperLineAndBall();
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                spinAdjust(4);
                helperLineAndBall();
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                spinAdjust(3);
                helperLineAndBall();
            }
            if (Input.GetKey(KeyCode.RightShift))
            {
                spinAdjust(5);
                helperLineAndBall();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                status = 4;
                //Debug.Log(status);
                HitBall();
                txt.GetComponent<UnityEngine.UI.Text>().text = "";
                helperReset = false;
                eyes.transform.position = billiard.transform.TransformPoint(new Vector3(0, 230f, 0));
                eyes.transform.LookAt(billiard.transform);
            }
        }
        if (status == 4 || status == 5)
        {
            if (noBallIsMoving())
            {
                StartCoroutine(wait(status));
                status = -1;
            }

        }
        else if (status ==6)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(white.transform.position);
            Vector3 mousePositionOnScreen = Input.mousePosition;
            mousePositionOnScreen.z = screenPosition.z;
            Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePositionOnScreen);
            mousePositionInWorld.z = Mathf.Min(zMax, mousePositionInWorld.z);
            mousePositionInWorld.z = Mathf.Max(zMin, mousePositionInWorld.z);
            mousePositionInWorld.x = Mathf.Min(xMax, mousePositionInWorld.x);
            mousePositionInWorld.x = Mathf.Max(xMin, mousePositionInWorld.x);
            //鼠标点击 判断是否合法：合法就摆放
            if (!collideWtihAnotherBallOnPos(mousePositionInWorld))
            {
                white.transform.position = mousePositionInWorld;
            }
            //status = 0;
            if (Input.GetMouseButtonDown(0))
            {
                if (!collideWtihAnotherBallOnPos(mousePositionInWorld))
                {
                    status = 0;
                }
                else 
                { 
                    //UI
                }
            }
        }
        else if (status==7)
        {
            status = -1;
            freeBall(7);
        }
        else if (status == 2)
        {
            status = -1;
            prevBallsInBag.AddRange(newBallsInBag);
            newBallsInBag.Clear();
            whiteTouched.Clear();
            oppo.Start();
            oppo.AIThinking(false);
        }
    }
}