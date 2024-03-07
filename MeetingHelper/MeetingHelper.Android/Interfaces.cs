using Android.Graphics;
using Java.IO;
using MeetingHelper.Droid;
using MeetingHelper.Models;
using MeetingHelper.Services;
using Org.Opencv.Core;
using Org.Opencv.Objdetect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Path = System.IO.Path;
using Rect = Org.Opencv.Core.Rect;

[assembly: Dependency(typeof(UserContactsServiceDroid))]
[assembly: Dependency(typeof(IFileManagementDroid))]
[assembly: Dependency(typeof(SQliteDroid))]
[assembly: Dependency(typeof(IPlatformDroid))]
[assembly: Dependency(typeof(IFaceOperationsDroid))]
namespace MeetingHelper.Droid
{
    public class IFaceOperationsDroid : IFaceOperations
    {
        //private readonly Org.Opencv.Face.EigenFaceRecognizer Recognizer = Org.Opencv.Face.EigenFaceRecognizer.Create();
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
            Mat Image = Org.Opencv.Imgcodecs.Imgcodecs.Imread(ImageP);
            Org.Opencv.Imgproc.Imgproc.CvtColor(Image, Image, Org.Opencv.Imgproc.Imgproc.ColorBgr2gray);
            CascadeClassifier FaceCascade = new CascadeClassifier(CascadePath);
            MatOfRect DetectedFaces = new MatOfRect();
            FaceCascade.DetectMultiScale(Image, DetectedFaces);
            if (DetectedFaces.ToArray().Length == 0)
            {
                return false;
            }
            //Org.Opencv.Imgproc.Imgproc.Resize(Image, Image, new Org.Opencv.Core.Size(200, 200));
            //Recognizer.
            //Recognizer.Train(new List<Mat>() { Image }, new MatOfInt(PersonID));
            //Recognizer.Write(TrainingPath);
            return true;
        }
        public async Task<int> RecognizeFace(string Face, List<ValueTuple<int, string>> Faces)
        {
            Org.Opencv.Face.EigenFaceRecognizer Recognizer = Org.Opencv.Face.EigenFaceRecognizer.Create();

            List<Mat> TrainFaces = new List<Mat>();
            List<int> TrainLabels = new List<int>();
            foreach (var item in Faces)
            {
                Mat mat = Org.Opencv.Imgcodecs.Imgcodecs.Imread(item.Item2);
                Org.Opencv.Imgproc.Imgproc.CvtColor(mat, mat, Org.Opencv.Imgproc.Imgproc.ColorBgr2gray);
                Org.Opencv.Imgproc.Imgproc.Resize(mat, mat, new Org.Opencv.Core.Size(1000, 1000));
                TrainFaces.Add(mat);
                TrainLabels.Add(item.Item1);
            }

            Mat mat2 = Org.Opencv.Imgcodecs.Imgcodecs.Imread(Face);
            Org.Opencv.Imgproc.Imgproc.CvtColor(mat2, mat2, Org.Opencv.Imgproc.Imgproc.ColorBgr2gray);
            Org.Opencv.Imgproc.Imgproc.Resize(mat2, mat2, new Org.Opencv.Core.Size(1000, 1000));

            Recognizer.Train(TrainFaces, new MatOfInt(TrainLabels.ToArray()));
            return Recognizer.Predict_label(mat2);
        }


        public byte[] GetByteView(Rectangle rectangle)
        {
            return GetMemoryStreamView(rectangle).ToArray();
        }

        public MemoryStream GetMemoryStreamView(Rectangle rectangle)
        {
            var view = new Org.Opencv.Android.JavaCamera2View(MainActivity.Instance, 0);
            //view.SetCvCameraViewListener(new Org.Opencv.Android.CameraBridgeViewBase.ICvCameraViewListenerImplementor(this));
            view.CameraFrame += View_CameraFrame;
            Bitmap b = Bitmap.CreateBitmap((int)rectangle.Width, (int)rectangle.Height, Bitmap.Config.Argb8888);
            Canvas c = new Canvas(b);
            view.Layout(view.Left, view.Top, view.Right, view.Bottom);
            view.Draw(c);
            MemoryStream baos = new MemoryStream();
            b.Compress(Bitmap.CompressFormat.Webp, 100, baos);
            return baos;
        }

        private Mat View_CameraFrame(Mat p0)
        {
            Debugger.Break();
            return p0;
        }

        public IEnumerable<Rectangle> GetRects(object Mat)
        {
            MatOfRect temp = new MatOfRect();
            new CascadeClassifier(CascadePath).DetectMultiScale(Mat as Mat, temp);
            return temp.ToArray().Select(ToRectangle);
        }

