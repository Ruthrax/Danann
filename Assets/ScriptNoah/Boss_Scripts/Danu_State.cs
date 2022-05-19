﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Danu_State
{
    public Danu_FSM fsm = null;
    public float idleTime;
    public string name { get; private set; }
    public bool isInit{get;private set;}
    public Danu_State( string name )
    {
        this.name = name;
    }
    public GlobalStates orig;
    public virtual void Init()
    {
        isInit=true;
    }
    public virtual void Begin()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void End()
    {

    }

}
