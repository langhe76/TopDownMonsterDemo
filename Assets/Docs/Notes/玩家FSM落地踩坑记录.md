





```


架构待会再搞，我先学习
Scripts
├── Framework
│   └── FSM
│       ├── FsmStateBase.cs
│       └── FsmStateMachine.cs
└── Player
    ├── PlayerState.cs          // 玩家状态枚举
    ├── PlayerController.cs     // 玩家主控：数据+输入+状态机入口
    └── FSMStates               // 玩家业务状态集合
        ├── PlayerIdleState.cs
        ├── PlayerRunState.cs
        ├── PlayerAttackState.cs
        └── PlayerHitState.cs
```



## 今日 Git 提交说明

```
feat(fsm): 完成玩家四状态FSM落地，实现待机/跑动/攻击/受击完整流转
```

> 提交详情：
>
> 1. 新增 PlayerController 主控类，统一管理输入、组件、公共数据
> 2. 实现 Idle、Run、Attack、Hit 四个玩家业务状态，复用通用 FSM 框架
> 3. 完成状态互斥逻辑：受击最高优先级、攻击锁移动、防重复触发
> 4. 新增玩家 FSM 落地踩坑记录，整理设计问题与优化方案



[RequireComponent(typeof(Rigidbody2D))]

它是 **Unity 的特性（Attribute，C# 特性）**，写在你的 MonoBehaviour 类上面，**不是代码函数，是给 Unity 编辑器看的标记**。

```
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{

}
```

一句话总结：

> **挂上这个脚本的物体，必须要有 Rigidbody2D 组件；如果没有，Unity 会自动帮你加上，而且你不能手动删掉这个 Rigidbody2D。**

**只在编辑器阶段生效，运行时不检查**

- 是编辑器的约束，**不是运行时动态检查**
- 适合写脚本的时候明确声明：我这个脚本**依赖 Rigidbody2D 才能正常工作**

typeof () 是什么

`typeof(类型名)` 是 C# 的运算符，**作用：拿到这个类型的「类型信息对象（Type）」**

区分两个极易搞混的东西：

1. `typeof(XXX)` → 写代码时**编译期**就确定，参数里直接写**类名**，不要写变量、不要写实例
2. `xxx.GetType()` → 运行时，从**已经创建出来的实例对象**身上获取它的类型



[中括号]

- `[Header("xxx")]`：标题文字
- `[Tooltip("描述文字")]`：鼠标悬停在变量上弹出提示

```
[Header("移动参数")]
[Tooltip("怪物正常移动速度")]
public float moveSpeed = 5f;
```

- `[Range(0,10)]`：把数字变成滑动条

```
[Header("移动参数")]
[Range(0,10)]
public float moveSpeed = 5f;
```

- `[SerializeField]`：私有变量也能在面板显示



#### Unity MonoBehaviour 生命周期（常用 + 执行顺序）

> 说明：下面是**主线标准顺序**，区分【初始化阶段】【游戏运行帧循环】【销毁 / 禁用阶段】，只列面试高频、项目里最常用的；`[ ]` 标注触发时机特点

##### 一、执行总顺序（从上到下）

1. **Awake()**

- 物体**激活时最先调用**（哪怕脚本是禁用 `enabled=false` 也会执行）
- 只执行**一次**；适合：获取自身组件、内部初始化，不要放跨物体引用（别的物体可能还没 Awake）

1. **OnEnable()**

- Awake 之后调用；**物体从禁用→激活、脚本从关闭→开启都会触发，可多次执行**
- 适合：注册事件、开启计时器、刷新显示

1. **Start()**

- 在 Awake、OnEnable 之后，**第一帧 Update 之前**调用；只执行一次
- 适合：和其他对象交互、找别的物体组件、初始化游戏逻辑

> ✅ 记忆：Awake 先给自己准备，Start 和别人打交道

------

##### 帧循环（每帧反复执行）

1. **FixedUpdate()**

- 按**固定时间步**执行（默认 0.02s，不受帧率影响）
- 物理相关代码放这里：Rigidbody 移动、力、碰撞检测

> 注意：一帧里可能跑 0 次 / 1 次 / 多次

1. **Update()**

- **每帧执行一次**，间隔不固定（受帧率波动）
- 绝大多数游戏逻辑：输入检测、角色移动、动画参数更新

1. **LateUpdate()**

- **同一帧 Update 全部跑完之后**才执行
- 适合：跟随相机、后处理、基于别的物体位置做更新（比如相机跟角色）

------

##### 渲染相关（面试常问）

1. **OnWillRenderObject()**

- 物体将要被相机渲染前调用

1. **OnRenderObject()**

- 渲染物体的时候调用，用来画 Gizmos 之外的自定义几何体

1. **OnGUI()**

- 每帧多次调用（UI 老版 IMGUI），不要放游戏逻辑，性能差

------

##### 禁用 & 销毁阶段

1. **OnDisable()**

- 脚本关闭 / 物体取消激活时触发，可多次执行
- 和 OnEnable 配对：**取消事件注册、停掉协程**，防止内存泄漏

1. **OnDestroy()**

- 物体被 `Destroy()` 销毁时调用，**只一次**
- 资源释放、清理引用

------

#### 极简流程图（背诵版）

```
Awake() → OnEnable() → Start()
→ [循环] FixedUpdate() → Update() → LateUpdate()
→ OnDisable() → OnDestroy()
```

##### ⚠️ 高频面试坑点

