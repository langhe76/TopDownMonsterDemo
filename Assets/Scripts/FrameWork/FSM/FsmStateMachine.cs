using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 状态机管理器
/// </summary>
/// <typeparam name="TState"></typeparam>
public class FsmStateMachine<TState> where TState : Enum
{
    // 只读，锁定的是这个字段所持有的引用不能变。状态字典。
    private readonly Dictionary<TState, FsmStateBase<TState>> _stateDic = new();
    public FsmStateBase<TState> CurrentState {get; private set;}
    public TState CurrentStateType => CurrentState.StateType;

    public void RegisterState(FsmStateBase<TState> state)
    {
        if (_stateDic.ContainsKey(state.StateType))
        {
            Debug.LogWarning($"状态{state.StateType}已经注册过");
            return;
        }
        state.Init(this);
        _stateDic.Add(state.StateType,state);
    }

    public void ChangeState(TState newStateType)
    {
        if(CurrentState != null && EqualityComparer<TState>.Default.Equals(CurrentState.StateType, newStateType))
        {
            return;
        }

        CurrentState?.Exit();

        if(!_stateDic.TryGetValue(newStateType,out var newState))
        {
            Debug.LogError($"未找到状态{newStateType}。请注册");
            return;
        }
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }


}
