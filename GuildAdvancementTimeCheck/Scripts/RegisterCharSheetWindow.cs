using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using Mono.CSharp;

public class RegisterCharSheetWindow : MonoBehaviour
{
    static Mod mod;
    public static bool showPct = false;

    public void Awake()
    {
        mod.LoadSettingsCallback = LoadSettings;
        mod.LoadSettings();
        mod.IsReady = true;
    }

    private void LoadSettings(ModSettings settings, ModSettingsChange change)
    {
        showPct = settings.GetValue<bool>("Settings", "ShowPercent");
    }

    public void Start()
    {
        UIWindowFactory.RegisterCustomUIWindow(UIWindowType.CharacterSheet, typeof(AsesinoCharSheetWindow));
        Debug.Log("registered Rest window");
    }

    [Invoke(StateManager.StateTypes.Start, 0)]
    public static void Init(InitParams initParams)
    {
        mod = initParams.Mod;
        var go = new GameObject(mod.Title);
        go.AddComponent<RegisterCharSheetWindow>();
    }
}
