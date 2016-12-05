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

using System.Collections.Generic;
using System.Net;
using FairfieldTekLLC.Tiny.Udp.Example.Server.Enumerations;
using FairfieldTekLLC.Tiny.Udp.Server.BaseClass;

namespace FairfieldTekLLC.Tiny.Udp.Example.Server.Tiny
{
    /// <summary>
    ///     This is our implementation of the ClientConnectionBase
    ///     Since we need to track more information than the stock implementation.
    ///     Remember to cast the connections from the interface to this class
    ///     when using.
    /// </summary>
    public class Connection : ConnectionBase
    {
        #region Properties

        public Connection(IPEndPoint endpoint, UdpServerBase parent) : base(endpoint, parent)
        {
            //Location = new Point3F();
            Zone = 0;
            LookingForGroup = false;
            IsGm = false;
            //mGroup = 0;
            //mGuild = 0;
            IsRolePlay = false;
            Friends = new List<string>();
            Ignore = new List<string>();
        }

        public bool IsAuthorized { get; set; }

        public ConnectionType ConnectionType { get; set; }

        public long PlayerCharacterId { get; set; }

        public Gender Gender { get; set; }

        //public string mPassPhrase
        //    {
        //    get { return _mPassPhrase; }
        //    set { _mPassPhrase = value; }
        //    }

        public List<string> Friends { get; set; }

        //public Point3F Location { get; set; }

        public short Zone { get; set; }

        public bool LookingForGroup { get; set; }

        public bool IsGm { get; set; }

        //public ushort mGroup { get; set; }

        //public uint mGuild { get; set; }

        public List<string> Ignore { get; set; }

        public bool IsRolePlay { get; set; }

        #endregion
    }
}