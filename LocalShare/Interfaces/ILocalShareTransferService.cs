using LocalShare.Models;
using System.Threading.Tasks;

namespace LocalShare.Interfaces
{
    public interface ILocalShareTransferService
    {
        //Task SendToClient(TcpClientModel client, string[] selectedPath, bool isDirectory);

        Task SendFilesToClient(TcpClientModel client, string[] selectedPath);

        Task SendFolderToClient(TcpClientModel client, string[] selectedPath);
    }
}
