using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;

public class RegisterInventoryWindow : MonoBehaviour
{
    static Mod mod;

    public void Awake()
    {
        mod.LoadSettingsCallback = LoadSettings;
        mod.LoadSettings();

        mod.IsReady = true;
        Debug.Log("Inventory Finished Init");
    }

    private void LoadSettings(ModSettings settings, ModSettingsChange change)
    {
        AsesinoInventoryWindow.SplitTabsOnLootDrops = settings.GetValue<bool>("TabSettings", "SplitTabsOnLootDrops");
        AsesinoInventoryWindow.SplitTabsOnWagon = settings.GetValue<bool>("TabSettings", "SplitTabsOnWagon");
        AsesinoTradeWindow.CheckGeneralStore = mod.GetSettings().GetValue<bool>("CheckStoreSplitForTabs", "CheckGeneralStore");
        AsesinoTradeWindow.CheckPawnShops = mod.GetSettings().GetValue<bool>("CheckStoreSplitForTabs", "CheckPawnShops");
        AsesinoTradeWindow.CheckArmorer = mod.GetSettings().GetValue<bool>("CheckStoreSplitForTabs", "CheckArmorer");
        AsesinoTradeWindow.CheckWeaponShop = mod.GetSettings().GetValue<bool>("CheckStoreSplitForTabs", "CheckWeaponShop");
        AsesinoTradeWindow.CheckAlchemist = mod.GetSettings().GetValue<bool>("CheckStoreSplitForTabs", "CheckAlchemist");
        AsesinoTradeWindow.CheckClothingStore = mod.GetSettings().GetValue<bool>("CheckStoreSplitForTabs", "CheckClothingStore");
        AsesinoTradeWindow.CheckBookStore = mod.GetSettings().GetValue<bool>("CheckStoreSplitForTabs", "CheckBookStore");
        AsesinoTradeWindow.CheckGemStore = mod.GetSettings().GetValue<bool>("CheckStoreSplitForTabs", "CheckGemStore");
        FilterUtilities.Amulet += settings.GetString("SearchTags", "Amulet");
        FilterUtilities.Bracelet += settings.GetString("SearchTags", "Bracelet");
        FilterUtilities.Bracer += settings.GetString("SearchTags", "Bracer");
        FilterUtilities.Ring += settings.GetString("SearchTags", "Ring");
        FilterUtilities.Mark += settings.GetString("SearchTags", "Mark");
        FilterUtilities.Crystal += settings.GetString("SearchTags", "Crystal");
        FilterUtilities.Head += settings.GetString("SearchTags", "Head");
        FilterUtilities.RightArm += settings.GetString("SearchTags", "RightArm");
        FilterUtilities.LeftArm += settings.GetString("SearchTags", "LeftArm");
        FilterUtilities.Cloak += settings.GetString("SearchTags", "Cloak");
        FilterUtilities.ChestArmor += settings.GetString("SearchTags", "ChestArmor");
        FilterUtilities.ChestClothes += settings.GetString("SearchTags", "ChestClothes");
        FilterUtilities.RightHand += settings.GetString("SearchTags", "RightHand");
        FilterUtilities.LeftHand += settings.GetString("SearchTags", "LeftHand");
        FilterUtilities.LegsArmor += settings.GetString("SearchTags", "LegsArmor");
        FilterUtilities.LegsClothes += settings.GetString("SearchTags", "LegsClothes");
        FilterUtilities.Feet += settings.GetString("SearchTags", "Feet");
    }

    public void Start()
    {
        UIWindowFactory.RegisterCustomUIWindow(UIWindowType.Inventory, typeof(AsesinoInventoryWindow));
        UIWindowFactory.RegisterCustomUIWindow(UIWindowType.Trade, typeof(AsesinoTradeWindow));
        Debug.Log("Inventory registered windows");
    }
    
    [Invoke(StateManager.StateTypes.Start, 0)]
    public static void Init(InitParams initParams)
    {
        mod = initParams.Mod;
        Debug.Log("Inventory about to register");
        var go = new GameObject(mod.Title);
        go.AddComponent<RegisterInventoryWindow>();
        Debug.Log("Inventory Registered");
    }
}
