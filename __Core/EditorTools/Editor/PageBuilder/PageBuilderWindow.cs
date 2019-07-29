﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace AltSalt
{
    public class PageBuilderWindow : EditorWindow
    {
        public delegate void SelectionChangedDelegate();
        public SelectionChangedDelegate selectionChangedDelegate;

        Dictionary<Type, string> childWindowData = new Dictionary<Type, string> {
            { typeof(CreateCoreElements), "create-core-elements" },
            { typeof(CreateSequenceElements), "create-sequence-elements" },
            { typeof(EditRectTransformComponent), "edit-rect-transform-component" },
            { typeof(EditTMProComponent), "edit-tm-pro-component" },
            { typeof(EditTMProColorClip), "edit-tm-pro-color-clip" },
            { typeof(EditRectTransformPosClip), "edit-rect-transform-pos-clip" },
            { typeof(EditSpriteColorClip), "edit-sprite-color-clip" },
            { typeof(EditRectTransformScaleClip), "edit-rect-transform-scale-clip" },
            { typeof(EditRectTransformRotationClip), "edit-rect-transform-rotation-clip" },
            { typeof(EditFloatVarClip), "edit-float-var-clip" },
            { typeof(EditColorVarClip), "edit-color-var-clip" },
        };

        static List<ChildUIElementsWindow> childWindows = new List<ChildUIElementsWindow>();

        enum ButtonNames
        {
            RefreshLayout
        }

        [MenuItem("Tools/AltSalt/Page Builder")]
        public static void ShowWindow()
        {
            var window = GetWindow<PageBuilderWindow>();
        }

        static void Init()
        {
            EditorWindow.GetWindow(typeof(PageBuilderWindow)).Show();
        }

        void OnEnable()
        {
            RenderLayout();
            Selection.selectionChanged += SelectionChangedCallback;
        }

        void OnDestroy()
        {
            Selection.selectionChanged -= SelectionChangedCallback;
        }

        void SelectionChangedCallback()
        {
            selectionChangedDelegate.Invoke();
        }

        void RenderLayout()
        {
            Selection.objects = null;
            foreach (ChildUIElementsWindow childWindow in childWindows) {
                DestroyImmediate(childWindow);
            }

            rootVisualElement.Clear();
            AssetDatabase.Refresh();

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Utils.GetStylesheetPath());
            rootVisualElement.styleSheets.Add(styleSheet);

            // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
            var pageBuilderTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_AltSalt/__Core/EditorTools/Editor/PageBuilder/PageBuilderWindow_UXML.uxml");
            VisualElement pageBuilderStructure = pageBuilderTree.CloneTree();
            rootVisualElement.Add(pageBuilderStructure);

            foreach(KeyValuePair<Type, string> childWindow in childWindowData) {
                childWindows.Add(EditorToolsCore.CreateAndBindChildWindow(childWindow.Key, this, childWindow.Value));
            }

            var buttons = rootVisualElement.Query<Button>();
            buttons.ForEach(SetupButton);
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.RefreshLayout):
                    button.clickable.clicked += () => {
                        RenderLayout();
                    };
                    break;
            }

            return button;
        }

    }
}
