using UnityEngine;

public class PlayerAttackState : FsmStateBase<PlayerState>
{
    private readonly PlayerController _player;

    public PlayerAttackState(PlayerController player)
    {
        StateType = PlayerState.Attack;
        _player = player;
    }

    public override void Enter()
    {
        _player.StopMove();
        _player.StateTimer = _player.attackDuration;
        _player.AttackCoolDownTimer = _player.attackCoolDown;

        Debug.Log("玩家：触发攻击");
    }

    public override void Update()
    {
        _player.StateTimer -= Time.deltaTime;
        if(_player.StateTimer <= 0)
        {
            ownnerMachine.ChangeState(_player.MoveInput.sqrMagnitude > 0.01f ? PlayerState.Run : PlayerState.Idle);
        }
    }

    public override void Exit(){ }
}
