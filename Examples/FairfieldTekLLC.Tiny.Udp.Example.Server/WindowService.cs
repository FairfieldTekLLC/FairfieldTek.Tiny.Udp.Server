// // Fairfield Tek L.L.C.
// // Copyright (c) 2016, Fairfield Tek L.L.C.
// // 
// // 
// // THIS SOFTWARE IS PROVIDED BY WINTERLEAF ENTERTAINMENT LLC ''AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
// //  INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// // PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL WINTERLEAF ENTERTAINMENT LLC BE LIABLE FOR ANY DIRECT, INDIRECT, 
// // INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// // SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
// // ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR 
// // OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH 
// // DAMAGE. 

using System.ServiceProcess;
using FairfieldTekLLC.Tiny.Udp.Example.Server.SystemService.Framework;
using FairfieldTekLLC.Tiny.Udp.Example.Server.Tiny;
using FairfieldTekLLC.Tiny.Udp.Server.BaseClass;

namespace FairfieldTekLLC.Tiny.Udp.Example.Server
{
    [WindowsService("Winterleaf.ChatServer.UDP.Host.Service", DisplayName = "Winterleaf.ChatServer.UDP.Host.Service",
         Description = "Winterleaf.ChatServer.UDP.Host.Service", EventLogSource = "Winterleaf.ChatServer.UDP.Host.Service",
         StartMode = ServiceStartMode.Automatic)]
    internal class WindowService : IWindowsService
    {
        private readonly int _port = 30000;
        private UdpServerBase _server;

        public void Dispose()
        {
            _server.Dispose();
        }

        public void OnContinue()
        {
        }

        public void OnPause()
        {
        }

        public void OnShutdown()
        {
        }

        public void OnStart(string[] args)
        {
            _server = new UdpServer(_port, 5000);
            _server.StartServer();
        }

        public void OnStop()
        {
            _server.StopServer();
        }
    }
}