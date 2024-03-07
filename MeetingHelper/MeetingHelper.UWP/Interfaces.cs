using MeetingHelper.Models;
using MeetingHelper.Services;
using MeetingHelper.UWP;
using OpenCvSharp;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Xamarin.Forms;

[assembly: Dependency(typeof(UserContactsServiceUWP))]
[assembly: Dependency(typeof(SQliteUWP))]
[assembly: Dependency(typeof(IFaceOperationsUWP))]
[assembly: Dependency(typeof(IPlatformUWP))]
[assembly: Dependency(typeof(FileManageUWP))]
namespace MeetingHelper.UWP
{

    public class IFaceOperationsUWP : IFaceOperations
    {
        //OpenCvSharp.Face.EigenFaceRecognizer Recognizer = OpenCvSharp.Face.EigenFaceRecognizer.Create();
        //string TrainingPath = Path.Combine(DependencyService.Get<ISQLiteGetPath>().GetDefault(), "MeetingHelper.Faces.yml");
        //string CascadePath = Path.Combine(DependencyService.Get<ISQLiteGetPath>().GetDefault(), "MeetingHelper.OpenCV.haarcascade_frontalface_alt.xml");
        //public IFaceOperationsUWP()
        //{
        //    Recognizer.Read(TrainingPath);
        //}
        //public bool DetectAndWriteToFile(string ImageP, int PersonID)
        //{
        //    Mat Image = new Mat(ImageP);
        //    CascadeClassifier FaceCascade = new CascadeClassifier(CascadePath);
        //    if (FaceCascade.DetectMultiScale(Image).Length == 0)
        //    {
        //        return false;
        //    }
        //    Recognizer.Train(new List<Mat>() { Image }, new List<int>() { PersonID });
        //    Recognizer.Write(TrainingPath);
        //    return true;
        //}
        //
        //public IEnumerable<Rectangle> GetRects(object Mat)
        //{
        //    return new CascadeClassifier(CascadePath).DetectMultiScale(Mat as Mat).Select(ToRectangle);
        //}
        //private Rectangle ToRectangle(Rect arg)
        //{
        //    return new Rectangle(arg.X, arg.Y, arg.Width, arg.Height);
        //}
        //public int RecognizeFace(object Face)
        //{
        //    return Recognizer.Predict((Mat)Face);
        //}
        //public int FrameGrabber(IDictionary<int, string> PresonsAndIDs, in bool whil)
        //{
        //    //Here |
        //    //     V
        //    try
        //    {
        //        OpenCvSharp.Face.LBPHFaceRecognizer Recognizer = OpenCvSharp.Face.LBPHFaceRecognizer.Create();
        //        Recognizer.Read(Path.Combine(System.IO.Path.GetDirectoryName(DependencyService.Get<ISQLiteGetPath>().GetPath()), "MeetingHelper.Faces.yml"));
        //
        //        CascadeClassifier FaceCascade = new CascadeClassifier(Path.Combine(Path.GetDirectoryName(DependencyService.Get<ISQLiteGetPath>().GetPath()), "MeetingHelper.haarcascade_frontalface_alt.xml"));
        //
        //        OpenCvSharp.VideoCapture cam = new OpenCvSharp.VideoCapture(CaptureDevice.Any);
        //        cam.Set(CaptureProperty.FrameWidth, 640);
        //        cam.Set(CaptureProperty.FrameHeight, 480);
        //        cam.Grab();
        //
        //        double minW = 0.1 * cam.Get(3);
        //        double minH = 0.1 * cam.Get(4);
        //        while (whil)
        //        {
        //            Mat ret = cam.RetrieveMat();
        //            Mat img = cam.RetrieveMat();
        //            Mat ProcImg = cam.RetrieveMat();
        //            Cv2.Flip(ProcImg, ProcImg, FlipMode.XY); // Flip vertically
        //
        //            Mat gray = new Mat();
        //            Cv2.CvtColor(ProcImg, gray, ColorConversionCodes.BGR2GRAY);
        //
        //            Rect[] faces = FaceCascade.DetectMultiScale(gray, 1.2, 5, (HaarDetectionType)0, new OpenCvSharp.Size(minW, minH));
        //
        //            foreach (Rect Rectangle in faces)
        //            {
        //                Cv2.Rectangle(img, new OpenCvSharp.Point(Rectangle.X, Rectangle.Y),
        //                    new OpenCvSharp.Point(Rectangle.X + Rectangle.Width, Rectangle.Y + Rectangle.Height),
        //                    new Scalar(0, 255, 0), 2);
        //                int PID = Recognizer.Predict(gray[Rectangle]);
        //                if (PID != -1)
        //                {
        //                    cam.Release();
        //                    Cv2.DestroyAllWindows();
        //                    return PID;
        //                }
        //                else
        //                {
        //                    continue;
        //                }
        //            }
        //            Cv2.ImShow("camera", img);
        //
        //            int k = Cv2.WaitKey(10) & 0xff; // Press 'ESC' for exiting video
        //            if (k == 27)
        //            {
        //                break;
        //            }
        //        }
        //        cam.Release();
        //        Cv2.DestroyAllWindows();
        //        return -1;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //        Debug.WriteLine(ex.StackTrace);
        //        return -1;
        //    }
        //}
        private readonly string CascadePath = Path.Combine(DependencyService.Get<ISQLiteGetPath>().GetDefault(), "MeetingHelper.OpenCV.haarcascade_frontalface_alt.xml");
        //public IFaceOperationsDroid()
        //{
        //    if (System.IO.File.Exists(TrainingPath))
        //    {
        //        Recognizer.Read(TrainingPath);
        //    }
        //}
        public async Task<bool> DetectAndWriteToFile(string ImageP, int PersonID)
        {
            Mat Image = OpenCvSharp.Cv2.ImRead(ImageP);
            OpenCvSharp.Cv2.CvtColor(Image, Image, OpenCvSharp.ColorConversionCodes.BGR2GRAY);
            CascadeClassifier FaceCascade = new CascadeClassifier(CascadePath);
            if (FaceCascade.DetectMultiScale(Image).Length == 0)
            {
                return false;
            }
            //OpenCvSharp.Cv2.Resize(Image, Image, new OpenCvSharp.Core.Size(200, 200));
            //Recognizer.
            //Recognizer.Train(new List<Mat>() { Image }, new MatOfInt(PersonID));
            //Recognizer.Write(TrainingPath);
            return true;
        }
        public async Task<int> RecognizeFace(string Face, List<ValueTuple<int, string>> Faces)
        {
            OpenCvSharp.Face.EigenFaceRecognizer Recognizer = OpenCvSharp.Face.EigenFaceRecognizer.Create();

            List<Mat> TrainFaces = new List<Mat>();
            List<int> TrainLabels = new List<int>();
            foreach (var item in Faces)
            {
                Mat mat = OpenCvSharp.Cv2.ImRead(item.Item2);
                OpenCvSharp.Cv2.CvtColor(mat, mat, OpenCvSharp.ColorConversionCodes.BGR2GRAY);
                OpenCvSharp.Cv2.Resize(mat, mat, new OpenCvSharp.Size(1000, 1000));
                TrainFaces.Add(mat);
                TrainLabels.Add(item.Item1);
            }

            Mat mat2 = OpenCvSharp.Cv2.ImRead(Face);
            OpenCvSharp.Cv2.CvtColor(mat2, mat2, OpenCvSharp.ColorConversionCodes.BGR2GRAY);
            OpenCvSharp.Cv2.Resize(mat2, mat2, new OpenCvSharp.Size(1000, 1000));

            Recognizer.Train(TrainFaces, new MatOfInt(TrainLabels.ToArray()));
            return Recognizer.Predict(mat2);
        }


