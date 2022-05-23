using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2DSpin: Danu_State
{
    public P2DSpin() : base(StateNames.P2D_SPIN){}

    private GameObject globalGO;
    Transform preview;
    Transform[] bladesPreview = new Transform[4];
    AttackData bladesParentAttackData;
    private Transform dSphereN,dSphereW,dSphereE,dSphereS;
    private float dist;
    private float rotationSpeed;
    private bool turningRight;
    private float maxWaitTime;
    private float lifetime;
    Vector3 n,w,e,s;

    List<GameObject> nblades = new List<GameObject>();
    List<GameObject> wblades = new List<GameObject>();
    List<GameObject> eblades = new List<GameObject>();
    List<GameObject> sblades = new List<GameObject>();
    private bool wait;
    private float waitTime;
    bool isSetUp;
    Pool pool;
    // Start is called before the first frame update
    public override void Begin()
    {
        if (!isInit)
            Init();
        //activation des helices, ou instantiation si c'est la premiere fois
        preview.localScale = new Vector3(1, 1, Vector3.Distance(fsm.transform.position, fsm.agent.GetArenaCenter()));
        preview.position = fsm.transform.position + (fsm.agent.GetArenaCenter() - fsm.transform.position) / 2;
        preview.LookAt(fsm.agent.GetArenaCenter());
        preview.gameObject.SetActive(true);
        waitTime = 0;
        wait = true;
    }
    public override void Init()
    {
        pool = fsm.GetPool();
        //setup des variables
        dist = fsm.GetP1Sp_Dist();
        globalGO = fsm.GetP1GlobalGO();
        bladesParentAttackData = globalGO.GetComponent<AttackData>();
        preview = fsm.GetP1sD_Preview();
        Transform[] nwesTrans = fsm.GetP1NWEMax();
        bladesPreview = fsm.GetBladesPreview();
        dSphereN = nwesTrans[0];
        dSphereE = nwesTrans[1];
        dSphereW = nwesTrans[2];
        dSphereS = nwesTrans[3];
        rotationSpeed = fsm.GetP1RotationSpeed();
        turningRight = fsm.GetP1TurningRight();
        maxWaitTime = fsm.GetP1MaxWaitTime();
        lifetime = fsm.GetP1SpinLifeTime();
        base.Init();
    }
    // Update is called once per frame
    public override void Update()
    {
        if (wait)
        {
            waitTime += Time.deltaTime;
            fsm.transform.position = Vector3.Lerp(fsm.transform.position, fsm.agent.GetArenaCenter(), waitTime / maxWaitTime);
            if (fsm.transform.position == fsm.agent.GetArenaCenter())
            {
                SpawnBladesPreview();
            }
            if (waitTime >= maxWaitTime)
            {
                wait = false;
                SpawnBlades();
            }
            else
                return;
        }
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            if (orig == null)
                fsm.agent.ToIdle();
            else
                orig.progression++;
        }
        Rotate();
    }

    private void SpawnBladesPreview()
    {
        Transform[] nwesTrans = fsm.GetP1NWEMax();
        Vector3 center = fsm.agent.GetArenaCenter();
        preview.gameObject.SetActive(false);
        for (int i = 0; i < bladesPreview.Length; i++)
        {
            bladesPreview[i].gameObject.SetActive(true);
            bladesPreview[i].localScale = new Vector3(1, 1, Vector3.Distance(center, nwesTrans[i].position));
            bladesPreview[i].position = center + (nwesTrans[i].position - center) / 2;
            bladesPreview[i].LookAt(nwesTrans[i]);
        }
    }

    private void SpawnBlades()
    {
        float delta = 1 / dist;
        bladesPreview[0].gameObject.SetActive(false);
        bladesPreview[1].gameObject.SetActive(false);
        bladesPreview[2].gameObject.SetActive(false);
        bladesPreview[3].gameObject.SetActive(false);
        n = dSphereN.position;
        w = dSphereW.position;
        e = dSphereE.position;
        s = dSphereS.position;
        for (int i = 0; i < dist; i++)
        {
            SetupBall(pool.SecondGet(), Vector3.Lerp(fsm.transform.position, dSphereN.position, delta * i), nblades);
            SetupBall(pool.SecondGet(), Vector3.Lerp(fsm.transform.position, dSphereW.position, delta * i), wblades);
            SetupBall(pool.SecondGet(), Vector3.Lerp(fsm.transform.position, dSphereE.position, delta * i), eblades);
            SetupBall(pool.SecondGet(), Vector3.Lerp(fsm.transform.position, dSphereS.position, delta * i), eblades);
        }
        globalGO.SetActive(true);
        bladesParentAttackData.GetChildrenHitboxes();
        bladesParentAttackData.SetupHitboxes();
        bladesParentAttackData.LaunchAttack();
        isSetUp = true;
    }

    void SetupBall(GameObject ball, Vector3 position, List<GameObject> blades)
    {
        ball.transform.position = position;
        ball.transform.parent = bladesParentAttackData.transform;
        blades.Add(ball);
        ball.SetActive(true);
    }

    public void Regenerate(SpinBullet ded)
    {
        int newind = ded.GetIndex();
        SpinBullet.bladeIndex blade = ded.GetBlade();
        float delta = 1 / dist;
        switch (blade)
        {
            case SpinBullet.bladeIndex.NORTH:
                Vector3 nPos;
                nPos = Vector3.Lerp(fsm.transform.position, dSphereN.position, delta * newind);
                nblades.Add(fsm.InstantiateStaticProjectile(nPos));
                break;
            case SpinBullet.bladeIndex.WEST:
                Vector3 wPos;
                wPos = Vector3.Lerp(fsm.transform.position, dSphereW.position, delta * newind);
                wblades.Add(fsm.InstantiateStaticProjectile(wPos));
                break;
            case SpinBullet.bladeIndex.EAST:
                Vector3 ePos;
                ePos = Vector3.Lerp(fsm.transform.position, dSphereE.position, delta * newind);
                eblades.Add(fsm.InstantiateStaticProjectile(ePos));
                break;
            case SpinBullet.bladeIndex.SOUTH:
                Vector3 sPos;
                sPos = Vector3.Lerp(fsm.transform.position, dSphereS.position, delta * newind);
                eblades.Add(fsm.InstantiateStaticProjectile(sPos));
                break;
        }
    }
    private void Rotate()
    {
        if (turningRight)
            globalGO.transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
        else
            globalGO.transform.Rotate(new Vector3(0, -rotationSpeed * Time.deltaTime, 0));
    }
    public override void End()
    {
        globalGO.SetActive(false);
        lifetime = fsm.GetP1SpinLifeTime();
        bladesParentAttackData.StopAttack();
        globalGO.transform.rotation = Quaternion.identity;
        float delta = 1 / dist;
        for (int i = 0; i < nblades.Count; i++)
        {
            nblades[i].SetActive(false);
            wblades[i].SetActive(false);
            eblades[i].SetActive(false);
            sblades[i].SetActive(false);
        }
    }
}
