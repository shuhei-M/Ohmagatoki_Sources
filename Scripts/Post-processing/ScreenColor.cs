/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Custom/ScreenColor")]
public sealed class ScreenColor : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensityR = new ClampedFloatParameter(0f, 0f, 1f);
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensityG = new ClampedFloatParameter(0f, 0f, 1f);
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensityB = new ClampedFloatParameter(0f, 0f, 1f);

    Material m_Material;

    public bool IsActive() => m_Material != null && (intensity.value > 0f || intensityR.value > 0f || intensityG.value > 0f || intensityB.value > 0f);

    // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    const string kShaderName = "Hidden/Shader/ScreenColor";

    public override void Setup()
    {
        if (Shader.Find(kShaderName) != null)
            m_Material = new Material(Shader.Find(kShaderName));
        else
            Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume ScreenColor is unable to load.");
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;

        m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetFloat("_IntensityR", intensityR.value);
        m_Material.SetFloat("_IntensityG", intensityG.value);
        m_Material.SetFloat("_IntensityB", intensityB.value);
        cmd.Blit(source, destination, m_Material, 0);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
