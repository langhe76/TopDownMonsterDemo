using UnityEngine;

// 玩家状态枚举
public enum PlayerState
{
    Idle,
    Run
}

public class PlayerFsmTest : MonoBehaviour
{
    private FsmStateMachine<PlayerState> _playerFsm;

    void Start()
    {
        // 1. 初始化状态机
        _playerFsm = new FsmStateMachine<PlayerState>();
        
        // 2. 注册测试状态
        _playerFsm.RegisterState(new PlayerIdleState());
        _playerFsm.RegisterState(new PlayerRunState());
        
        // 3. 启动默认状态
        _playerFsm.ChangeState(PlayerState.Idle);
    }

    void Update()
    {
        // 驱动状态机更新
        _playerFsm.Update();
        
        // 按空格测试状态切换
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _playerFsm.ChangeState(
                _playerFsm.CurrentStateType == PlayerState.Idle 
                    ? PlayerState.Run 
                    : PlayerState.Idle
            );
        }
    }
}

// 测试用空状态
public class PlayerIdleState : FsmStateBase<PlayerState>
{
    public PlayerIdleState()
    {
        StateType = PlayerState.Idle;
    }
    
    public override void Enter() => Debug.Log("进入待机状态");
    public override void Update() => Debug.Log("待机中...");
    public override void Exit() => Debug.Log("退出待机状态");
}

public class PlayerRunState : FsmStateBase<PlayerState>
{
    public PlayerRunState()
    {
        StateType = PlayerState.Run;
    }
    
    public override void Enter() => Debug.Log("进入跑动状态");
    public override void Update() => Debug.Log("跑动中...");
    public override void Exit() => Debug.Log("退出跑动状态");
}
