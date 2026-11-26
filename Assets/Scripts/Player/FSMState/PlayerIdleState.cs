using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// 待机状态
/// </summary>
public class PlayerIdleState : FsmStateBase<PlayerState>
{
    
    private readonly PlayerController _player;

    public PlayerIdleState(PlayerController player)
    {
        StateType = PlayerState.Idle;
        _player = player;
    }

    public override void Enter()
    {
        _player.StopMove();
        // 测试代码，控制台日志输出
        Debug.Log("待机");
    }

    public override void Update()
    {
        if(_player.MoveInput.sqrMagnitude > 0.01f)
        {
            ownnerMachine.ChangeState(PlayerState.Run);
        }
    }

    public override void Exit(){}
}
