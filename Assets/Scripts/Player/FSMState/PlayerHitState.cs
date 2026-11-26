using UnityEngine;

public class PlayerHitState : FsmStateBase<PlayerState>
{
    private readonly PlayerController _player;

    public PlayerHitState(PlayerController player)
    {
        StateType = PlayerState.Hit;
        _player = player;
    }

    public override void Enter()
    {
        _player.StopMove();
        _player.StateTimer = _player.hitDuration;

        Debug.Log("玩家：进入受击状态");
    }

    public override void Update()
    {
        _player.StateTimer -= Time.deltaTime;
        if(_player.StateTimer <= 0)
        {
            ownnerMachine.ChangeState(PlayerState.Idle);
        }
    }

    public override void Exit()
    {

    }
}
