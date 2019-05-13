using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// This script implements the blur effect and also applies the bloom filter to get the entire effect

// The blur effect is implemented with progressive upsampling/downsampling on the entire scene view,
// i.e. the scene view is divided into texels and merged together similar to the matrix kernel method

// Based on the tutorial by Catlike Coding (https://catlikecoding.com/unity/tutorials/advanced-rendering/bloom/)

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class BloomEffect : MonoBehaviour
{
    //--- Sliders to control bloom effect variables ---//
    [Range(1, 16)]
	public int iterations = 1;

    [Range(0, 10)]
	public float threshold = 1;

    [Range(0, 1)]
	public float softThreshold = 0.5f;

    [Range(0, 10)]
	public float intensity = 1;
    //-------//
    
    public Shader bloomShader;

    [NonSerialized]
	Material bloom;

    RenderTexture[] textures = new RenderTexture[16];

    // Constants to control which shader pass we want to use
    const int BoxDownPrefilterPass = 0;
    const int BoxDownPass = 1;
	const int BoxUpPass = 2;
    const int ApplyBloomPass = 3;

    void OnRenderImage (RenderTexture source, RenderTexture destination) 
    {
        // Make sure we have the bloom material with the appropriate flags
        if (bloom == null) {
			bloom = new Material(bloomShader);
			bloom.hideFlags = HideFlags.HideAndDontSave;
		}

        // Calculate the threshold for soft and hard filtering
        float knee = threshold * softThreshold;
		Vector4 filter;
		filter.x = threshold;
		filter.y = filter.x - knee;
		filter.z = 2f * knee;
		filter.w = 0.25f / (knee + 0.00001f);
		bloom.SetVector("_Filter", filter);
        bloom.SetFloat("_Intensity", Mathf.GammaToLinearSpace(intensity));

        // Split the screen into four parts to downsample
        int width = source.width / 2;
		int height = source.height / 2;
		RenderTextureFormat format = source.format;

        // Grab a temporary render texture with no depth buffer (third argument)
        RenderTexture currentDestination = textures[0] = RenderTexture.GetTemporary(width, height, 0, format);

        // Blit from the source texture to the temporary
		Graphics.Blit(source, currentDestination, bloom, BoxDownPrefilterPass);

        RenderTexture currentSource = currentDestination;

        // Progressive Downsampling
        int i = 1;
        for (; i < iterations; i++) 
        {
			width /= 2;
			height /= 2;
            if (height < 2) 
            {
				break;
			}
			currentDestination = textures[i] = RenderTexture.GetTemporary(width, height, 0, format);
			Graphics.Blit(currentSource, currentDestination, bloom, BoxDownPass);
			currentSource = currentDestination;
		}

        // Progressive Upsampling
        for (i -= 2; i >= 0; i--) 
        {
			currentDestination = textures[i];
			textures[i] = null;
			Graphics.Blit(currentSource, currentDestination, bloom, BoxUpPass);
			RenderTexture.ReleaseTemporary(currentSource);
			currentSource = currentDestination;
		}

        // Blit from the temporary texture to the screen and apply the bloom shader
        bloom.SetTexture("_SourceTex", source);
		Graphics.Blit(currentSource, destination, bloom, ApplyBloomPass);

        // Get rid of the temporary when we are done
        RenderTexture.ReleaseTemporary(currentSource);
	}
}
