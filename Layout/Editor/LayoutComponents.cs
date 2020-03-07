using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.Video;

namespace AltSalt.Maestro.Layout
{
    public class LayoutComponents : ModuleWindow
    {
        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);
            
            UpdateDisplay();
            ControlPanel.selectionChangedDelegate += UpdateDisplay;
            
            return this;
        }
        
        
        void OnDestroy()
        {
            ControlPanel.selectionChangedDelegate -= UpdateDisplay;
        }
        
        static GameObject createdGameObject;

        private bool selectCreatedObject => controlPanel.objectCreation.selectCreatedObject;
        
        private string objectName => controlPanel.objectCreation.objectName;
        
        VisualElementToggleData toggleData = new VisualElementToggleData();


        private enum EnableCondition
        {
            GameObjectSelected,
            RectTransformSelected,
            TextSelected,
            CameraSelected,
            SpriteSelected,
            ImageSelected,
            VideoPlayerSelected,
            MeshRendererSelected,
        }

        private enum ButtonNames
        {
            RelativePage,
            RelativeElementController,
            AutoPosition,
            AutoScale,
            AutoScaleDimensions,
            AutoWidthHeight,
            Scale,
            WidthHeight,
            Touchable,
            Draggable,
            TextSize,
            AutoViewport,
            Viewport,
            SlicedSpriteSize,
            TextExtensions,
            RectTransformExtensions,
            SpriteExtensions,
            ImageExtensions,
            VideoPlayerUtils,
            MeshRendererSorter,
            MeshRendererUpdater,
            DrawBoundingBox,
            DrawTextBox
        }

        private void UpdateDisplay()
        {
            if (Selection.gameObjects.Length > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.GameObjectSelected, true);
            }
            else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.GameObjectSelected, false);
            }
            
            if (Utils.TargetComponentSelected(Selection.gameObjects, typeof(RectTransform)) == true) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.RectTransformSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.RectTransformSelected, false);
            }
            
            if (Utils.TargetComponentSelected(Selection.gameObjects, typeof(TMP_Text)) == true) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TextSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TextSelected, false);
            }
            
            if (Utils.TargetComponentSelected(Selection.gameObjects, typeof(Camera)) == true) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.CameraSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.CameraSelected, false);
            }
            
            if (Utils.TargetComponentSelected(Selection.gameObjects, typeof(SpriteRenderer)) == true) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SpriteSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SpriteSelected, false);
            }
            
            if (Utils.TargetComponentSelected(Selection.gameObjects, typeof(UnityEngine.UI.Image)) == true) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ImageSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ImageSelected, false);
            }
            
            if (Utils.TargetComponentSelected(Selection.gameObjects, typeof(VideoPlayer)) == true) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.VideoPlayerSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.VideoPlayerSelected, false);
            }
            
            if (Utils.TargetComponentSelected(Selection.gameObjects, typeof(MeshRenderer)) == true) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.MeshRendererSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.MeshRendererSelected, false);
            }
        }

        private Button SetupButton(Button button)
        {
            switch (button.name) {
                
                case nameof(ButtonNames.RelativePage):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(RelativePage));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;
                
                case nameof(ButtonNames.RelativeElementController):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(RelativeElementController));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected, button);
                    break;
                
                case nameof(ButtonNames.AutoPosition):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(ResponsiveAutoPosition));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;
                
                case nameof(ButtonNames.AutoScale):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(ResponsiveAutoScale));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;
                
                case nameof(ButtonNames.AutoScaleDimensions):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(ResponsiveAutoScaleDimensions));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;
                
                case nameof(ButtonNames.AutoWidthHeight):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(ResponsiveAutoWidthHeight));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;
                
                case nameof(ButtonNames.Scale):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(ResponsiveScale));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;
                
                case nameof(ButtonNames.WidthHeight):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(ResponsiveWidthHeight));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;
                
                case nameof(ButtonNames.Touchable):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(Touchable));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected, button);
                    break;
                
                case nameof(ButtonNames.TextSize):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(ResponsiveTextSize));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.TextSelected, button);
                    break;
                
                case nameof(ButtonNames.AutoViewport):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(ResponsiveAutoViewport));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.CameraSelected, button);
                    break;
                
                case nameof(ButtonNames.Viewport):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(ResponsiveViewport));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.CameraSelected, button);
                    break;
                
                case nameof(ButtonNames.SlicedSpriteSize):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(ResponsiveSlicedSpriteSize));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.SpriteSelected, button);
                    break;
                
                case nameof(ButtonNames.TextExtensions):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(TextExtensions));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.TextSelected, button);
                    break;
                
                case nameof(ButtonNames.RectTransformExtensions):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(RectTransformExtensions));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;
                
                case nameof(ButtonNames.SpriteExtensions):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(SpriteExtensions));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.SpriteSelected, button);
                    break;
                
                case nameof(ButtonNames.ImageExtensions):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(ImageExtensions));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ImageSelected, button);
                    break;
                
                case nameof(ButtonNames.VideoPlayerUtils):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(VideoPlayerUtils));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.VideoPlayerSelected, button);
                    break;
                
                case nameof(ButtonNames.MeshRendererSorter):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(MeshRendererSorter));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.MeshRendererSelected, button);
                    break;
                
                case nameof(ButtonNames.MeshRendererUpdater):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(MeshRendererUpdater));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.MeshRendererSelected, button);
                    break;
                
                case nameof(ButtonNames.DrawBoundingBox):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(DrawBoundingBox));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;
                
                case nameof(ButtonNames.DrawTextBox):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(DrawTextBox));
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.TextSelected, button);
                    break;
  
            }

            return button;
        }

    }
}
