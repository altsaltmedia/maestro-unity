using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Animation
{
    public class PopulateAnimationClips : PopulateClips
    {
        public override PlayableAsset PopulateClip(PlayableDirector targetDirector, TrackAsset parentTrack, EasingFunction.Ease easeType, TimelineClip timelineClip)
        {
            UnityEngine.Object sourceObject = null;
            foreach (PlayableBinding playableBinding in parentTrack.outputs) {
                sourceObject = targetDirector.GetGenericBinding(playableBinding.sourceObject);
            }

            switch (timelineClip.asset.GetType().Name) {

                case nameof(TMProColorClip): {
                        TMProColorClip asset = timelineClip.asset as TMProColorClip;
                        TMP_Text component = sourceObject as TMP_Text;
                        if (component != null) {
                            asset.template.initialValue = component.color;
                            asset.template.targetValue = component.color;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(RectTransformPosClip): {
                        RectTransformPosClip asset = timelineClip.asset as RectTransformPosClip;
                        RectTransform component = sourceObject as RectTransform;
                        if (component != null) {
                            asset.template.initialValue = component.anchoredPosition3D;
                            asset.template.targetValue = component.anchoredPosition3D;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(SpriteColorClip): {
                        SpriteColorClip asset = timelineClip.asset as SpriteColorClip;
                        SpriteRenderer component = sourceObject as SpriteRenderer;
                        if (component != null) {
                            asset.template.initialValue = component.color;
                            asset.template.targetValue = component.color;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(RectTransformScaleClip): {
                        RectTransformScaleClip asset = timelineClip.asset as RectTransformScaleClip;
                        RectTransform component = sourceObject as RectTransform;
                        if (component != null) {
                            asset.template.initialValue = component.localScale;
                            asset.template.targetValue = component.localScale;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(RectTransformRotationClip): {
                        RectTransformRotationClip asset = timelineClip.asset as RectTransformRotationClip;
                        RectTransform component = sourceObject as RectTransform;
                        if (component != null) {
                            asset.template.initialValue = component.localEulerAngles;
                            asset.template.targetValue = component.localEulerAngles;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(LerpFloatVarClip): {
                        LerpFloatVarClip asset = timelineClip.asset as LerpFloatVarClip;
                        FloatVariable component = sourceObject as FloatVariable;
                        if (component != null) {
                            asset.template.initialValue = component.value;
                            asset.template.targetValue = component.value;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(LerpColorVarClip): {
                        LerpColorVarClip asset = timelineClip.asset as LerpColorVarClip;
                        ColorVariable component = sourceObject as ColorVariable;
                        if (component != null) {
                            asset.template.initialValue = component.value;
                            asset.template.targetValue = component.value;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(TMProCharSpacingClip): {
                        TMProCharSpacingClip asset = timelineClip.asset as TMProCharSpacingClip;
                        TMP_Text component = sourceObject as TMP_Text;
                        if (component != null) {
                            asset.template.initialValue = component.characterSpacing;
                            asset.template.targetValue = component.characterSpacing;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(ResponsiveVector3Clip): {
                        ResponsiveVector3Clip asset = timelineClip.asset as ResponsiveVector3Clip;
                        RectTransform component = sourceObject as RectTransform;
                        if (component != null) {

                            if(parentTrack is ResponsiveRectTransformPosTrack) {
                                asset.template.breakpointInitialValue.Add(component.anchoredPosition3D);
                                asset.template.breakpointTargetValue.Add(component.anchoredPosition3D);
                            } else if (parentTrack is ResponsiveRectTransformScaleTrack) {
                                asset.template.breakpointInitialValue.Add(component.localScale);
                                asset.template.breakpointTargetValue.Add(component.localScale);
                            }
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }
            }

            return null;
        }
    }
}