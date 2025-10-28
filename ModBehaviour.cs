using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Duckov.Economy;
using Duckov.UI;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using Duckov.Modding; // For ModManager/ModInfo

namespace FishSeller
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        // 固定商人ID
        private const string MerchantID = "fish_seller";
        private const string MOD_NAME = "FishSeller"; // 用于 ModConfig 键前缀

        //需填写：所有鱼类的 TypeID（请在这里填充）
        public int[] FishTypeIds = new int[]
        {
            1124,
            1122,
            1113,
            1112,
            1123,
            1120,
            1114,
            1116,
            1121,
            1119,
            1118,
            1117,
            1107,
            1125,
            1103,
            1111,
            1102,
            1101,
            1100,
            1098,
            1109,
            1126,
            1097,
            1110,
            1108,
            1099,
            1115,
            1104,
            1106,
            1105
        };

        // 上架配置（将通过 ModConfig 可调整）
        [Range(1, 999)] public int DefaultMaxStock = 10;
        [Range(0.01f, 10f)] public float BuyPriceFactor = 1.0f; // 买价系数（基础价值 *该系数）
        public bool UsePrefabMaxStack = true; // 若能获取到 prefab 的 MaxStackCount，则优先使用
        public Vector3 PlaceOffset = new Vector3(0f, 0f, -4f); // 克隆售货机的摆放偏移

        //运行时持有当前商店实例，便于在配置变更时刷新
        private StockShop _currentShop;

        // 防止重复注册配置项/事件
        private bool _configOptionsRegistered = false;
        private bool _optionsChangedSubscribed = false;

        protected override void OnAfterSetup()
        {
            //订阅场景加载
            SceneManager.sceneLoaded -= OnAfterSceneInit;
            SceneManager.sceneLoaded += OnAfterSceneInit;

            // 初始化 ModConfig交互（不建立强依赖，无 ModConfig也不会抛异常）
            TrySetupModConfig();
            // 尝试读取配置（若无 ModConfig，则保持默认值）
            LoadConfigFromModConfig();
        }

        protected override void OnBeforeDeactivate()
        {
            SceneManager.sceneLoaded -= OnAfterSceneInit;

            //取消订阅 ModConfig事件
            ModManager.OnModActivated -= OnModActivated;
            if (_optionsChangedSubscribed)
            {
                ModConfigAPI.SafeRemoveOnOptionsChangedDelegate(OnModConfigOptionsChanged);
                _optionsChangedSubscribed = false;
            }
        }

        private void OnEnable()
        {
            //保障在启用时也能拿到 ModConfig 激活事件
            ModManager.OnModActivated -= OnModActivated;
            ModManager.OnModActivated += OnModActivated;

            // 如果 ModConfig 已经可用，则立即进行一次设置
            TrySetupModConfig();
            LoadConfigFromModConfig();
        }

        private void OnDisable()
        {
            ModManager.OnModActivated -= OnModActivated;
            if (_optionsChangedSubscribed)
            {
                ModConfigAPI.SafeRemoveOnOptionsChangedDelegate(OnModConfigOptionsChanged);
                _optionsChangedSubscribed = false;
            }
        }

        private void OnModActivated(ModInfo info, Duckov.Modding.ModBehaviour behaviour)
        {
            if (info.name == ModConfigAPI.ModConfigName)
            {
                Debug.Log("FishSeller: ModConfig activated, setting up options...");
                TrySetupModConfig();
                LoadConfigFromModConfig();
                // 配置更新后刷新商店
                RefreshShopWithCurrentConfig();
            }
        }

        private void TrySetupModConfig()
        {
            if (!ModConfigAPI.IsAvailable())
                return;

            //仅注册一次变更监听
            if (!_optionsChangedSubscribed)
            {
                ModConfigAPI.SafeAddOnOptionsChangedDelegate(OnModConfigOptionsChanged);
                _optionsChangedSubscribed = true;
            }

            // 配置项仅添加一次，避免重复显示
            if (_configOptionsRegistered)
                return;

            // 简单本地化（中文/非中文）
            var chineseLangs = new[]
            {
                SystemLanguage.Chinese,
                SystemLanguage.ChineseSimplified,
                SystemLanguage.ChineseTraditional
            };
            bool isChinese = chineseLangs.Contains(Application.systemLanguage);

            // 简洁标题（说明移动到 README）
            string labelStock = isChinese ? "每种物品的最大库存" : "Max stock per item";
            string labelPriceFactor = isChinese ? "买入价格系数" : "Buy price factor";
            // 更新名称：使用默认物品售卖上限 / Use default item sale limit
            string labelUsePrefab = isChinese ? "使用默认物品售卖上限" : "Use default item sale limit";

            // 在 ModConfig 中注册可调整项
            // 库存上限（整数滑条1~999）
            ModConfigAPI.SafeAddInputWithSlider(
                MOD_NAME,
                "DefaultMaxStock",
                labelStock,
                typeof(int),
                DefaultMaxStock,
                new Vector2(1f, 999f)
            );

            //价格系数（浮点0.01~10）
            ModConfigAPI.SafeAddInputWithSlider(
                MOD_NAME,
                "BuyPriceFactor",
                labelPriceFactor,
                typeof(float),
                BuyPriceFactor,
                new Vector2(0.01f, 10f)
            );

            // 使用默认物品售卖上限（布尔）
            ModConfigAPI.SafeAddBoolDropdownList(
                MOD_NAME,
                "UsePrefabMaxStack",
                labelUsePrefab,
                UsePrefabMaxStack
            );

            _configOptionsRegistered = true;
        }

        private void OnModConfigOptionsChanged(string key)
        {
            //仅处理本 Mod 的键
            if (string.IsNullOrEmpty(key) || !key.StartsWith(MOD_NAME + "_"))
                return;

            LoadConfigFromModConfig();
            RefreshShopWithCurrentConfig();
            Debug.Log($"FishSeller: Config updated - {key}");
        }

        private void LoadConfigFromModConfig()
        {
            // 即使没有 ModConfig，这些调用也会安全地返回默认值
            int loadedStock = ModConfigAPI.SafeLoad<int>(MOD_NAME, "DefaultMaxStock", DefaultMaxStock);
            float loadedFactor = ModConfigAPI.SafeLoad<float>(MOD_NAME, "BuyPriceFactor", BuyPriceFactor);
            bool loadedUsePrefab = ModConfigAPI.SafeLoad<bool>(MOD_NAME, "UsePrefabMaxStack", UsePrefabMaxStack);

            //约束到有效范围
            DefaultMaxStock = Mathf.Clamp(loadedStock, 1, 999);
            BuyPriceFactor = Mathf.Clamp(loadedFactor, 0.01f, 10f);
            UsePrefabMaxStack = loadedUsePrefab;
        }

        private void RefreshShopWithCurrentConfig()
        {
            if (_currentShop == null)
                return;

            try
            {
                // 更新数据库配置，并让商店重新初始化 entries
                EnsureMerchantProfileWithFishItems(MerchantID, FishTypeIds);

                var initializeEntriesMethod = typeof(StockShop).GetMethod("InitializeEntries", BindingFlags.NonPublic | BindingFlags.Instance);
                if (initializeEntriesMethod != null)
                {
                    initializeEntriesMethod.Invoke(_currentShop, null);
                }

                // 刷新库存
                var refreshMethod = typeof(StockShop).GetMethod("DoRefreshStock", BindingFlags.NonPublic | BindingFlags.Instance);
                if (refreshMethod != null)
                {
                    refreshMethod.Invoke(_currentShop, null);
                }

                var lastTimeField = typeof(StockShop).GetField("lastTimeRefreshedStock", BindingFlags.NonPublic | BindingFlags.Instance);
                if (lastTimeField != null)
                {
                    lastTimeField.SetValue(_currentShop, DateTime.UtcNow.ToBinary());
                }

                //预缓存条目
                TryInvokeCacheItemInstances(_currentShop);

                Debug.Log("FishSeller: Shop refreshed with new config");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"FishSeller: Failed to refresh shop after config change: {ex.Message}");
            }
        }

        private void OnAfterSceneInit(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"加载场景：{scene.name}，模式：{mode}");
            if (scene.name == "Base_SceneV2")
            {
                StartCoroutine(DelayedSetup());
            }
        }

        private IEnumerator DelayedSetup()
        {
            // 等待场景对象初始化
            yield return new WaitForSeconds(1f);

            var baseSaleMachine = GameObject.Find("Buildings/SaleMachine");
            if (baseSaleMachine == null)
            {
                Debug.LogWarning("未找到 Buildings/SaleMachine");
                yield break;
            }

            // 克隆售货机，作为“卖鱼商人”
            var fishSaleMachine = UnityEngine.Object.Instantiate(baseSaleMachine);
            fishSaleMachine.transform.SetParent(baseSaleMachine.transform.parent, true);
            fishSaleMachine.name = "FishSaleMachine";
            fishSaleMachine.transform.position = baseSaleMachine.transform.position + PlaceOffset;

            // 商店组件通常在此子节点
            var shopRoot = fishSaleMachine.transform.Find("PerkWeaponShop");
            var stockShop = InitShopItems(shopRoot);

            fishSaleMachine.SetActive(true);
            Debug.Log("FishSaleMachine 已生成并激活");

            if (stockShop != null)
            {
                _currentShop = stockShop;

                // 刷新库存（DoRefreshStock）并更新 lastTimeRefreshedStock
                var refreshMethod = typeof(StockShop).GetMethod("DoRefreshStock", BindingFlags.NonPublic | BindingFlags.Instance);
                if (refreshMethod != null)
                {
                    try { refreshMethod.Invoke(stockShop, null); }
                    catch (Exception ex) { Debug.LogError($"调用 DoRefreshStock 异常: {ex.Message}"); }
                }
                else
                {
                    Debug.LogWarning("未找到 DoRefreshStock 方法");
                }

                var lastTimeField = typeof(StockShop).GetField("lastTimeRefreshedStock", BindingFlags.NonPublic | BindingFlags.Instance);
                if (lastTimeField != null)
                {
                    try { lastTimeField.SetValue(stockShop, DateTime.UtcNow.ToBinary()); }
                    catch (Exception ex) { Debug.LogError($"设置 lastTimeRefreshedStock 异常: {ex.Message}"); }
                }

                //预缓存所有条目实例，避免购买时 GetItemInstanceDirect 为 null
                TryInvokeCacheItemInstances(stockShop);
            }
        }

        // 初始化商店并绑定 fish_seller 配置
        private StockShop InitShopItems(Transform shopRoot)
        {
            if (shopRoot == null)
            {
                Debug.LogWarning("未找到 FishSaleMachine 的商店根节点");
                return null;
            }

            var stockShop = shopRoot.GetComponent<StockShop>();
            if (stockShop == null)
            {
                Debug.LogWarning("商店根节点上未找到 StockShop组件");
                return null;
            }

            // 清空现有条目
            stockShop.entries.Clear();

            // 将商人 ID 改为 fish_seller
            var merchantIDField = typeof(StockShop).GetField("merchantID", BindingFlags.NonPublic | BindingFlags.Instance);
            if (merchantIDField != null)
            {
                try { merchantIDField.SetValue(stockShop, MerchantID); }
                catch (Exception ex) { Debug.LogError($"设置 merchantID 异常: {ex.Message}"); }
            }
            else
            {
                Debug.LogWarning("未找到 merchantID 字段");
            }

            // 建立/补充 fish_seller 的数据库配置，仅包含鱼类 ID（使用当前配置值）
            EnsureMerchantProfileWithFishItems(MerchantID, FishTypeIds);

            // 调用 InitializeEntries让 StockShop读取配置到 entries
            var initializeEntriesMethod = typeof(StockShop).GetMethod("InitializeEntries", BindingFlags.NonPublic | BindingFlags.Instance);
            if (initializeEntriesMethod != null)
            {
                try { initializeEntriesMethod.Invoke(stockShop, null); }
                catch (Exception ex) { Debug.LogError($"调用 InitializeEntries 异常: {ex.Message}"); }
            }
            else
            {
                Debug.LogWarning("未找到 InitializeEntries 方法");
            }

            return stockShop;
        }

        // 创建或更新 fish_seller 的 MerchantProfile，仅添加 FishTypeIds 指定的物品
        private void EnsureMerchantProfileWithFishItems(string merchantId, IEnumerable<int> fishIds)
        {
            if (fishIds == null)
            {
                Debug.LogWarning("FishTypeIds为空，未添加任何鱼类");
                return;
            }

            var idSet = new HashSet<int>(fishIds.Where(i => i > 0));
            if (idSet.Count == 0)
            {
                Debug.LogWarning("FishTypeIds 未包含有效的 TypeID");
                return;
            }

            var db = StockShopDatabase.Instance;
            if (db == null)
            {
                Debug.LogError("StockShopDatabase.Instance 不可用");
                return;
            }

            var profiles = db.merchantProfiles;
            var profile = profiles.FirstOrDefault(p => p != null && p.merchantID == merchantId);
            if (profile == null)
            {
                profile = new StockShopDatabase.MerchantProfile { merchantID = merchantId };
                profiles.Add(profile);
            }

            // 索引所有 ItemEntries以便获取 prefab 和 MaxStackCount（若可用）
            var allItemEntries = ItemAssetsCollection.Instance?.entries;
            Dictionary<int, Item> prefabById = null;
            if (allItemEntries != null)
            {
                prefabById = new Dictionary<int, Item>();
                foreach (var e in allItemEntries)
                {
                    try { if (e != null && e.prefab != null) prefabById[e.typeID] = e.prefab; }
                    catch { /* ignore */ }
                }
            }

            int added = 0;
            foreach (var typeId in idSet)
            {
                // 跳过重复
                if (profile.entries.Any(e => e != null && e.typeID == typeId))
                {
                    // 已存在条目则同步更新可配置字段（价格系数/库存上限）
                    var exist = profile.entries.FirstOrDefault(e => e != null && e.typeID == typeId);
                    if (exist != null)
                    {
                        // 决定库存上限
                        int maxStock = DefaultMaxStock;
                        if (UsePrefabMaxStack && prefabById != null && prefabById.TryGetValue(typeId, out var prefab))
                        {
                            if (prefab != null && prefab.MaxStackCount > 0)
                                maxStock = prefab.MaxStackCount;
                        }

                        exist.maxStock = Mathf.Max(1, maxStock);
                        exist.priceFactor = Mathf.Max(0.01f, BuyPriceFactor);
                    }
                    continue;
                }

                // 验证该 ID 至少有元数据（避免明显无效 ID）
                var meta = ItemAssetsCollection.GetMetaData(typeId);
                // 将原来的 if (meta == null) 替换为判断 id 是否为0（假定无效 ItemMetaData 的 id 为0）
                if (meta.id == 0)
                {
                    Debug.LogWarning($"跳过无效的 TypeID（无元数据）：{typeId}");
                    continue;
                }

                // 决定库存上限
                int maxStockNew = DefaultMaxStock;
                if (UsePrefabMaxStack && prefabById != null && prefabById.TryGetValue(typeId, out var prefabNew))
                {
                    if (prefabNew != null && prefabNew.MaxStackCount > 0)
                        maxStockNew = prefabNew.MaxStackCount;
                }

                var entry = new StockShopDatabase.ItemEntry
                {
                    typeID = typeId,
                    maxStock = Mathf.Max(1, maxStockNew),
                    forceUnlock = true,
                    priceFactor = Mathf.Max(0.01f, BuyPriceFactor),
                    // -1 跳过0..1 概率逻辑，交给 DoRefreshStock 的其他分支处理（通常会上架）
                    possibility = -1f,
                    lockInDemo = false
                };

                profile.entries.Add(entry);
                added++;
            }

            Debug.Log($"fish_seller 配置已更新：新增鱼类 {added} 个，合计 {profile.entries.Count} 个条目。");
        }

        // 调用私有 CacheItemInstances进行预缓存
        private void TryInvokeCacheItemInstances(StockShop shop)
        {
            try
            {
                var mi = typeof(StockShop).GetMethod("CacheItemInstances",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (mi != null)
                {
                    //触发即可，返回值忽略
                    mi.Invoke(shop, Array.Empty<object>());
                }
            }
            catch (Exception ex)
            {
                // 如果是 TargetInvocationException，打印 InnerException
                var inner = ex.InnerException != null ? " | Inner: " + ex.InnerException : "";
                Debug.LogWarning($"预缓存失败: {ex.Message}{inner}");
            }
        }
    }
}