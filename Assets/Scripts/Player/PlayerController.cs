using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移动速度")]
    public float moveSpeed = 5f;

    [Header("攻击参数")]
    public float attackDuration = 0.3f;
    public float attackCoolDown = 0.5f;

    [Header("受击参数")]
    public float hitDuration = 0.2f;

    public Rigidbody2D Rb {get; private set;}

    public FsmStateMachine<PlayerState> Fsm {get; private set;}

    public Vector2 MoveInput {get; private set;}
    public float AttackCoolDownTimer {get; set;}
    public float StateTimer {get; set;}

    void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Rb.gravityScale = 0;
        Rb.freezeRotation = true;

        InitFsm();
    }

    void InitFsm()
    {
        Fsm = new FsmStateMachine<PlayerState>();

        Fsm.RegisterState(new PlayerIdleState(this));
        Fsm.RegisterState(new PlayerRunState(this));
        Fsm.RegisterState(new PlayerAttackState(this));
        Fsm.RegisterState(new PlayerHitState(this));
    }

    void Start()
    {
        Fsm.ChangeState(PlayerState.Idle);
    }

    void Update()
    {
        GatherInput();

        if(AttackCoolDownTimer > 0)
            AttackCoolDownTimer -= Time.deltaTime;
        Fsm.Update();
        
    }

    void GatherInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        MoveInput = new Vector2(horizontal,vertical).normalized;
    
        if(Input.GetMouseButton(0) && AttackCoolDownTimer <= 0 && 
        Fsm.CurrentStateType != PlayerState.Hit)
        {
            Fsm.ChangeState(PlayerState.Attack);
        }

        // 测试用，模拟受击
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage();
        }

    }

    public void TakeDamage()
    {
        Fsm.ChangeState(PlayerState.Hit);
    }

    public void Move()
    {
        Rb.velocity = MoveInput*moveSpeed;
    }

    public void StopMove()
    {
        Rb.velocity = Vector2.zero;
    }
}
