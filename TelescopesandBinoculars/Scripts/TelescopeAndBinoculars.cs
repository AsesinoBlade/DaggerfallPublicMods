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
        public static bool RainShowing = false;

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
        //public static Light lightComponent;
        public static bool nightVision;

        // material properties
        public  float MatSize = 1.5f;
        public  float MatTime = 1;
        public  float MatRainDistortion = 2f;
        public  float MatThunderDistortion = 2f;
        public float MatRainBlur = 0.0003f;
        public float MatThunderBlur = 0.01f;
        public Color MatNightVisionColor = Color.green;
        public  float MatBaseIntensity = 2.3f;
        public float MatNightVisionIntensity = 20;
        public float MatRatio = 1.77f;


        public static TelescopeAndBinoculars Instance
        {
            get; set;
        }

        static Mod mod;

        public static Mod myMod;
        public static bool curParalyzed;
        public static bool isStationary;
        public static Material material;
        static Camera fovCamera;
        static float prevFov;
        static float origFov;

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
            itemHelper.RegisterCustomItem(ItemBinoculars.templateIndex, ItemGroups.UselessItems2, typeof(ItemBinoculars));
            itemHelper.RegisterCustomItem(ItemNVGoggles.templateIndex, ItemGroups.UselessItems2, typeof(ItemNVGoggles));

            pos = new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height));
            material = mod.GetAsset<Material>("LensMaterial");
            GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
            if (camObj)
            {
                fovCamera = camObj.GetComponent<Camera>();
                origFov = fovCamera.fieldOfView;
                prevFov = origFov;
            }
            var screenX = Screen.width;
            var screenY = Screen.height;

            MatRatio = (float)screenX / screenY;

            guiStyle.normal.textColor = Color.yellow;
            guiStyle.fontSize = 20;
            pe = GameManager.Instance.PlayerEntity;
            PopulateZoomLevel();


        }

        private void LoadSettings(ModSettings settings, ModSettingsChange change)
        {

            MatSize = settings.GetValue<float>("Options", "RainSize");
            MatRainDistortion = settings.GetValue<float>("Options", "RainDistortion");
            MatThunderDistortion = settings.GetValue<float>("Options", "ThunderDistortion");
            MatRainBlur = (float) (settings.GetValue<int>("Options", "RainBlur")) / 1000f;
            MatThunderBlur = (float) (settings.GetValue<int>("Options", "ThunderBlur")) / 1000f;
            MatNightVisionColor = settings.GetColor("Options", "NightVisionColor");
            MatBaseIntensity = settings.GetValue<float>("Options", "BaseIntensity");
            MatNightVisionIntensity = (float)settings.GetValue<int>("Options", "NightVisionIntensity");
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
                return;
            }


            if (DaggerfallUI.UIManager.TopWindow.GetType() != typeof(DaggerfallHUD))
            {
                fovCamera.fieldOfView = origFov;
                return;
            }

            fovCamera.fieldOfView = prevFov;

            if (Input.GetKeyDown(toggleKey) || Input.GetKeyDown(escKey))
            {
                TelescopeEnabled = !TelescopeEnabled;
                fovCamera.fieldOfView = origFov;
                pe.IsParalyzed = curParalyzed;
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

        private  void InitMod()
        {
            Debug.Log("Begin mod init: Telescopes And Binoculars");
            //var settings = mod.GetSettings();
            mod.LoadSettingsCallback = LoadSettings;
            mod.LoadSettings();

            Debug.Log("Finished mod init: Telescopes and Binoculars");
        }

        private bool CompareTexture(Texture2D first, Texture2D second)
        {
            Color[] firstPix = first.GetPixels();
            Color[] secondPix = second.GetPixels();
            if (firstPix.Length != secondPix.Length)
            {
                return false;
            }
            for (int i = 0; i < firstPix.Length; i++)
            {
                if (firstPix[i] != secondPix[i])
                {
                    return false;
                }
            }

            return true;
        }

        private void OnGUI()
        {
            
            if (!TelescopeEnabled)
                return;

            if (Event.current.type.Equals(EventType.Repaint) && !GameManager.IsGamePaused)
            {
                GUI.depth = 1;
                if (GameManager.Instance.PlayerEnterExit.IsPlayerInside)
                    RainShowing = false;
                else
                    RainShowing = GameManager.Instance.WeatherManager.IsRaining ||
                              GameManager.Instance.WeatherManager.IsStorming;

                var ThunderShowing = GameManager.Instance.WeatherManager.IsStorming;


                if (telescopeOverlay)
                {
                    if (RainShowing && !ThunderShowing)
                    {
                        material.SetFloat("_Size", MatSize);
                        material.SetFloat("_Time", MatTime);
                        material.SetFloat("_Distortion", MatRainDistortion);
                        material.SetFloat("_Blur", MatRainBlur);
                        material.SetFloat("_Ratio", MatRatio);
                        material.SetFloat("_NightVision", nightVision ? 1 : 0);
                        material.SetColor("_NightVisionColor", MatNightVisionColor);
                        material.SetFloat("_NightVisionIntensity", MatNightVisionIntensity);

                        material.SetFloat("_Raining", 1);

                        material.SetFloat("_BaseIntensity", MatBaseIntensity);

                        Graphics.DrawTexture(pos, telescopeLens, 0, Screen.width, 0, Screen.height, material);
                    }
                    else if (RainShowing && ThunderShowing)
                    {
                        material.SetFloat("_Size", MatSize);
                        material.SetFloat("_Time", MatTime);
                        material.SetFloat("_Distortion", MatThunderDistortion);
                        material.SetFloat("_Blur", MatThunderBlur);
                        material.SetFloat("_Ratio", MatRatio);
                        material.SetFloat("_NightVision", nightVision ? 1 : 0);
                        material.SetColor("_NightVisionColor", MatNightVisionColor);
                        material.SetFloat("_NightVisionIntensity", MatNightVisionIntensity);

                        material.SetFloat("_Raining", 1);

                        material.SetFloat("_BaseIntensity", MatBaseIntensity);

                        Graphics.DrawTexture(pos, telescopeLens, 0, Screen.width, 0, Screen.height, material);
                    }
                    else if (nightVision)
                    {

                        material.SetFloat("_Size", MatSize);
                        material.SetFloat("_Time", MatTime);
                        material.SetFloat("_Distortion", MatThunderDistortion);
                        material.SetFloat("_Blur", MatThunderBlur);
                        material.SetFloat("_Ratio", MatRatio);
                        material.SetFloat("_NightVision", nightVision ? 1 : 0);
                        material.SetColor("_NightVisionColor", MatNightVisionColor);
                        material.SetFloat("_Raining", 0);

                        material.SetFloat("_BaseIntensity", MatBaseIntensity);
                        material.SetFloat("_NightVisionIntensity", MatNightVisionIntensity); material.SetColor("_NightVisionColor", MatNightVisionColor);
                        material.SetFloat("_NightVision", 1);
                        material.SetFloat("_NightVisionIntensity", MatNightVisionIntensity);

                        Graphics.DrawTexture(pos, telescopeLens, 0, Screen.width, 0, Screen.height, material);

                    }
                    else
                    {
                        GUI.DrawTexture(pos, telescopeLens, ScaleMode.StretchToFill);
                    }

                }

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
                scope = GetCustomItem(831);
                pe.Items.AddItem(scope);
                scope = GetCustomItem(832);
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
                //if (TelescopeAndBinoculars.lightComponent != null)
                //{
                //    int a;
                //    if (args.Length > 0 && Int32.TryParse(args[0], out a))
                //        if (a > 0 && a <= 5)
                //        {
                //           // TelescopeAndBinoculars.lightComponent.intensity = a;
                //            return $"Night Vision Intensity adjusted to {a}";
                //        }
                //        else
                //            return usage;
                //    else
                //        return usage;
                //}
                //else
                    return noLS ;
            }
        }


    }
}