using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    Transform target;
    [SerializeField] float speed;
    [SerializeField] float turnRate;
    [SerializeField] float turnTime;
    [SerializeField] int nbTurn;
    float lifeTime = 0;
    float dist;
    Pool origin;
    [SerializeField] float maxLifeTime;
    [SerializeField] AttackData _attackData;

    // Start is called before the first frame update
    void Start()
    {
        
        transform.LookAt(target);
        if (maxLifeTime == 0)
            maxLifeTime = 12;
        if (nbTurn==0)
        {
            maxLifeTime=8;
        }
    }
    private void OnEnable()
    {
        transform.LookAt(target);
        lifeTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        turnTime += Time.deltaTime;
        lifeTime += Time.deltaTime;
        if (turnTime > turnRate && nbTurn > 0)
        {
            turnTime = 0;
            Vector3 newTarget = Vector3.Lerp(transform.position + transform.forward, target.position, 0.25f);
            transform.LookAt(newTarget);
            nbTurn--;
        }
        if( target!=null)
        {
        if (Vector3.Distance(transform.position, target.position) <= 2.5f)
            nbTurn = 0;

        }
        if (lifeTime > maxLifeTime) 
        {

            if (origin!=null) 
            {
                transform.position=origin.transform.position;
                gameObject.SetActive(false);
                origin.Back(gameObject);
                return; 
            }
            transform.position = Vector3.zero; 
            gameObject.SetActive(false);
        }
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    public void SetOrigin(Pool pool)
    {
        origin = pool;
    }
    public void SetSpeed(float newSpeed) { speed = newSpeed; }
    public void SetLifeTime(float newLT) {if (nbTurn!=0) maxLifeTime = newLT; }

    internal void SetSpeed(object projSpeed)
    {
        throw new NotImplementedException();
    }


}
