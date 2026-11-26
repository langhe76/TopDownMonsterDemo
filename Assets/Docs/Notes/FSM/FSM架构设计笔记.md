#### 文件 1：`Assets/Scripts/Framework/FSM/FsmStateBase.cs`

```
/// <summary>
/// 状态基类：所有具体状态都继承该类
/// TState：状态枚举类型（如 PlayerState、MonsterState）
/// </summary>
public abstract class FsmStateBase<TState> where TState : System.Enum
{
    public TState StateType { get; protected set; }
    protected FsmStateMachine<TState> ownerMachine;

    /// <summary>
    /// 初始化状态，绑定所属状态机
    /// </summary>
    public void Init(FsmStateMachine<TState> stateMachine)
    {
        OwnerMachine = stateMachine;
    }

    /// <summary>
    /// 进入状态：单次执行，用于初始化、变量重置
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// 帧更新：每帧执行，放核心逻辑
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// 退出状态：单次执行，用于清理资源、注销事件
    /// </summary>
    public abstract void Exit();
}
```

------

#### 文件 2：`Assets/Scripts/Framework/FSM/FsmStateMachine.cs`

```
using System.Collections.Generic;

/// <summary>
/// 通用状态机管理器
/// TState：状态枚举类型
/// </summary>
public class FsmStateMachine<TState> where TState : System.Enum
{
    // 状态字典：枚举 -> 具体状态实例
    private readonly Dictionary<TState, FsmStateBase<TState>> _stateDict = new();
    // 当前运行状态
    public FsmStateBase<TState> CurrentState { get; private set; }
    public TState CurrentStateType => CurrentState.StateType;

    /// <summary>
    /// 注册状态到状态机
    /// </summary>
    public void RegisterState(FsmStateBase<TState> state)
    {
        if (_stateDict.ContainsKey(state.StateType))
        {
            UnityEngine.Debug.LogWarning($"状态 {state.StateType} 已注册，请勿重复添加");
            return;
        }
        
        state.Init(this);
        _stateDict.Add(state.StateType, state);
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    public void ChangeState(TState newStateType)
    {
        // 防御：切换到当前状态直接跳过，避免重复Enter
        if (CurrentState != null && EqualityComparer<TState>.Default.Equals(CurrentState.StateType, newStateType))
        {
            return;
        }

        // 1. 退出旧状态
        CurrentState?.Exit();

        // 2. 获取新状态
        if (!_stateDict.TryGetValue(newStateType, out var newState))
        {
            UnityEngine.Debug.LogError($"未找到状态 {newStateType}，请先注册");
            return;
        }

        // 3. 进入新状态
        CurrentState = newState;
        CurrentState.Enter();
    }

    /// <summary>
    /// 帧更新驱动：由外部MonoBehaviour的Update调用
    /// </summary>
    public void Update()
    {
        CurrentState?.Update();
    }
}
```

------

#### 文件 3：冒烟测试脚本（同步更新类名，可直接运行验证）

```
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
```



## 二、结构化笔记模板（Assets/Docs/Notes/FSM架构设计笔记.md`）

# 通用 FSM 有限状态机架构设计笔记

## 1. 设计思想

- **职责分离**：状态逻辑与状态管理解耦，每个状态独立成类，符合单一职责原则
- **泛型复用**：基于枚举泛型实现，一套框架适配玩家、怪物、UI 等多场景状态管理
- **防御式设计**：内置重复状态切换拦截，避免状态堆叠、重复初始化问题

## 2. 核心架构

```
状态机(StateMachine)
├── 状态字典：管理所有已注册的状态实例
├── 当前状态：记录当前运行的状态
├── 核心方法：注册状态、切换状态、帧更新驱动
└── 状态基类(StateBase)
    ├── Enter()：进入状态一次性执行
    ├── Update()：每帧持续执行
    └── Exit()：退出状态一次性执行
```

## 3. 生命周期流程

状态切换完整链路： `触发切换条件 → 旧状态.Exit() → 切换当前状态引用 → 新状态.Enter() → 每帧执行新状态.Update()`

## 4. 适用场景

- 角色行为控制：待机、跑动、攻击、受击等互斥行为
- 怪物 AI 逻辑：巡逻、追击、攻击、死亡等状态流转
- UI 界面管理：弹窗、页面的显隐切换
- 游戏流程控制：主菜单、加载、战斗、结算等阶段切换

## 5. 设计亮点与踩坑预案

1. **防重入机制**：切换同状态直接返回，避免重复触发 Enter 导致变量重置、事件重复注册

2. **纯逻辑实现**：不依赖 Unity 动画系统，纯 C# 逻辑，可移植性强，面试可讲「逻辑层与表现层分离」

3. **后续扩展点**：可叠加状态切换条件表、状态层级（子状态机）、事件驱动切换等进阶功能

