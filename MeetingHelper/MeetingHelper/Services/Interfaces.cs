using MeetingHelper.Models;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MeetingHelper.Services
{
    public interface IPlatform
    {
        string UWPResult(); 
        string iOSResult();
        string AndroidResult();
        string HomeDir();
    }
    public interface ISQLiteGetPath
    {
        string GetPath();
        string GetDefault();
    }
    public interface IUserContactsService
    {
        Task<List<Contact>> GetAllContacts();
    }
    public interface IFileManagement
    {
        Task Copy(string SourceFile, string DestFile);
        Task Delete(string File);
        Task<Stream> GetStream(string File);
        bool FileExists(string File);
        Task Export();
    }
    public interface IFaceOperations
    {
        Task<bool> DetectAndWriteToFile(string ImageP, int PersonID);
        Task<int> RecognizeFace(string Face, List<ValueTuple<int, string>> Faces);
        IEnumerable<Rectangle> GetRects(object Mat);
        byte[] GetByteView(Rectangle rectangle);
        MemoryStream GetMemoryStreamView(Rectangle rectangle);
    }
}