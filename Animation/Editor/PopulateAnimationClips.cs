using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

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

            switch (parentTrack.GetType().Name) {

                case nameof(TMProColorTrack): {
                        TMProColorClip asset = timelineClip.asset as TMProColorClip;
                        TMP_Text component = sourceObject as TMP_Text;
                        if (component != null) {
                            asset.template.initialValue = component.color;
                            asset.template.targetValue = component.color;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(RectTransformPosTrack): {
                        ResponsiveVector3Clip asset = timelineClip.asset as ResponsiveVector3Clip;
                        RectTransform component = sourceObject as RectTransform;
                        if (component != null) {
                            asset.template.breakpointInitialValue.Add(component.anchoredPosition3D);
                            asset.template.breakpointTargetValue.Add(component.anchoredPosition3D);
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(SpriteColorTrack): {
                        ColorClip asset = timelineClip.asset as ColorClip;
                        SpriteRenderer component = sourceObject as SpriteRenderer;
                        if (component != null) {
                            asset.template.initialValue = component.color;
                            asset.template.targetValue = component.color;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }
                
                case nameof(ImageUIColorTrack): {
                    ColorClip asset = timelineClip.asset as ColorClip;
                    UnityEngine.UI.Image component = sourceObject as UnityEngine.UI.Image;
                    if (component != null) {
                        asset.template.initialValue = component.color;
                        asset.template.targetValue = component.color;
                    }
                    asset.template.ease = easeType;
                    return asset;
                }

                case nameof(RectTransformScaleTrack): {
                        ResponsiveVector3Clip asset = timelineClip.asset as ResponsiveVector3Clip;
                        RectTransform component = sourceObject as RectTransform;
                        if (component != null) {
                            asset.template.breakpointInitialValue.Add(component.localScale);
                            asset.template.breakpointTargetValue.Add(component.localScale);
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(RectTransformRotationTrack): {
                        ResponsiveVector3Clip asset = timelineClip.asset as ResponsiveVector3Clip;
                        RectTransform component = sourceObject as RectTransform;
                        if (component != null) {
                            asset.template.breakpointInitialValue.Add(component.localEulerAngles);
                            asset.template.breakpointTargetValue.Add(component.localEulerAngles);
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }
                
                case nameof(TransformPosTrack): {
                    ResponsiveVector3Clip asset = timelineClip.asset as ResponsiveVector3Clip;
                    Transform component = sourceObject as Transform;
                    if (component != null) {
                        asset.template.breakpointInitialValue.Add(component.localPosition);
                        asset.template.breakpointTargetValue.Add(component.localPosition);
                    }
                    asset.template.ease = easeType;
                    return asset;
                }
                
                case nameof(TransformRotationTrack): {
                    ResponsiveVector3Clip asset = timelineClip.asset as ResponsiveVector3Clip;
                    Transform component = sourceObject as Transform;
                    if (component != null) {
                        asset.template.breakpointInitialValue.Add(component.localEulerAngles);
                        asset.template.breakpointTargetValue.Add(component.localEulerAngles);
                    }
                    asset.template.ease = easeType;
                    return asset;
                }
                
                case nameof(ManualVideoPlayerTimeTrack): {
                    ManualVideoPlayerTimeClip asset = timelineClip.asset as ManualVideoPlayerTimeClip;
                    ManualVideoPlayer component = sourceObject as ManualVideoPlayer;
                    if (component != null) {
                        asset.template.initialValueIOS = (float)component.masterTime;
                        asset.template.targetValueIOS = (float)component.masterTime;
                        asset.template.initialValueAndroid = (float)component.masterTime;
                        asset.template.targetValueAndroid = (float)component.masterTime;
                    }
                    asset.template.ease = easeType;
                    return asset;
                }

                case nameof(LerpFloatVarTrack): {
                        FloatClip asset = timelineClip.asset as FloatClip;
                        FloatVariable component = sourceObject as FloatVariable;
                        if (component != null) {
                            asset.template.initialValue = component.value;
                            asset.template.targetValue = component.value;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(LerpColorVarTrack): {
                        ColorClip asset = timelineClip.asset as ColorClip;
                        ColorVariable component = sourceObject as ColorVariable;
                        if (component != null) {
                            asset.template.initialValue = component.value;
                            asset.template.targetValue = component.value;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(TMProCharSpacingTrack): {
                        ResponsiveFloatClip asset = timelineClip.asset as ResponsiveFloatClip;
                        TMP_Text component = sourceObject as TMP_Text;
                        if (component != null) {
                            asset.template.breakpointInitialValue.Add(component.characterSpacing);
                            asset.template.breakpointTargetValue.Add(component.characterSpacing);
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                default:
                {
                    //Debug.Log("Track type not recognized");
                    return null;
                }
            }
        }
    }
}