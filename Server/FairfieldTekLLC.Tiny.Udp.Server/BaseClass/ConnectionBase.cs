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
using System.Net;
using FairfieldTekLLC.Tiny.Udp.Server.Common.BaseClass;
using FairfieldTekLLC.Tiny.Udp.Server.Common.Common;
using FairfieldTekLLC.Tiny.Udp.Server.Common.Datagram;

namespace FairfieldTekLLC.Tiny.Udp.Server.BaseClass
{
    /// <summary>
    ///     The base class for all client Udp Socket Connections to the server.
    ///     This class tracks connection information
    /// </summary>
    public class ConnectionBase
    {
        private volatile bool _gotPong;
        private readonly VolatileVar<string> _handle = new VolatileVar<string>();
        private volatile byte _lastPing;
        private volatile int _numberOfMissedPongs;
        private readonly UdpServerBase _parent;
        private readonly VolatileVar<DateTime?> _pingTime = new VolatileVar<DateTime?>();
        private readonly VolatileVar<DateTime?> _pongTime = new VolatileVar<DateTime?>();

        public ConnectionBase(IPEndPoint endpoint, UdpServerBase parent)
        {
            Handle = string.Empty;
            GotPong = false;
            PingTime = null;
            PongTime = null;
            LastPing = 0;
            EndPoint = endpoint;
            _parent = parent;
        }

        /// <summary>
        ///     Endpoint of the client
        /// </summary>
        internal IPEndPoint EndPoint { get; }

        /// <summary>
        ///     Used to track if the client responded in time with a Pong Datagram
        /// </summary>
        public bool GotPong
        {
            get { return _gotPong; }
            internal set { _gotPong = value; }
        }

        /// <summary>
        ///     String used to identify the Client with a friendly name
        /// </summary>
        public string Handle
        {
            get { return _handle.Get(); }
            set { _handle.Set(value); }
        }

        /// <summary>
        ///     Last time a Ping was sent to the socket.
        /// </summary>
        public byte LastPing
        {
            get { return _lastPing; }
            internal set { _lastPing = value; }
        }

        /// <summary>
        ///     The number of missed replies to Pings sent from the server.
        /// </summary>
        public int NumberOfMissedPongs
        {
            get { return _numberOfMissedPongs; }
            internal set { _numberOfMissedPongs = value; }
        }

        /// <summary>
        ///     Reference to the Parent UDP Server
        /// </summary>
        public UdpServerBase Parent => _parent;

        /// <summary>
        ///     Date time last ping was sent
        /// </summary>
        public DateTime? PingTime
        {
            get { return _pingTime.Get(); }
            internal set { _pingTime.Set(value); }
        }

        /// <summary>
        ///     Datetime Last Ping response recorded.
        /// </summary>
        public DateTime? PongTime
        {
            get { return _pongTime.Get(); }
            internal set { _pongTime.Set(value); }
        }

        /// <summary>
        ///     Sends a datagram to the client socket
        /// </summary>
        /// <param name="datagram"></param>
        public void SendData(DatagramBase datagram)
        {
            if (!OnBeforeSendData(datagram))
                return;
            try
            {
                Parent.Send(datagram.Pack().Data, EndPoint);
            }
            catch (Exception)
            {
                Parent.RemoveClient(this);
            }
            OnAfterSendData(datagram);
        }

        /// <summary>
        ///     Sends a ping to the socket.
        /// </summary>
        internal void SendPing()
        {
            GotPong = false;
            PingTime = DateTime.Now;
            PongTime = null;
            if (LastPing < byte.MaxValue - 1)
                LastPing++;
            else
                LastPing = 0;

            PingPong data = new PingPong {ChkVal = LastPing};
            SendData(data);
        }

        #region Virtual Functions

        /// <summary>
        ///     Called after sending a datagram
        /// </summary>
        /// <param name="datagram"></param>
        public virtual void OnAfterSendData(DatagramBase datagram)
        {
        }

        /// <summary>
        ///     Called before sending a datagram
        /// </summary>
        /// <param name="datagram"></param>
        /// <returns>If false, it will not send datagram</returns>
        public virtual bool OnBeforeSendData(DatagramBase datagram)
        {
            return true;
        }

        #endregion
    }
}