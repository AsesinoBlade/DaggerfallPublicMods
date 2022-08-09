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
        AsesinoInventoryWindow.Amulet = settings.GetString("SearchTags", "Amulet");
        AsesinoInventoryWindow.Bracelet = settings.GetString("SearchTags", "Bracelet");
        AsesinoInventoryWindow.Bracer = settings.GetString("SearchTags", "Bracer");
        AsesinoInventoryWindow.Ring = settings.GetString("SearchTags", "Ring");
        AsesinoInventoryWindow.Mark = settings.GetString("SearchTags", "Mark");
        AsesinoInventoryWindow.Crystal = settings.GetString("SearchTags", "Crystal");
        AsesinoInventoryWindow.Head = settings.GetString("SearchTags", "Head");
        AsesinoInventoryWindow.RightArm = settings.GetString("SearchTags", "RightArm");
        AsesinoInventoryWindow.LeftArm = settings.GetString("SearchTags", "LeftArm");
        AsesinoInventoryWindow.Cloak = settings.GetString("SearchTags", "Cloak");
        AsesinoInventoryWindow.ChestArmor = settings.GetString("SearchTags", "ChestArmor");
        AsesinoInventoryWindow.ChestClothes = settings.GetString("SearchTags", "ChestClothes");
        AsesinoInventoryWindow.RightHand = settings.GetString("SearchTags", "RightHand");
        AsesinoInventoryWindow.LeftHand = settings.GetString("SearchTags", "LeftHand");
        AsesinoInventoryWindow.LegsArmor = settings.GetString("SearchTags", "LegsArmor");
        AsesinoInventoryWindow.LegsClothes = settings.GetString("SearchTags", "LegsClothes");
        AsesinoInventoryWindow.Feet = settings.GetString("SearchTags", "Feet");
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
