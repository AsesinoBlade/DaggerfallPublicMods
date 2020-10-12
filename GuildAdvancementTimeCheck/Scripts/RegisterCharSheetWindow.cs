using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

public class RegisterCharSheetWindow : MonoBehaviour
{
    static Mod mod;

    public void Awake()
    {
        mod.IsReady = true;
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
