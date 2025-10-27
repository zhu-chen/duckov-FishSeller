# 逃离鸭科夫 鱼贩 Mod / Fish Seller Mod

钓鱼太烦？想直接买？这个 Mod 会在基地克隆一个售货机并添加“鱼贩”商人，你可以直接购买各种鱼类。

- 可配置库存上限与售价系数
- 与游戏内商店 UI、购买与存档系统无缝衔接

本mod已经上传至steam创意工坊，[直接访问](https://steamcommunity.com/sharedfiles/filedetails/?id=3594623408)即可订阅

---

##中文说明

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
- 地图中会出现 `FishSaleMachine`（基于游戏自带 `SaleMachine` 克隆，默认在原位置的 X轴偏移）
- 打开售货机即可向 `fish_seller`购买鱼类

4) 可配置项（在代码里修改）
- 文件：`ref.cs`（命名空间 `FishSeller`，类 `ModBehaviour`）
 - `FishTypeIds`：鱼类的全部 TypeID（已预填一组 ID，可按需增删）
 - `DefaultMaxStock`：默认库存上限（优先使用物品的 MaxStackCount，当 `UsePrefabMaxStack=true`）
 - `BuyPriceFactor`：购买价系数（最终价格 = 基础价值 *该系数）
 - `UsePrefabMaxStack`：若为 `true`，库存上限优先取物品的 `MaxStackCount`
 - `PlaceOffset`：克隆出来的 `FishSaleMachine` 相对原机器的位置偏移

5)兼容性与注意事项
-目标框架：.NET Standard2.1
- 若你修改了场景名，请调整代码中对 `Base_SceneV2` 的判断
- 本 Mod参考官方示例与第三方 SuperPerkShop 的实现思路，尽量不破坏原有商店与存档逻辑

6)未来更新方向
- 调整更合适的售货机位置
- 在游戏内提供配置界面（无需改代码）

7) 鸣谢 /参考
- 官方 MOD 示例: https://github.com/xvrsl/duckov_modding
- SuperPerkShop（Lexcellent）: https://github.com/Lexcellent/SuperPerkShop
- 鱼类 ID参考: https://wiki.biligame.com/duckov/%E5%88%86%E7%B1%BB:%E9%B1%BC


---

## English

1) Overview
This mod clones a vending machine in the base and adds a dedicated merchant "fish_seller" so you can buy fish items directly.

- Configurable stock cap and price factor
- Integrates with the in-game shop UI, purchase flow, and save system

2) Installation
1. Build from source
 - Open the project in your IDE (targeting .NET Standard2.1)
 - Build the project (Project name: `FishSeller`) to produce the DLL
 - Package the DLL as a complete mod and place it into the game’s `Mods` folder (follow the official modding guide; for icons/metadata, refer to the `FishSeller/` folder in this repo)
2. Direct use
 - Download `FishSeller.zip` from Releases or from the project directory and extract it
 - Copy the extracted folder into the game’s `Mods` directory (create it if it doesn’t exist)

3) How to use
- Enter the base scene (sample logic targets `Base_SceneV2` by default)
- A cloned vending machine named `FishSaleMachine` will appear (based on `SaleMachine`, offset along the X axis)
- Interact with it to purchase fish from merchant `fish_seller`

4) Configuration (edit in source code)
- File: `ref.cs` (namespace `FishSeller`, class `ModBehaviour`)
 - `FishTypeIds`: All fish TypeIDs (pre-filled; adjust as needed)
 - `DefaultMaxStock`: Default stock (prefers item `MaxStackCount` when `UsePrefabMaxStack=true`)
 - `BuyPriceFactor`: Price factor (final price = base value * factor)
 - `UsePrefabMaxStack`: Use item `MaxStackCount` as stock cap when available
 - `PlaceOffset`: Position offset for the cloned `FishSaleMachine`

5) Compatibility & Notes
- Target Framework: .NET Standard2.1
- If your scene name differs, update the `Base_SceneV2` check accordingly
- Implementation follows the official sample and SuperPerkShop approach to avoid breaking existing shop/save logic

6) Roadmap
- Improve the vending machine placement
- Provide in-game configuration UI (no code edits required)

7) Credits / References
- Official mod sample: https://github.com/xvrsl/duckov_modding
- SuperPerkShop (Lexcellent): https://github.com/Lexcellent/SuperPerkShop
- Fish IDs reference: https://wiki.biligame.com/duckov/%E5%88%86%E7%B1%BB:%E9%B1%BC

---

声明 / Disclaimer
- 本仓库作者对 C# 不太熟悉，编写时参考了开源项目并使用了 AI 辅助。
- This project was built with references to open-source examples and with AI assistance.

