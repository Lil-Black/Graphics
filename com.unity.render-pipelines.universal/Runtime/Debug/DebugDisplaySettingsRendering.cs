using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering
{
    public class DebugDisplaySettingsRendering : IDebugDisplaySettingsData
    {
        internal DebugFullScreenMode debugFullScreenMode { get; private set; } = DebugFullScreenMode.None;
        internal DebugSceneOverrideMode debugSceneOverrideMode { get; private set; } = DebugSceneOverrideMode.None;
        internal DebugMipInfoMode debugMipInfoMode { get; private set; } = DebugMipInfoMode.None;

        public DebugPostProcessingMode debugPostProcessingMode { get; private set; } = DebugPostProcessingMode.Auto;
        public bool enableMsaa { get; private set; } = true;
        public bool enableHDR { get; private set; } = true;

        internal static class WidgetFactory
        {
            internal static DebugUI.Widget CreateFullScreenModes(DebugDisplaySettingsRendering data) => new DebugUI.EnumField
            {
                displayName = "Full Screen Modes",
                autoEnum = typeof(DebugFullScreenMode),
                getter = () => (int)data.debugFullScreenMode,
                setter = (value) => {},
                getIndex = () => (int)data.debugFullScreenMode,
                setIndex = (value) => data.debugFullScreenMode = (DebugFullScreenMode)value
            };

            internal static DebugUI.Widget CreateSceneDebugModes(DebugDisplaySettingsRendering data) => new DebugUI.EnumField
            {
                displayName = "Scene Debug Modes",
                autoEnum = typeof(DebugSceneOverrideMode),
                getter = () => (int)data.debugSceneOverrideMode,
                setter = (value) => {},
                getIndex = () => (int)data.debugSceneOverrideMode,
                setIndex = (value) => data.debugSceneOverrideMode = (DebugSceneOverrideMode)value
            };

            internal static DebugUI.Widget CreateMipModesDebug(DebugDisplaySettingsRendering data) => new DebugUI.EnumField
            {
                displayName = "Mip Modes Debug",
                autoEnum = typeof(DebugMipInfoMode),
                getter = () => (int)data.debugMipInfoMode,
                setter = (value) => {},
                getIndex = () => (int)data.debugMipInfoMode,
                setIndex = (value) => data.debugMipInfoMode = (DebugMipInfoMode)value
            };

            internal static DebugUI.Widget CreatePostProcessing(DebugDisplaySettingsRendering data) => new DebugUI.EnumField
            {
                displayName = "Post-processing",
                autoEnum = typeof(DebugPostProcessingMode),
                getter = () => (int)data.debugPostProcessingMode,
                setter = (value) => data.debugPostProcessingMode = (DebugPostProcessingMode)value,
                getIndex = () => (int)data.debugPostProcessingMode,
                setIndex = (value) => data.debugPostProcessingMode = (DebugPostProcessingMode)value
            };

            internal static DebugUI.Widget CreateMSAA(DebugDisplaySettingsRendering data) => new DebugUI.BoolField
            {
                displayName = "MSAA",
                getter = () => data.enableMsaa,
                setter = (value) => data.enableMsaa = value
            };

            internal static DebugUI.Widget CreateHDR(DebugDisplaySettingsRendering data) => new DebugUI.BoolField
            {
                displayName = "HDR",
                getter = () => data.enableHDR,
                setter = (value) => data.enableHDR = value
            };
        }

        private class SettingsPanel : DebugDisplaySettingsPanel
        {
            public override string PanelName => "Rendering";

            public SettingsPanel(DebugDisplaySettingsRendering data)
            {
                AddWidget(WidgetFactory.CreateFullScreenModes(data));
                AddWidget(WidgetFactory.CreateSceneDebugModes(data));
                AddWidget(WidgetFactory.CreateMipModesDebug(data));
                AddWidget(WidgetFactory.CreatePostProcessing(data));
                AddWidget(WidgetFactory.CreateMSAA(data));
                AddWidget(WidgetFactory.CreateHDR(data));
            }
        }

        #region IDebugDisplaySettingsData
        public bool AreAnySettingsActive => (debugPostProcessingMode != DebugPostProcessingMode.Auto) ||
        (debugFullScreenMode != DebugFullScreenMode.None) ||
        (debugSceneOverrideMode != DebugSceneOverrideMode.None) ||
        (debugMipInfoMode != DebugMipInfoMode.None);

        public bool IsPostProcessingAllowed => (debugPostProcessingMode != DebugPostProcessingMode.Disabled) &&
        (debugSceneOverrideMode == DebugSceneOverrideMode.None) &&
        (debugMipInfoMode == DebugMipInfoMode.None);

        public bool IsLightingActive => (debugSceneOverrideMode == DebugSceneOverrideMode.None) &&
        (debugMipInfoMode == DebugMipInfoMode.None);

        public bool TryGetScreenClearColor(ref Color color)
        {
            switch (debugSceneOverrideMode)
            {
                case DebugSceneOverrideMode.None:
                case DebugSceneOverrideMode.ShadedWireframe:
                    return false;

                case DebugSceneOverrideMode.Overdraw:
                    color = Color.black;
                    return true;

                case DebugSceneOverrideMode.Wireframe:
                case DebugSceneOverrideMode.SolidWireframe:
                    color = new Color(0.1f, 0.1f, 0.1f, 1.0f);
                    return true;

                default:
                    throw new ArgumentOutOfRangeException(nameof(color));
            }       // End of switch.
        }

        public IDebugDisplaySettingsPanelDisposable CreatePanel()
        {
            return new SettingsPanel(this);
        }

        #endregion
    }
}