        public byte[] GetByteView(Rectangle rectangle)
        {
            return GetMemoryStreamView(rectangle).ToArray();
        }

        public MemoryStream GetMemoryStreamView(Rectangle rectangle)
        {
            //var view = new OpenCvSharp.Android.JavaCamera2View(MainActivity.Instance, 0);
            ////view.SetCvCameraViewListener(new OpenCvSharp.Android.CameraBridgeViewBase.ICvCameraViewListenerImplementor(this));
            //view.CameraFrame += View_CameraFrame;
            //Bitmap b = Bitmap.CreateBitmap((int)rectangle.Width, (int)rectangle.Height, Bitmap.Config.Argb8888);
            //Canvas c = new Canvas(b);
            //view.Layout(view.Left, view.Top, view.Right, view.Bottom);
            //view.Draw(c);
            //MemoryStream baos = new MemoryStream();
            //b.Compress(Bitmap.CompressFormat.Webp, 100, baos);
            //return baos;
            OpenCvSharp.Cv2.CreateFrameSource_Camera(0).NextFrame();
            throw new NotSupportedException("Not Supported on this platform");
        }

        private Mat View_CameraFrame(Mat p0)
        {
            Debugger.Break();
            return p0;
        }

        public IEnumerable<Rectangle> GetRects(object Mat)
        {
            return new CascadeClassifier(CascadePath).DetectMultiScale(Mat as Mat).Select(ToRectangle);
        }

        private Rectangle ToRectangle(Rect arg)
        {
            return new Rectangle(arg.X, arg.Y, arg.Width, arg.Height);
        }
    }
    public class IPlatformUWP : IPlatform
    {
        public string UWPResult()
        {
            return "UWP";
        }
        public string iOSResult()
        {
            return "";
        }
        public string AndroidResult()
        {
            return "";
        }

        public string HomeDir()
        {
            return ApplicationData.Current.LocalFolder.Path;
        }
    }
    public class SQliteUWP : ISQLiteGetPath
    {
        public string GetDefault()
        {
            return ApplicationData.Current.LocalFolder.Path;
        }