        private Rectangle ToRectangle(Rect arg)
        {
            return new Rectangle(arg.X, arg.Y, arg.Width, arg.Height);
        }
    }
    public class IFileManagementDroid : IFileManagement
    {
        public Task Copy(string SourceFile, string DestFile)
        {
            return new Task(() => { });
        }

        public Task Delete(string File)
        {
            return new Task(() => { });
        }

        public Task Export()
        {
            return new Task(() => { });
        }

        public bool FileExists(string File)
        {
            return false;
        }

        public Task<Stream> GetStream(string File)
        {
            return null;
        }
    }
    public class IPlatformDroid : IPlatform
    {
        public string UWPResult()
        {
            return "";
        }
        public string AndroidResult()
        {
            return "Android";
        }
        public string iOSResult()
        {
            return "";
        }

        public string HomeDir()
        {
            return "/storage/emulated/0/";
        }
    }
    public class SQliteDroid : ISQLiteGetPath
    {
        public string GetDefault()
        {
            return System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        }

        public string GetPath()
        {
            return System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "Data.db");
        }

    }

    //public class PicturePicker : IPicturePicker
    //{
    //    public async Task<StreamPath> GetImageStreamAsync()
    //    {
    //        // Define the Intent for getting images
    //        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)//Marshmallow
    //        {
    //            await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Storage);
    //        }
    //        Intent intent = new Intent();
    //        intent.SetType("image/*");
    //        intent.SetAction(Intent.ActionGetContent);
    //        // Start the picture-picker activity (resumes in MainActivity.cs)
    //        MainActivity.Instance.StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), MainActivity.PickImageId);
    //
    //        // Save the TaskCompletionSource object as a MainActivity property
    //        MainActivity.Instance.PickImageTaskCompletionSource = new TaskCompletionSource<StreamPath>();
    //
    //        // Return Task object
    //        return await MainActivity.Instance.PickImageTaskCompletionSource.Task;
    //    }
    //}
    internal class UserContactsServiceDroid : IUserContactsService
    {
        public async Task<List<Contact>> GetAllContacts()
        {
            await Plugin.Permissions.PermissionsImplementation.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Contacts);
            if (await Plugin.Permissions.PermissionsImplementation.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Contacts) == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                IList<Plugin.ContactService.Shared.Contact> contacts = await new Plugin.ContactService.ContactServiceImplementation().GetContactListAsync();
                List<Contact> Returned = new List<Contact>();
                foreach (Plugin.ContactService.Shared.Contact item in contacts)
                {
                    try
                    {
                        Returned.Add(new Contact()
                        {
                            Id = contacts.IndexOf(item).ToString(),
                            Name = item.Name,
                            Address = "",
                            PhoneN = item.Number,
                            Image = ImageSource.FromUri(new Uri(item.PhotoUri))
                        });
                    }
                    catch (Exception)
                    {
                        Returned.Add(new Contact()
                        {
                            Id = contacts.IndexOf(item).ToString(),
                            Name = item.Name,
                            PhoneN = item.Number,
                            Image = ImageSource.FromFile(System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "MeetingHelper.Person.png"))
                        });
                    }
                }
                return Returned.OrderBy(item => item.Name).ToList();
                //var contactList = new List<Contact>();
                //var uri = ContactsContract.CommonDataKinds.Phone.ContentUri;
                //string[] projection = { ContactsContract.Contacts.InterfaceConsts.Id,
                //ContactsContract.Contacts.InterfaceConsts.DisplayName,
                //ContactsContract.CommonDataKinds.Phone.Number, ContactsContract.Contacts.Photo.InterfaceConsts.PhotoUri };
                //await Task.Run(() =>
                //{
                //    var cursor = Android.App.Application.Context.ContentResolver.Query(uri, projection, null, null, null);

                //    if (cursor.MoveToFirst())
                //    {
                //        do
                //        {
                //            var muser = new Contact();
                //            var number = cursor.GetString(cursor.GetColumnIndex(projection[2]));
                //            //var number = Regex.Replace(cursor.GetString(cursor.GetColumnIndex(projection[2])), @"[^\u0000-\u007F]+", string.Empty);
                //            //  number.Replace(" ", "").Replace("(", "").Replace(")","");

                //            muser.ID = cursor.GetString(cursor.GetColumnIndex(projection[0]));
                //            muser.Name = cursor.GetString(cursor.GetColumnIndex(projection[1]));
                //            muser.PhoneN = cursor.GetString(cursor.GetColumnIndex(projection[2]));
                //            try
                //            {
                //                muser.Image = ImageSource.FromUri(new Uri(cursor.GetString(cursor.GetColumnIndex(projection[3]))));
                //            }
                //            catch (Exception)
                //            {
                //                muser.Image = null;
                //            }
                //            if (!string.IsNullOrEmpty(number) && !contactList.Any(x => x.ID == muser.ID))
                //                contactList.Add(muser);
                //        } while (cursor.MoveToNext());
                //    }
                //});
                //return contactList.OrderBy(item => item.Name).ToList();
            }
            else
            {
                return new List<Contact>();
            }
        }


    }
}