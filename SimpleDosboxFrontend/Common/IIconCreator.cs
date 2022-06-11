using System.Drawing;
using SimpleDosboxFrontend.Data;
using SimpleDosboxFrontend.Services;

namespace SimpleDosboxFrontend.Common
{
    interface IIconCreator : IService
    {
        Image CreateGenericImage(IProfile profile);
    }
}
