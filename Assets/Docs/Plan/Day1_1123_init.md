```
# Day1 11.23｜仓库初始化 & 工程规范搭建
## 今日目标
完成本周Git工程规范任务，搭建项目目录、Gitignore、初始化仓库，为FSM、对象池开发做准备。

## 今日产出
1. 新建Unity 2022 LTS 3D Core工程
2. 搭建标准化目录结构
3. 编写Unity专用.gitignore，过滤缓存文件
4. 创建GitHub公共仓库，两次规范提交
5. 编写项目README初稿

## 遇到的问题 & 踩坑记录
1. GitHub推送慢、容易中断
    - 方案：分批小commit，不一次性提交大量文件；后续配置Gitee做国内镜像备份
2. Markdown图片路径管理
    - 方案：统一截图存放路径 `Assets/Docs/ProfilerData/`，VS Code Paste Image插件自动生成相对路径，插件方案暂时放弃过于复杂。md里不放截图。

## 今日时长统计
Unity主线：180min
Java后端：0min
刷题杂项：0min

## 自查校验
- [x] 仓库可见性：Public，面试官可直接访问
- [x] 没有自动生成README，避免冲突
- [x] Library、Temp等缓存文件会被gitignore过滤
- [x] 没有写入任何FSM、对象池业务代码，守住本周边界
```



标准化目录结构

TopDownMonsterDemo/
├─ Assets/
│  ├─ Scripts/
│  │  ├─ Framework/        # 通用框架代码：FSM、对象池、事件总线等（代码凭证）
│  │  └─ Demo/              # 业务代码：玩家、怪物逻辑
│  ├─ Prefabs/
│  ├─ Scenes/
│  └─ Docs/                 # ✅ 【所有非代码凭证统一放这里】
│     ├─ Notes/             # 结构化学习笔记（md）
│     │  ├─ FSM/
│     │  │  ├─ FSM设计与踩坑.md
│     │  │  └─ FSM架构图.png
│     │  └─ ObjectPool/
│     │        ├─ 对象池设计与踩坑.md
│     │        └─ 对象池时序图.png
│     ├─ ProfilerData/      # ✅ 性能量化凭证（对象池对比截图、录制文件）
│     │  ├─ Instantiate对比池复用_对比截图.png
│     │  └─ profiler_raw.data
│     └─ Plan/              # 计划、周快照、复盘文档
│        ├─ 11.23-11.29周计划.md
│        ├─ 每日打卡记录.md
│        └─ 周复盘校验.md
├─ .gitignore
├─ README.md
└─ 【仓库外】LocalBackup/    # 可选！本地额外备份（不提交git）
   └─ Snapshot/             # 压缩包、大文件，不进版本库