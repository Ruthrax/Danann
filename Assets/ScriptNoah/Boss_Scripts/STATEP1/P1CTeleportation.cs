using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class P1CTeleportation : Danu_State
{
    public P1CTeleportation() : base(StateNames.P1C_TELEPORTATION) { }
    GameObject arrival;
    GameObject boomBox;
    AttackData boomBoxAttackData;
    [SerializeField] Transform target;
    [SerializeField] destPoints destination;
    [SerializeField] float MaxFadeTime;
    [SerializeField] float MaxSartup;
    [SerializeField] float offsetValue;
    [SerializeField] float maxReco;
    [SerializeField] float maxActive;
    [SerializeField] float farDist;
    float fadeTime;
    float startup;
    float active;
    Vector3 arenaCenter;
    CinemachineTargetGroup cam;
    public enum destPoints
    {
        FAR,
        CLOSE
    }
    float reco;
    private bool _lerpOut;
    private bool _lerpIn;
    float camWeight;
    // Start is called before the first frame update
    public override void Begin()
    {        
        if (!isInit)
            Init();
        destination=fsm.GetP1TP_Destination();
        arrival.SetActive(false);
        if (destination == destPoints.FAR)
        {
            float dist = farDist / Vector3.Distance(fsm.transform.position, target.position);
            Vector3 dir = fsm.transform.position - target.position;
            dir.Normalize();
            dir *= farDist;
            arrival.transform.position = fsm.transform.position + dir;
            if (Vector3.Distance(arrival.transform.position, arenaCenter) >= fsm.agent.GetArenaRadius())
            {
                dir = fsm.transform.position - target.position;
                dir.Normalize();
                arrival.transform.position = fsm.agent.GetArenaRadius() * dir;
            }
        }
        else
        {
            Vector2 rand = Random.insideUnitCircle;
            Vector3 offset = new Vector3(rand.x, 0, rand.y) * offsetValue;
            arrival.transform.position = target.position + offset;
        }
        camWeight=0.4f;
        _lerpIn=true;
        startup=0;
        fadeTime=0;
        active=0;
        reco=0;
    }
    public override void Init()
    {
        cam=fsm.agent.GetCam();
        arrival = fsm.GetP1TP_Arrival();
        boomBox = fsm.GetP1TP_Boombox();
        boomBoxAttackData = boomBox.GetComponent<AttackData>();
        destination = fsm.GetP1TP_Destination();
        MaxFadeTime = fsm.GetP1TP_Fadetime();
        MaxSartup = fsm.GetP1TP_Startup();
        offsetValue = fsm.GetP1TP_Offset();
        maxReco = fsm.GetP1TP_Recovery();
        maxActive = fsm.GetP1TP_Active();
        farDist = fsm.GetP1TP_FarDist();
        target = fsm.agent.GetPlayer();
        arenaCenter = fsm.agent.GetArenaCenter();
        base.Init();
    }
    // Update is called once per frame
    public override void Update()
    {
        TP();
        LerpIn();
        LerpOut();
    }

    private void LerpIn()
    {
        if (!_lerpIn)
            return;
        float realtime=startup+fadeTime;
        float lerpValue=Mathf.Clamp(realtime,0,1);
        
        cam.m_Targets[cam.m_Targets.Length-1].weight=lerpValue;
        if (lerpValue>=camWeight)
        {
            _lerpIn=false;
        }


    }

    private void LerpOut()
    {
        if (!_lerpOut)
            return;
        float lerpValue=Mathf.Lerp(0,maxReco,(maxReco-reco)*camWeight);
        
        cam.m_Targets[cam.m_Targets.Length-1].weight=lerpValue;
        if (lerpValue==0)
        {
            _lerpOut=false;
        }
    }

    void TP()
    {
        if (startup <= MaxSartup)
        {
            startup += Time.deltaTime;
        }
        else if (fadeTime <= MaxFadeTime)
        {
            arrival.SetActive(true);
            fadeTime += Time.deltaTime;
        }
        else if (active <= maxActive)
        {
            //boomBox.SetActive(true);
            boomBoxAttackData.LaunchAttack();
            fsm.transform.position = arrival.transform.position;
            active += Time.deltaTime;
        }
        else if (reco <= maxReco)
        {
            boomBoxAttackData.StopAttack();
            //boomBox.SetActive(false);

            _lerpOut=true;
            arrival.SetActive(false);
            reco += Time.deltaTime;
        }
        else
        {
            if (orig == null)
            {
                fsm.agent.ToIdle();
            }
            else
            {
                orig.AddWaitTime(2);
                orig.FlowControl();
            }
        }
    }
    public override void End()
    {

    }

    
}