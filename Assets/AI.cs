using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{

    public GameObject bagPos1;
    public GameObject bagPos2;
    public GameObject bagPos3;
    public GameObject bagPos4;
    public GameObject bagPos5;
    public GameObject bagPos6;
    public GameObject redDot;
    public Balls ballsScript;
    public int AIColor = 0;
    public List<GameObject> possibleTargets;

    private Vector3[] bagPos;
    private GameObject ttarget;
    private int targetBag;
    private float ballR;
    private float scale;
    private float xMin;
    private float xMax;
    private float zMin;
    private float zMax;
    private float y;
    private float mass;
    private Vector3 hitPt;
    private bool timeToShootBlack;
    private GameObject billiard;
    private GameObject[] balls;
    public GameObject powerBar;

    // Start is called before the first frame update
    public void Start()
    {
        ttarget = null;
        hitPt = new Vector3(0, 0, 0);
        possibleTargets.Clear();
        timeToShootBlack = false;
        balls = ballsScript.balls;
        ballR = ballsScript.ballR;
        mass = ballsScript.mass;
        scale = ballsScript.scale;
        billiard = ballsScript.billiard;

        xMin = billiard.transform.TransformPoint(new Vector3(-1.24f, 0f, 0f) * scale).x;
        xMax = billiard.transform.TransformPoint(new Vector3(1.24f, 0f, 0f) * scale).x;
        zMin = billiard.transform.TransformPoint(new Vector3(0f, 0f, -0.6f) * scale).z;
        zMax = billiard.transform.TransformPoint(new Vector3(0f, 0f, 0.6f) * scale).z;
        y = billiard.transform.TransformPoint(new Vector3(0f, 0.8117664f, 0f) * scale).y;
        bagPos = new Vector3[6];
        bagPos[0] = bagPos1.transform.position;
        bagPos[1] = bagPos2.transform.position;
        bagPos[2] = bagPos3.transform.position;
        bagPos[3] = bagPos4.transform.position;
        bagPos[4] = bagPos5.transform.position;
        bagPos[5] = bagPos6.transform.position;
    }



    int ballNumToColor(int ball)
    {
        if (AIColor == 0)
            return 0;
        if (ball <= 7)
            return 1;
        return 2;
    }

    GameObject findABallStraightToBag(GameObject white = null)
    {
        if (white == null)
        {
            //找自己球：自己球到洞口之间没有障碍
            if (timeToShootBlack)
            {
                for (int i = 0; i < 6; i++)
                {
                    RaycastHit hit;
                    if (!balls[8].GetComponent<Rigidbody>().SweepTest(bagPos[i] - balls[8].transform.position, out hit, (bagPos[i] - balls[8].transform.position).magnitude))
                    {
                        targetBag = i;
                        return balls[8];
                    }
                }
                targetBag = -1;
                return null;
            }
            else
            {
                float minLength = 10000f;
                GameObject minTarget = balls[0];
                for (int i = 1; i <= 15; i++)
                {
                    if (ballNumToColor(i) != AIColor || i == 8 || balls[i].GetComponent<ball0Script>().inbag)
                        continue;
                    Rigidbody currRb = balls[i].GetComponent<Rigidbody>();
                    Vector3 currPos = balls[i].transform.position;
                    for (int j = 0; j < 6; j++)
                    {
                        RaycastHit hit;
                        float currLength = (bagPos[j] - currPos).magnitude;
                        if (!currRb.SweepTest(bagPos[j] - currPos, out hit, currLength))
                        {
                            if (currLength < minLength)
                            {
                                minLength = currLength;
                                targetBag = j;
                                minTarget = balls[i];
                            }
                            possibleTargets.Add(balls[i]);
                        }
                    }
                }
                if (possibleTargets.Count == 0)
                {
                    targetBag = -1;
                    return null;
                }
                return minTarget;
            }
        }
        else
        {
            //找自己球：自己球到白球之间没有障碍
            Vector3 whitePos = balls[0].transform.position;
            Rigidbody currRb = balls[0].GetComponent<Rigidbody>();
            if (timeToShootBlack)
            {
                RaycastHit hit;
                if (!currRb.SweepTest(balls[8].transform.position - whitePos, out hit, (balls[8].transform.position - whitePos).magnitude))
                {
                    return balls[8];
                }
                return null;
            }
            else
            {
                float minLength = 10000f;
                GameObject minTarget = balls[0];
                for (int i = 1; i <= 15; i++)
                {
                    if (ballNumToColor(i) != AIColor || i == 8)
                        continue;
                    RaycastHit hit;
                    float currLength = (balls[i].transform.position - whitePos).magnitude;
                    if (!currRb.SweepTest(balls[i].transform.position - whitePos, out hit, currLength))
                    {
                        if (currLength < minLength)
                        {
                            minLength = currLength;
                            minTarget = balls[i];
                        }
                    }
                }
                if (minTarget == balls[0])
                    return null;
                return minTarget;
            }
            return null;
        }
        //返回自己球
        return null;
    }

    bool ballExistOnPath(Vector3 currPos, Vector3 direction)
    {
        currPos += direction * 2f * ballR;
        if (!isOnTable(currPos))
            return false;

        if (timeToShootBlack)
        {
            if ((balls[8].transform.position - currPos).magnitude <= 2f * ballR)
                return true;
        }
        else
        {
            for (int i = 1; i < 16; i++)
            {
                if (ballNumToColor(i) == AIColor || balls[i].GetComponent<ball0Script>().inbag)
                {
                    if ((balls[i].transform.position - currPos).magnitude <= 2f * ballR)
                        return true;
                }
            }
        }
        return ballExistOnPath(currPos, direction);
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

    bool isOnTable(Vector3 pos)
    {
        return pos.x <= xMax && pos.x >= xMin && pos.z >= zMin && pos.z <= zMax;
    }

    void placeFreeBall(GameObject target = null)
    {
        if (target == null)
        {
            //随机摆放在自己球旁边
            float x = Random.Range(xMin, xMax);
            float z = Random.Range(zMin, zMax);
            balls[0].transform.position = new Vector3(x, y, z);
            target = findABallStraightToBag(balls[0]);
            int ct = 0;
            while (collideWtihAnotherBallOnPos(new Vector3(x, y, z)) && target == null && ct <= 100)
            {
                ct++;
                x = Random.Range(xMin, xMax);
                z = Random.Range(zMin, zMax);
                balls[0].transform.position = new Vector3(x, y, z);
                target = findABallStraightToBag(balls[0]);
            }
            StartCoroutine(waitForWhiteReplaced(target));
        }
        else
        {
            float distance = Random.Range(6f * ballR, 20f * ballR);

            Vector3 newWhitePos = (target.transform.position - bagPos[targetBag]).normalized * distance + target.transform.position;
            balls[0].transform.position = newWhitePos;
            GameObject currTarget = findABallStraightToBag(balls[0]);
            int ct = 0;
            while (!isOnTable(newWhitePos) && collideWtihAnotherBallOnPos(newWhitePos) && target != currTarget && ct <= 100)
            {
                distance = Random.Range(6f * ballR, 20f * ballR);
                newWhitePos = (target.transform.position - bagPos[targetBag]).normalized * distance + target.transform.position;
                balls[0].transform.position = newWhitePos;
                currTarget = findABallStraightToBag(balls[0]);
                ct++;
            }
            StartCoroutine(waitForWhiteReplaced((-target.transform.position + bagPos[targetBag]).normalized, target));
            //随机摆放在自己球到洞的直线上
        }
        //AIShoot();
    }
    IEnumerator waitForWhiteReplaced(GameObject target)
    {
        balls[0].GetComponent<ball0Script>().isfreeball = false;
        balls[0].SetActive(true);
        yield return new WaitForSeconds(1f);
        AIShoot(target);
    }

    IEnumerator waitForWhiteReplaced(Vector3 d, GameObject target)
    {
        balls[0].SetActive(true);
        balls[0].GetComponent<ball0Script>().isfreeball = false;
        yield return new WaitForSeconds(1f);
        Shoot(d, target);
    }

    float NextGaussian()//credit to https://blog.csdn.net/georgeandgeorge/article/details/89480224
    {
        float v1, v2, s;
        do
        {
            v1 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            v2 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0f);

        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);

        return v1 * s;
    }


    float NextGaussian(float mean, float standard_deviation)//credit to https://blog.csdn.net/georgeandgeorge/article/details/89480224
    {
        return mean + NextGaussian() * standard_deviation;
    }

    float NextGaussian(float mean, float standard_deviation, float min, float max)//credit to https://blog.csdn.net/georgeandgeorge/article/details/89480224
    {
        float x;
        do
        {
            x = NextGaussian(mean, standard_deviation);
        } while (x < min || x > max);
        return x;
    }

    Vector3 findAngleToWall()
    {
        Rigidbody currRb = balls[0].GetComponent<Rigidbody>();
        //Vector3 whitePos = balls[0].transform.position;
        for (int i = 0; i < 360; i += 2)
        {
            Vector3 currD = new Vector3(Mathf.Cos(i), 0, Mathf.Sin(i));
            RaycastHit hit;
            if (currRb.SweepTest(currD, out hit, scale * 100))
            {
                if (hit.collider.name.Contains("wall"))
                {
                    if (ballExistOnPath(hit.point + hit.normal * ballR, Vector3.Reflect(currD, hit.normal)))
                    {
                        ttarget = hit.collider.gameObject;
                        return currD;
                    }
                }
            }
        }
        //返回目标方向，否则返回(0,1,0)
        return Vector3.up;
    }

    Vector3 findAngleToshoot(GameObject target)
    {
        Vector3 whitePos = balls[0].transform.position;
        Rigidbody currRb = target.GetComponent<Rigidbody>();
        for (int i = 0; i < 6; i++)
        {
            RaycastHit hit;
            if (currRb.SweepTest(bagPos[i] - whitePos, out hit, (bagPos[i] - whitePos).magnitude))
            {
                if (!hit.collider.name.Contains("ball"))
                {
                    hitPt = hit.point + hit.normal * ballR;
                    return (hitPt - whitePos).normalized;
                }
            }
            else
            {

                hitPt = hit.point + hit.normal * ballR;
                return (hitPt - whitePos).normalized;
            }
        }
        //返回目标方向，否则返回(0,1,0)
        return Vector3.up;
    }

    void RandomShoot()
    {
        float W = NextGaussian(0f, 5f, -40f, 40f);
        float H = NextGaussian(0f, 5f, -40f, 40f);
        float power = NextGaussian(0.37f, 0.002f, 0.07f, 0.7f);
        float angle = Random.Range(0f, 360f);
        Vector3 Direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        Vector3 whitePos = balls[0].transform.position;

        Vector3 spinV = Quaternion.Euler(0, -W * 45f / 80f, 0) * Quaternion.AngleAxis(H * 45f / 80f, Quaternion.Euler(0, 90, 0) * Direction) * (-Direction) * ballR;

        RectTransform m_RectTransform = redDot.GetComponent<RectTransform>();
        m_RectTransform.anchoredPosition= new Vector3( W,H,0f);

        float a = 102.73574623625f;
        float b = -1.91154301352143f;
        float c = 0.410501861817418f;
        float d = -0.186904776344724f;
        float h = (a - d) / (1f + Mathf.Pow((power / c), b)) + d;
        powerBar.GetComponent<Healthbar>().SetHealth((int)h);

        balls[0].GetComponent<ball0Script>().hittedByCue(Direction * power * scale * scale, whitePos + spinV, scale);
        ballsScript.status = 5;
        Debug.Log("随即击球");
    }

    void backShoot(Vector3 direction, GameObject tragetBall)
    {
        //calculate min v
        Vector3 whitePos = balls[0].transform.position;
        float a = ballsScript.rollDrag;
        float k = ballsScript.ballBallCoefficientOfRestitution;
        float l1 = (bagPos[targetBag] - tragetBall.transform.position).magnitude;
        float l2 = (hitPt - whitePos).magnitude;
        float theta = Vector3.Angle(direction, bagPos[targetBag] - tragetBall.transform.position);
        float vMin = Mathf.Sqrt(
            (
            2f * a * (l1 + l2)
            )
            /
            (
            Mathf.Cos(theta) * Mathf.Cos(theta) * k * k
            ));
        float power = NextGaussian(vMin * 1.5f * mass, 0.002f, vMin * mass, vMin * 2f * mass);
        float W = NextGaussian(0f, 1f, -40f, 40f);
        float H = NextGaussian(-10f, 2f, -40f, 0f);
        Vector3 spinV = Quaternion.Euler(0, -W * 45f / 80f, 0) * Quaternion.AngleAxis(H * 45f / 80f, Quaternion.Euler(0, 90, 0) * direction) * (-direction) * ballR;


        float aa = 102.73574623625f;
        float bb = -1.91154301352143f;
        float cc = 0.410501861817418f;
        float dd = -0.186904776344724f;
        float h = (aa - dd) / (1f + Mathf.Pow((power / cc), bb)) + dd;
        powerBar.GetComponent<Healthbar>().SetHealth((int)h);

        RectTransform m_RectTransform = redDot.GetComponent<RectTransform>();
        m_RectTransform.anchoredPosition = new Vector3(W, H, 0f);

        balls[0].GetComponent<ball0Script>().hittedByCue(direction * power * scale, whitePos, scale);
        ballsScript.status = 5;
        Debug.Log("低杆击球 目标："+ tragetBall.name);
    }

    void Shoot(Vector3 direction)
    {
        //calculate min v
        Vector3 whitePos = balls[0].transform.position;
        float power = NextGaussian(0.3f, 0.002f, 0.07f, 0.7f);
        //Debug.DrawRay(whitePos, direction * power, Color.white, 15f);


        float a = 102.73574623625f;
        float b = -1.91154301352143f;
        float c = 0.410501861817418f;
        float d = -0.186904776344724f;
        float h = (a - d) / (1f + Mathf.Pow((power / c), b)) + d;
        powerBar.GetComponent<Healthbar>().SetHealth((int)h);



        balls[0].GetComponent<ball0Script>().hittedByCue(direction * power * scale * scale, whitePos, scale);
        ballsScript.status = 5;
        Debug.Log("普通击球改变：" + ballsScript.status);
        Debug.Log("普通击球");
    }

    void Shoot(Vector3 direction, GameObject tragetBall)
    {
        //calculate min v
        Vector3 whitePos = balls[0].transform.position;
        float a = ballsScript.rollDrag;
        float k = ballsScript.ballBallCoefficientOfRestitution;
        float l1 = (bagPos[targetBag] - tragetBall.transform.position).magnitude;
        float l2 = (hitPt - whitePos).magnitude;
        float theta = Vector3.Angle(direction, bagPos[targetBag] - tragetBall.transform.position);
        float vMin = Mathf.Sqrt(
            (
            2f * a * (l1 + l2)
            )
            /
            (
            Mathf.Cos(theta) * Mathf.Cos(theta) * k * k
            ));
        float power = NextGaussian(vMin * mass, 0.002f, 0.8f * vMin * mass, vMin * 1.2f * mass);
        //Debug.DrawRay(whitePos, direction * power, Color.white, 15f);
        balls[0].GetComponent<ball0Script>().hittedByCue(direction * power * scale, whitePos, scale);
        ballsScript.status = 5;
        Debug.Log("普通击球改变：" + ballsScript.status);
        Debug.Log("普通击球");
    }

    bool getWhiteOutputToBag(GameObject target, Vector3 shootDirection)
    {
        Vector3 v1 = target.transform.position - hitPt;
        Vector3 v2 = shootDirection - Vector3.Project(shootDirection, v1);
        for (int i = 0; i < 6; i++)
        {
            if (Vector3.Angle(bagPos[i] - hitPt, v2) < 3f)
                return true;
        }
        return false;
    }

    public void AIThinking(bool isFreeBall)
    {
        //#1
        Debug.Log("AIthinking #1");
        if (isFreeBall)
        {
            //#2
            Debug.Log("AIthinking #2");
            GameObject tragetBall = findABallStraightToBag();
            if (tragetBall == null)
            {
                //#4
                Debug.Log("AIthinking #4");
                placeFreeBall();
            }
            else
            {
                //#5
                Debug.Log("AIthinking #5");
                placeFreeBall(tragetBall);
            }
        }
        else
        {
            //#3
            Debug.Log("AIthinking #3");
            AIShoot();
        }
    }

    void AIShoot(GameObject tragetBall = null)
    {
        if (tragetBall == null)
        {
            //#1
            Debug.Log("AIShoot #1");
            tragetBall = findABallStraightToBag(balls[0]);
            if (tragetBall == null)
            {
                //#3
                Debug.Log("AIShoot #3");
                Vector3 shootDirection = findAngleToWall();
                if (shootDirection != Vector3.up)
                {
                    //#7
                    Debug.Log("AIShoot #7");
                    Shoot(shootDirection);
                }
                else
                {
                    //#6
                    Debug.Log("AIShoot #6");
                    RandomShoot();
                }
            }
            else
            {
                //#2
                Debug.Log("AIShoot #2");
                Vector3 shootDirection = findAngleToshoot(tragetBall);
                if (shootDirection == Vector3.up)
                {
                    //#5
                    Debug.Log("AIShoot #5");
                    if (possibleTargets.Count > 1)
                    {
                        for (int i = 1; i < possibleTargets.Count; i++)
                        {
                            shootDirection = findAngleToshoot(possibleTargets[i]);
                            if (shootDirection != Vector3.up)
                            {
                                tragetBall = possibleTargets[i];
                                break;
                            }

                        }
                        if (shootDirection != Vector3.up)
                        {

                            //
                            if (getWhiteOutputToBag(tragetBall, shootDirection))
                            {
                                //#8
                                Debug.Log("AIShoot #8");
                                backShoot(shootDirection, tragetBall);
                            }
                            else
                            {
                                //#9
                                Debug.Log("AIShoot #9");
                                Shoot(shootDirection, tragetBall);
                            }
                            //
                        }
                        else
                        {
                            RandomShoot();
                        }
                    }
                    else
                    {
                        RandomShoot();
                    }
                }
                else
                {
                    //
                    //#4
                    Debug.Log("AIShoot #4");
                    if (getWhiteOutputToBag(tragetBall, shootDirection))
                    {
                        //#8
                        Debug.Log("AIShoot #8");
                        backShoot(shootDirection, tragetBall);
                    }
                    else
                    {
                        //#9
                        Debug.Log("AIShoot #9");
                        Shoot(shootDirection, tragetBall);
                    }
                    //
                }
            }
        }
        else
        {
            Debug.Log("AIShoot with target");
            //Debug.Log(findAngleToshoot(tragetBall)); Debug.DrawRay(balls[0].transform.position, findAngleToshoot(tragetBall), Color.white, 15f);
            Shoot(findAngleToshoot(tragetBall), tragetBall);
        }
    }
}