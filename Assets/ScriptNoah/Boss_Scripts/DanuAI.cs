using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Cinemachine;
public class DanuAI : MonoBehaviour
{
    private Danu_FSM m_fsm=null;
    enum GlobalPattern
    {
        SHOOT,
        DASH,
        TP,
        BR,
        IDLE
    }
    [SerializeField] CinemachineTargetGroup cam;
    public CinemachineTargetGroup GetCam(){return cam;}
    public bool isRevengeHigh{get;private set;}
    public bool wasParried{get;private set;}
    public float distLimit{get;private set;}
    [SerializeField] GlobalPattern globalStates;
    GlobalPattern actualState;
    [Range(1,2),SerializeField]private int phase;
    private int patternIndex;
    private float dist;
    [SerializeField] private Transform player;
    [SerializeField] private AnimationCurve distEvaluator;
    [SerializeField] private float revenge;
    [SerializeField] private float maxRevenge;

    public float GetMovementRange()
    {
        return movementRange;
    }
    [SerializeField] private float waitingTime;
    
    [SerializeField] private float arenaRadius;
    [SerializeField] private Vector3 arenaCenter;
    [SerializeField] private float movementRange;
    [SerializeField] private float maxChain;
    float chain;
    [SerializeField] private GameObject projectile;
    private bool isPushed;
    private float hp;
    private float maxHP;
    private List<string> lastStates=new List<string>();
    [SerializeField]int maxCap;
    [SerializeField] bool goingRandom;
    [SerializeField] private int maxProjectilePool;
    public bool isStun{get;private set;}
    float stunTime;
    float maxStunTime;
    Danu_GlobalFSM gfsm;
    float revengeTime;
    float revengeMaxTime;

    [SerializeField] public bool followsGlobal{get;private set;}
    private float revengeSpeed;

    public bool GetFollowingGlobal(){return followsGlobal;}
    private void Awake() {
        if (m_fsm==null)
            m_fsm=GetComponent<Danu_FSM>();
        m_fsm.agent=this;
        if (gfsm==null)
            gfsm=GetComponent<Danu_GlobalFSM>();
        gfsm.agent=this;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (followsGlobal)
        {
            gfsm.AddState(new GP1Dash());
            gfsm.AddState(new GP1Boomerang());
            gfsm.AddState(new GP1Shoot());
            gfsm.AddState(new GP1Teleportation());
            gfsm.ChangeState(StateNames.P1IDLE);
        }
        else
        {
            m_fsm.AddState(new P1DShoot());
            m_fsm.AddState(new P1Idle());
            m_fsm.AddState(new P1CSlam());
            m_fsm.AddState(new P1DSpin());
            m_fsm.AddState(new P1DBoomerang());
            m_fsm.AddState(new P1CDash());
            m_fsm.AddState(new P1CTeleportation());
            m_fsm.AddState(new P1CMixDash());
            this.m_fsm.ChangeState( StateNames.P1D_SPIN); 

        }
    }

