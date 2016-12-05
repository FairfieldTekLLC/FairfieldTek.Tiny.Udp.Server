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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using FairfieldTekLLC.Tiny.Udp.Server.Common.BaseClass;
using FairfieldTekLLC.Tiny.Udp.Server.Common.Datagram;

namespace FairfieldTekLLC.Tiny.Udp.Server.Common.Client
{
    public abstract class UdpConnect
    {
        private UdpClient _client;
        private IPEndPoint _endPoint;
        private string _host;
        private volatile bool _isConnected;
        private int _port;

        protected UdpConnect(string host, int port)
        {
            _port = port;
            _host = host;
        }

        protected UdpConnect()
        {
        }

        public bool IsConnected => _isConnected;

        private void _OnConnect()
        {
            OnConnect();
            Send(new PingPong());
        }

        private void BwListener_DoWork(object sender, DoWorkEventArgs e)
        {
            _endPoint = new IPEndPoint(IPAddress.Any, _port);
            _client = new UdpClient(_endPoint);
            _client.DontFragment = true;
            _client.Connect(_host, _port);
            Thread.Sleep(100);
            _isConnected = true;
            _OnConnect();
            while (_isConnected)
            {
                try
                {
                    NewData(_client.Receive(ref _endPoint));
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

        public void Connect()
        {
            if (_host == null || _port == 0)
                return;
            BackgroundWorker bwListener = new BackgroundWorker();
            bwListener.DoWork += BwListener_DoWork;
            bwListener.RunWorkerAsync(this);
        }

        public void Connect(string address, int port)
        {
            _port = port;
            _host = address;
            BackgroundWorker bwListener = new BackgroundWorker();
            bwListener.DoWork += BwListener_DoWork;
            bwListener.RunWorkerAsync(this);
        }

        public void Disconnect()
        {
            _isConnected = false;
        }

        public abstract void NewData(IList<byte> data);

        public virtual void OnConnect()
        {
        }

        public virtual void OnDisconnect()
        {
        }

        public void Send(DatagramBase datagram)
        {
            if (!_isConnected)
                return;
            var toSend = datagram.Pack().Data.ToArray();
            _client.Send(toSend, toSend.Length);
        }
    }
}