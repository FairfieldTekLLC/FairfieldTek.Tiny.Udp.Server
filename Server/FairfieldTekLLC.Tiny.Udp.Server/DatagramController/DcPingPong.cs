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
using FairfieldTekLLC.Tiny.Udp.Server.BaseClass;
using FairfieldTekLLC.Tiny.Udp.Server.Common.Common;
using FairfieldTekLLC.Tiny.Udp.Server.Common.Datagram;
using FairfieldTekLLC.Tiny.Udp.Server.Properties;

namespace FairfieldTekLLC.Tiny.Udp.Server.DatagramController
{
    /// <summary>
    ///     This is the controller used to handle Ping/Pong build in action for maintaining
    ///     connections to the server.
    /// </summary>
    internal class DcPingPong : DatagramControllerBase
    {
        /// <summary>
        ///     To prevent Garbage collection, we define these once
        /// </summary>
        readonly PingPong _data = new PingPong();

        public DcPingPong(UdpServerBase server) : base(server, 0, "Ping/Pong")
        {
        }

        public override void OnProcessNewData(ConnectionBase connection, StreamProcessor streamProcessor)
        {
            lock (_data)
            {
                try
                {
                    _data.Unpack(streamProcessor);
                    connection.GotPong = true;
                    connection.PongTime = DateTime.Now;
                    var missedPingMessage = string.Format(Resources.Message_MissedPing, connection.EndPoint, connection.LastPing, _data.ChkVal);
                    var pingMessage = string.Format(Resources.Message_Ping, connection.EndPoint, _data.ChkVal);
                    Server.LogMessage(connection.LastPing != _data.ChkVal ? missedPingMessage : pingMessage);
                }
                catch (Exception ex)
                {
                    Server.LogMessage(ex.Message);
                }
            }
        }

        public override void OnStartController()
        {
            Server.LogMessage("Datagram Controller for Ping/Pong Started.");
        }
    }
}