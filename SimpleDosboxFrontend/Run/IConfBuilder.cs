using System.IO;
using SimpleDosboxFrontend.Data;
using SimpleDosboxFrontend.Services;

namespace SimpleDosboxFrontend.Run
{
    interface IConfBuilder : IService
    {
        FileInfo GetOrCreateConfFile(IProfile profile);
    }
}