        public string GetPath()
        {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data.db");
        }
    }
    public class FileManageUWP : IFileManagement
    {
        public async Task Copy(string SourceFile, string DestFile)
        {
            StorageFile SFile = await StorageFile.GetFileFromPathAsync(SourceFile);
            StorageFolder DFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(DestFile));
            await SFile.CopyAsync(DFolder, Path.GetFileName(DestFile));
        }
        public async Task Delete(string File)
        {
            StorageFile SFile = await StorageFile.GetFileFromPathAsync(File);
            await SFile.DeleteAsync();
        }

        public async Task Export()
        {
            StorageFolder AppF = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalFolder.Path);
            IReadOnlyList<StorageFile> Files = await AppF.GetFilesAsync();
            await AppF.CreateFolderAsync("MHD");
            foreach (var item in Files)
            {
                await item.CopyAsync(await AppF.GetFolderAsync("MHD"));
            }
            System.IO.Compression.ZipFile.CreateFromDirectory(Path.Combine(ApplicationData.Current.LocalFolder.Path, "MHD"), Path.Combine(ApplicationData.Current.LocalFolder.Path, "مساعد الاجتماع.mhd"), System.IO.Compression.CompressionLevel.Fastest, false);
            StorageFile File = await StorageFile.GetFileFromPathAsync(Path.Combine(ApplicationData.Current.LocalFolder.Path, "مساعد الاجتماع.mhd"));
            await File.CopyAsync(Windows.Storage.KnownFolders.DocumentsLibrary);
            await File.DeleteAsync();
            await (await AppF.GetFolderAsync("MHD")).DeleteAsync();
        }

        public bool FileExists(string File)
        {
            return new FileInfo(File).Exists;
        }

        public async Task<Stream> GetStream(string File)
        {
            StorageFile SFile = await StorageFile.GetFileFromPathAsync(File);
            return (await SFile.OpenReadAsync()).AsStreamForRead();
        }
    }
    //public class PicturePicker : IPicturePicker
    //{
    //    public async Task<StreamPath> GetImageStreamAsync()
    //    {
    //        await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Storage);
    //        if (await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage) == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
    //        {
    //            StreamPath Result = new StreamPath();
    //            // Create and initialize the FileOpenPicker
    //            FileOpenPicker openPicker = new FileOpenPicker
    //            {
    //                ViewMode = PickerViewMode.Thumbnail,
    //                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
    //            };
    //
    //            openPicker.FileTypeFilter.Add(".jpg");
    //            openPicker.FileTypeFilter.Add(".jpeg");
    //            openPicker.FileTypeFilter.Add(".png");
    //            openPicker.FileTypeFilter.Add(".gif");
    //            openPicker.FileTypeFilter.Add(".bmp");
    //
    //            // Get a file and return a Stream
    //            StorageFile storageFile = await openPicker.PickSingleFileAsync();
    //
    //            if (storageFile == null)
    //            {
    //                return null;
    //            }
    //            Result.Path = storageFile.Path;
    //
    //            IRandomAccessStreamWithContentType raStream = await storageFile.OpenReadAsync();
    //            Result.Stream = raStream.AsStream();
    //            return Result;
    //        }
    //        else
    //        {
    //            return null;
    //        }
    //    }
    //
    //}
    public class UserContactsServiceUWP : IUserContactsService
    {

        public async Task<List<Contact>> GetAllContacts()
        {
            try
            {
                Windows.ApplicationModel.Contacts.ContactPicker contactPicker = new Windows.ApplicationModel.Contacts.ContactPicker();
                Windows.ApplicationModel.Contacts.Contact Picked = await contactPicker.PickContactAsync();
                Contact Contact = new Contact
                {
                    Id = Picked.Id,
                    Name = Picked.FullName,
                    PhoneN = Picked.Phones.ElementAt(0).Number,
                    Address = Picked.Addresses.ElementAt(0).PostalCode + ", " + Picked.Addresses.ElementAt(0).StreetAddress + ", " + Picked.Addresses.ElementAt(0).Locality + ", " + Picked.Addresses.ElementAt(0).Region + ", " + Picked.Addresses.ElementAt(0).Country
                };
                List<Contact> returned = new List<Contact>();
                try
                {
                    IRandomAccessStreamWithContentType stream = await Picked.SourceDisplayPicture.OpenReadAsync();
                    using (StreamReader sr = new StreamReader(stream.AsStream()))
                    {
                        Contact.Image = ImageSource.FromStream(() => sr.BaseStream);
                    }
                }
                catch (Exception ex)
                {
                    Contact.Image = ImageSource.FromFile(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "MeetingHelper.Person.png"));
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
                returned.Add(Contact);
                return returned;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Debug.WriteLine(ex.StackTrace);
                return new List<Contact>();
            }
        }
    }
}
