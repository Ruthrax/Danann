using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2CTeleportation : Danu_State
{
    public P2CTeleportation() : base(StateNames.P2C_TELEPORTATION){}

    GameObject arrival, fakeArrival;
    GameObject boomBox,fakeBoomBox;
    AttackData boomBoxAttackData,fakeBoomBoxAttackData;
    Transform target;
    [SerializeField] P1CTeleportation.destPoints destination;
    [SerializeField] float MaxFadeTime;
    [SerializeField] float MaxSartup;
    [SerializeField] float offsetValue;
    [SerializeField] float maxReco;
    [SerializeField] float maxActive;
    [SerializeField] float farDist;
    float fadeTime;
    float startup;
    float active;
    [SerializeField]Vector3 arenaCenter;
    public enum destPoints
    {
        FAR,
        CLOSE
    }
    float reco;

    [SerializeField]float arenaRadius;
    public override void Init()
    {
        arrival=fsm.GetP2TP_Arrival();
        fakeArrival=fsm.GetP2TP_FakeArrival();
        boomBox=fsm.GetP2TP_Boombox();
        fakeBoomBox=fsm.GetP2TP_FakeBoombox();
        boomBoxAttackData=boomBox.GetComponent<AttackData>();
        fakeBoomBoxAttackData=fakeBoomBox.GetComponent<AttackData>();
        target=fsm.agent.GetPlayer();
        arenaCenter = fsm.agent.GetArenaCenter();

    }
    // Start is called before the first frame update
    public override void Begin() 
    {        
        Init();
        destination=fsm.GetP1TP_Destination();
        arrival.SetActive(false);
        fakeArrival.SetActive(false);
        if (destination == P1CTeleportation.destPoints.FAR)
        {
            float dist = farDist / Vector3.Distance(fsm.transform.position, target.position);
            Vector3 dir = fsm.transform.position - target.position;
            dir.Normalize();
            dir *= farDist;
            Vector2 rand = Random.insideUnitCircle;
            Vector3 offset = new Vector3(rand.x, 0, rand.y) * offsetValue;
            arrival.transform.position = fsm.transform.position + dir-offset;
            fakeArrival.transform.position = fsm.transform.position + dir+offset;
            if (Vector3.Distance(arrival.transform.position, arenaCenter) >= arenaRadius)
            {
                dir = fsm.transform.position - target.position;
                dir.Normalize();
                arrival.transform.position =arenaCenter+ arenaRadius * dir;
            }            
            if (Vector3.Distance(fakeArrival.transform.position, arenaCenter) >= arenaRadius)
            {
                dir = fsm.transform.position - target.position;
                dir.Normalize();
                fakeArrival.transform.position =arenaCenter+ arenaRadius * dir;
            }
        }
        else
        {
            Vector2 rand = Random.insideUnitCircle;
            Vector3 offset = new Vector3(rand.x, 0, rand.y) * offsetValue;
            arrival.transform.position = target.position + offset;
            fakeArrival.transform.position = target.position - offset;
        }
        startup=0;
        fadeTime=0;
        active=0;
        reco=0;
    }

    // Update is called once per frame
    public override void Update() 
    {
        TP();
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
            fakeArrival.SetActive(true);
            fadeTime += Time.deltaTime;
        }
        else if (active <= maxActive)
        {
            //boomBox.SetActive(true);
            boomBoxAttackData.LaunchAttack();
            fakeBoomBoxAttackData.LaunchAttack();
            fsm.transform.position = arrival.transform.position;
            active += Time.deltaTime;
        }
        else if (reco <= maxReco)
        {
            boomBoxAttackData.StopAttack();
            fakeBoomBoxAttackData.StopAttack();
            arrival.SetActive(false);
            fakeArrival.SetActive(false);
            reco += Time.deltaTime;
        }
        else
        {
            Debug.Log("over");
        }
    }


    
}