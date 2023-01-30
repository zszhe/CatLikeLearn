//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using System;
//using System.IO;
//using System.Windows;
//using Emgu.CV;
//using Emgu.CV.Stitching;
//using Emgu.CV.Structure;
//using Emgu.CV.Util;


//public class PhotoTest2 : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }

//    Stitcher stitcher = new Stitcher(Stitcher.Mode.Panorama);
//    Mat[] mat1 = new Mat[18];
//    private static ushort width = 640;
//    private static ushort height = 480;

//    public void HandlePicture()
//    {
//        //image.Source = null;
//        DirectoryInfo directory1 = new DirectoryInfo(@"E:\PicRes11");
//        FileInfo[] filelist = directory1.GetFiles("*", SearchOption.AllDirectories);

//        for (int i = 0; i < 18; i++)
//        {
//            try
//            {
//                Image<Bgr, byte> sou = new Image<Bgr, byte>(filelist[i].FullName).Resize(width, height, Emgu.CV.CvEnum.Inter.Nearest, true);
//                mat1[i] = sou.Mat;
//            }
//            catch (Exception ex)
//            {

//            }
//        }

//        Mat Out_img_1 = new Mat();
//        VectorOfMat vectorOfMat1 = new VectorOfMat();
//        try
//        {
//            vectorOfMat1.Push(mat1);
//            var status = stitcher.Stitch(vectorOfMat1, Out_img_1);
//        }
//        catch (Exception ex)
//        {
//        }
//        finally
//        {
//            vectorOfMat1.Clear();
//            vectorOfMat1.Dispose();
//            for (int i = 0; i < mat1.Length; i++)
//            {
//                if (mat1[i] != null)
//                {
//                    mat1[i].Dispose();
//                }
//            }
//        }
//        if (Out_img_1.Bitmap != null)
//        {
//            IntPtr hBitmap = Out_img_1.Bitmap.GetHbitmap();
//            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
//                hBitmap,
//                IntPtr.Zero,
//                Int32Rect.Empty,
//                BitmapSizeOptions.FromEmptyOptions());
//            //image.Source = wpfBitmap;
//            //如果要保存合成的图片：
//            Out_img_1.Save($@"E:\PicRes1\1_{DateTime.Now.ToString("HHmmss")}.jpg");
//        }

//    }
//}
