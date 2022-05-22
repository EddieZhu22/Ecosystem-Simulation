﻿//━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━																												
// Copyright 2020, Alexander Ameye, All rights reserved.
// https://alexander-ameye.gitbook.io/stylized-water/
//━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━	

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace StylizedWater
{
    class RunOnImport : AssetPostprocessor
    {
        private static string registryKeyShowWindow = "ameye.stylizedwaterforurp.showsupportwindow";
        public static bool showWindow
        {
            get { return EditorPrefs.GetBool(registryKeyShowWindow, true); }
            set { EditorPrefs.SetBool(registryKeyShowWindow, value); }
        }

        static RunOnImport() => EditorApplication.update += CheckToOpenSupportWindow;

        static void CheckToOpenSupportWindow()
        {
            if (!EditorApplication.isUpdating && showWindow)
            {
                SupportWindow.ShowWindow();
                showWindow = false;
                EditorApplication.update -= CheckToOpenSupportWindow;
            }
        }
    }

    public class SupportWindow : EditorWindow
    {
        private VisualElement root;

        private VisualElement supportTab;
        private VisualElement aboutTab;
        private VisualElement acknowledgementsTab;
        private VisualElement errorsTab;

        private VisualTreeAsset support;
        private VisualTreeAsset about;
        private VisualTreeAsset acknowledgements;
        private VisualTreeAsset errors;

        private StyleSheet styleSheet;

        static ListRequest listRequest;
        static SearchRequest urpSearchRequest;
        static AddRequest addRequest;

        static Label unityVersionLabel;
        static Label activeRendererLabel;
        static Label URPVersionLabel;
        static Label depthTextureLabel;
        static Label opaqueTextureLabel;
        static Label graphicsAPILabel;
        static Label planarReflectionsLabel;

        static VisualElement unityVersionIcon;
        static VisualElement activeRendererIcon;
        static VisualElement URPVersionIcon;
        static VisualElement depthTextureIcon;
        static VisualElement opaqueTextureIcon;
        static VisualElement graphicsAPIIcon;
        static VisualElement planarReflectionsIcon;

        static Button unityVersionFix;
        static Button URPVersionFix;
        static Button activeRendererFix;
        static Button depthTextureFix;
        static Button opaqueTextureFix;
        static Button graphicsAPIFix;
        static Button planarReflectionsFix;

        static Button aboutButton;
        static Button supportButton;
        static Button configureButton;
        static Button acknowledgementsButton;
        static Button graphicsAPIButton;
        static Button planarReflectionsButton;

        static Texture2D neutral;
        static Texture2D positive;
        static Texture2D negative;

        public enum PipelineType
        {
            Custom,
            Default,
            Lightweight,
            Universal,
            HighDefinition
        }

        [MenuItem("Tools/Stylized Water For URP/About and Support")]
        public static SupportWindow ShowWindow()
        {
            var window = GetWindow<SupportWindow>();
            window.minSize = new Vector2(400, 400);
            window.maxSize = new Vector2(400, 400);
            return window;
        }

        private void OnEnable()
        {
            titleContent.text = "Support";
            titleContent.image = EditorGUIUtility.IconContent("Settings").image;
            Init();
        }

        private void Init()
        {
            root = this.rootVisualElement;
            supportTab = new VisualElement();
            aboutTab = new VisualElement();
            errorsTab = new VisualElement();
            acknowledgementsTab = new VisualElement();

            support = GetUXML("Support");
            about = GetUXML("About");
            acknowledgements = GetUXML("Acknowledgements");
            errors = GetUXML("Find Errors");

            if (support) support.CloneTree(supportTab);
            if (about) about.CloneTree(aboutTab);
            if (acknowledgements) acknowledgements.CloneTree(acknowledgementsTab);
            if (errors) errors.CloneTree(errorsTab);

            root.Add(supportTab);
            root.Add(aboutTab);
            root.Add(acknowledgementsTab);
            root.Add(errorsTab);

            supportTab.style.display = DisplayStyle.None;
            aboutTab.style.display = DisplayStyle.None;
            errorsTab.style.display = DisplayStyle.Flex;
            acknowledgementsTab.style.display = DisplayStyle.None;

            unityVersionLabel = root.Q<Label>("Unity Version");
            activeRendererLabel = root.Q<Label>("Active Renderer");
            URPVersionLabel = root.Q<Label>("URP_Version");
            depthTextureLabel = root.Q<Label>("Depth Texture");
            opaqueTextureLabel = root.Q<Label>("Opaque Texture");
            graphicsAPILabel = root.Q<Label>("Graphics_API");
            planarReflectionsLabel = root.Q<Label>("Planar_Reflections");

            unityVersionIcon = root.Q<VisualElement>("Unity Version Icon");
            activeRendererIcon = root.Q<VisualElement>("Active Renderer Icon");
            URPVersionIcon = root.Q<VisualElement>("URP Version Icon");
            depthTextureIcon = root.Q<VisualElement>("Depth Texture Icon");
            opaqueTextureIcon = root.Q<VisualElement>("Opaque Texture Icon");
            graphicsAPIIcon = root.Q<VisualElement>("Graphics_API_Icon");
            planarReflectionsIcon = root.Q<VisualElement>("Planar_Reflections_Icon");

            unityVersionFix = root.Q<Button>("Unity Version Fix");
            activeRendererFix = root.Q<Button>("Active Renderer Fix");
            URPVersionFix = root.Q<Button>("URP_version_fix");
            depthTextureFix = root.Q<Button>("Depth Texture Fix");
            opaqueTextureFix = root.Q<Button>("Opaque Texture Fix");
            graphicsAPIFix = root.Q<Button>("Graphics_API_Fix");
            planarReflectionsFix = root.Q<Button>("Planar_Reflections_Fix");

            supportButton = root.Q<Button>("SupportButton");
            aboutButton = root.Q<Button>("AboutButton");
            acknowledgementsButton = root.Q<Button>("AcknowledgementsButton");
            configureButton = root.Q<Button>("ConfigureButton");
            planarReflectionsButton = root.Q<Button>("Planar_Reflections_Button");

            neutral = Resources.Load<Texture2D>("Icons/Neutral");
            positive = Resources.Load<Texture2D>("Icons/Positive");
            negative = Resources.Load<Texture2D>("Icons/Negative");

            root.Query<Button>().ForEach((button) =>
               {
                   button.clickable.clickedWithEventInfo += Clickedbutton;
               });

            SetUnchecked();
        }

        public static PipelineType DetectPipeline()
        {
            if (GraphicsSettings.renderPipelineAsset != null)
            {
                var type = GraphicsSettings.renderPipelineAsset.GetType().ToString();
                if (type.Contains("HDRenderPipelineAsset")) return PipelineType.HighDefinition;
                else if (type.Contains("UniversalRenderPipelineAsset")) return PipelineType.Universal;
                else if (type.Contains("LightweightRenderPipelineAsset")) return PipelineType.Lightweight;
                else return PipelineType.Custom;
            }
            return PipelineType.Default;
        }

        public static StyleSheet GetStyleSheet(string name)
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(StyleSheet)} {name}");
            if (guids.Length == 0)
                return null;
            var sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(guids[0]));
            return sheet;
        }

        public static VisualTreeAsset GetUXML(string name)
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(VisualTreeAsset)} {name}");
            if (guids.Length == 0)
                return null;
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
            return uxml;
        }

        private void Clickedbutton(EventBase tab)
        {
            Button button = tab.target as Button;
            if (button.name == "SupportButton")
            {
                supportTab.style.display = DisplayStyle.Flex;
                aboutTab.style.display = DisplayStyle.None;
                errorsTab.style.display = DisplayStyle.None;
                acknowledgementsTab.style.display = DisplayStyle.None;
            }

            else if (button.name == "AboutButton")
            {
                supportTab.style.display = DisplayStyle.None;
                aboutTab.style.display = DisplayStyle.Flex;
                errorsTab.style.display = DisplayStyle.None;
                acknowledgementsTab.style.display = DisplayStyle.None;
            }

            else if (button.name == "ConfigureButton")
            {
                supportTab.style.display = DisplayStyle.None;
                aboutTab.style.display = DisplayStyle.None;
                errorsTab.style.display = DisplayStyle.Flex;
                acknowledgementsTab.style.display = DisplayStyle.None;
            }

            else if (button.name == "AcknowledgementsButton")
            {
                supportTab.style.display = DisplayStyle.None;
                aboutTab.style.display = DisplayStyle.None;
                errorsTab.style.display = DisplayStyle.None;
                acknowledgementsTab.style.display = DisplayStyle.Flex;
            }

            else if (button.name == "Check")
            {
                SetUnchecked();
                CheckUnityVersion();
            }

            else if (button.name == "Manual")
            {
                Application.OpenURL("https://alexander-ameye.gitbook.io/stylized-water/support/troubleshooting");
            }

            else if (button.name == "Forum")
            {
                Application.OpenURL("https://forum.unity.com/threads/stylized-water-for-urp-desktop-mobile-released.846313/");
            }

            else if (button.name == "Contact")
            {
                Application.OpenURL("https://discord.gg/6QQ5JCc");
            }

            else if (button.name == "Review")
            {
                Application.OpenURL("https://assetstore.unity.com/packages/vfx/shaders/stylized-water-for-urp-162025");
            }

            else if (button.name == "Twitter")
            {
                Application.OpenURL("https://twitter.com/alexanderameye");
            }

            else if (button.name == "Website")
            {
                Application.OpenURL("https://alexanderameye.github.io/");
            }

            else if (button.name == "Unity Version Fix")
            {
                Application.OpenURL("https://unity3d.com/get-unity/download");
            }

            else if (button.name == "Active Renderer Fix")
            {
                SettingsService.OpenProjectSettings("Project/Graphics");
            }

            else if (button.name == "Shader_Variant_Limit_Fix")
            {
                EditorPrefs.SetInt("UnityEditor.ShaderGraph.VariantLimit", (int)(object)256);
                FindErrors();
            }

            else if (button.name == "Planar_Reflections_Fix")
            {
                if (Camera.main != null)
                {
                    Selection.activeGameObject = Camera.main.gameObject;
                    EditorGUIUtility.PingObject(Camera.main);
                }
            }

            else if (button.name == "Depth Texture Fix")
            {
                RenderPipelineAsset activeRenderer = GraphicsSettings.currentRenderPipeline;
                UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset pipeline = activeRenderer as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
                EditorGUIUtility.PingObject(pipeline);
            }

            else if (button.name == "Opaque Texture Fix")
            {
                RenderPipelineAsset activeRenderer = GraphicsSettings.currentRenderPipeline;
                UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset pipeline = activeRenderer as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
                EditorGUIUtility.PingObject(pipeline);
            }

            else if (button.name == "KeenanWoodall")
            {
                Application.OpenURL("https://twitter.com/keenanwoodall");
            }

            else if (button.name == "JoshSauter")
            {
                Application.OpenURL("https://github.com/JoshSauter");
            }
        }

        private void SetUnchecked()
        {
            unityVersionLabel.text = "Untested";
            activeRendererLabel.text = "Untested";
            URPVersionLabel.text = "Untested";
            depthTextureLabel.text = "Untested";
            opaqueTextureLabel.text = "Untested";
            graphicsAPILabel.text = "Untested";
            planarReflectionsLabel.text = "Untested";

            unityVersionIcon.style.backgroundImage = Background.FromTexture2D(neutral);
            activeRendererIcon.style.backgroundImage = Background.FromTexture2D(neutral);
            URPVersionIcon.style.backgroundImage = Background.FromTexture2D(neutral);
            depthTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);
            opaqueTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);
            graphicsAPIIcon.style.backgroundImage = Background.FromTexture2D(neutral);
            planarReflectionsIcon.style.backgroundImage = Background.FromTexture2D(neutral);

            unityVersionFix.style.visibility = Visibility.Hidden;
            URPVersionFix.style.visibility = Visibility.Hidden;
            activeRendererFix.style.visibility = Visibility.Hidden;
            depthTextureFix.style.visibility = Visibility.Hidden;
            opaqueTextureFix.style.visibility = Visibility.Hidden;
            graphicsAPIFix.style.visibility = Visibility.Hidden;
            planarReflectionsFix.style.visibility = Visibility.Hidden;
        }

        private void CheckUnityVersion()
        {
            string unityVersion = Application.unityVersion;
            unityVersionLabel.text = unityVersion;

            if (unityVersion.StartsWith("2019.3") || unityVersion.StartsWith("2019.4") || unityVersion.StartsWith("2020.1"))
            {
                unityVersionIcon.style.backgroundImage = Background.FromTexture2D(positive);
                CheckSRPVersion();
            }

            else if (unityVersion.StartsWith("2020.2") || unityVersion.StartsWith("2020.3") || unityVersion.StartsWith("2020.4") || unityVersion.StartsWith("2021.1") || unityVersion.StartsWith("2021.2"))
            {
                unityVersionIcon.style.backgroundImage = Background.FromTexture2D(neutral);
            }

            else
            {
                unityVersionIcon.style.backgroundImage = Background.FromTexture2D(negative);
                unityVersionFix.style.visibility = Visibility.Visible;
            }
        }

        private void CheckSRPVersion()
        {
            URPVersionLabel.text = "Testing ...";
            URPVersionIcon.style.backgroundImage = Background.FromTexture2D(neutral);
            listRequest = Client.List();
            urpSearchRequest = Client.Search("com.unity.render-pipelines.universal");
            EditorApplication.update += FindSRPVersion;
        }

        static void FindErrors()
        {
            activeRendererIcon.style.backgroundImage = Background.FromTexture2D(neutral);
            depthTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);
            opaqueTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);

            int planarReflectionComponents = 0;
            PlanarReflections[] planarReflections = FindObjectsOfType<PlanarReflections>();
            foreach (PlanarReflections planarReflection in planarReflections) if (planarReflection.enabled) planarReflectionComponents += 1;

            if (planarReflectionComponents == 1)
            {
                planarReflectionsLabel.text = "Enabled";
                planarReflectionsIcon.style.backgroundImage = Background.FromTexture2D(positive);
            }

            else if (planarReflectionComponents > 1)
            {
                planarReflectionsLabel.text = "Multiple";
                planarReflectionsIcon.style.backgroundImage = Background.FromTexture2D(negative);
            }

            else if (planarReflectionComponents == 0)
            {
                planarReflectionsLabel.text = "Disabled";
                planarReflectionsIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                if (Camera.main != null) planarReflectionsFix.style.visibility = Visibility.Visible;
            }

            switch (SystemInfo.graphicsDeviceType)
            {
                case GraphicsDeviceType.Vulkan:
                    graphicsAPILabel.text = "Vulkan";
                    graphicsAPIIcon.style.backgroundImage = Background.FromTexture2D(positive);
                    break;
                case GraphicsDeviceType.Direct3D11:
                    graphicsAPILabel.text = "Direct3D11";
                    graphicsAPIIcon.style.backgroundImage = Background.FromTexture2D(positive);
                    break;
                case GraphicsDeviceType.OpenGLES3:
                    graphicsAPILabel.text = "OpenGLES3";
                    graphicsAPIIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                    break;
                case GraphicsDeviceType.Direct3D12:
                    graphicsAPILabel.text = "Direct3D12";
                    graphicsAPIIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                    break;
                case GraphicsDeviceType.OpenGLES2:
                    graphicsAPILabel.text = "OpenGLES2";
                    graphicsAPIIcon.style.backgroundImage = Background.FromTexture2D(negative);
                    break;
                case GraphicsDeviceType.Null:
                    graphicsAPILabel.text = "Null";
                    graphicsAPIIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                    break;
                case GraphicsDeviceType.PlayStation4:
                    graphicsAPILabel.text = "PlayStation4";
                    graphicsAPIIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                    break;
                case GraphicsDeviceType.XboxOne:
                    graphicsAPILabel.text = "XboxOne";
                    graphicsAPIIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                    break;
                case GraphicsDeviceType.Metal:
                    graphicsAPILabel.text = "Metal";
                    graphicsAPIIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                    break;
                case GraphicsDeviceType.OpenGLCore:
                    graphicsAPILabel.text = "OpenGLCore";
                    graphicsAPIIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                    break;
                case GraphicsDeviceType.XboxOneD3D12:
                    graphicsAPILabel.text = "XboxOneD3D12";
                    graphicsAPIIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                    break;
                case GraphicsDeviceType.Switch:
                    graphicsAPILabel.text = "Switch";
                    graphicsAPIIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                    break;
            }

            switch (DetectPipeline())
            {
                case PipelineType.Custom:
                    activeRendererLabel.text = "Custom";
                    depthTextureLabel.text = "Not tested";
                    opaqueTextureLabel.text = "Not tested";

                    activeRendererIcon.style.backgroundImage = Background.FromTexture2D(negative);
                    depthTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                    opaqueTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);

                    depthTextureFix.style.visibility = Visibility.Hidden;
                    activeRendererFix.style.visibility = Visibility.Hidden;
                    opaqueTextureFix.style.visibility = Visibility.Hidden;
                    break;
                case PipelineType.Default:
                    activeRendererLabel.text = "Default";
                    depthTextureLabel.text = "Not tested";
                    opaqueTextureLabel.text = "Not tested";

                    activeRendererIcon.style.backgroundImage = Background.FromTexture2D(negative);
                    depthTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                    opaqueTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);

                    depthTextureFix.style.visibility = Visibility.Hidden;
                    activeRendererFix.style.visibility = Visibility.Hidden;
                    opaqueTextureFix.style.visibility = Visibility.Hidden;
                    break;
                case PipelineType.Lightweight:
                    activeRendererLabel.text = "Lightweight";
                    depthTextureLabel.text = "Not tested";
                    opaqueTextureLabel.text = "Not tested";

                    activeRendererIcon.style.backgroundImage = Background.FromTexture2D(negative);
                    depthTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                    opaqueTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);

                    depthTextureFix.style.visibility = Visibility.Hidden;
                    activeRendererFix.style.visibility = Visibility.Hidden;
                    opaqueTextureFix.style.visibility = Visibility.Hidden;
                    break;
                case PipelineType.Universal:
