// Project:         RepairTools mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2020 Kirk.O
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Created On: 	    6/27/2020, 4:00 PM
// Last Edit:		8/2/2020, 10:00 PM
// Version:			1.05
// Special Thanks:  Hazelnut and Ralzar
// Modifier:		Hazelnut

using UnityEngine;
using System;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallConnect;

namespace Telescopes
{

    public class ItemTelescope : AbstractItemScopes
    {
        public const int templateIndex = 830;
        public override int DurabilityLoss => 2;


        public override bool IsStationary => true;

        public override int MaxZoom => 40;

        public override bool NightVision => false;

        /*
        public override Rect TelescopeDirectionPos
        {
            get { return TelescopeDirectionPos; }
            set { TelescopeDirectionPos = value; }
        }

        public override Texture2D TelescopeLens
        {
            get { return TelescopeLens; }
            set { TelescopeLens = value; }
        }

        */
        public override void UpdateCost(int val)
        {
            this.value = val;
        }

        public ItemTelescope() : base(ItemGroups.UselessItems2, templateIndex)
        {
            UpdateCost(Telescopes.TelescopeAndBinoculars.TelescopeCost);
        }

        public override uint GetItemID()
        {
            return templateIndex;
        }




        public override Rect CalculateTelescopeDirectionPos()
        {
            int posX = 0;
            int posY = 0;
            var screenX = Screen.width;
            var screenY = Screen.height;

            if (Telescopes.TelescopeAndBinoculars.telescopeOverlay)
                if (screenX > screenY)
                {
                    posX = (screenX / 2 + screenY / 2) + 10;
                    posY = screenY / 2 - 100;
                }
                else
                {
                    posX = screenX / 2 - 100;
                    posY = (screenX / 2 + screenY / 2) + 10; ;
                }
            else
            {
                posX = screenX / 2 - 100;
                posY = (screenY - 250);
            }

            return new Rect(posX, posY, 200, 200);
        }

        public override Texture2D CalculateTexture()
        {
            return AbstractItemScopes.CalculateTelescopeTexture();
        }


        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(ItemTelescope).ToString();
            return data;
        }


    }

    public class ItemBinoculars : AbstractItemScopes
    {
        public const int templateIndex = 831;
        public override int DurabilityLoss => 2;


        public override bool IsStationary => false;

        public override int MaxZoom => 10;

        public override bool NightVision => false;

        /*
        public override Rect TelescopeDirectionPos
        {
            get { return TelescopeDirectionPos; }
            set { TelescopeDirectionPos = value; }
        }

        public override Texture2D TelescopeLens
        {
            get { return TelescopeLens; }
            set { TelescopeLens = value; }
        }

        */
        public override void UpdateCost(int val)
        {
            this.value = val;
        }

        public ItemBinoculars() : base(ItemGroups.UselessItems2, templateIndex)
        {
            UpdateCost(Telescopes.TelescopeAndBinoculars.TelescopeCost);
        }

        public override uint GetItemID()
        {
            return templateIndex;
        }




        public override Rect CalculateTelescopeDirectionPos()
        {
            int posX = 0;
            int posY = 0;
            var screenX = Screen.width;
            var screenY = Screen.height;

            if (Telescopes.TelescopeAndBinoculars.telescopeOverlay)
                if (screenX > screenY)
                {
                    posX = (screenX / 2 + screenY / 2) + 10;
                    posY = screenY / 2 - 100;
                }
                else
                {
                    posX = screenX / 2 - 100;
                    posY = (screenX / 2 + screenY / 2) + 10; ;
                }
            else
            {
                posX = screenX / 2 - 100;
                posY = (screenY - 250);
            }

            return new Rect(posX, posY, 200, 200);
        }

        public override Texture2D CalculateTexture()
        {
            return AbstractItemScopes.CalculateBinocularTexture();
        }

        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(ItemBinoculars).ToString();
            return data;
        }


    }

    public class ItemNVGoggles : AbstractItemScopes
    {
        public const int templateIndex = 832;
        public override int DurabilityLoss => 2;


        public override bool IsStationary => false;

        public override int MaxZoom => 5;

        public override bool NightVision => true;

        /*
        public override Rect TelescopeDirectionPos
        {
            get { return TelescopeDirectionPos; }
            set { TelescopeDirectionPos = value; }
        }

        public override Texture2D TelescopeLens
        {
            get { return TelescopeLens; }
            set { TelescopeLens = value; }
        }

        */
        public override void UpdateCost(int val)
        {
            this.value = val;
        }

        public ItemNVGoggles() : base(ItemGroups.UselessItems2, templateIndex)
        {
            UpdateCost(Telescopes.TelescopeAndBinoculars.TelescopeCost);
        }

        public override uint GetItemID()
        {
            return templateIndex;
        }




        public override Rect CalculateTelescopeDirectionPos()
        {
            int posX = 0;
            int posY = 0;
            var screenX = Screen.width;
            var screenY = Screen.height;

            if (Telescopes.TelescopeAndBinoculars.telescopeOverlay)
                if (screenX > screenY)
                {
                    posX = (screenX / 2 + screenY / 2) + 10;
                    posY = screenY / 2 - 100;
                }
                else
                {
                    posX = screenX / 2 - 100;
                    posY = (screenX / 2 + screenY / 2) + 10; ;
                }
            else
            {
                posX = screenX / 2 - 100;
                posY = (screenY - 250);
            }

            return new Rect(posX, posY, 200, 200);
        }

        public override Texture2D CalculateTexture()
        {
            return AbstractItemScopes.CalculateBinocularTexture();
        }

        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(ItemNVGoggles).ToString();
            return data;
        }


    }



}

