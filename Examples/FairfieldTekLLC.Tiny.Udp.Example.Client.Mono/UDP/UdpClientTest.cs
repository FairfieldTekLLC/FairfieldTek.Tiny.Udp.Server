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
using System.Threading;
using FairfieldTekLLC.Tiny.Udp.Example.Client.Mono.Datagram;
using FairfieldTekLLC.Tiny.Udp.Example.Client.Mono.Properties;
using FairfieldTekLLC.Tiny.Udp.Server.Common.Client;
using FairfieldTekLLC.Tiny.Udp.Server.Common.Common;

namespace FairfieldTekLLC.Tiny.Udp.Example.Client.Mono.UDP
{
    internal class UdpClientTest : UdpConnect
    {
        private volatile bool _handleResponded;
        private volatile int _loginAttemptNumber;
        private readonly VolatileVar<string> _myHandle = new VolatileVar<string>(null);

        public UdpClientTest(string host, int port) : base(host, port)
        {
        }

        public UdpClientTest()
        {
        }

        public override void NewData(IList<byte> data)
        {
            var packetType = data[0];
            switch (packetType)
            {
                case 1:
                {
                    Authenticate auth = new Authenticate();
                    auth.Unpack(new StreamProcessor(data));
                    if (string.IsNullOrEmpty(auth.Handle))
                        TrySetHandle();
                    else
                    {
                        Console.WriteLine($"Handle is set to {auth.Handle}");
                        _myHandle.Set(auth.Handle);
                    }
                    _handleResponded = true;
                    //Authenticate Response
                    break;
                }
                case 2:
                {
                    Say say = new Say();
                    say.Unpack(new StreamProcessor(data));
                    Console.WriteLine(say.From.Equals(_myHandle.Get(), StringComparison.InvariantCultureIgnoreCase)
                        ? $"You said, '{say.Message}'."
                        : $"{say.From} said, '{say.Message}'.");
                    break;
                }
            }
        }

        public override void OnConnect()
        {
            Console.WriteLine(StringResource.Connected);
            BackgroundWorker trashSender = new BackgroundWorker();
            trashSender.DoWork += TrashSender_DoWork;
            trashSender.RunWorkerAsync();
        }

        public override void OnDisconnect()
        {
            Console.WriteLine(StringResource.Disconnected);
        }

        private void TrashSender_DoWork(object sender, DoWorkEventArgs e)
        {
            while (IsConnected)
            {
                if (_myHandle.Get() == string.Empty && !_handleResponded)
                {
                    _handleResponded = false;
                    TrySetHandle();
                    Thread.Sleep(5000);
                }
                else
                {
                    Say say = new Say {Message = $"The Current Time is {DateTime.Now:F} "};
                    Send(say);
                    Thread.Sleep(1000);
                }
            }
        }

        private void TrySetHandle()
        {
            _loginAttemptNumber++;
            Authenticate auth = new Authenticate {Handle = "Client" + _loginAttemptNumber};
            Send(auth);
        }
    }
}