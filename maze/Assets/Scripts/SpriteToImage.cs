using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SpriteToImage
{
    public static void CaptureAndSaveImage(List<SpriteRenderer> spriteRenderers, string savePath, string fileName = "combined_image.png")
    {
        Debug.Log("Starting CaptureAndSaveImage process...");

        if (spriteRenderers == null || spriteRenderers.Count == 0)
        {
            Debug.LogError("No SpriteRenderers provided! Aborting process.");
            return;
        }
        Debug.Log($"Found {spriteRenderers.Count} SpriteRenderer(s).");

        Debug.Log(spriteRenderers.Count);
        Bounds combinedBounds = spriteRenderers[0].bounds;
        foreach (var renderer in spriteRenderers)
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }
        Debug.Log($"Combined bounds calculated: Center={combinedBounds.center}, Size={combinedBounds.size}");

        int width = Mathf.CeilToInt(combinedBounds.size.x * 100); 
        int height = Mathf.CeilToInt(combinedBounds.size.y * 100);
        Debug.Log($"Creating RenderTexture with dimensions: {width}x{height}");
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        renderTexture.Create();
        Debug.Log("RenderTexture created successfully.");

        Debug.Log("Setting up temporary camera...");
        GameObject tempCameraObject = new GameObject("TempCamera");
        Camera tempCamera = tempCameraObject.AddComponent<Camera>();
        tempCamera.orthographic = true;
        tempCamera.orthographicSize = combinedBounds.size.y / 2;
        tempCamera.transform.position = combinedBounds.center + new Vector3(0, 0, -10);
        tempCamera.clearFlags = CameraClearFlags.Color;
        tempCamera.backgroundColor = Color.clear;
        tempCamera.targetTexture = renderTexture;
        Debug.Log($"Temporary camera set up: Position={tempCamera.transform.position}, OrthographicSize={tempCamera.orthographicSize}");

        Debug.Log("Rendering Sprites with temporary camera...");
        tempCamera.Render();
        Debug.Log("Rendering completed.");

        Debug.Log("Reading pixels from RenderTexture...");
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();
        Debug.Log("Pixels read and applied to Texture2D.");

        Debug.Log("Cleaning up: Releasing RenderTexture and destroying temporary camera...");
        RenderTexture.active = null;
        renderTexture.Release();
        Object.Destroy(tempCameraObject);
        Debug.Log("Cleanup completed.");

        Debug.Log($"Checking save path: {savePath}");
        if (!Directory.Exists(savePath))
        {
            Debug.Log($"Save path does not exist. Creating directory: {savePath}");
            Directory.CreateDirectory(savePath);
        }

        Debug.Log("Encoding Texture2D to PNG and saving...");
        byte[] bytes = texture.EncodeToPNG();
        string fullPath = Path.Combine(savePath, fileName);
        File.WriteAllBytes(fullPath, bytes);
        Debug.Log($"Image saved successfully to: {fullPath}");

        Debug.Log("Destroying Texture2D...");
        Object.Destroy(texture);
        Debug.Log("Texture2D destroyed. Process completed.");
    }
}