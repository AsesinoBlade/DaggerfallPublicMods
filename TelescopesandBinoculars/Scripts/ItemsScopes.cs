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


        public override bool IsStationary => false;

        public override int MaxZoom => 20;

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
            int w = Screen.width;
            int h = Screen.height;
            int cw = w;
            int ch = h;
            Texture2D telescopeLens = new Texture2D(cw, ch);


            bool portrait = w > h ? false : true;

            int r = portrait ? cw / 2 : ch / 2;

            for (int i = 0; i < cw; i++)
                for (int j = 0; j < ch; j++)
                    telescopeLens.SetPixel(i, j, Color.black);
            telescopeLens.Apply();

            for (int i = (int)(cw - r); i < cw + r; i++)
            {
                for (int j = (int)(ch - r); j < ch + r; j++)
                {
                    float dw = i - cw;
                    float dh = j - ch;
                    float d = Mathf.Sqrt(dw * dw + dh * dh);
                    if (d <= r)
                        if (portrait)
                            telescopeLens.SetPixel(i - (int)(cw - r), j + (ch / 2) - r - (int)(ch - r), Color.clear);
                        else
                            telescopeLens.SetPixel(i + (cw / 2) - r - (int)(cw - r), j - (int)(ch - r), Color.clear);
                }
            }
            telescopeLens.Apply();
            telescopeLens.Compress(true);
            return telescopeLens;
        }


        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(ItemTelescope).ToString();
            return data;
        }


    }

}

