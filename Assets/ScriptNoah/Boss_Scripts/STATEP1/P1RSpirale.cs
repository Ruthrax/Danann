using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1RSpirale : Danu_State
{
    public P1RSpirale() : base(StateNames.P1R_SPIRALE) { }
    // Start is called before the first frame update
    [SerializeField] int nb;
    float arenaRadius;
    Vector3 arenaCenter;
    GameObject proj;
    Pool pool;
    float lifetime;
    GameObject[] spirales;
    int nbBullets;
    float maxDelay;
    bool wait;
    private float waitTime;
    private float maxWaitTime;
    Transform preview;
    public override void Init()
    {
        base.Init();
        arenaCenter=fsm.agent.GetArenaCenter();
        arenaRadius=fsm.GetArenaDist();
        pool=fsm.GetRosacePool();
        proj = fsm.GetStraightProj();
        nb=fsm.GetRosaceNumber();
        nbBullets=fsm.GetRosaceBulletNB();
        maxDelay=fsm.GetRosaceDelay();
        maxWaitTime=fsm.GetP1MaxWaitTime();
        spirales=new GameObject[nb];
        preview = fsm.GetAOEPreview();

    }
    // Start is called before the first frame update
    public override void Begin()
    {
        if (!isInit)
            Init();
        lifetime=nbBullets*maxDelay+5;
        waitTime=0;
        fsm.transform.LookAt(fsm.agent.GetArenaCenter());
        preview.gameObject.SetActive(true);
        wait=true;
        SoundManager.Instance.PlayBossAllOut();
        fsm.agent.vfx[5].SetActive(true);

    }

    // Update is called once per frame
    public override void Update() 
    {
        if (wait)
        {
            waitTime += Time.deltaTime;
            fsm.transform.position = Vector3.Lerp(fsm.transform.position, fsm.agent.GetArenaCenter(), waitTime / maxWaitTime);
            
            if (waitTime >= maxWaitTime)
            {
                wait = false;
                preview.gameObject.SetActive(false);
                Start();
            }
            else
                return;
        }
        lifetime-=Time.deltaTime;
        if  (lifetime<=0)
        {
            Debug.Log("ee");
            fsm.agent.vfx[5].SetActive(false);
             if (orig == null)
            {
                Debug.Log("stop");
                fsm.agent.ToIdle();
            }
            else
            {
                orig.AddWaitTime(2);
                orig.FlowControl();
            }
        }
    }
    void Start()
    {

        int delta =360/nb;
        for (int i=0;i<nb;i++)
        {
            Vector3 pos = arenaCenter;
            float rad= delta*Mathf.Deg2Rad;
            Vector3 dest=new Vector3(Mathf.Cos(rad*i),0,Mathf.Sin(rad*i)).normalized;
            pos+=dest*arenaRadius;
            GameObject go= pool.Get();
            go.transform.position=pos;
            Pool pooll= go.GetComponent<Pool>();
            go.SetActive(true);
            pooll.SetUp(null,null,proj);
            go.GetComponentInChildren<MovingSpirale>().SetBullets(nbBullets);
            go.GetComponentInChildren<MovingSpirale>().SetDelay(maxDelay);            
            go.GetComponentInChildren<MovingSpirale>().SetOffset(true,rad*Mathf.Rad2Deg*i);
            spirales[i]=go;
        }
    }
    public override void End()
    {
        for (int i=0;i<nb;i++)
        {
            pool.Back(spirales[i]);
            fsm.agent.m_anims.SetTrigger("RosaceOver");
            spirales[i].SetActive(false);
            spirales[i]=null;

        }
    }

}
