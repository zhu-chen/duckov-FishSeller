#����Ѽ�Ʒ� �㷷 Mod / Fish Seller Mod

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
	1. ��Դ�빹��
		- ʹ������ IDE �򿪱���Ŀ��.NET Standard2.1��
		- �������� DLL����Ŀ����`FishSeller`��
		- �����ɵ� DLL ��װ������MOD��������Ϸ ModĿ¼���ο��ٷ� Modding �ĵ��İ�װ˵����ͼ�����Ϣ�ο���Ŀ�ĵ��µ�`FishSeller\`Ŀ¼��
	2.ֱ��ʹ��
		- ���ط���ҳ������ĿĿ¼�е�`FishSeller.zip`����ѹ
		-���Ƶ���Ϸ��Ŀ¼�µ� `Mods` �ļ����У��������½���

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

6)δ�����·���
- ���������ʵ��ۻ���λ��
- ����Ϸ���ṩ���ý��棨����Ĵ��룩

7) ��л /�ο�
- �ٷ� MOD ʾ��: https://github.com/xvrsl/duckov_modding
- SuperPerkShop��Lexcellent��: https://github.com/Lexcellent/SuperPerkShop
- ���� ID�ο�: https://wiki.biligame.com/duckov/%E5%88%86%E7%B1%BB:%E9%B1%BC


---

English

1) Overview
This mod clones a vending machine in the base and adds a dedicated merchant "fish_seller" so you can buy fish items directly.

- Configurable stock cap and price factor
- Integrates with the in-game shop UI, purchase flow, and save system

2) Installation
1. Build from source
 - Open the project in your IDE (targeting .NET Standard2.1)
 - Build the project (Project name: `FishSeller`) to produce the DLL
 - Package the DLL as a complete mod and place it into the game��s `Mods` folder (follow the official modding guide; for icons/metadata, refer to the `FishSeller/` folder in this repo)
2. Direct use
 - Download `FishSeller.zip` from Releases or from the project directory and extract it
 - Copy the extracted folder into the game��s `Mods` directory (create it if it doesn��t exist)

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

���� / Disclaimer
- ���ֿ����߶� C# ��̫��Ϥ����дʱ�ο��˿�Դ��Ŀ��ʹ���� AI ������
- This project was built with references to open-source examples and with AI assistance.

