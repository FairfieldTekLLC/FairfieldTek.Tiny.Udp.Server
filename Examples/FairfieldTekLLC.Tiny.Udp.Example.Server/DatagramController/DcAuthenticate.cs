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

using FairfieldTekLLC.Tiny.Udp.Example.Server.Datagram;
using FairfieldTekLLC.Tiny.Udp.Example.Server.Tiny;
using FairfieldTekLLC.Tiny.Udp.Server.BaseClass;
using FairfieldTekLLC.Tiny.Udp.Server.Common.Common;

namespace FairfieldTekLLC.Tiny.Udp.Example.Server.DatagramController
{
    class DcAuthenticate : DatagramControllerBase
    {
        private readonly Authenticate _auth = new Authenticate();

        public DcAuthenticate(UdpServerBase server) : base(server, 1, "Authenticate")
        {
        }

        public override void OnProcessNewData(ConnectionBase connection, StreamProcessor streamProcessor)
        {
            var conn = connection as Connection;
            var server = Server as UdpServer;

            if (conn == null || server == null)
                return;
            _auth.Unpack(streamProcessor);

            if (_auth.Handle == null || _auth.Handle.Trim() == string.Empty)
                return;

            var handle = _auth.Handle.Trim();

            if (!server.IsHandleUsed(handle))
            {
                connection.Handle = handle;
                _auth.Handle = handle;
                conn.IsAuthorized = true;
            }
            else
                _auth.Handle = string.Empty;
            connection.SendData(_auth);
        }

        public override void OnStartController()
        {
            Server.LogMessage("Datagram Controller for Authenticate started...");
        }
    }
}