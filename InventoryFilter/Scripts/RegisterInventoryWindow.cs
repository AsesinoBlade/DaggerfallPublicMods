using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;

public class RegisterInventoryWindow : MonoBehaviour
{
    static Mod mod;
    public static string Amulet { get; private set; }
    public static string Bracelet { get; private set; }
    public static string Bracer { get; private set; }
    public static string Ring { get; private set; }
    public static string Mark { get; private set; }
    public static string Crystal { get; private set; }
    public static string Head { get; private set; }
    public static string RightArm { get; private set; }
    public static string LeftArm { get; private set; }
    public static string Cloak { get; private set; }
    public static string ChestArmor { get; private set; }
    public static string ChestClothes { get; private set; }
    public static string RightHand { get; private set; }
    public static string LeftHand { get; private set; }
    public static string LegsArmor { get; private set; }
    public static string LegsClothes { get; private set; }
    public static string Feet { get; private set; }

    public void Awake()
    {
        mod.LoadSettingsCallback = LoadSettings;
        mod.LoadSettings();

        mod.IsReady = true;
        Debug.Log("Inventory Finished Init");
    }

    private void LoadSettings(ModSettings settings, ModSettingsChange change)
    {
        Amulet = settings.GetString("SearchTags", "Amulet");
        Bracelet = settings.GetString("SearchTags", "Bracelet");
        Bracer = settings.GetString("SearchTags", "Bracer");
        Ring = settings.GetString("SearchTags", "Ring");
        Mark = settings.GetString("SearchTags", "Mark");
        Crystal = settings.GetString("SearchTags", "Crystal");
        Head = settings.GetString("SearchTags", "Head");
        RightArm = settings.GetString("SearchTags", "RightArm");
        LeftArm = settings.GetString("SearchTags", "LeftArm");
        Cloak = settings.GetString("SearchTags", "Cloak");
        ChestArmor = settings.GetString("SearchTags", "ChestArmor");
        ChestClothes = settings.GetString("SearchTags", "ChestClothes");
        RightHand = settings.GetString("SearchTags", "RightHand");
        LeftHand = settings.GetString("SearchTags", "LeftHand");
        LegsArmor = settings.GetString("SearchTags", "LegsArmor");
        LegsClothes = settings.GetString("SearchTags", "LegsClothes");
        Feet = settings.GetString("SearchTags", "Feet");
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
