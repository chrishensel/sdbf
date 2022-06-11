using SimpleDosboxFrontend.Data;
using SimpleDosboxFrontend.Services;

namespace SimpleDosboxFrontend.Run
{
    interface IRunService : IService
    {
        void Run(IProfile profile);
    }
}