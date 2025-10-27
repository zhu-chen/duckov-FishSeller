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

namespace FishSeller
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        // 固定商人ID
        private const string MerchantID = "fish_seller";

        // 需填写：所有鱼类的 TypeID（请在这里填充）
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

        // 上架配置
        [Range(1, 999)] public int DefaultMaxStock = 10;
        [Range(0.01f, 10f)] public float BuyPriceFactor = 1.0f; // 买价系数（基础价值 * 该系数）
        public bool UsePrefabMaxStack = true;                    // 若能获取到 prefab 的 MaxStackCount，则优先使用
        public Vector3 PlaceOffset = new Vector3(0f, 0f, -4f);  // 克隆售货机的摆放偏移

        protected override void OnAfterSetup()
        {
            SceneManager.sceneLoaded -= OnAfterSceneInit;
            SceneManager.sceneLoaded += OnAfterSceneInit;
        }

        protected override void OnBeforeDeactivate()
        {
            SceneManager.sceneLoaded -= OnAfterSceneInit;
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

                // 预缓存所有条目实例，避免购买时 GetItemInstanceDirect 为 null
                TryInvokeCacheItemInstances(stockShop);
            }
        }

        // 初始化商店并绑定 fish_seller 配置
        private StockShop? InitShopItems(Transform shopRoot)
        {
            if (shopRoot == null)
            {
                Debug.LogWarning("未找到 FishSaleMachine 的商店根节点");
                return null;
            }

            var stockShop = shopRoot.GetComponent<StockShop>();
            if (stockShop == null)
            {
                Debug.LogWarning("商店根节点上未找到 StockShop 组件");
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

            // 建立/补充 fish_seller 的数据库配置，仅包含鱼类 ID
            EnsureMerchantProfileWithFishItems(MerchantID, FishTypeIds);

            // 调用 InitializeEntries 让 StockShop 读取配置到 entries
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
                Debug.LogWarning("FishTypeIds 为空，未添加任何鱼类");
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

            // 索引所有 ItemEntries 以便获取 prefab 和 MaxStackCount（若可用）
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
                    continue;

                // 验证该 ID 至少有元数据（避免明显无效 ID）
                var meta = ItemAssetsCollection.GetMetaData(typeId);
                // 将原来的 if (meta == null) 替换为判断 id 是否为 0（假定无效 ItemMetaData 的 id 为 0）
                if (meta.id == 0)
                {
                    Debug.LogWarning($"跳过无效的 TypeID（无元数据）：{typeId}");
                    continue;
                }

                // 决定库存上限
                int maxStock = DefaultMaxStock;
                if (UsePrefabMaxStack && prefabById != null && prefabById.TryGetValue(typeId, out var prefab))
                {
                    if (prefab != null && prefab.MaxStackCount > 0)
                        maxStock = prefab.MaxStackCount;
                }

                var entry = new StockShopDatabase.ItemEntry
                {
                    typeID = typeId,
                    maxStock = Mathf.Max(1, maxStock),
                    forceUnlock = true,
                    priceFactor = Mathf.Max(0.01f, BuyPriceFactor),
                    // -1 跳过 0..1 概率逻辑，交给 DoRefreshStock 的其他分支处理（通常会上架）
                    possibility = -1f,
                    lockInDemo = false
                };

                profile.entries.Add(entry);
                added++;
            }

            Debug.Log($"fish_seller 配置已更新：新增鱼类 {added} 个，合计 {profile.entries.Count} 个条目。");
        }

        // 调用私有 CacheItemInstances 进行预缓存
        private void TryInvokeCacheItemInstances(StockShop shop)
        {
            try
            {
                var mi = typeof(StockShop).GetMethod("CacheItemInstances",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (mi != null)
                {
                    // 触发即可，返回值忽略
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