    public void Stun(float sTime)
    {
        if (isStun)
            maxStunTime+= sTime/2;
        else
            maxStunTime=sTime;
        isStun=true;
    }
    // Update is called once per frame
    void Update()
    {
        float nx=Mathf.Clamp(transform.position.x,arenaCenter.x-arenaRadius,arenaCenter.x+arenaRadius);
        float nz=Mathf.Clamp(transform.position.z,arenaCenter.z-arenaRadius,arenaCenter.z+arenaRadius);
        transform.position=new Vector3(nx,arenaCenter.y,nz);
        if (isStun)
        {
            stunTime+=Time.deltaTime;
            if (stunTime>=maxStunTime)
            {
                stunTime=0;
                maxStunTime=0;
                isStun=false;
            }
            return;
        }
        if (revenge>0 && revengeTime>=revengeMaxTime)
        {
            revenge=Mathf.Clamp(revenge-Time.deltaTime*revengeSpeed,0,maxRevenge);
            return;
        }
        if (revenge>0)
        {
            revengeTime=Mathf.Clamp( revengeTime+Time.deltaTime,0,revengeMaxTime);
        }

    }
    public void BuildUpRevenge(float add)
    {
        revenge=Mathf.Clamp(revenge+add,0,maxRevenge);
        if (revenge>70)
            isRevengeHigh=true;
        else
            isRevengeHigh=false;
        revengeTime=0;
    }
    [Button]
    public void NextPattern() 
    {
                    this.m_fsm.ChangeState( StateNames.P1C_SLAM); 
                        return;
        if (phase==1)
        {
            float revengePercent=revenge*100/maxRevenge;
            if (goingRandom)
            {
                int chance=Random.Range(1,7);
                    switch (chance)
                    {
                        case 1:
                        
                            int rand=Random.Range(0,2);
                            if (rand==1)
                                m_fsm.ChangeState(StateNames.P1C_MIXDASH);
                            else
                                m_fsm.ChangeState(StateNames.P1C_DASH);
                            break;
                        case 2:
                            m_fsm.ChangeState(StateNames.P1C_SLAM);
                            break;
                        case 3:
                            Vector3 playerPos=player.position;
                            Vector3 agentPos=transform.position;
                            float dist=Vector3.Distance(playerPos,agentPos);
                            if (dist>distLimit)
                            {
                                m_fsm.SetTPDest(P1CTeleportation.destPoints.FAR);  
                            }
                            else
                            {
                                m_fsm.SetTPDest(P1CTeleportation.destPoints.CLOSE);  
                            }
                            m_fsm.ChangeState(StateNames.P1C_TELEPORTATION);
                            break;
                        case 4:
                            m_fsm.ChangeState(StateNames.P1D_BOOMERANG);
                            break;
                        case 5:
                            m_fsm.ChangeState(StateNames.P1D_SHOOT);
                            break;
                        case 6:
                            m_fsm.ChangeState(StateNames.P1D_SPIN);
                            break;                    

                    }
                    return;
            }
            else
            {
                m_fsm.ChangeState(StateNames.P1C_MIXDASH);
                if (!goingRandom)
                    return;
            }
        }
    }
    public void NextGlobalPattern()
    {
            float revengePercent=revenge*100/maxRevenge;
            dist = Vector3.Distance(transform.position, player.position);
            float mod = distEvaluator.Evaluate(dist);
            if (mod <= 1.1f) //short ranged patterns
            {
                Debug.Log("smol");
                if (lastStates.Contains(StateNames.P1C_TELEPORTATION))
                {
                    if (isRevengeHigh)
                    {
                        m_fsm.ChangeState(StateNames.P1C_MIXDASH);
                        return;
                    }
                    m_fsm.ChangeState(StateNames.P1C_DASH);
                    return;
                }
                m_fsm.ChangeState(StateNames.P1C_TELEPORTATION);
                lastStates.Clear();
                return;

            }
            else if (mod > 1.1f) //long ranged patterns
            {
                Debug.Log("loooong");
                if (lastStates.Contains(StateNames.P1D_SHOOT)||lastStates.Contains(StateNames.P1D_BOOMERANG))
                {
                    lastStates.Clear();
                    m_fsm.ChangeState(StateNames.P1C_TELEPORTATION);
                    return;
                }
                
                    if (isRevengeHigh)
                    {
                        m_fsm.ChangeState(StateNames.P1D_BOOMERANG);
                        return;
                    }
                    m_fsm.ChangeState(StateNames.P1D_SHOOT);
                    return;


            }
            else //all patterns possible
            {
                m_fsm.ChangeState(StateNames.P1D_SPIN);
                Debug.Log("midwest");
            }
    }
    public void NextPhase()
    {
        if (phase != 1)
            return;
        phase++;
        //Add all P2 states and remove all P1 states
        NextPattern();
    }
    public int GetPhase() { return phase; }
    public Transform GetPlayer(){return player;}
    public GameObject GetProjectile(){return projectile;}
    public void SpawnProjectile()
    {
        m_fsm.AddProjectile();
        m_fsm.GetProjectile().SetActive(true);
    }
    public void ToIdle()
    {/*
        chain=Mathf.Clamp(chain--,0,maxChain);
        if (phase==1)
        {
            if (chain==0)
                m_fsm.ChangeState(StateNames.P1IDLE);
            else
                NextPattern();
        }
        else if (phase==2)
            m_fsm.ChangeState(StateNames.P2IDLE);
    */
        /*if (m_fsm.curr.name==StateNames.P1IDLE)
            return;*/
        lastStates.Add(m_fsm.curr.name);
        if(lastStates.Count>maxCap)
            lastStates.RemoveAt(0);
         m_fsm.ChangeState(StateNames.P1IDLE);
    }
    public float GetWaitingTime()
    {
        return waitingTime;
    }
    public void SetWaitingTime(float newWT)
    {
        waitingTime=newWT;
    }
    public void RestoreChain(){
        chain=maxChain;
    }
    public Vector3 GetArenaCenter(){return arenaCenter;}
    public float GetArenaRadius(){return arenaRadius;}
    public bool GetIsPushed(){return isPushed;}
}
