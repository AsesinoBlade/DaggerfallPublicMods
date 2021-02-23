// Project:         RepairTools mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2020 Kirk.O
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Created On: 	    6/27/2020, 4:00 PM
// Last Edit:		8/1/2020, 12:05 AM
// Version:			1.00
// Special Thanks:  Hazelnut and Ralzar
// Modifier:		Hazelnut

using System.Collections.Generic;
using UnityEngine;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallConnect;

namespace Telescopes
{
    /// <summary>
    /// Abstract class for all repair items common behaviour
    /// </summary>
    public abstract class AbstractItemScopes : DaggerfallUnityItem
    {
        UserInterfaceManager uiManager = DaggerfallUI.Instance.UserInterfaceManager;

        public AbstractItemScopes(ItemGroups itemGroup, int templateIndex) : base(itemGroup, templateIndex)
        {
        }

        public abstract void UpdateCost(int v);

        public abstract int DurabilityLoss { get; }

        public abstract bool IsStationary { get; }

        public abstract uint GetItemID();

        public abstract int MaxZoom { get;}

        public abstract bool NightVision { get; }


        //public abstract Rect TelescopeDirectionPos { get; set; }

        //public abstract Texture2D TelescopeLens { get; set; }

        public abstract Texture2D CalculateTexture();

        public abstract Rect CalculateTelescopeDirectionPos();

        public override bool UseItem(ItemCollection collection)
        {
            if (!GameManager.Instance.PlayerEnterExit.IsPlayerSubmerged)
            {
                DaggerfallWorkshop._startTelescopes.TelescopeEnabled = true;
                Telescopes.TelescopeAndBinoculars.curParalyzed = GameManager.Instance.PlayerEntity.IsParalyzed;
                Telescopes.TelescopeAndBinoculars.isStationary = IsStationary;
                Telescopes.TelescopeAndBinoculars.maxLevel = MaxZoom;
                Telescopes.TelescopeAndBinoculars.TelescopeDirectionPos = CalculateTelescopeDirectionPos();
                Telescopes.TelescopeAndBinoculars.telescopeLens = CalculateTexture();
                Telescopes.TelescopeAndBinoculars.maxLevel = Mathf.Min(Telescopes.TelescopeAndBinoculars.absMaxLevel, MaxZoom);
                Telescopes.TelescopeAndBinoculars.nightVision = NightVision;
                DaggerfallUI.UIManager.PopWindow();
                Telescopes.TelescopeAndBinoculars.TelescopeEnabled = true;
                return true;
            }
            else
                DaggerfallUI.MessageBox($"{this.ItemName} does not work under water.");
            return false;
        }


    }
}