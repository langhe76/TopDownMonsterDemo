using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// 状态基类
/// Tstate 枚举基础类
/// </summary>
/// <typeparam name="Tstate"></typeparam>
public abstract class FsmStateBase<Tstate> : MonoBehaviour where Tstate : Enum
{
    
    public Tstate StateType{get;protected set;}
    // 所属状态机
    protected FsmStateMachine<Tstate> ownnerMachine;
    //状态机自动初始化 
    public void Init(FsmStateMachine<Tstate> fsmStateMachine)   
    {
        ownnerMachine = fsmStateMachine;
    }
    // 进入状态，初始化
    public abstract void Enter();
    // 核心逻辑
    public abstract void Update();
    // 退出执行
    public abstract void Exit();
    
 }
