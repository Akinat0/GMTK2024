
using UnityEngine;

public static class Utility
{
    public static Texture2D ToTexture2D(
        RenderTexture renderTexture,
        Texture2D targetTexture = null,
        TextureFormat? format = null
    )
    {
        if (targetTexture == null)
            targetTexture = new Texture2D(renderTexture.width, renderTexture.height, format ?? TextureFormat.ARGB32, false);

        int width = renderTexture.width;
        int height = renderTexture.height;

        targetTexture.Reinitialize(width, height, format ?? targetTexture.format, targetTexture.mipmapCount > 1);

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = renderTexture;

        targetTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        RenderTexture.active = prev;

        targetTexture.Apply(false, false);

        return targetTexture;
    }
}
