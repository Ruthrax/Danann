using System.Collections.Generic;
using UnityEngine;

public class P1DShoot : Danu_State
{
    public P1DShoot() : base(StateNames.P1D_SHOOT) { }
    float timer;
    GameObject preview;
    float delay = 0.2f;
    int index;
    int nbShot = 6;
    List<GameObject> proj;
    Pool pool;
    float maxLifeTime;
    float speed;
    private bool wait;
    private float waitTime;
    private float maxWaitTime=0.2f;
    Transform target;

    // Start is called before the first frame update
    public override void Begin()
    {
        if (!isInit)
            Init();

        index = 0;
        timer = 0;
        waitTime=0;
        wait=true;
        Vector3 dir = (-fsm.transform.position + target.position).normalized;
        Vector3 straightTarget =new Vector3( target.position.x,fsm.transform.position.y,target.position.z);
        fsm.transform.LookAt(straightTarget);
        preview.gameObject.SetActive(true);

    }
    public override void Init()
    {
        pool = fsm.GetPool();
        maxLifeTime = fsm.GetP1d_ProjLifeTime();
        delay = fsm.GetP1d_delay();
        nbShot = fsm.GetP1d_nbShot();
        speed = fsm.GetP1d_ShotSpeed();
        target=fsm.agent.GetPlayer();
        Vector3 straightTarget =new Vector3( target.position.x,fsm.transform.position.y,target.position.z);
        fsm.transform.LookAt(straightTarget);
        preview=fsm.GetP1sD_Preview();
        base.Init();
    }
    // Update is called once per frame
    public override void Update()
    {
        if (wait)
        {
            waitTime+=Time.deltaTime;

            Vector3 straightTarget =new Vector3( target.position.x,fsm.transform.position.y,target.position.z);
            fsm.transform.LookAt(straightTarget);
            float rotateValue=fsm.transform.rotation.eulerAngles.y;
            fsm.transform.rotation=Quaternion.Euler(0,rotateValue,0);
            Vector3 dir = (-fsm.transform.position + target.position).normalized;
            if (waitTime>=maxWaitTime)
            {
                preview.gameObject.SetActive(false);
                wait=false;
            }
            return;
        }
        timer += Time.deltaTime;
        Vector3 straightTargets =new Vector3( target.position.x,fsm.transform.position.y,target.position.z);
            fsm.transform.LookAt(straightTargets);
            float rotateValues=fsm.transform.rotation.eulerAngles.y;
            fsm.transform.rotation=Quaternion.Euler(0,rotateValues,0);
        if (timer > delay)
        {
            timer = 0;
            if (index > nbShot)
            {
                fsm.agent.m_anims.SetTrigger("ShootOver");
                if (!fsm.agent.followsGlobal)
                {
                    fsm.agent.SetWaitingTime(2);
                    fsm.agent.ToIdle();
                }
                else
                {
                    orig.AddWaitTime(2);
                    orig.FlowControl();
                }
                return;
            }
            else
            {

                index++;
                SoundManager.Instance.PlayBossShoot();
                GameObject go = pool.Get();
                go.GetComponent<Projectiles>().SetSpeed(speed);
                go.transform.position = fsm.transform.position;
                go.SetActive(true);
                go.GetComponent<AttackData>().LaunchAttack();
            }
        }
    }
    public override void End()
    {

    }
}
