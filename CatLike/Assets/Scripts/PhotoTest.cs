using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class PhotoTest : MonoBehaviour
{
    Camera cam;
    RenderTexture cubemap;
    RenderTexture equirect;
    [Header("生成次数  true为连续生成")] [SerializeField] private bool ison;
    void Start()
    {
        cam = Camera.main;
        cubemap = new RenderTexture(4096, 4096, 32);
        cubemap.dimension = TextureDimension.Cube;
        equirect = new RenderTexture(4096, 2048, 32);
        StartCoroutine(B());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ison = !ison;
        }

        if (ison)
        {
            StartCoroutine(B());
        }
    }

    IEnumerator B()
    {
        if (ison)
        {
            while (true)
            {
                Creat();
                yield return new WaitForSecondsRealtime(0.04F);
            }
        }
        else
        {
            Creat();
        }


        yield return null;
    }


    public void Creat()
    {
        cam.RenderToCubemap(cubemap, 63, Camera.MonoOrStereoscopicEye.Mono);
        cubemap.ConvertToEquirect(equirect, Camera.MonoOrStereoscopicEye.Mono);
        RenderTexture.active = equirect;
        Texture2D tex = new Texture2D(equirect.width, equirect.height, TextureFormat.ARGB32, false, true);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        RenderTexture.active = null;
        GL.Clear(true, true, Color.black);
        tex.Apply();
        byte[] bytes = tex.EncodeToTGA();
        CreateDirectroryOfFile(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\全景图\\)");

        System.IO.File.WriteAllBytes(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop)
            + "\\全景图\\" + System.DateTime.Now.Ticks + ".tga", bytes);
    }
    public static void CreateDirectroryOfFile(string filePath)
    {
        Debug.Log($"CreateDirectrory {filePath}[folder_path],");
        if (!string.IsNullOrEmpty(filePath))
        {
            string dir_name = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir_name))
            {
                Debug.Log($"No Exists {dir_name}[dir_name],");
                Directory.CreateDirectory(dir_name);
            }
            else
            {
                Debug.Log($"Exists {dir_name}[dir_name],");
            }
        }
    }
}
