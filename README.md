# 逃离鸭科夫 鱼贩 Mod / Fish Seller Mod

钓鱼太烦？想直接买？这个 Mod 会在基地克隆一个售货机并添加“鱼贩”商人，你可以直接购买各种鱼类。

- 可配置库存上限与售价系数
- 与游戏内商店 UI、购买与存档系统无缝衔接

Steam 创意工坊地址：[https://steamcommunity.com/sharedfiles/filedetails/?id=3594623408](https://steamcommunity.com/sharedfiles/filedetails/?id=3594623408)

---

## 更新公告 / Changelog

- v1.1.0 (2025-10-28)
 - 新增：游戏内可调配置（需安装 ModConfig），支持“每种物品的最大库存”、“买入价格系数”、“使用默认物品售卖上限”。
 - 改进：为中文玩家提供本地化设置项标题；将开关命名为“使用默认物品售卖上限 / Use default item sale limit”。
 - 修复：重复注册导致设置项显示两次的问题。
 - 调整：将详细说明从设置项标题移至 README，使界面更简洁。

提示 / Notes
- 想自定义固定库存数量，请将“使用默认物品售卖上限 / Use default item sale limit”设置为 false。
- 当其为 true 时，库存可能会被其它修改库存数量的 Mod 修改。
- 未安装 ModConfig 时，本 Mod 使用默认配置运行。

---

## 中文说明

1) 功能
- 在场景中生成一个名为 `FishSaleMachine` 的售货机并挂载新的 `StockShop`
- 新增商人 ID：`fish_seller`
- 上架 `FishTypeIds` 中配置的全部鱼类，可直接购买

2) 安装
	1. 从源码构建
		- 使用任意 IDE 打开本项目（.NET Standard2.1）
 - 编译生成 DLL（项目名：`FishSeller`）
		- 将生成的 DLL 包装成完整MOD并放入游戏 Mod目录（参考官方 Modding 文档的安装说明，图标和信息参考项目文档下的`FishSeller\`目录）
	2.直接使用
		- 下载发布页或者项目目录中的`FishSeller.zip`并解压
		-复制到游戏根目录下的 `Mods` 文件夹中（若无则新建）

3) 使用
-进入基地场景（默认示例在 `Base_SceneV2` 中启用）
- 地图中会出现 `FishSaleMachine`（基于 `SaleMachine` 克隆）
- 打开售货机即可向 `fish_seller`购买鱼类

4) 可配置项（在游戏内通过 ModConfig 调整）
- 每种物品的最大库存（键：`DefaultMaxStock`，范围：1~999）
- 买入价格系数（键：`BuyPriceFactor`，范围：0.01~10）
- 使用默认物品售卖上限（键：`UsePrefabMaxStack`）
 - 为 `true` 时，库存上限优先使用物品的 `MaxStackCount`

5)兼容性与注意事项
-目标框架：.NET Standard2.1
- 若你的场景名不同，请修改代码中 `Base_SceneV2` 的判断
- 实现尽量不破坏原有商店与存档逻辑

6)未来更新方向
- 优化售货机默认摆放位置
- 提供更多可配置项目

7) 鸣谢 /参考
- 官方 MOD 示例: https://github.com/xvrsl/duckov_modding
- SuperPerkShop（Lexcellent）: https://github.com/Lexcellent/SuperPerkShop
- ModConfig（FrozenFish259）: https://github.com/FrozenFish259/duckov_mod_config
- 鱼类 ID参考: https://wiki.biligame.com/duckov/%E5%88%86%E7%B1%BB:%E9%B1%BC

---

## English

1) Overview
This mod clones a vending machine in the base and adds a dedicated merchant "fish_seller" so you can buy fish items directly.

- Configurable stock cap and price factor
- Integrates with the in-game shop UI, purchase flow, and save system

2) Installation
- Build from source
 - Open the project in your IDE (targeting .NET Standard2.1)
 - Build the project (Project name: `FishSeller`) to produce the DLL
 - Package the DLL as a complete mod and place it into the game’s `Mods` folder (follow the official modding guide)
- Direct use
 - Download `FishSeller.zip` from Releases and extract it
 - Copy the extracted folder into the game’s `Mods` directory (create it if it doesn’t exist)

3) How to use
- Enter the base scene (sample logic targets `Base_SceneV2` by default)
- A cloned vending machine named `FishSaleMachine` will appear (based on `SaleMachine`)
- Interact with it to purchase fish from merchant `fish_seller`

4) Configurable options (in-game via ModConfig)
- Max stock per item (key: `DefaultMaxStock`, range:1–999)
- Buy price factor (key: `BuyPriceFactor`, range:0.01–10)
- Use default item sale limit (key: `UsePrefabMaxStack`)
 - When `true`, stock cap prefers item `MaxStackCount` if available

5) Compatibility & Notes
- Target Framework: .NET Standard2.1
- If your scene name differs, update the `Base_SceneV2` check accordingly
- Implementation follows official samples to avoid breaking shop/save logic

6) Roadmap
- Better vending machine placement
- More configurable options

7) Credits / References
- Official mod sample: https://github.com/xvrsl/duckov_modding
- SuperPerkShop (Lexcellent): https://github.com/Lexcellent/SuperPerkShop
- ModConfig (FrozenFish259): https://github.com/FrozenFish259/duckov_mod_config
- Fish IDs reference: https://wiki.biligame.com/duckov/%E5%88%86%E7%B1%BB:%E9%B1%BC

---

声明 / Disclaimer
- 本项目参考了开源示例并使用了 AI 辅助。
- This project was built with references to open-source examples and with AI assistance.

