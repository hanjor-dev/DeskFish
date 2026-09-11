# DeskFish

DeskFish 是一个 Windows 桌面鱼缸技术验证工程。

## 项目文档

- [项目语境](CONTEXT.md)
- [产品与 MVP 基线](docs/PRODUCT-MVP.md)
- [完整产品需求](docs/PRODUCT-REQUIREMENTS.md)
- [实施路线](docs/ROADMAP.md)
- [架构设计](docs/ARCHITECTURE.md)
- [技术决策 ADR](docs/adr/0001-unity-windows-desktop-client.md)
- [领域规则与 Unity 表现分离](docs/adr/0002-domain-logic-independent-from-unity.md)
- [离线优先与版本化快照存档](docs/adr/0003-offline-versioned-snapshot-save.md)
- [排行榜服务端权威边界](docs/adr/0004-server-authoritative-ranking-boundary.md)

## 当前验证目标

- 2D URP 鱼缸场景
- 3 条占位鱼的基础游动
- 水草、底砂、气泡和点击波纹
- Windows 无边框、透明、置顶窗口
- `Ctrl+Shift+F12` 切换鼠标穿透/交互模式

## 使用方式

1. 使用 Unity Hub 打开 `DeskFish` 目录。
2. 在 Unity 菜单执行 `DeskFish > Create Prototype Scene`。
3. 打开 `Assets/DeskFish/Scenes/PrototypeTank.unity` 并运行。
4. Windows 独立包中验证透明窗口和快捷键；编辑器中仅验证场景与交互。

## 当前状态

第一阶段技术验证已通过：鱼缸运行时内容、基础动画和点击反馈可以在 Unity Game 视图中显示。

## 当前限制

当前仍是第一阶段技术验证，不包含喂食、成长、存档、排行榜和最终美术资产。紫红色背景是相机调试色，方块鱼是占位表现。