4. 常见坑提前规避

   ：

   - 禁止在 Exit 里再次触发状态切换，容易造成递归调用
   - 状态内不直接持有其他状态的引用，所有切换通过状态机统一调度
   - 初始化必须先注册所有状态再启动默认状态，避免运行时动态注册带来的不确定性



# 今日学习总结（Unity FSM 有限状态机）

今天我们把这套通用 FSM 的底层逻辑、容易混淆的卡点、踩坑点全部理顺了，核心围绕**驱动分离的纯 C# 状态机**：

## ✅ 核心设计结论

1. **FsmStateMachine、FsmStateBase 都不继承 MonoBehaviour** 它们是普通 C# 类，没有 Unity 自带的`Update`消息，不能靠引擎自动调用方法； 由外层挂载在 GameObject 上的 Ctrl（PlayerCtrl 这类 Mono 脚本）做驱动：Mono 的`Update`里手动调用`_fsm.Update()`。
2. 两个叫 Update 的东西一定要分清
   - `MonoBehaviour.Update()`：引擎自动回调的 Unity 消息；
   - 状态机 / 状态里的`Update()`：我们自己约定的帧回调，只是名字撞了，由状态机调度调用。
3. **泛型 `TState` 的作用** 绑定这套状态机专属的枚举（PlayerState / MonsterState），编译期类型校验，防止传错别的枚举；字典 key 也用 TState，查表方便安全。
4. **调用链路** 引擎 → PlayerCtrl (Mono).Update () → FsmStateMachine.Update () → CurrentState.Update () 切换状态时：`ChangeState()` 自动执行旧状态`Exit()`、新状态`Enter()`。

## 📌 踩坑 & 漏洞记录（重点）

1. ❌ 错误：给状态机加上 `: MonoBehaviour`，然后`new`实例化 原理：Mono 只能通过`AddComponent`或者挂载物体，不能 new，会直接报 Unity 警告。
2. ❌ 空引用坑：
   - `CurrentState` 在没有设置初始状态时是 null；访问`CurrentState.StateType`会炸；
   - 修复：判空 + `?.`空条件运算符；规范：初始化状态机后必须立刻`ChangeState`设置初始状态。
3. ❌ 判空顺序：判断是否重复状态时，要先判断`CurrentState != null`，再拿它的 StateType 对比。

## 🧩 结构分工

- `FsmStateBase<TState>`：状态模板，约定 Enter/Update/Exit，持有`OwnerMachine`（状态机引用），状态内部可以跳转其他状态；
- `FsmStateMachine<TState>`：调度中心，字典存所有注册状态，负责切换状态、驱动当前状态更新；
- `XXXCtrl : MonoBehaviour`：对接 Unity 引擎的外壳，new 出状态机、注册所有状态、驱动状态机。

## 💡 最重要的认知收获（对应你之前的思考）

教材 / 教程很多直接给成品框架，跳过 “为什么要这么设计”；这套 FSM 的**驱动分离**本质就是解耦：状态机逻辑和 Unity 引擎生命周期解绑，它可以复用在任何地方，不限于 GameObject 上的物体。 同时也理解了：很多新手被劝退，就是分不清「引擎消息」和「自定义方法」，看不懂框架里谁在调用谁，误以为自己理解能力不行。



public abstract class StateBase<TState> where TState : System.Enum {  解答：C#语法，where对泛型限制。

init函数给状态机管理器用的

```
public TState StateType { get; protected set; }
```

✅ `StateType` 就是**属性（Property）** ✅ 这里的 `{ get; protected set; }` 是 C# 的**自动属性简写语法**，不是代码块、不是函数体



C# 【属性、类名、方法名】默认 **PascalCase（大驼峰）**

## ✅ C# 官方命名规范（微软 C# 设计指南，Unity 社区也跟着这套）

1. 表格

