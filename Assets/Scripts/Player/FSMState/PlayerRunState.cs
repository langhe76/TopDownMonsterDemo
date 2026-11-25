using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : FsmStateBase<PlayerState>
{
    private readonly PlayerController _player;

    public PlayerRunState(PlayerController player)
    {
        StateType = PlayerState.Run;
        _player = player;
    }

    public override void Enter() { }

    public override void Update()
    {
        _player.Move();
        
        // 判断有没有输入
        if(_player.MoveInput.sqrMagnitude <= 0.01f)
        {
            ownnerMachine.ChangeState(PlayerState.Idle);
        }
    }

    public override void Exit()
    {
        _player.StopMove();
    }
}
