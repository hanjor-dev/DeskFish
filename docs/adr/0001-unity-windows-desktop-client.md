---
status: accepted
---

# 使用 Unity 构建 Windows 桌面客户端

DeskFish 的桌面端采用 Unity 构建，目标平台优先锁定 Windows。选择 Unity 是因为项目的核心风险集中在持续动画、2D/2.5D 场景、鼠标交互和后续视觉表现；透明、无边框、置顶和鼠标穿透则作为 Windows 平台适配层处理。这样可以先快速验证鱼缸体验，同时把平台相关能力隔离在窗口控制模块中。
