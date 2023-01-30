using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;
using System.IO;
using System.Runtime.InteropServices;
//using Emgu.CV;
//using Emgu.CV.Stitching;
//using Emgu.CV.Structure;
//using Emgu.CV.Util;
//using Uk.Org.Adcock.Parallel;

public class Utils
{
    /// <summary>
    /// Mat转Texture2D
    /// </summary>
    /// <param name = "mat" ></ param >
    /// < returns ></ returns >
    public static Texture2D MatToTexture2D(Mat mat)
    {
        Texture2D t2d = new Texture2D(mat.Width, mat.Height);
        t2d.LoadImage(mat.ToBytes());
        t2d.Apply();
        //赋值完后为什么要Apply
        //因为在贴图更改像素时并不是直接对显存进行更改，而是在另外一个内存空间中更改，这时候GPU还会实时读取旧的贴图位置。
        //当Apply后，CPU会告诉GPU你要换个地方读贴图了。
        return t2d;
        //Get the height and width of the Mat
        //int imgHeight = mat.Height;
        //int imgWidth = mat.Width;

        //byte[] matData = new byte[imgHeight * imgWidth];

        ////Get the byte array and store in matData
        //mat.GetArray(0, 0, matData);
        ////Create the Color array that will hold the pixels
        //Color32[] c = new Color32[imgHeight * imgWidth];

        ////Get the pixel data from parallel loop
        //Parallel.For(0, imgHeight, i =>
        //{
        //    for (var j = 0; j < imgWidth; j++)
        //    {
        //        byte vec = matData[j + i * imgWidth];
        //        var color32 = new Color32
        //        {
        //            r = vec,
        //            g = vec,
        //            b = vec,
        //            a = 0
        //        };
        //        c[j + i * imgWidth] = color32;
        //    }
        //});

        ////Create Texture from the result
        //Texture2D tex = new Texture2D(imgWidth, imgHeight, TextureFormat.RGBA32, true, true);
        //tex.SetPixels32(c);
        //tex.Apply();
    }

    public static Mat Texture2DToMad(Texture2D tex)
    {
        Mat mat = Mat.FromImageData(tex.EncodeToPNG());
        return mat;
    }
}

public class PhotoTest1 : MonoBehaviour
{
    [DllImport("Dll1", EntryPoint = "?Initialize@Functions@UsingOpenCV@@SAXXZ")]
    private static extern void Initialize();

    [DllImport("Dll1", EntryPoint = "?ProcessFrame@Functions@UsingOpenCV@@SAPEAEXZ")]
    private static extern byte[] ProcessFrame();

    public List<RawImage> rawImages;

    public int index = 0;

    public Camera cam;

    public RenderTexture rt;

    public List<Texture2D> images = new List<Texture2D>();

    public int width, height;

    public float dampAngle;

    public float lastAngleY;

    public bool ison = false;

    public int testAspect;

    public RawImage result;
    // Start is called before the first frame update
    void Start()
    {
        width = Screen.width;
        height = Screen.height;

        if (cam)
        {
            float cameraHeight = 2.0f * cam.nearClipPlane * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);//视锥体高度
            var cameraWidth = cameraHeight * cam.aspect;//视锥体宽度
            var horizontalfov = 2 * Mathf.Atan(cameraWidth * 0.5f / cam.nearClipPlane) * Mathf.Rad2Deg;//水平FOV
            dampAngle = horizontalfov;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            CreatePhoto();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CombineTexture();
        }

        if (!ison)
        {
            return;
        }

        if (index >= 3)
        {
            EndPhoto(true);
        }