   | 成员                     | C# 规范                | Java 规范                   |
   | ------------------------ | ---------------------- | --------------------------- |
   | 类 / 抽象类              | Pascal 大驼峰          | Pascal 大驼峰               |
   | 公开属性 public property | Pascal 大驼峰          | 无原生属性，用 get/set 方法 |
   | private/protected 字段   | _+camel `_moveSpeed`   | camel 小驼峰 `moveSpeed`    |
   | 方法                     | Pascal `ChangeState()` | camel `changeState()`       |
   | 局部变量、参数           | camel 小驼峰           | camel 小驼峰                |

- **字段（field）**：类里面存数据的**变量**，底层的存储盒子，一般带下划线 `_xxx`
- **属性（property）**：给字段套的一层**访问包装器**，用来控制读写，对外暴露用

```
public class Player
{
    // ✅ 这是【字段】，内部存数据，一般private
    private int _hp;

    // ✅ 这是【属性】，用来读写上面的字段
    public int Hp
    {
        get { return _hp; }
        set { _hp = value; }
    }
}
```



`new()` 是 C# **目标类型推断（target-typed new）**，是 C# 9.0 新增的语法糖

```
// 你写的（C#9+）
private readonly Dictionary<TState, StateBase<TState>> _stateDict = new();

// 完整写法（旧版本C#必须这样）
private readonly Dictionary<TState, StateBase<TState>> _stateDict = new Dictionary<TState, StateBase<TState>>();
```



public TState CurrentStateType => CurrentState.StateType;

\- `CurrentStateType`：表达式体只读**属性**，等价于 

```
```csharp public TState CurrentStateType 
{    
	get    
	{        
		return CurrentState.StateType;    
	} 
}
```



Tstate枚举泛型参数比较旧版C#写法，新版可以直接比较

```
if (CurrentState != null && EqualityComparer<TState>.Default.Equals(CurrentState.StateType, newStateType))
```



```
CurrentState?.Exit();
```

这是 C# 的 **空条件运算符（Null‑conditional operator）**，读作：**如果 CurrentState 不为 null，才调用 Exit ()；如果是 null，这一行直接什么都不做，不会抛空指针异常**。



`Dictionary.TryGetValue` 签名简化：

```
public bool TryGetValue(TKey key, out TValue value)
```

`out var xxx` = 让这个方法**把结果装进 xxx 给我带出来**。

✅ **var 不是动态类型！不是 object！** ✅ 编译器会**根据右边表达式，自动推断出变量的真实类型**；类型在编译期就确定，之后不能改。 ✅ **只能用于局部变量**（方法里面、代码块里），不能写在字段、属性、方法返回值上。



**StateMachine 本身不继承 MonoBehaviour，它只是一个普通 C# 类（纯数据 + 逻辑）；它被「一个继承 MonoBehaviour 的脚本（PlayerCtrl）持有，由 PlayerCtrl 来驱动它」。**

> 不是把 StateMachine 挂在 GameObject 上；是把 PlayerCtrl 挂 GameObject，PlayerCtrl 里面包含一个 StateMachine 实例。

## 1. 先区分两个东西，很多人在这里混淆

- **MonoBehaviour（简称 Mono）**： 只有挂在 GameObject 上、在 Unity 场景里的脚本，才需要继承它。 继承 Mono 的特权：能拿到 Unity 的消息 ——`Start()`、`Update()`、`OnTriggerEnter()`等，由 Unity 引擎自动调用。 代价：**必须依附 GameObject，不能随便 new，生命周期受引擎管理，不方便随便复用、序列化也有坑**。
- **普通 C# 类（class，不继承 Mono）**： 可以直接 `new` 出来，是纯逻辑容器，**没有引擎自动消息**。 好处：随便 new、可以给怪物、玩家、甚至纯后台逻辑用，不一定非要绑物体。 代价：没人自动给你跑 Update，**必须有人手动调用它的 Update 方法**。



private readonly Dictionary<TState, FsmStateBase<TState>> _stateDict = new();

> **这行代码里的 `_stateDict` 字典容器本身，正常情况下永远不会被更换。** `readonly` 锁住的是：**`_stateDict` 这个变量的引用不能再指向别的字典**，但是字典**内部可以增删元素**。

```
private readonly Dictionary<TState, FsmStateBase<TState>> _stateDict = new();
```

------

## 拆开两个概念（非常关键，Java 也一样）

1. `_stateDict` 是一个**引用变量**，存的是字典对象在堆上的地址
2. `readonly`：这个变量**只能在构造函数或者声明处赋值一次**，之后不能再写：

```
_stateDict = new Dictionary<...>(); // ❌ 后面任何地方都不能再这么写，编译报错
```



## C# 所有常用的泛型约束清单（Unity 天天见）

表格 

| 约束                  | 含义                                                         |
| :-------------------- | :----------------------------------------------------------- |
| `where T : class`     | T 必须是**引用类型**（类、接口、委托；不能是 int、struct 等值类型） |
| `where T : struct`    | T 必须是**值类型**（int、bool、自定义 struct，注意：enum 也是值类型） |
| `where T : new()`     | T 必须有**无参构造函数**，`new T()` 才能用                   |
| `where T : 父类名`    | T 必须继承 / 就是这个父类                                    |
| `where T : 接口名`    | T 必须实现这个接口                                           |
| `where T : Enum`      | T 必须是枚举（C# 7.3 + 新增，很适合 FSM）                    |
| `where T : unmanaged` | 非托管类型，写底层、JobSystem、Burst 常用                    |

### 重点：`where T : class` 不只是你自己写的 class！

满足 `where T : class` 的都算引用类型：

1. 用 `class` 定义的自定义类（MonoBehaviour、普通 class）
2. 接口 `interface`
3. 委托 `delegate`
4. 数组 `int[]`

❌ **枚举 enum、所有 struct（包括自定义 struct、int、bool）都不属于 `class` 约束**

而 `where T : struct`：T 必须是**非可空值类型**

- int、float、bool
- enum（枚举本质是特殊 struct）
- 自己 `struct` 定义的结构体



