1. `Awake` 脚本 `enabled=false` 照样跑；`Start` 脚本禁用就不会跑
2. `FixedUpdate` 是固定时间，不是每帧一次；物理代码放这里
3. `OnEnable / OnDisable` 可以反复触发，不是只一次
4. 物体**关闭（SetActive (false)）**：触发 OnDisable，**不会触发 OnDestroy**；物体 Destroy 才会 OnDestroy
5. 协程：StartCoroutine 是在当前生命周期函数返回后，在帧间隙执行



#### 不声明访问权限，就是默认 public 吗？

❌ 重点易错点：**C# 类里面，成员默认是 private！**

```
void Test(){} // 等价于 private void Test(){}
```

访问修饰符：

- `protected`：子类 + 自身
- `internal`：同程序集



#### `Vector2` 是什么？`.normalized` 什么意思？

`Vector2`：二维向量，只有 x、y，用来表示平面上的方向、位移。 比如 `(1,0)` 向右，`(0,1)`向上，`(1,1)` 右下。

向量有一个属性叫**模长（magnitude）**：向量的长度。 `(1,1)` 的长度是 √2 ≈1.414。 `.normalized` = **归一化**：保持方向不变，把向量长度强制变成 1。

- `new Vector2(1,1).normalized` → 变成 (0.707, 0.707)，方向不变，长度 = 1

👉 你的用途：斜向移动的时候，速度不会变快，保证八个方向移动速度一致。我自己的理解：没有归一化时，速度为1时，在平面上的速度范围是一个正方形，斜向更快。归一化后，速度范围是一个圆。



**字段（field） → 一般是 camelCase（小驼峰，私有习惯加下划线 `_moveSpeed`）**

- **字段 field**：直接一块内存变量，写在类里面

```
public float moveSpeed; // 字段
```

- **属性 property**：是包装器，本质是两个方法 `get_MoveSpeed()` / `set_MoveSpeed()`，带 `{get;set;}`

```
public float MoveSpeed { get; set; } // 属性，规范大驼峰
```

> 你之前疑惑：“moveSpeed 是属性为什么不大驼峰” → 根源：它**不是属性，是 public 字段**。





#### public float StateTimer {get; set;}和public float StateTimer 

差别大吗，我感觉效果相同

先说结论：**运行起来简单赋值读取的时候，看起来效果一样，但底层完全不是一回事，写 FSM 的时候坑很多，差别挺大。**

```
// ① 自动属性
public float StateTimer { get; set; }

// ② 普通字段
public float StateTimer;
```

##### 1. 本质区别

- `public float StateTimer;`：**字段（field）** 就是一个直接的内存变量，你读写直接碰这块内存，没有包装。

- ```
  public float StateTimer {get;set;}
  ```

  ：

  自动实现属性（auto-property）

  编译器偷偷帮你生成一个隐藏的私有后台字段，再套一层 

  ```
  get
  ```

  、

  ```
  set
  ```

   方法。 等价于编译器帮你生成这段：

  ```
  private float <StateTimer>k__BackingField;
  public float StateTimer
  {
      get { return <StateTimer>k__BackingField; }
      set { <StateTimer>k__BackingField = value; }
  }
  ```

##### 2. 什么时候 “看起来一样”

你只是这样读写：

```
StateTimer = 0;
Debug.Log(StateTimer);
```

这时候手感、结果一样，所以你感觉没区别。

##### 3. 真正的差别（对你写怪物 FSM 非常重要）

##### ✅ 差别 1：属性以后方便加逻辑，字段不行

FSM 计时器经常要加限制，比如**不能是负数**。 如果一开始写属性，后面改很轻松：

```
private float _stateTimer;
public float StateTimer
{
    get => _stateTimer;
    set => _stateTimer = Mathf.Max(0, value); // 赋值自动钳制≥0
}
```

如果你一开始写的是 `public float StateTimer;` 字段，你就**没有统一入口**，所有赋值的地方都要手动加 `Mathf.Max`，漏一处就出 bug。

##### ✅ 差别 2：反射、编辑器序列化、Inspector 表现不一样（Unity 重点！）

1. **public 字段默认会被 Unity 序列化，显示在 Inspector 面板**
2. **{get;set;} 自动属性默认不会序列化！Inspector 看不到，改不了**

> 这是 Unity 最容易踩的大坑！ 如果你希望属性也能在面板显示，要加 `[SerializeField]` 或者用 `[field: SerializeField]`

```
[field: SerializeField]
public float StateTimer { get; set; }
```

##### ✅ 差别 3：接口、绑定、框架偏好

很多框架（UniRx、状态机框架、面板绑定）只认**Property**，不认字段。 字段就是原始数据，属性是 “访问接口”。



#### `GatherInput()` // 统一采集输入，保证输入源唯一，什么意思？

一句话：**所有按键、鼠标输入，全部只在这一个地方读一次，别的地方不许再写 `Input.GetXXX`**。

好处：

1. 输入只采集一次，这一帧里所有状态读到的 `MoveInput` 是同一个值，不会出现 “A 状态读的是上一帧，B 状态读的是当前帧” 数据不一致；
2. 以后想换输入系统（新 Input System），**只改这一个函数**，所有状态代码完全不用动；
3. 逻辑分层：输入采集 和 状态判断、移动逻辑拆开，状态只管 “拿到输入之后该干什么”，不用管怎么读按键。

> 举反例：如果你在 IdleState 里面写一次 Input，RunState 里面又写一次 Input，代码散、以后改起来很麻烦。

















































































































