        if (Mathf.Abs(cam.transform.eulerAngles.y - lastAngleY) >= dampAngle)
        {
            lastAngleY += dampAngle;
            CreatePhoto();
        }
    }

    public void ReadyTakePhoto()
    {
        images.Clear();
        lastAngleY = cam.transform.eulerAngles.y;
        ison = true;
        index = 0;
        testAspect = 0;
    }

    public void TakeShot()
    {
        if (!ison)
        {
            ReadyTakePhoto();
            CreatePhoto();
        }
    }

    public void CreatePhoto()
    {
        StartCoroutine(DoTakeScreenShot());
    }

    public void EndPhoto(bool isMax)
    {
        var angle = Mathf.Abs(cam.transform.eulerAngles.y - lastAngleY);
        var aspect = angle / dampAngle;

        testAspect = Mathf.FloorToInt(width * aspect);
        if (!isMax)
        {
            CreatePhoto();
        }

        // 后续处理
        CombineTexture();
    }

    IEnumerable<Mat> GenerateImages()
    {
        foreach (var image in images)
        {
            yield return Utils.Texture2DToMad(image);
        }
    }

    public void CombineTexture()
    {
        Texture2D res;
        IEnumerable<Mat> mats = GenerateImages();
        //Mat[] mats = new Mat[images.Count];
        //for (int i = 0; i < images.Count; i++)
        //{
        //    mats[i] = Utils.Texture2DToMad(images[i]);
        //}

        //using (var stitcher = new Stitcher(Stitcher.Mode.Panorama))
        using (var stitcher = Stitcher.Create(Stitcher.Mode.Panorama))
        //using (var stitcher = Stitcher.Create(true))
        using (var pano = new Mat())
        {
            var status = stitcher.Stitch(mats, pano);
            if (status != Stitcher.Status.OK)
            {
                Debug.LogError("失败：" + status.ToString());
                return;
            }
            else
            {
                stitcher.Dispose(); //处理掉
                res = Utils.MatToTexture2D(pano);
            }
        }

        result.GetComponent<RectTransform>().sizeDelta = new Vector2(res.width, res.height);
        result.texture = res;

        //int newWidth = 0;
        //int newHeight = height;
        //foreach(var image in images)
        //{
        //    newWidth += image.width;
        //}

        //Texture2D tex = new Texture2D(newWidth, newHeight, TextureFormat.ARGB32, false, true);
        //for (int i = 0; i < images.Count; i++)
        //{
        //    Color32[] color = images[i].GetPixels32(0);
        //    tex.SetPixels32(width * i, 0, images[i].width, images[i].height, color);
        //}
        //tex.Apply();

        //res.GetComponent<RectTransform>().sizeDelta = new Vector2(tex.width, tex.height);
        //res.texture = tex;
    }

    //public void EndPhoto()
    //{

    //    EndPhoto(aspect);
    //}

    public IEnumerator DoTakeScreenShot()
    {
        yield return new WaitForEndOfFrame();

        Texture2D tex = new Texture2D(width - testAspect, height, TextureFormat.ARGB32, false, true);
        tex.ReadPixels(new UnityEngine.Rect(testAspect, 0, tex.width, tex.height), 0, 0, false);
        tex.Apply();

        images.Add(tex);
        RawImage temp = rawImages[index++];
        temp.GetComponent<RectTransform>().sizeDelta = new Vector2(tex.width, tex.height);
        temp.texture = tex;
    }

    public void OnStitch()
    {

    }

    public void GetTexture2D()
    {
        //for (int i = 0; i < images.Count; i++)
        //{
        //    byte[] dataBytes = images[i].EncodeToPNG();
        //    string strSaveFile = Application.streamingAssetsPath + "/texture/rt_" + System.DateTime.Now.Minute + "_" + System.DateTime.Now.Second + "_" + i + ".jpg";
        //    FileStream fs = File.Open(strSaveFile, FileMode.OpenOrCreate);
        //    fs.Write(dataBytes, 0, dataBytes.Length);
        //    fs.Flush();
        //    fs.Close();
        //}
        // ChangeTexture2D();
    }

    public void ChangeTexture2D()
    {
        //List<byte[]> bytes = new List<byte[]>();
        //for (int i = 0; i < images.Count; i++)
        //{
        //    byte[] dataBytes = images[i].EncodeToPNG();
        //    bytes.Add(dataBytes);
        //}

        //byte[][] by = bytes.ToArray();
        byte[] vytes = ProcessFrame();
    }
}
