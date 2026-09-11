# DeskFish 美术方向与 Unity 资源技术契约

状态：MVP 视觉纵切片契约（2026-09-11）  
资源包状态：`PLACEHOLDER`，用于验证构图、导入和表现层接线，不代表最终美术质量。

## 1. 视觉目标

DeskFish 是桌面陪伴型鱼缸。画面应当安静、清爽、有呼吸感，长时间观看不疲劳；鱼只需要有清晰轮廓、可辨识的颜色/花纹和轻微的个体差异。所有占位资源采用低细节扁平矢量风格，方便验证透明窗口、鱼缸边界和点击命中。

- 基调：温暖的玻璃青、深海蓝绿、珊瑚橙、柔和金黄。
- 光照：软顶光；不依赖实时焦散、后处理或声音。
- 对比：鱼只与水体保持足够明度/色相差；交互反馈可短时提高饱和度。
- 动画：鱼身慢摆、鱼鳍轻摆、植物低频摇曳；长时间运行不得持续闪烁。
- 占位标记：所有当前资源在元数据和资源说明中标明 `PLACEHOLDER`，替换最终资源时保持契约字段和语义名称不变。

## 2. 资源目录

```text
Assets/DeskFish/Art/
  README.md
  art-contract.json
  Environment/
    aquarium-frame.svg
    substrate.svg
    plant-anubias.svg
  Fish/
    fish-amber.svg
    fish-blue.svg
    fish-coral.svg
  Interaction/
    ripple-ring.svg
```

SVG 是当前最小包的占位源文件。项目未安装 `com.unity.vectorgraphics`，所以不把 SVG 当作 SpriteRenderer 专用导入格式；正式接入时可选择安装 Vector Graphics，或按同名契约导出 PNG/WebP。当前资源仍可作为 Unity `TextAsset`/DefaultImporter 可识别的源文件保存和审阅。

## 3. Unity 技术契约

| 资源类别 | 逻辑尺寸 | 原点/锚点 | 推荐导入 | Sorting Layer | 备注 |
|---|---:|---|---|---|---|
| aquarium-frame | 1600×900 | 中心 | Sprite, PPU 100 | `Environment` | 仅视觉边框，不承担碰撞 |
| substrate | 1600×240 | 中心底部 | Sprite, PPU 100 | `Environment` | 可平铺或整幅替换 |
| plant-anubias | 220×420 | 茎部底端 | Sprite, PPU 100 | `Environment` | 可实例化 3–6 份 |
| fish-* | 240×120 | 几何中心 | Sprite, PPU 100 | `Fish` | 朝右为源向；向左由表现层翻转 |
| ripple-ring | 320×320 | 中心 | Sprite, PPU 100 | `Effects` | 仅交互反馈，不进入存档 |

统一约定：

1. 源文件使用 kebab-case；同一语义资源替换时不得改变逻辑文件名。
2. 透明背景，颜色空间按项目默认设置；不得把背景色烘焙进鱼只或特效。
3. 鱼只绘制边界应留至少 8 px 安全边距，避免翻转/动画裁切。
4. 鱼只 prefab/表现控制器负责位置、朝向、动画和颜色变化；资源不持有领域状态。
5. 鱼缸、水草和底砂是表现资源；鱼种配置只引用 `speciesKey` 与资源地址，不把 Unity 对象写入领域快照。
6. 交互高亮、惊扰、吸引等状态使用材质/特效叠加，不复制一套鱼只贴图。
7. 低/中/高性能档可替换动画帧率、粒子和特效；不得删除鱼只资源或改变鱼只身份。

## 4. 鱼种与表现映射

| speciesKey | 初始状态 | 视觉占位 | 行为倾向（表现层） |
|---|---|---|---|
| `amber_guppy` | 默认鱼 1 | amber | 活泼、短距离转向 |
| `blue_tetra` | 默认鱼 2 | blue | 中速、偏中层巡游 |
| `coral_platy` | 默认鱼 3 | coral | 慢速、偏底层巡游 |
| `silver_molly` | 可解锁 | 后续资源 | 未提供占位实例 |
| `jade_betta` | 可解锁 | 后续资源 | 未提供占位实例 |

表现层只能依据鱼种配置选择资源和行为参数；鱼只成长阶段可在同一资源上调整缩放、色彩和装饰层，不能通过改变 `Fish` 领域实体的资源引用来实现规则逻辑。

## 5. 验收清单

- [ ] 资源文件名与 `art-contract.json` 一致。
- [ ] 所有资源明确标记 `PLACEHOLDER`，不得在商店/发布包中误称最终美术。
- [ ] 透明背景、逻辑尺寸、锚点和排序层符合本文件。
- [ ] #3 可用三种鱼种资源创建默认三鱼；#4 可用同名 key 替换资源而无需改领域代码。
- [ ] 长按水体的 ripple 只作为瞬时表现，不写入存档。
- [ ] 最终美术验收另行确认：风格稿、动画稿、导出图集、授权来源和性能预算。

## 6. 当前限制与替换路径

这套资源没有声称达到最终画质，也没有包含最终鱼鳞、复杂骨骼、焦散、粒子或逐帧动画。最终资源到位后，优先替换 `Fish/` 和 `Environment/` 下同名文件，再由 Unity 导入设置生成 Sprite/Prefab；若选择 Vector Graphics 包，需在包变更的独立任务中确认版本和构建影响。
