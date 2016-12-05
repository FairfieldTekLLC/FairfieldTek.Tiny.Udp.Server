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
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using FairfieldTekLLC.Tiny.Udp.Server.Common.BaseClass;
using FairfieldTekLLC.Tiny.Udp.Server.Common.Common;
using FairfieldTekLLC.Tiny.Udp.Server.Common.Datagram;

namespace FairfieldTekLLC.Tiny.Udp.Server.Common.Client
{
    public abstract class UdpConnect
    {
        /// <summary>
        /// UDPClient Object
        /// </summary>
        private UdpClient _client;

        /// <summary>
        /// Endpoint to connecting from 
        /// </summary>
        private IPEndPoint _endPoint;

        /// <summary>
        /// IP Address or DNS of server
        /// </summary>
        private string _host;

        /// <summary>
        /// Flag indicating whether it is connected.
        /// </summary>
        private volatile bool _isConnected;

        /// <summary>
        /// Port to connect on.
        /// </summary>
        private int _port;

        /// <summary>
        /// Create client to the host on port
        /// </summary>
        /// <param name="host">IP Address or DNS</param>
        /// <param name="port">Port</param>
        protected UdpConnect(string host, int port)
        {
            _port = port;
            _host = host;
        }

        /// <summary>
        /// Connect
        /// </summary>
        protected UdpConnect()
        {
        }

        /// <summary>
        /// Is connected to Host
        /// </summary>
        public bool IsConnected => _isConnected;

        //To reduce garbage collection, using readonly vars
        private readonly StreamProcessor _stringProcessor = new StreamProcessor();

        //To reduce garbage collection, using readonly vars
        private readonly PingPong _pingPong = new PingPong();

        /// <summary>
        /// Worker which does the actual listening.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BwListener_DoWork(object sender, DoWorkEventArgs e)
        {
            
            int countCheck = 0;
            int lPort = _port;

            while (countCheck < 1000)
            {

                try
                {
                    _endPoint = new IPEndPoint(IPAddress.Any, lPort);
                    _client = new UdpClient(_endPoint) {EnableBroadcast = false};
                    break;
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    if (ex.SocketErrorCode != SocketError.AddressAlreadyInUse)
                        continue;
                    countCheck++;
                    lPort++;
                }
            }

            _client.Connect(_host, _port);
            bool setConnected = false;
            _isConnected = true;
            while (_isConnected)
            {
                try
                {
                    //If first time connecting, set a ping to get
                    //the servers attention.
                    if (!setConnected)
                        Send(_pingPong);

                    //Listen for data
                    var data = _client.Receive(ref _endPoint);

                    //Is this the first response?
                    if (!setConnected)
                    {
                        setConnected = true;
                        OnConnect();
                    }

                    //Did the server send us a pingpong?
                    if (data[0] == 0)
                    {
                        _stringProcessor.SetData(data);
                        _pingPong.Unpack(_stringProcessor);
                        Send(_pingPong);
                    }
                    else
                        NewData(data);

                }
                catch (Exception)
                {
                    _isConnected = false;
                }
            }
            try
            {
                _client.Close();
                _client = null;
                _endPoint = null;
            }
            catch (Exception)
            {
                //
            }

            OnDisconnect();
        }

        /// <summary>
        /// Connect
        /// </summary>
        public void Connect()
        {
            if (_host == null || _port == 0)
                return;
            CreateListenerWorker();
        }

        /// <summary>
        /// Connect to address on port
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public void Connect(string address, int port)
        {
            _port = port;
            _host = address;
            CreateListenerWorker();

        }

        /// <summary>
        /// Creates worker to do listening
        /// </summary>
        private void CreateListenerWorker()
        {
            BackgroundWorker bwListener = new BackgroundWorker();
            bwListener.DoWork += BwListener_DoWork;
            bwListener.RunWorkerAsync(this);
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        public void Disconnect()
        {
            _isConnected = false;
        }

        /// <summary>
        /// New data recieved from the server.
        /// </summary>
        /// <param name="data"></param>
        public abstract void NewData(IList<byte> data);

        /// <summary>
        /// Connection Established
        /// </summary>
        public virtual void OnConnect()
        {
        }

        /// <summary>
        /// Connection Lost
        /// </summary>
        public virtual void OnDisconnect()
        {
        }

        /// <summary>
        /// Send Datagram to host.
        /// </summary>
        /// <param name="datagram"></param>
        public void Send(DatagramBase datagram)
        {
            if (!_isConnected)
                return;
            byte[] toSend = datagram.Pack().Data.ToArray();
            _client.Send(toSend, toSend.Length);
        }
    }
}