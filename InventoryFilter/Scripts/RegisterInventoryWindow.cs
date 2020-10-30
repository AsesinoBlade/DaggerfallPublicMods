using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

public class RegisterInventoryWindow : MonoBehaviour
{
    static Mod mod;

    public void Awake()
    {
        mod.IsReady = true;
        Debug.Log("Inventory Finished Init");
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
