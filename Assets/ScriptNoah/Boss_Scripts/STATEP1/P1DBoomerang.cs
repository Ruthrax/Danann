using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1DBoomerang : Danu_State
{
    public P1DBoomerang() : base(StateNames.P1D_BOOMERANG) { }
    GameObject boomerangL, boomerangR;
    AttackData boomerangAttackData;
    Transform target;
    GameObject preview;
    float speed;
    float MaxStraightTime;
    Transform curveMidL, curveMidR;
    float MaxCurveTime;
    float straightTime;
    Vector3 curveStartL,curveStartR;
    Vector3 baseStartL,baseStartR;
    Vector3 startL,startR;
    Vector3 curveEnd;
    float curveTime = 0;
    bool startCurve;
    float waitTime;
    bool wait;
    float maxWaitTime = 1;
    float maxDistance;

    // Start is called before the first frame update
    public override void Begin()
    {
        if (!isInit)
            Init();
        curveTime=0;
        straightTime=0;
        Vector3 straightTarget =new Vector3( target.position.x,fsm.transform.position.y,target.position.z);
        fsm.transform.LookAt(straightTarget);
        boomerangR.transform.localPosition=baseStartR;
        boomerangL.transform.localPosition=baseStartL;
        startL=boomerangL.transform.position;
        startR=boomerangR.transform.position;
        curveStartL = boomerangL.transform.position + fsm.transform.forward * maxDistance ;
        curveStartR = boomerangR.transform.position + fsm.transform.forward * maxDistance;
        curveEnd = fsm.transform.position;
        Vector3 straightEnd = (curveStartL + curveStartR) / 2;
        preview.gameObject.SetActive(true);
        fsm.transform.LookAt(straightTarget);
        SoundManager.Instance.PlayBossBoomerangCharge();
        wait = true;
        waitTime=0;
    }
    public override void Init()
    {
        boomerangAttackData = fsm.GetBoomerangAttackData();
        target = fsm.agent.GetPlayer();

        preview = fsm.GetP1sD_Preview();
        boomerangL = fsm.GetP1BRL();
        boomerangR = fsm.GetP1BRR();
        curveMidL = fsm.GetP1BoomeRangcurveMidL();
        curveMidR = fsm.GetP1BoomeRangcurveMidR();

        speed = fsm.GetP1BoomeRangSpeed();
        maxDistance=fsm.GetP1BR_MaxDist();
        MaxCurveTime = fsm.GetP1BoomeRangMaxCurveTime();
        MaxStraightTime = fsm.GetP1BoomeRangMaxStraightTime();
        maxWaitTime = fsm.GetP1BR_Startup();
        baseStartL=boomerangL.transform.localPosition;
        baseStartR=boomerangR.transform.localPosition;   
        base.Init();
    }
    // Update is called once per frame
    public override void Update()
    {
        if (wait)
        {
            waitTime += Time.deltaTime;
            if (waitTime >= maxWaitTime)
            {
                wait = false;
                SoundManager.Instance.PlayBossBoomerangGo();
                boomerangAttackData.LaunchAttack();
            }
            return;
        }
        if (startCurve)
        {
            UpdateCurve();
        }
        if (!startCurve)
        {
            UpdateStraight();
        }
    }
    void UpdateStraight()
    {
        
        boomerangL.transform.position = Vector3.Lerp(startL, curveStartL, straightTime / MaxStraightTime);
        boomerangR.transform.position = Vector3.Lerp(startR, curveStartR, straightTime / MaxStraightTime);
        straightTime += Time.deltaTime;
        if (boomerangL.transform.position == curveStartL||boomerangR.transform.position==curveStartR ||straightTime>=MaxStraightTime)
        {    
            preview.gameObject.SetActive(false);
            startCurve = true;
            SoundManager.Instance.PlayBossBoomerangBack();
            UpdateCurve();
        }
    }
    void UpdateCurve()
    {
        curveTime += Time.deltaTime;
        if (curveTime > MaxCurveTime)
        {
            curveTime=0;
            straightTime=0;
            startCurve=false;
            fsm.agent.m_anims.SetTrigger("BoomerangOver");
            boomerangAttackData.StopAttack();
            if (orig == null)
            {
                fsm.agent.ToIdle();
            }
            else
            {
                orig.AddWaitTime(2);
                orig.FlowControl();
            }
            return;
        }
        boomerangL.transform.position = Curve(boomerangL);
        boomerangR.transform.position = Curve(boomerangR);
    }
    private Vector3 Curve(GameObject boom)
    {
        float t = curveTime / MaxCurveTime;
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 point;
        if (boom == boomerangL)
        {
            point = uu * curveStartL;
            point += 2 * u * t * curveMidL.position;
        }
        else
        {
            point = uu * curveStartR;
            point += 2 * u * t * curveMidR.position;
        }
        point += tt * curveEnd;
        return point;
    }
    public override void End()
    {
        boomerangL.SetActive(false);
        boomerangR.SetActive(false);
        boomerangR.transform.localPosition=baseStartR;
        boomerangL.transform.localPosition=baseStartL;
        boomerangAttackData.StopAttack();
        base.End();
    }
}