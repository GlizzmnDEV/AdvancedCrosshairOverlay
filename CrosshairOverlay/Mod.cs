//Legit could not have done any of this without Kestral being a god. W Kestral for allowing me to do this and helping along the way.

//Also my favorite song right now is either "Pure Judgement" by amore OR "dancingwiththestars" by Gunnr


using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CrosshairOverlay
{
    [BepInPlugin("glizzmn.straftat.advancedcrosshairoverlay", "Advanced Crosshair Overlay", "1.0.0")]
    public class Mod : BaseUnityPlugin
    {
        public static Mod Instance { get; private set; }
        public static string PluginPath { get; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        internal static new ManualLogSource Logger;

        private static string imagePath;
        private static Sprite overlaySprite;
        private static Texture2D overlayTexture;
        private static GameObject root;
        private static Image imageComponent;
        private static RectTransform transformComponent;
        private static bool queueImageReload;
        private static bool imageLoaded;
        private static readonly string loadBearingColonThree = ":3";

        private static GameObject builderRoot;
        private static readonly string builderName = "Advanced Crosshair Overlay";

        private void Awake()
        {
            if (loadBearingColonThree != ":3")
                Application.Quit();

            gameObject.hideFlags = HideFlags.HideAndDontSave;
            Instance = this;
            Logger = base.Logger;
            Configs.BindTo(base.Config);
            base.Config.SettingChanged += OnConfigChanged;

            imagePath = Path.Combine(PluginPath, Configs.filename.Value);
            if (!File.Exists(imagePath))
                Logger.LogWarning("No image file found at " + imagePath);
            else
                StartCoroutine(LoadCrosshairImage());

            new Harmony("glizzmn.straftat.advancedcrosshairoverlay").PatchAll();
            Logger.LogInfo("Advanced Crosshair Overlay loaded! Please do not dm Glizzmn to fix something cause he probly won't out of spite (JK DM ME ABT ISSUES I'M BORED)");
        }

        private void OnConfigChanged(object sender, SettingChangedEventArgs e)
        {
            queueImageReload = true;

            if (e.ChangedSetting == Configs.filename)
            {
                imagePath = Path.Combine(PluginPath, Configs.filename.Value);
                if (!File.Exists(imagePath))
                    Logger.LogWarning("No image file found at " + imagePath);
                else
                    StartCoroutine(LoadCrosshairImage());
            }
        }

        private static IEnumerator LoadCrosshairImage()
        {
            if (!File.Exists(imagePath))
                yield break;

            byte[] data = File.ReadAllBytes(imagePath);
            overlayTexture = new Texture2D(2, 2);
            overlayTexture.LoadImage(data);

            overlaySprite = Sprite.Create(
                overlayTexture,
                new Rect(0, 0, overlayTexture.width, overlayTexture.height),
                new Vector2(0.5f, 0.5f),
                100f
            );

            imageLoaded = true;
            queueImageReload = true;
            yield break;
        }

        private static void SetupCrosshair(Transform uiRoot = null)
        {
            if (root != null) GameObject.Destroy(root);
            root = new GameObject("Advanced Crosshair Overlay");
            transformComponent = root.AddComponent<RectTransform>();
            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
            imageComponent = root.AddComponent<Image>();
            if (uiRoot != null)
                root.transform.SetParent(uiRoot, false);

            if (builderRoot != null) GameObject.Destroy(builderRoot);
            builderRoot = new GameObject(builderName);
            builderRoot.transform.SetParent(uiRoot, false);
        }

        //Scales images with CanvasScaler allowing for possibly better quality over different Resolutions. “Success isn’t always about greatness. It’s about consistency. Consistent hard work leads to success. Greatness will come.” – Dwayne Johnson
        private static void ReloadCrosshair()
        {
            if (Configs.useImageMode.Value)
            {
                if (overlaySprite == null || overlayTexture == null)
                    return;

                CanvasScaler scaler = root.GetComponentInParent<CanvasScaler>();
                float canvasScaleFactor = scaler ? scaler.scaleFactor : 1f;

                float baseWidth = overlayTexture.width;
                float baseHeight = overlayTexture.height;

                float finalWidth = baseWidth * Configs.scale.Value * canvasScaleFactor;
                float finalHeight = baseHeight * Configs.scale.Value * canvasScaleFactor;

                transformComponent.sizeDelta = new Vector2(finalWidth, finalHeight);
                transformComponent.anchoredPosition = new Vector2(
                    Configs.offsetX.Value * canvasScaleFactor,
                    Configs.offsetY.Value * canvasScaleFactor
                );

                imageComponent.color = new Color32(
                    Configs.tintR.Value,
                    Configs.tintG.Value,
                    Configs.tintB.Value,
                    Configs.tintA.Value
                );

                overlayTexture.filterMode = Configs.filteringMode.Value;
                imageComponent.sprite = overlaySprite;

                if (!Configs.overlay.Value)
                    root.transform.SetAsFirstSibling();
                else
                    root.transform.SetAsLastSibling();

                builderRoot.SetActive(false);
                root.SetActive(true);
            }
            else
            {
                root.SetActive(false);
                BuildProceduralCrosshair();
            }
        }

        //Builds off CanvasScaler to properly scale for DPI/Resolution. If its bad thats Unity's fault not mine :P
        private static void BuildProceduralCrosshair()
        {
            builderRoot.SetActive(true);

            foreach (Transform child in builderRoot.transform)
                GameObject.Destroy(child.gameObject);

            float canvasScaleFactor = builderRoot.GetComponentInParent<CanvasScaler>()?.scaleFactor ?? 1f;

            if (Configs.builderShape.Value == "Cross")
            {
                
                CreateBuilderArm(new Vector2(0, Configs.builderSpacing.Value), canvasScaleFactor, true);
                
                CreateBuilderArm(new Vector2(0, -Configs.builderSpacing.Value), canvasScaleFactor, true);
                
                CreateBuilderArm(new Vector2(Configs.builderSpacing.Value, 0), canvasScaleFactor, false);
                
                CreateBuilderArm(new Vector2(-Configs.builderSpacing.Value, 0), canvasScaleFactor, false);
            }
            else if (Configs.builderShape.Value == "Dot")
            {
                CreateBuilderDot(canvasScaleFactor);
            }
            else if (Configs.builderShape.Value == "Circle")
            {
                CreateBuilderCircle(canvasScaleFactor);
            }
        }

        private static void CreateBuilderArm(Vector2 offset, float scaleFactor, bool vertical)
        {
            GameObject arm = new GameObject("BuilderArm");
            arm.transform.SetParent(builderRoot.transform, false);
            var img = arm.AddComponent<Image>();
            img.color = Configs.builderFillColor.Value;
            RectTransform rt = arm.GetComponent<RectTransform>();

            // Allows for proper length/width so it doesnt make each line grow in two different directions etc
            if (vertical)
            {
                rt.sizeDelta = new Vector2(Configs.builderWidth.Value * scaleFactor, Configs.builderLength.Value * scaleFactor);

                if (offset.y > 0)
                    rt.pivot = new Vector2(0.5f, 0f);
                else              
                    rt.pivot = new Vector2(0.5f, 1f);
            }
            else
            {
                rt.sizeDelta = new Vector2(Configs.builderLength.Value * scaleFactor, Configs.builderWidth.Value * scaleFactor);

                if (offset.x > 0)
                    rt.pivot = new Vector2(0f, 0.5f);
                else              
                    rt.pivot = new Vector2(1f, 0.5f);
            }

            rt.anchoredPosition = offset * scaleFactor;
            rt.localRotation = Quaternion.identity;
        }

        private static void CreateBuilderArm(Vector2 offset, float scaleFactor)
        {
            GameObject arm = new GameObject("BuilderArm");
            arm.transform.SetParent(builderRoot.transform, false);
            var img = arm.AddComponent<Image>();
            img.color = Configs.builderFillColor.Value;
            RectTransform rt = arm.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(Configs.builderWidth.Value * scaleFactor, Configs.builderLength.Value * scaleFactor);
            rt.anchoredPosition = offset * scaleFactor;
            rt.localRotation = Quaternion.Euler(0, 0, Configs.builderRotation.Value);
        }

        //Creates a really shitty dot cause its a square not a circle LMAO
        private static void CreateBuilderDot(float scaleFactor)
        {
            GameObject dot = new GameObject("BuilderDot");
            dot.transform.SetParent(builderRoot.transform, false);
            var img = dot.AddComponent<Image>();
            img.color = Configs.builderFillColor.Value;
            RectTransform rt = dot.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(Configs.builderLength.Value * scaleFactor, Configs.builderLength.Value * scaleFactor);
            rt.anchoredPosition = Vector2.zero;
        }

        //Same thing.. Square not Circle.. oops I'm tired
        private static void CreateBuilderCircle(float scaleFactor)
        {
            GameObject circle = new GameObject("BuilderCircle");
            circle.transform.SetParent(builderRoot.transform, false);
            var img = circle.AddComponent<Image>();
            img.color = Configs.builderFillColor.Value;
            RectTransform rt = circle.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(Configs.builderLength.Value * scaleFactor, Configs.builderLength.Value * scaleFactor);
            rt.anchoredPosition = Vector2.zero;
        }

        [HarmonyPatch(typeof(Crosshair))]
        public static class CrosshairPatch
        {
            [HarmonyPatch("Start"), HarmonyPrefix]
            public static void SetupCrosshairOverlay(Crosshair __instance)
            {
                if (Configs.hideDefault.Value)
                    __instance.transform.localScale = Vector3.zero;

                SetupCrosshair(__instance.transform.parent);
                if (imageLoaded || !Configs.useImageMode.Value)
                    ReloadCrosshair();
            }

            [HarmonyPatch("Update"), HarmonyPrefix]
            public static void UpdateOverlayVisibility(ref Crosshair __instance)
            {
                if (queueImageReload && (imageLoaded || !Configs.useImageMode.Value))
                {
                    ReloadCrosshair();
                    queueImageReload = false;
                }

                bool shouldHide = false;

                if (Configs.hideWhenScoped.Value && __instance.player != null && __instance.canScopeAim && __instance.player.isAiming)
                    shouldHide = true;
                else if (Configs.hideInMenus.Value && SceneManager.GetActiveScene().name == "MainMenu")
                    shouldHide = true;

                if (Configs.useImageMode.Value)
                    root.SetActive(!shouldHide);
                else
                    builderRoot.SetActive(!shouldHide);
            }
        }
    }
}
