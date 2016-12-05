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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using FairfieldTekLLC.Tiny.Udp.Server.Common.Common;

namespace FairfieldTekLLC.Tiny.Udp.Server.BaseClass
{
    [ComVisible(false)]
    [DebuggerDisplay("Name = {PacketHandlerTypeName}")]
    public abstract class DatagramControllerBase
    {
        /// <summary>
        ///     Stream processor used to handle the data.
        /// </summary>
        private readonly StreamProcessor _processor = new StreamProcessor();

        /// <summary>
        ///     Concurent Queue used to store incomming packets
        /// </summary>
        private readonly ConcurrentQueue<KeyValuePair<ConnectionBase, IList<byte>>> _queue = new ConcurrentQueue<KeyValuePair<ConnectionBase, IList<byte>>>();

        /// <summary>
        ///     Background worker used to process packets
        /// </summary>
        private BackgroundWorker _worker;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="server">Server Hosting the controller</param>
        /// <param name="packetHandlerTypeId">Number 1-255 identifying handler and packets</param>
        /// <param name="packetHandlerTypeName">Debugging name</param>
        /// <param name="checkDelayMs">Delay controller is paused when no packets are found.</param>
        protected DatagramControllerBase(UdpServerBase server, byte packetHandlerTypeId, string packetHandlerTypeName, int checkDelayMs = 10)
        {
            PacketHandlerTypeId = packetHandlerTypeId;
            PacketHandlerTypeName = packetHandlerTypeName;
            Server = server;
            CheckDelayMs = checkDelayMs;
        }

        /// <summary>
        ///     When no packets are found in the queue, this is the time it will pause the thread.
        /// </summary>
        public int CheckDelayMs { get; }

        /// <summary>
        ///     The packet Type Id, always first byte in stream.
        /// </summary>
        public byte PacketHandlerTypeId { get; }

        /// <summary>
        ///     Friendly name of handler
        /// </summary>
        public string PacketHandlerTypeName { get; }

        /// <summary>
        ///     Reference to the server hosting the UDP server.
        /// </summary>
        public UdpServerBase Server { get; }

        /// <summary>
        ///     Background worker function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (Server.Run)
            {
                KeyValuePair<ConnectionBase, IList<byte>> data;
                if (_queue.TryDequeue(out data))
                {
                    _processor.SetData(data.Value);
                    OnProcessNewData(data.Key, _processor);
                    _processor.Clear();
                }
                else
                {
                    Thread.Sleep(CheckDelayMs);
                }
            }
        }

        /// <summary>
        ///     How to process the data stream
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="dataStream"></param>
        /// <returns></returns>
        internal void ProcessNewData(ConnectionBase connection, IList<byte> dataStream = null)
        {
            _queue.Enqueue(new KeyValuePair<ConnectionBase, IList<byte>>(connection, dataStream));
        }

        /// <summary>
        ///     Starts the worker which processes the packets
        /// </summary>
        internal void StartController()
        {
            _worker = new BackgroundWorker();
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerAsync();
            OnStartController();
        }

        #region Abstract Functions

        /// <summary>
        ///     Called when the controller needs to process a packet.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="streamProcessor"></param>
        public abstract void OnProcessNewData(ConnectionBase connection, StreamProcessor streamProcessor);

        /// <summary>
        ///     Called After the controller starts.
        /// </summary>
        public virtual void OnStartController()
        {
        }

        #endregion
    }
}