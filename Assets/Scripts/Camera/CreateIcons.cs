using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateIcons : MonoBehaviour
{
    public Camera cam;
    int height = 1024;
    int width = 1024;
    int depth = 24;


    private void Start() 
    {
        cam = GetComponent<Camera>();
    }
    //method to render from camera
    public Sprite CaptureScreen()
    {
        RenderTexture renderTexture = new RenderTexture(width, height, depth);
        Rect rect = new Rect(0, 0, width, height);
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        cam.targetTexture = renderTexture;
        cam.Render();

        RenderTexture currentRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();

        cam.targetTexture = null;
        RenderTexture.active = currentRenderTexture;
        Destroy(renderTexture);

        Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);

        return sprite;
    }
}
