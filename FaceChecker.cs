using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlookInvitePrevention
{
    static class FaceChecker
    {
        public static bool GuessIfIAmInFrontOfMyLaptop()
        {
            try
            {
                string photoFile = ShootPhoto();
                return CompareWithMe(photoFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Horrible failure= " + ex);
                return false;
            }
        }


        private static string ShootPhoto()
        {
            string fileName = string.Format(@"D:\Temporal\{0}.png", DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss"));
            using (VideoCapture capture = new VideoCapture(0))
            {
                //capture.Set(CaptureProperty.XI_LedMode, 0);
                //capture.Set(CaptureProperty.Settings, 1);

                capture.Open(0);
                if (capture.IsOpened())
                {
                    Mat frame = new Mat();
                    capture.Read(frame);
                    var image = BitmapConverter.ToBitmap(frame);
                    image.Save(fileName, ImageFormat.Png);
                }
            }
            return fileName;
        }


        private static bool CompareWithMe(string photoFile)
        {
            // look for indexed face, but only the bigger face on the photo
            float similarityThreshold = 90F;
            String collectionId = "InvitePreventCollection";

            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient();
            Image image = new Image { Bytes = new MemoryStream(File.ReadAllBytes(photoFile)) };

            SearchFacesByImageRequest searchFacesByImageRequest = new SearchFacesByImageRequest()
            {
                CollectionId = collectionId,
                Image = image,
                FaceMatchThreshold = similarityThreshold,
                MaxFaces = 2
            };

            SearchFacesByImageResponse searchFacesByImageResponse = rekognitionClient.SearchFacesByImage(searchFacesByImageRequest);
            if (searchFacesByImageResponse.FaceMatches?.Count > 0) return true;

            // if not indexed, then compare my photo with all the faces
            string myPhotoFile = @"D:\Usuarios\johamepl\Downloads\bluepages.png";

            Image myImage = new Image { Bytes = new MemoryStream(File.ReadAllBytes(myPhotoFile)) };

            CompareFacesRequest compareFacesRequest = new CompareFacesRequest()
            {
                SourceImage = myImage,
                TargetImage = image,
                SimilarityThreshold = similarityThreshold
            };

            CompareFacesResponse compareFacesResponse = rekognitionClient.CompareFaces(compareFacesRequest);
            if (compareFacesResponse.FaceMatches?.Count > 0) return true;

            // not myself
            return false;
        }
    }
}
