﻿// // Fairfield Tek L.L.C.
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

using System;
using System.Collections.Generic;
using System.Net;
using FairfieldTekLLC.Tiny.Udp.Example.Server.DatagramController;
using FairfieldTekLLC.Tiny.Udp.Server.BaseClass;

namespace FairfieldTekLLC.Tiny.Udp.Example.Server.Tiny
{
    internal class UdpServer : UdpServerBase
    {
        /// <summary>
        ///     The distance a say will travel
        /// </summary>
        private const float _mSayDistance = 10.0f;

        public UdpServer(int port, int connectionverifydelay) : base(port, connectionverifydelay)
        {
            List<DatagramControllerBase> customControllers = new List<DatagramControllerBase>();
            customControllers.Add(new DcAuthenticate(this));
            customControllers.Add(new DcSay(this));
            AddControllers(customControllers);
        }

        public override ConnectionBase CreateNewConnection(IPEndPoint endpoint, UdpServerBase parent)
        {
            LogMessage("New Client: " + endpoint);
            return new Connection(endpoint, parent);
        }

        /// <summary>
        ///     Fetches the Guild Name
        /// </summary>
        /// <param name="guildid"></param>
        /// <returns></returns>
        public string FetchGuildName(uint guildid)
        {
            return "";
        }

        /// <summary>
        ///     Fetches the zone name
        /// </summary>
        /// <param name="zoneid"></param>
        /// <returns></returns>
        public string FetchZoneName(short zoneid)
        {
            return "";
        }

        /// <summary>
        ///     Logs a message to the console
        /// </summary>
        /// <param name="msg"></param>
        public override void LogMessage(string msg)
        {
#if DEBUG
            Console.WriteLine($@"{DateTime.Now:s}  {msg}");
#endif
        }
    }
}