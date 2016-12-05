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

namespace FairfieldTekLLC.Tiny.Udp.Server.Common.Datagram
{
    /// <summary>
    ///     A system defined datagram used to keep communication
    ///     between the server and client alive.
    /// </summary>
    public class PingPong : DatagramBase
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public PingPong() : base(0)
        {
        }

        /// <summary>
        ///     A byte that will count 0-255 with each iteration of pings, restarts at 0 after 255
        /// </summary>
        public byte ChkVal { get; set; }

        /// <summary>
        ///     Datagram specific Packing code.
        /// </summary>
        /// <param name="streamProcessor"></param>
        /// <returns></returns>
        protected override StreamProcessor OnPacked(StreamProcessor streamProcessor)
        {
            streamProcessor.WriteBytes(ChkVal);
            return streamProcessor;
        }

        /// <summary>
        ///     Datagram specific Unpacking code.
        /// </summary>
        /// <param name="streamProcessor"></param>
        /// <returns></returns>
        protected override StreamProcessor OnUnpacked(StreamProcessor streamProcessor)
        {
            ChkVal = streamProcessor.ReadU8();
            return streamProcessor;
        }
    }
}