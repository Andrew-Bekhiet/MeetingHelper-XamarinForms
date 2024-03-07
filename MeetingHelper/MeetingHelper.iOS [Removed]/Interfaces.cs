using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AddressBook;
using Contacts;
using Foundation;
using MeetingHelper.iOS;
using MeetingHelper.Models;
using MeetingHelper.Services;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Dependency(typeof(UserContactServiceiOS))]
[assembly: Dependency(typeof(PicturePicker))]
[assembly: Dependency(typeof(SQliteiOS))]
[assembly: Dependency(typeof(IPlatformiOS))]
[assembly: Dependency(typeof(IImageExtractoriOS))]
namespace MeetingHelper.iOS
{
    public class IImageExtractoriOS : IImageExtractor
    {
        public async System.Threading.Tasks.Task ExtractAll(ImageSource image, string ImageName, System.IO.Stream s)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = s.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                File.WriteAllBytes(System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), ImageName), ((MemoryStream)ms).ToArray());
            }
            //UIImage photo = await new StreamImagesourceHandler().LoadImageAsync(image);
            //Byte[] Bytes;
            //using (NSData imageData = photo.AsPNG())
            //{
            //    Bytes = new Byte[imageData.Length];
            //    System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, Bytes, 0, Convert.ToInt32(imageData.Length));
            //}
            //File.WriteAllBytes(System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), ImageName), Bytes);
        }
    }
    public class IPlatformiOS : IPlatform
    {
        public string UWPResult()
        {
            return "";
        }
        public string AndroidResult()
        {
            return "";
        }
        public string iOSResult()
        {
            return "iOS";
        }
    }
    public class SQliteiOS : ISQLiteGetPath
    {
        public string GetPath()
        {
            return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "Data.db");
        }
    }
    public class PicturePicker : IPicturePicker
    {
        TaskCompletionSource<StreamPath> taskCompletionSource;
        UIImagePickerController imagePicker;

        public async Task<StreamPath> GetImageStreamAsync()
        {
            // Create and define UIImagePickerController
            imagePicker = new UIImagePickerController
            {
                SourceType = UIImagePickerControllerSourceType.PhotoLibrary,
                MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary)
            };
            await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Storage);
            // Set event handlers
            imagePicker.FinishedPickingMedia += OnImagePickerFinishedPickingMedia;
            imagePicker.Canceled += OnImagePickerCancelled;

            // Present UIImagePickerController;
            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            var viewController = window.RootViewController;
            viewController.PresentModalViewController(imagePicker, true);

            // Return Task object
            taskCompletionSource = new TaskCompletionSource<StreamPath>();
            return await taskCompletionSource.Task;
        }
        void OnImagePickerFinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs args)
        {
            UIImage image = args.EditedImage ?? args.OriginalImage;

            if (image != null)
            {
                // Convert UIImage to .NET Stream object
                NSData data = image.AsJPEG(1);
                StreamPath stream = new StreamPath() { Stream = data.AsStream(), Path = args.ImageUrl.Path };

                UnregisterEventHandlers();

                // Set the Stream as the completion of the Task
                taskCompletionSource.SetResult(stream);
            }
            else
            {
                UnregisterEventHandlers();
                taskCompletionSource.SetResult(null);
            }
            imagePicker.DismissModalViewController(true);
        }

        void OnImagePickerCancelled(object sender, EventArgs args)
        {
            UnregisterEventHandlers();
            taskCompletionSource.SetResult(null);
            imagePicker.DismissModalViewController(true);
        }

        void UnregisterEventHandlers()
        {
            imagePicker.FinishedPickingMedia -= OnImagePickerFinishedPickingMedia;
            imagePicker.Canceled -= OnImagePickerCancelled;
        }
    }
    public class UserContactServiceiOS : IUserContactsService
    {
        public async Task<List<Contact>> GetAllContacts()
        {
            await Plugin.Permissions.PermissionsImplementation.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Contacts);
            if (await Plugin.Permissions.PermissionsImplementation.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Contacts) == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                IList<Plugin.ContactService.Shared.Contact> contacts = await Plugin.ContactService.CrossContactService.Current.GetContactListAsync();
                List<Contact> Returned = new List<Contact>();
                foreach (Plugin.ContactService.Shared.Contact item in contacts)
                {
                    try
                    {
                        Returned.Add(new Contact()
                        {
                            ID = contacts.IndexOf(item).ToString(),
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
                            ID = contacts.IndexOf(item).ToString(),
                            Name = item.Name,
                            PhoneN = item.Number,
                            Image = ImageSource.FromFile(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "MeetingHelper.Person.png"))
                        });
                    }
                }
                return Returned.OrderBy(item => item.Name).ToList();
            }
            else
            {
                return new List<Contact>();
            }
        //public CNContact[] contactList { get; set; }
        //    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Contacts);
        //    if (status != PermissionStatus.Granted)
        //    {
        //        var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Contacts);
        //        status = results[Permission.Contacts];
        //    }
        //    if (status != PermissionStatus.Granted)
        //    {
        //        return new List<Contact>();
        //    }
        //    var ab = new ABAddressBook().GetPeople();

        //    var contacts = new List<Contact>();
        //    foreach (var item in ab)
        //    {
        //        var numbers = item.GetPhones();
        //        if (numbers != null)
        //        {
        //            foreach (var item2 in numbers)
        //            {
        //                contacts.Add(new Contact
        //                {
        //                    Name = item.FirstName + item.MiddleName + item.LastName,
        //                    ID = item.Id.ToString(),
        //                    Image = ImageSource.FromStream(() => item.Image.AsStream()),
        //                    PhoneN = item2.Value
        //                });
        //            }
        //        }
        //        else
        //        {
        //            contacts.Add(new Contact
        //            {
        //                Name = item.FirstName + item.MiddleName + item.LastName,
        //                ID = item.Id.ToString(),
        //                Image = ImageSource.FromStream(() => item.Image.AsStream())
        //            });
        //        }

        //    }
        }
    }
}