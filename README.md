# ����Ѽ�Ʒ� �㷷 Mod / Fish Seller Mod

����̫������ֱ������� Mod ���ڻ��ؿ�¡һ���ۻ�������ӡ��㷷�����ˣ������ֱ�ӹ���������ࡣ

- �����ÿ���������ۼ�ϵ��
- ����Ϸ���̵� UI��������浵ϵͳ�޷��ν�

---

����˵��

1) ����
- �ڳ���������һ����Ϊ `FishSaleMachine` ���ۻ����������µ� `StockShop`
- �������� ID��`fish_seller`
- �ϼ� `FishTypeIds` �����õ�ȫ�����࣬��ֱ�ӹ���

2) ��װ
- ʹ������ IDE �򿪱���Ŀ��.NET Standard2.1��
- �������� DLL����Ŀ����`FishSeller`��
- �����ɵ� DLL ������Ϸ ModĿ¼����ο��ٷ� Modding �ĵ��İ�װ˵����
- ������Ϸ

3) ʹ��
-������س�����Ĭ��ʾ���� `Base_SceneV2` �����ã�
- ��ͼ�л���� `FishSaleMachine`��������Ϸ�Դ� `SaleMachine` ��¡��Ĭ����ԭλ�õ� X��ƫ�ƣ�
- ���ۻ��������� `fish_seller`��������

4) ��������ڴ������޸ģ�
- �ļ���`ref.cs`�������ռ� `FishSeller`���� `ModBehaviour`��
 - `FishTypeIds`�������ȫ�� TypeID����Ԥ��һ�� ID���ɰ�����ɾ��
 - `DefaultMaxStock`��Ĭ�Ͽ�����ޣ�����ʹ����Ʒ�� MaxStackCount���� `UsePrefabMaxStack=true`��
 - `BuyPriceFactor`�������ϵ�������ռ۸� = ������ֵ *��ϵ����
 - `UsePrefabMaxStack`����Ϊ `true`�������������ȡ��Ʒ�� `MaxStackCount`
 - `PlaceOffset`����¡������ `FishSaleMachine` ���ԭ������λ��ƫ��

5)��������ע������
-Ŀ���ܣ�.NET Standard2.1
- �����޸��˳�����������������ж� `Base_SceneV2` ���ж�
- �� Mod�ο��ٷ�ʾ��������� SuperPerkShop ��ʵ��˼·���������ƻ�ԭ���̵���浵�߼�

6) ��л /�ο�
- �ٷ� MOD ʾ��: https://github.com/xvrsl/duckov_modding
- SuperPerkShop��Lexcellent��: https://github.com/Lexcellent/SuperPerkShop
- ���� ID�ο�: https://wiki.biligame.com/duckov/%E5%88%86%E7%B1%BB:%E9%B1%BC

---

English

1) Overview
This mod adds a ��Fish Seller�� merchant by cloning an in-base vending machine so you can buy all fish items directly.

- Configurable stock and price factor
- Integrates with the in-game shop UI, purchase flow, and save system

2) Installation
- Open the project in your IDE (targeting .NET Standard2.1)
- Build the project (Project name: `FishSeller`) to produce the DLL
- Place the built DLL into the game��s Mods folder (or follow the official modding guide)
- Launch the game

3) How to use
- Enter the base scene (sample logic targets `Base_SceneV2` by default)
- A cloned vending machine named `FishSaleMachine` will appear (offset from the original `SaleMachine`)
- Interact with it to purchase fish from merchant `fish_seller`

4) Configuration (edit in source code)
- File: `ref.cs` (namespace `FishSeller`, class `ModBehaviour`)
 - `FishTypeIds`: All fish TypeIDs (pre-filled; adjust freely)
 - `DefaultMaxStock`: Default stock (prefers item `MaxStackCount` when `UsePrefabMaxStack=true`)
 - `BuyPriceFactor`: Price factor (final price = base value * factor)
 - `UsePrefabMaxStack`: Use item `MaxStackCount` as stock cap if available
 - `PlaceOffset`: Position offset for the cloned `FishSaleMachine`

5) Compatibility & Notes
- Target Framework: .NET Standard2.1
- If your scene name differs, update the `Base_SceneV2` check accordingly
- The implementation follows the official sample and SuperPerkShop approach and aims not to break existing shop/save logic

6) Credits / References
- Official mod sample: https://github.com/xvrsl/duckov_modding
- SuperPerkShop (Lexcellent): https://github.com/Lexcellent/SuperPerkShop
- Fish IDs reference: https://wiki.biligame.com/duckov/%E5%88%86%E7%B1%BB:%E9%B1%BC

---

���� / Disclaimer
- ���ֿ����߶� C# ��̫��Ϥ����дʱ�ο��˿�Դ��Ŀ��ʹ���� AI ������
- This project was built with references to open-source examples and with AI assistance.

