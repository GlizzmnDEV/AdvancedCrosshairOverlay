using BepInEx.Configuration;
using UnityEngine;

namespace CrosshairOverlay
{
    public static class Configs
    {
        public static ConfigEntry<string> filename;
        public static ConfigEntry<bool> overlay;
        public static ConfigEntry<FilterMode> filteringMode;
        public static ConfigEntry<bool> hideDefault;
        public static ConfigEntry<bool> hideInMenus;
        public static ConfigEntry<bool> hideWhenScoped;
        public static ConfigEntry<float> scale;
        public static ConfigEntry<float> offsetX;
        public static ConfigEntry<float> offsetY;
        public static ConfigEntry<byte> tintR;
        public static ConfigEntry<byte> tintG;
        public static ConfigEntry<byte> tintB;
        public static ConfigEntry<byte> tintA;

        public static ConfigEntry<bool> useImageMode;
        public static ConfigEntry<string> builderShape;
        public static ConfigEntry<float> builderWidth;
        public static ConfigEntry<float> builderSpacing;
        public static ConfigEntry<float> builderLength;
        public static ConfigEntry<float> builderRotation;
        public static ConfigEntry<float> builderStrokeWidth;
        public static ConfigEntry<Color> builderStrokeColor;
        public static ConfigEntry<Color> builderFillColor;

        public static void BindTo(ConfigFile config)
        {
            useImageMode = config.Bind(
                "Mode",
                "Use Image Mode",
                true,
                "If true, use an image crosshair. If false, use the built-in crosshair builder."
            );

            filename = config.Bind(
                "ImageMode.General",
                "File name",
                "default-crosshair.png",
                "The name of the image file to load. png and jpeg formats are supported, and the file should be placed in the mod's directory. (" + Mod.PluginPath + ")"
            );

            overlay = config.Bind(
                "ImageMode.General",
                "Overlay",
                true,
                "Whether the crosshair image should be on top of the UI. If disabled, the image will be under the UI."
            );

            filteringMode = config.Bind(
                "ImageMode.General",
                "Image Filter Mode",
                FilterMode.Point,
                "The texture filtering mode to use for the crosshair image."
            );

            hideDefault = config.Bind(
                "General",
                "Hide default crosshair",
                true,
                "Whether to hide the default crosshair."
            );

            hideInMenus = config.Bind(
                "General",
                "Hide in menu",
                true,
                "Whether to hide the custom crosshair when in the main menu."
            );

            hideWhenScoped = config.Bind(
                "General",
                "Hide when scoped",
                true,
                "Whether to hide the custom crosshair when aiming with a scoped weapon."
            );

            scale = config.Bind(
                "ImageMode.Style.Size",
                "Scale",
                1.0f,
                new ConfigDescription(
                    "Scale factor relative to the crosshair image's native size. 1.0 = normal, 0.5 = half, 2.0 = double.",
                    new AcceptableValueRange<float>(0.1f, 5.0f)
                )
            );

            offsetX = config.Bind(
                "ImageMode.Style.Offset",
                "X Offset",
                0f,
                "Horizontal offset of the crosshair image relative to screen center."
            );

            offsetY = config.Bind(
                "ImageMode.Style.Offset",
                "Y Offset",
                0f,
                "Vertical offset of the crosshair image relative to screen center."
            );

            tintR = config.Bind(
                "ImageMode.Style.Color",
                "1 Tint R",
                byte.MaxValue,
                new ConfigDescription(
                    "The red channel tint of the crosshair image.",
                    new AcceptableValueRange<byte>(0, byte.MaxValue)
                )
            );

            tintG = config.Bind(
                "ImageMode.Style.Color",
                "2 Tint G",
                byte.MaxValue,
                new ConfigDescription(
                    "The green channel tint of the crosshair image.",
                    new AcceptableValueRange<byte>(0, byte.MaxValue)
                )
            );

            tintB = config.Bind(
                "ImageMode.Style.Color",
                "3 Tint B",
                byte.MaxValue,
                new ConfigDescription(
                    "The blue channel tint of the crosshair image.",
                    new AcceptableValueRange<byte>(0, byte.MaxValue)
                )
            );

            tintA = config.Bind(
                "ImageMode.Style.Color",
                "4 Tint A",
                byte.MaxValue,
                new ConfigDescription(
                    "The alpha channel (opacity) tint of the crosshair image.",
                    new AcceptableValueRange<byte>(0, byte.MaxValue)
                )
            );

            builderShape = config.Bind(
                "BuilderMode.General",
                "Shape",
                "Cross",
                "Shape of the builder crosshair. Options: Cross, Dot, Circle. (Only Cross works correctly LMAO) "
            );

            builderWidth = config.Bind(
                "BuilderMode.Style",
                "Width",
                2.0f,
                new ConfigDescription(
                    "Width of each crosshair arm or element.",
                    new AcceptableValueRange<float>(0.1f, 20.0f)
                )
            );

            builderSpacing = config.Bind(
                "BuilderMode.Style",
                "Spacing",
                4.0f,
                new ConfigDescription(
                    "Spacing between the center and each arm.",
                    new AcceptableValueRange<float>(0.2f, 100.0f)
                )
            );

            builderLength = config.Bind(
                "BuilderMode.Style",
                "Length",
                15.0f,
                new ConfigDescription(
                    "Length of each crosshair arm.",
                    new AcceptableValueRange<float>(1.0f, 200.0f)
                )
            );
            //Thought this would be cool but it's just not working and probly never will tbh. Too little of pixels to work with for this to work correctly IMO
            builderRotation = config.Bind(
                "BuilderMode.Style",
                "Rotation",
                0.0f,
                new ConfigDescription(
                    "Rotation of the crosshair in degrees. (Not Working) ",
                    new AcceptableValueRange<float>(0.0f, 0.1f)
                )
            );

            builderStrokeWidth = config.Bind(
                "BuilderMode.Stroke",
                "Stroke Width",
                0.0f,
                new ConfigDescription(
                    "Stroke outline width for the builder crosshair. (Not Working Correctly) ",
                    new AcceptableValueRange<float>(1.0f, 10.0f)
                )
            );

            builderStrokeColor = config.Bind(
                "BuilderMode.Stroke",
                "Stroke Color",
                Color.black,
                "Stroke color for the builder crosshair. (Not Working) "
            );

            builderFillColor = config.Bind(
                "BuilderMode.Fill",
                "Fill Color",
                Color.white,
                "Fill color for the builder crosshair."
            );
        }
    }
}
