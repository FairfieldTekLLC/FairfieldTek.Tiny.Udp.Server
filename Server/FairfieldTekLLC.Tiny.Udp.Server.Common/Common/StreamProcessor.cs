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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FairfieldTekLLC.Tiny.Udp.Server.Common.Common
{
    public sealed class StreamProcessor
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="data">Byte Stream</param>
        public StreamProcessor(IList<byte> data)
        {
            Data = new List<byte>(data);
            Position = 0;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public StreamProcessor()
        {
            Data = new List<byte>();
            Position = 0;
        }

        /// <summary>
        ///     The data currently in the Stream Processor
        /// </summary>
        public List<byte> Data { get; }

        /// <summary>
        ///     Number of Bytes in the stream
        /// </summary>
        public int Length => Data.Count - 1;

        /// <summary>
        ///     Current position in the datastream
        /// </summary>
        public int Position { get; private set; }

        public void Clear()
        {
            Position = 0;
            Data.Clear();
        }

        /// <summary>
        ///     Reads a boolean from the stream
        /// </summary>
        /// <returns></returns>
        public bool ReadBool()
        {
            int oldpos = Position;
            Position++;
            return Data[oldpos] != 0;
        }

        /// <summary>
        ///     Reads a Int from the stream
        /// </summary>
        /// <returns></returns>
        public int ReadI32()
        {
            int oldpos = Position;
            Position += 4;
            return BitConverter.ToInt32(Data.ToArray(), oldpos);
        }

        /// <summary>
        ///     Reads a short from the stream
        /// </summary>
        /// <returns></returns>
        public short ReadS16()
        {
            int oldpos = Position;
            Position += 2;
            return BitConverter.ToInt16(Data.ToArray(), oldpos);
        }

        /// <summary>
        ///     Reads a SByte from the stream
        /// </summary>
        /// <returns></returns>
        public sbyte ReadS8()
        {
            int oldpos = Position;
            Position++;
            return (sbyte) Data[oldpos];
        }

        /// <summary>
        ///     Reads a string from the stream
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            int len = ReadU8();
            int oldpos = Position;
            if (len <= 0)
                return string.Empty;
            Position += len;
            return Encoding.UTF8.GetString(Data.ToArray(), oldpos, len);
        }

        /// <summary>
        ///     Reads a UShort from the stream
        /// </summary>
        /// <returns></returns>
        public ushort ReadU16()
        {
            int oldpos = Position;
            Position += 2;
            return BitConverter.ToUInt16(Data.ToArray(), oldpos);
        }

        /// <summary>
        ///     Reads a UInt from the stream
        /// </summary>
        /// <returns></returns>
        public uint ReadU32()
        {
            int oldpos = Position;
            Position += 4;
            return BitConverter.ToUInt32(Data.ToArray(), oldpos);
        }

        /// <summary>
        ///     Reads a byte from teh stream
        /// </summary>
        /// <returns></returns>
        public byte ReadU8()
        {
            int oldpos = Position;
            Position++;
            return Data[oldpos];
        }

        /// <summary>
        ///     Clears the stream and sets it equial to passed data.
        /// </summary>
        /// <param name="data"></param>
        public void SetData(IList<byte> data)
        {
            Data.Clear();
            Data.AddRange(data);
            Position = 0;
        }

        /// <summary>
        ///     Writes a Byte to the stream
        /// </summary>
        /// <param name="u8"></param>
        public void WriteBytes(byte u8)
        {
            Data.Add(u8);
            Position++;
        }

        /// <summary>
        ///     Writes a SByte to the stream
        /// </summary>
        /// <param name="s8"></param>
        public void WriteBytes(sbyte s8)
        {
            Data.Add((byte) s8);
            Position++;
        }

        /// <summary>
        ///     Writes a Short to the stream
        /// </summary>
        /// <param name="s16"></param>
        public void WriteBytes(short s16)
        {
            Data.AddRange(BitConverter.GetBytes(s16));
            Position += 2;
        }

        /// <summary>
        ///     Writes a UShort to the stream
        /// </summary>
        /// <param name="u16"></param>
        public void WriteBytes(ushort u16)
        {
            Data.AddRange(BitConverter.GetBytes(u16));
            Position += 2;
        }

        /// <summary>
        ///     Writes a Int to the stream
        /// </summary>
        /// <param name="i32"></param>
        public void WriteBytes(int i32)
        {
            Data.AddRange(BitConverter.GetBytes(i32));
            Position += 4;
        }

        /// <summary>
        ///     Writes a UInt to the stream
        /// </summary>
        /// <param name="u32"></param>
        public void WriteBytes(uint u32)
        {
            Data.AddRange(BitConverter.GetBytes(u32));
            Position += 4;
        }

        /// <summary>
        ///     Writes a string to the stream, first byte is length, then comes string
        /// </summary>
        /// <param name="message">Max length is 255 charaters</param>
        public void WriteBytes(string message)
        {
            if (message == null)
                message = "";

            Debug.Assert(message.Length < 255);

            if (message.Length > 255)
                message = message.Substring(0, 255);

            WriteBytes((byte) message.Length);
            Position++;
            if (message.Length <= 0)
                return;
            Data.AddRange(Encoding.UTF8.GetBytes(message));
            Position += message.Length;
        }

        /// <summary>
        ///     Writes a Boolean to the stream in the form of a Byte.
        /// </summary>
        /// <param name="flag"></param>
        public void WriteBytes(bool flag)
        {
            Data.Add(flag ? (byte) 1 : (byte) 0);
            Position++;
        }
    }
}