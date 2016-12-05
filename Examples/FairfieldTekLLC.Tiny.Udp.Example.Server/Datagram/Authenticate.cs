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

using FairfieldTekLLC.Tiny.Udp.Server.Common.BaseClass;
using FairfieldTekLLC.Tiny.Udp.Server.Common.Common;

namespace FairfieldTekLLC.Tiny.Udp.Example.Server.Datagram
{
    internal class Authenticate : DatagramBase
    {
        public Authenticate() : base(1)
        {
        }

        public string Handle { get; set; } = string.Empty;

        protected override StreamProcessor OnPacked(StreamProcessor streamProcessor)
        {
            streamProcessor.WriteBytes(Handle);
            return streamProcessor;
        }

        protected override StreamProcessor OnUnpacked(StreamProcessor streamProcessor)
        {
            Handle = streamProcessor.ReadString();
            return streamProcessor;
        }
    }
}