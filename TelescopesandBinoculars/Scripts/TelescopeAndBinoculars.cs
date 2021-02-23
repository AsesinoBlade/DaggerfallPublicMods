// Project:         RepaisrTools mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2020 Kirk.O
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Created On: 	    6/27/2020, 4:00 PM
// Last Edit:		8/2/2020, 10:00 PM
// Version:			1.05
// Special Thanks:  Hazelnut and Ralzar
// Modifier:		Hazelnut	

using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallConnect.FallExe;
using Wenzil.Console;

namespace Telescopes
{
    public class TelescopeAndBinoculars : MonoBehaviour
    {

        public static int[] levelTbl = new int[20];
        public static int[] zoomTbl = new int[20];

        public static int currZoom = 1;
        public static int maxLevel = 1;
        public static int absMaxLevel = 1;
        public static Texture2D telescopeLens;
        public static bool telescopeOverlay = true;
        public static Rect TelescopeDirectionPos;
        public static int TelescopeCost = 100;

        public static Rect pos;
        public static GUIStyle guiStyle = new GUIStyle();

        public static PlayerEntity pe;
        public static bool TelescopeEnabled { get; set; }

        private KeyCode currKey;
        public KeyCode toggleKey = KeyCode.End;
        public KeyCode escKey = KeyCode.Escape;
        public string toggleString = "END";
        public KeyCode upKey = KeyCode.UpArrow;
        public string upString = "Up Arrow";
        public KeyCode downKey = KeyCode.DownArrow;
        public string downString = "Down Arrow";
        public static GameObject lightGameObject;
        public static Light lightComponent;
        public static bool nightVision;


        public static TelescopeAndBinoculars Instance
        {
            get; set;
        }

        static Mod mod;

        public static Mod myMod;
        public static bool curParalyzed;
        public static bool isStationary;

        static Camera fovCamera;
        static float prevFov;
        static float origFov;

        //public const string audioClips = "Zero Gravity 1.ogg";

        //list of audio clip assets bundled in mod


        void Awake()
        {

            InitMod();

            mod.IsReady = true;
        }

        void Start()
        {

            //get reference to mod object.  
            myMod = mod;
            RegisterCommands();
            ItemHelper itemHelper = DaggerfallUnity.Instance.ItemHelper;
            itemHelper.RegisterCustomItem(ItemTelescope.templateIndex, ItemGroups.UselessItems2, typeof(ItemTelescope));

            pos = new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height));

            GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
            if (camObj)
            {
                fovCamera = camObj.GetComponent<Camera>();
                origFov = fovCamera.fieldOfView;
                prevFov = origFov;
            }
            var screenX = Screen.width;
            var screenY = Screen.height;

            guiStyle.normal.textColor = Color.yellow;
            guiStyle.fontSize = 20;
            pe = GameManager.Instance.PlayerEntity;
            PopulateZoomLevel();

