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

using System;
using FairfieldTekLLC.Tiny.Udp.Server.Common.Common;

namespace FairfieldTekLLC.Tiny.Udp.Server.Common.BaseClass
{
    /// <summary>
    ///     Abstract class used as the base for all Packet Types passed
    ///     by the server.  This class ensures that the first bit is
    ///     always the packet type.
    /// </summary>
    public abstract class DatagramBase
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="packetTypeId">The Byte specifying what type of packet the data is for</param>
        protected DatagramBase(byte packetTypeId)
        {
            PacketTypeId = packetTypeId;
        }

        /// <summary>
        ///     The Packet Type Id
        /// </summary>
        public byte PacketTypeId { get; internal set; }

        /// <summary>
        ///     Called when you Pack an object to send over Udp socket
        /// </summary>
        /// <param name="streamProcessor">An initialized StreamProcessor moved to the correct byte position</param>
        /// <returns>A stream processor</returns>
        protected abstract StreamProcessor OnPacked(StreamProcessor streamProcessor);

        /// <summary>
        ///     Called when you Unpack a Udp packet from the stream to a object.
        /// </summary>
        /// <param name="streamProcessor"></param>
        /// <returns>A stream processor</returns>
        protected abstract StreamProcessor OnUnpacked(StreamProcessor streamProcessor);

        /// <summary>
        ///     Base function used to ensure the first byte is always the packet type Id
        /// </summary>
        /// <returns>A stream processor</returns>
        public StreamProcessor Pack()
        {
            var streamProcessor = new StreamProcessor();
            streamProcessor.WriteBytes(PacketTypeId);
            return OnPacked(streamProcessor);
        }

        /// <summary>
        ///     Unpacks the Byte array into the Datagram
        /// </summary>
        /// <param name="streamProcessor"></param>
        /// <returns></returns>
        public StreamProcessor Unpack(StreamProcessor streamProcessor)
        {
            if (streamProcessor == null)
                throw new Exception("Stream Processor cannot be null.");
            PacketTypeId = streamProcessor.ReadU8();

            return OnUnpacked(streamProcessor);
        }
    }
}