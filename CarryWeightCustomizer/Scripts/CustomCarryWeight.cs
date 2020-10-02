using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallConnect;

public class CustomCarryWeight : MonoBehaviour {

    static Mod mod;
    static float multiplier = 1.5f;
    static float wagonWeightMultipler = 1.0f;
    static PlayerEntity pe;
    static bool useModifier;
    static float modifier;
    public void Awake()
    {
        
        var settings = mod.GetSettings();
        bool useCustomStrengthMultiplier = settings.GetValue<bool>("CustomCarryWeightSelector", "UseCustomStrengthMultiplier");
        if (useCustomStrengthMultiplier)
            multiplier = settings.GetValue<float>("CustomCarryWeightSelector", "Multiplier");

        useModifier = settings.GetValue<bool>("ModifyCarryWeight", "UseModifyCarryWeight");
        modifier = settings.GetValue<float>("ModifyCarryWeight", "Modifier");

        bool useCustomWagonWeight = settings.GetValue<bool>("CustomWagonWeight", "useCustomWagonWeight");
        if (useCustomWagonWeight)
        {
            float wagonWeight = ItemHelper.WagonKgLimit;
            wagonWeightMultipler = settings.GetValue<float>("CustomWagonWeight", "WagonWeightMultiplier");
            wagonWeight *= wagonWeightMultipler;
            ItemHelper.WagonKgLimit = (int)Mathf.Round(wagonWeight);
        }
        
        pe = FindPlayerEntity();

        // DaggerfallWorkshop.Game.Formulas.FormulaHelper.formula_1i.Add("MaxEncumbrance", (int strength) => {
        FormulaHelper.RegisterOverride(mod, "MaxEncumbrance", (Func<int, int>)MyMaxEncumbrance);
       // });

        
        mod.IsReady = true;

    }

    public int MyMaxEncumbrance(int strength)
    {
        int encModifier = 0;
        if (useModifier)
        {
            int climbing = pe.Skills.GetLiveSkillValue(DFCareer.Skills.Climbing);
            int jumping = pe.Skills.GetLiveSkillValue(DFCareer.Skills.Jumping);
            int running = pe.Skills.GetLiveSkillValue(DFCareer.Skills.Running);
            int swimming = pe.Skills.GetLiveSkillValue(DFCareer.Skills.Swimming);
            encModifier = (int)((climbing + jumping + running + swimming) / 4f * modifier);
        }
        return (int)Mathf.Floor((float)strength * multiplier + encModifier);
   }

[Invoke(StateManager.StateTypes.Start, 0)]
    public static void Init(InitParams initParams)
    {
        mod = initParams.Mod;
        var go = new GameObject(mod.Title);
        go.AddComponent<CustomCarryWeight>();
    }

    PlayerEntity FindPlayerEntity()
    {
        DaggerfallEntityBehaviour entityBehaviour;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        entityBehaviour = player.GetComponent<DaggerfallEntityBehaviour>();
        PlayerEntity playerEntity = entityBehaviour.Entity as PlayerEntity;

        return playerEntity;
    }

}