#if UNIVERSAL_RENDERER

                    UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset pipeline = GraphicsSettings.currentRenderPipeline as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;

#if UNIVERSAL_731 // GetRenderer() was introduced in 7.3.1
                    if (pipeline.GetRenderer(0).GetType().ToString().Contains("Renderer2D"))
                    {
                        activeRendererLabel.text = "2D Renderer";
                        activeRendererIcon.style.backgroundImage = Background.FromTexture2D(negative);
                        activeRendererFix.style.visibility = Visibility.Visible;
                    }

                    else
                    {
                        activeRendererLabel.text = "Universal";
                        activeRendererIcon.style.backgroundImage = Background.FromTexture2D(positive);
                        activeRendererFix.style.visibility = Visibility.Hidden;
                    }
#else
                    activeRendererLabel.text = "Universal";
                    activeRendererIcon.style.backgroundImage = Background.FromTexture2D(positive);
                    activeRendererFix.style.visibility = Visibility.Hidden;
#endif

                    if (pipeline.supportsCameraDepthTexture)
                    {
                        depthTextureLabel.text = "Enabled";
                        depthTextureIcon.style.backgroundImage = Background.FromTexture2D(positive);
                        depthTextureFix.style.visibility = Visibility.Hidden;
                    }

                    else
                    {
                        depthTextureLabel.text = "Disabled";
                        depthTextureIcon.style.backgroundImage = Background.FromTexture2D(negative);
                        depthTextureFix.style.visibility = Visibility.Visible;
                    }

                    if (pipeline.supportsCameraOpaqueTexture)
                    {
                        opaqueTextureLabel.text = "Enabled";
                        opaqueTextureIcon.style.backgroundImage = Background.FromTexture2D(positive);
                        opaqueTextureFix.style.visibility = Visibility.Hidden;
                    }

                    else
                    {
                        opaqueTextureLabel.text = "Disabled";
                        opaqueTextureIcon.style.backgroundImage = Background.FromTexture2D(negative);
                        opaqueTextureFix.style.visibility = Visibility.Visible;
                    }
