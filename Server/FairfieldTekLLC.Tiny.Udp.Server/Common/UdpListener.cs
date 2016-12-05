//  Fairfield Tek L.L.C.
//  Copyright (c) 2016, Fairfield Tek L.L.C.
//  
//  
// THIS SOFTWARE IS PROVIDED BY WINTERLEAF ENTERTAINMENT LLC ''AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL WINTERLEAF ENTERTAINMENT LLC BE LIABLE FOR ANY DIRECT, INDIRECT, 
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR 
// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH 
// DAMAGE. 
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FairfieldTekLLC.Tiny.Udp.Server.Common
{
    /// <summary>
    ///     Internal class used for handing Udp Sockets
    /// </summary>
    internal sealed class UdpListener : IDisposable
    {
        public UdpListener()
        {
            Client = new UdpClient();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="port"></param>
        public UdpListener(int port) : this(new IPEndPoint(IPAddress.Any, port))
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="endpoint"></param>
        public UdpListener(IPEndPoint endpoint)
        {
            Client = new UdpClient(endpoint);
        }

        private UdpClient Client { get; set; }

        /// <summary>
        ///     Disposal
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Disposal
        ///     The bulk of the clean-up code is implemented in Dispose(bool)
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            try
            {
                Client?.Close();
            }
            catch (Exception)
            {
                // ignored
            }
            Client = null;
        }

        ~UdpListener()
        {
            // Finalizer calls Dispose(false)  
            Dispose(false);
        }

        public async Task<ReceivedData> Receive()
        {
            UdpReceiveResult result = await Client.ReceiveAsync();
            return new ReceivedData(result.RemoteEndPoint, result.Buffer);
        }

        /// <summary>
        ///     Sends bytes to the Udp Socket
        /// </summary>
        /// <param name="data"></param>
        /// <param name="endpoint"></param>
        public void Send(IList<byte> data, IPEndPoint endpoint)
        {
            Client.SendAsync(data.ToArray(), data.Count, endpoint);
        }
    }
}