            // create light source for night vision
            lightGameObject = new GameObject("Night Vision");
            lightComponent = lightGameObject.AddComponent<Light>();
            lightComponent.color = Color.green;
            lightGameObject.transform.position = new Vector3(0, 5, 0);
            lightComponent.intensity = 3;
            lightComponent.enabled = false;


        }


        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            var go = new GameObject(mod.Title);
            Instance = go.AddComponent<TelescopeAndBinoculars>();
        }


        public void Update()
        {
            if (!TelescopeEnabled)
            {
                lightComponent.enabled = false;
                return;
            }


            if (DaggerfallUI.UIManager.TopWindow.GetType() != typeof(DaggerfallHUD))
            {
                fovCamera.fieldOfView = origFov;
                lightComponent.enabled = false;
                return;
            }

            fovCamera.fieldOfView = prevFov;

            if (GameManager.Instance.StateManager.CurrentState == StateManager.StateTypes.Game && (Input.GetKeyDown(toggleKey) || Input.GetKeyDown(escKey)))
            {
                TelescopeEnabled = !TelescopeEnabled;
                fovCamera.fieldOfView = origFov;
                pe.IsParalyzed = curParalyzed;
                lightComponent.enabled = false;
                currZoom = 1;
            }

            if (TelescopeEnabled)
            {
                if (isStationary)
                    pe.IsParalyzed = true;

                if ((Input.GetAxis("Mouse ScrollWheel") > 0f) || (Input.GetKeyDown(upKey)))
                {

                    if (currZoom < maxLevel)
                        prevFov = zoomTbl[++currZoom];
                    fovCamera.fieldOfView = prevFov;
                }

                if ((Input.GetAxis("Mouse ScrollWheel") < 0f) || (Input.GetKeyDown(downKey)))
                {

                    if (currZoom > 1)
                        prevFov = zoomTbl[--currZoom];
                    fovCamera.fieldOfView = prevFov;
                }
            }
        }

        private void PopulateZoomLevel()
        {
            double factor;
            double zoomedFOV;
            int cnt = 1;
            int pos = 1;
            int magnification = 1;
            int prevZFov = 0;
            int zFOV = 2;

            factor = 2.0 * (Math.Tan(0.5 * origFov * Math.PI / 180.0));
            while (cnt <= 16)
            {
                zoomedFOV = 2.0 * Math.Atan(factor / (2.0 * magnification)) * 180.0 / Math.PI;
                zFOV = (int)Math.Round(zoomedFOV, 0);
                if (zFOV < 1)
                    zFOV = 1;
                if (zFOV != prevZFov)
                {
                    levelTbl[pos] = magnification;
                    zoomTbl[pos] = zFOV;
                    prevZFov = zFOV;
                    pos++;
                }

                if (cnt >= 10)
                    magnification += 5;
                else
                    magnification++;
                cnt++;
            }
            cnt = 16;
            absMaxLevel = pos - 1;
        }



        #region InitMod and Settings

        private static void InitMod()
        {
            Debug.Log("Begin mod init: Telescopes And Binoculars");
            //var settings = mod.GetSettings();

 
            Debug.Log("Finished mod init: Telescopes and Binoculars");
        }

        private void OnGUI()
        {
            if (!TelescopeEnabled)
                return;

            if (Event.current.type.Equals(EventType.Repaint) && !GameManager.IsGamePaused)
            {
                GUI.depth = 1;
                if (nightVision)
                {
                    lightGameObject.transform.position = GameManager.Instance.PlayerObject.transform.position;
                    lightComponent.color = Color.green;
                    lightComponent.type = LightType.Point;
                    lightComponent.range = 50;
                    lightComponent.enabled = true;
                }



                if (telescopeOverlay)
                    GUI.DrawTexture(pos, telescopeLens, ScaleMode.StretchToFill);

                /*
                if (showScopeDirections)
                {
                    string str = levelTbl[currZoom].ToString() + " X\n\n" + "Press [" + toggleString + "] to exit. \n\n" + "Press [" + upString + "] to zoom In.\n" + "Press [" + downString + "] to zoom out.";
                    GUI.Label(TelescopeDirectionPos, str, guiStyle);
                }
                */
            }
        }

        KeyCode AssignFromDropDown(int dropDownKey, out string keyString)
        {
            KeyCode key = KeyCode.End;
            switch (dropDownKey)
            {
                case 0: //End
                    key = KeyCode.End;
                    keyString = "End";
                    break;
                case 1: //Del
                    key = KeyCode.Delete;
                    keyString = "Delete";
                    break;
                case 2:  //Home
                    key = KeyCode.Home;
                    keyString = "Home";
                    break;
                case 3:  //Ins
                    key = KeyCode.Insert;
                    keyString = "Insert";
                    break;
                case 4:  //Page Up
                    key = KeyCode.PageUp;
                    keyString = "Page Up";
                    break;
                case 5: //Page Down
                    key = KeyCode.PageDown;
                    keyString = "Page Down";
                    break;
                case 6: //Up Arrow
                    key = KeyCode.UpArrow;
                    keyString = "Up Arrow";
                    break;
                case 7: //Down Arrow
                    key = KeyCode.DownArrow;
                    keyString = "Down Arrow";
                    break;
                case 8:  //Left Arrow
                    key = KeyCode.LeftArrow;
                    keyString = "Left Arrow";
                    break;
                case 9: //Right Arrow
                    key = KeyCode.RightArrow;
                    keyString = "Right Arrow";
                    break;
                case 10: //F8
                    key = KeyCode.F8;
                    keyString = "F8";
                    break;
                case 11: //F9
                    key = KeyCode.F9;
                    keyString = "F9";
                    break;
                case 12:  //F10
                    key = KeyCode.F10;
                    keyString = "F10";
                    break;
                case 13: //F11
                    key = KeyCode.F11;
                    keyString = "F11";
                    break;
                case 14: //F12
                    key = KeyCode.F12;
                    keyString = "F12";
                    break;
                default:
                    keyString = String.Format("{0}", (char)(dropDownKey + 50));
                    key = (KeyCode)Enum.Parse(typeof(KeyCode), keyString);
                    break;
            }
            return key;
        }
        #endregion
        public static DaggerfallUnityItem GetCustomItem(int r)
        {
            DaggerfallUnityItem item = new DaggerfallUnityItem();

            int[] customItemTemplates = DaggerfallUnity.Instance.ItemHelper.GetCustomItemsForGroup(ItemGroups.UselessItems2);

            for (int j = 0; j < customItemTemplates.Length; j++)
            {
                ItemTemplate itemTemplate = DaggerfallUnity.Instance.ItemHelper.GetItemTemplate(ItemGroups.UselessItems2, customItemTemplates[j]);
                if (itemTemplate.index == r)
                {
                    item = ItemBuilder.CreateItem(ItemGroups.UselessItems2, customItemTemplates[j]);
                    break;
                }

            }

            return item;

        }

        const string noInstanceMessage = "Telescopes and Binoculars instance not found.";

        public static void RegisterCommands()
        {
            try
            {
                ConsoleCommandsDatabase.RegisterCommand(AddScopes.name, AddScopes.description, AddScopes.usage, AddScopes.Execute);
                ConsoleCommandsDatabase.RegisterCommand(Scope_Change_Intensity.name, Scope_Change_Intensity.description, Scope_Change_Intensity.usage, Scope_Change_Intensity.Execute);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Error Registering TelescopesAndBinoculars Console commands: {0}", e.Message));
            }
        }


        private static class AddScopes
        {
            public static readonly string name = "Add_Scopes";
            public static readonly string description = "Places  all types of scopes in players inventory";
            public static readonly string usage = "adds to inventory";

            public static string Execute(params string[] args)
            {
               DaggerfallUnityItem scope = GetCustomItem(830);
               pe.Items.AddItem(scope);

                return "Scopes added";


            }
        }

        private static class Scope_Change_Intensity
        {
            public static readonly string name = "Scope_Change_Intensity";
            public static readonly string description = "Changes Night Vision Intensity";
            public static readonly string usage = "Scope_Change_Intensity value where value is an integer from 1 to 5";
            public static readonly string noLS = "Error Light Source not created; please use the scope first and then execute this command to change intensity. ";

            public static string Execute(params string[] args)
            {
                if (TelescopeAndBinoculars.lightComponent != null)
                {
                    int a;
                    if (args.Length > 0 && Int32.TryParse(args[0], out a))
                        if (a > 0 && a <= 5)
                        {
                            TelescopeAndBinoculars.lightComponent.intensity = a;
                            return $"Night Vision Intensity adjusted to {a}";
                        }
                        else
                            return usage;
                    else
                        return usage;
                }
                else
                    return noLS ;
            }
        }


    }
}