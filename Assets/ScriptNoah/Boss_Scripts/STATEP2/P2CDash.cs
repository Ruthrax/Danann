using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2CDash : Danu_State
{
    public P2CDash() : base(StateNames.P2C_DASH){}
[SerializeField] Transform target;
    [SerializeField] GameObject preview;
    [SerializeField] float maxDashTime;
    [SerializeField] int dashCount;
    [SerializeField] int maxDashCount;
    [SerializeField] float dashSpeed;
    [SerializeField] float maxChargingTime;
    [SerializeField] AttackData dashAttackData;
    Vector3 dir;
    float dashTime;
    float chargingTime;
    bool isDashing;
    Vector3 maxArrival;
    private Vector3 startPos;
    // Start is called before the first frame update
    public override void Begin()
    {
        if (!isInit)
            Init();
        StartDash();
        dashCount = maxDashCount;
        chargingTime = 0;

    }
    public override void Init()
    {
        maxChargingTime = fsm.GetP2sD_ChargingTime();
        dashSpeed = fsm.GetP2sD_DashSpeed();
        maxDashTime = fsm.GetP2sD_MDashT();
        maxDashCount = 3;
        dashAttackData = fsm.GetP2DashAttackData();
        preview = fsm.GetP1sD_Preview();
        target = fsm.agent.GetPlayer();
        base.Init();

    }

    // Update is called once per frame
    public override void Update()
    {
        Dash();
    }
    private void Dash()
    {
        if (!isDashing)
        {
            return;
        }
        if (dashCount == 0)
        {
            return;
        }
        if (chargingTime <= maxChargingTime)
        {
            preview.gameObject.SetActive(true);
            chargingTime += Time.deltaTime;
            fsm.transform.position += dir * Time.deltaTime;
            //Vector3 arrival= transform.position+dir*dashSpeed*maxDashTime ;
            //arrival=new Vector3(arrival.x,3.72f,arrival.z);
            if (chargingTime>=maxChargingTime)
            {
                dashAttackData.LaunchAttack();
                fsm.agent.vfx[0].SetActive(true);
            }
            return;
        }
        dashTime += Time.deltaTime;
        fsm.transform.position += dir * dashSpeed * Time.deltaTime;
        if (dashTime >= maxDashTime)
        {
            preview.gameObject.SetActive(false);
            dashCount--;
            chargingTime/=2;
            fsm.agent.vfx[0].SetActive(false);
            StartDash();
            if (dashCount!=0)
                return;
            fsm.agent.vfx[0].SetActive(false);

             if (orig == null)
            {
                Debug.Log("stop");
                fsm.agent.ToIdle();
                dashAttackData.StopAttack();
            }
            else
            {
                orig.AddWaitTime(2);
                orig.FlowControl();
                dashAttackData.StopAttack();
            }
        }

    }
    void StartDash()
    {
        dashTime = 0;
        isDashing = true;
        dir = (-fsm.transform.position + target.position).normalized;
        startPos = fsm.transform.position;
        maxArrival = fsm.transform.position + dir * dashSpeed * dashTime;
        if (Vector3.Distance( maxArrival,fsm.agent.GetArenaCenter())>=fsm.agent.GetArenaRadius())
            maxArrival= maxArrival-fsm.agent.GetArenaCenter()*(fsm.agent.GetArenaRadius()/1.3f);
        Vector3 straightTarget =new Vector3( target.position.x,fsm.transform.position.y,target.position.z);
        fsm.transform.LookAt(straightTarget);


    }
    public override void End()
    {
        preview.gameObject.SetActive(false);
            fsm.agent.vfx[0].SetActive(false);
                dashAttackData.StopAttack();
    }
}