#endif
                    break;
                case PipelineType.HighDefinition:
                    activeRendererLabel.text = "High Definition";
                    depthTextureLabel.text = "Not tested";
                    opaqueTextureLabel.text = "Not tested";

                    activeRendererIcon.style.backgroundImage = Background.FromTexture2D(negative);
                    depthTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                    opaqueTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);

                    activeRendererFix.style.visibility = Visibility.Hidden;
                    depthTextureFix.style.visibility = Visibility.Hidden;
                    opaqueTextureFix.style.visibility = Visibility.Hidden;
                    break;
            }
        }

        static void FindSRPVersion()
        {
#if UNIVERSAL_RENDERER
            if (listRequest.IsCompleted)
            {
                if (listRequest.Status == StatusCode.Success)
                {
                    foreach (var package in listRequest.Result)
                    {
                        if (package.name == "com.unity.render-pipelines.universal")
                        {
                            var version = package.version;
                            URPVersionLabel.text = version;

                            if (version.StartsWith("7.3") || version.StartsWith("7.4") ||
                                version.StartsWith("7.5") || version.StartsWith("7.6") || version.StartsWith("8") || version.StartsWith("9") ||
                                version.StartsWith("10") || version.StartsWith("11") || version.StartsWith("12"))
                            {
                                URPVersionIcon.style.backgroundImage = Background.FromTexture2D(positive);
                                FindErrors();
                                break;
                            }
                            else
                            {
                                URPVersionIcon.style.backgroundImage = Background.FromTexture2D(negative);
                                break;
                            }
                        }
                    }
                }
                else if (listRequest.Status >= StatusCode.Failure)
                    Debug.Log(listRequest.Error.message);

                EditorApplication.update -= FindSRPVersion;
            }
#else
            URPVersionLabel.text = "Not Installed";
            URPVersionIcon.style.backgroundImage = Background.FromTexture2D(negative);
#endif
        }
    }
}
