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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using FairfieldTekLLC.Tiny.Udp.Server.Common;
using FairfieldTekLLC.Tiny.Udp.Server.DatagramController;
using FairfieldTekLLC.Tiny.Udp.Server.Properties;

namespace FairfieldTekLLC.Tiny.Udp.Server.BaseClass
{
    /// <summary>
    ///     Base class used for the udp server.
    /// </summary>
    [ComVisible(false)]
    [DebuggerDisplay("Client Count = {ClientCount}")]
    public abstract class UdpServerBase : IDisposable
    {
        /// <summary>
        ///     Used to stop the loop that listens for incoming messages.
        /// </summary>
        internal volatile bool Run;

        /// <summary>
        ///     Constructor, expects the port to listen to.
        /// </summary>
        /// <param name="port">Port to listen for incoming UDP messages.</param>
        /// <param name="pingInterval">Delay (ms) to wait for checking if a connection is still live after sending ping.  </param>
        /// <param name="customControllers"></param>
        protected UdpServerBase(int port, int pingInterval, IList<DatagramControllerBase> customControllers = null)
        {
            Controllers = new Dictionary<byte, DatagramControllerBase> { { 0, new DcPingPong(this) } };
            if (customControllers != null)
                foreach (DatagramControllerBase controller in customControllers)
                {
                    if (Controllers.ContainsKey(controller.PacketHandlerTypeId))
                        throw new Exception("Packet Type Id in use.");

                    Debug.Assert(!string.IsNullOrWhiteSpace(controller.PacketHandlerTypeName), "Packet Type Name is null.");

                    Controllers.Add(controller.PacketHandlerTypeId, controller);
                }

            _pingInterval = pingInterval;
            _port = port;
            _connectedClients = new Dictionary<IPEndPoint, ConnectionBase>();
        }

        private int _ClientCount = 0;
        private int _CountClientVersion = 0;
        private IReadOnlyCollection<ConnectionBase> Clients = null;

        public int ClientCount {
            get {

                if (_CountClientVersion == ClientVersion())
                    return _ClientCount;

                lock (_connectedClients)
                {
                    _ClientCount = _connectedClients.Count;
                    ClientVersionIncrement();
                }
                return _ClientCount;
            }
        }

        #region Public Properties

        /// <summary>
        ///     Retrieves a list of all the connected clients.
        /// </summary>
        public IReadOnlyCollection<ConnectionBase> ConnectedClients {
            get {
                if (_CountClientVersion == ClientVersion() && Clients != null)
                    return Clients;

                lock (_connectedClients)
                {
                    Clients = new ReadOnlyCollection<ConnectionBase>(_connectedClients.Values.ToList());
                }
                return Clients;
            }
        }

        #endregion

        #region Abstract Functions

        /// <summary>
        ///     Function used to create the new client connection object when something connects to the server.
        ///     By Deriving from ClientBase you can extend the data being tracked by the server.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="parent"></param>
        /// <returns>The new ClientBase derrived object.</returns>
        public abstract ConnectionBase CreateNewConnection(IPEndPoint endpoint, UdpServerBase parent);

        #endregion

        #region Public Functions

        public void AddControllers(IList<DatagramControllerBase> customControllers)
        {
            foreach (DatagramControllerBase controller in customControllers)
            {
                if (Controllers.ContainsKey(controller.PacketHandlerTypeId))
                    throw new Exception("Packet Type Id in use.");

                Debug.Assert(!string.IsNullOrWhiteSpace(controller.PacketHandlerTypeName), "Packet Type Name is null.");

                Controllers.Add(controller.PacketHandlerTypeId, controller);
            }
        }

        /// <summary>
        ///     Function used to check if a handle is in use on the server.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public bool IsHandleUsed(string handle)
        {
            lock (_connectedClients)
                return
                    ConnectedClients.Any(clientConnectionBase => clientConnectionBase.Handle.Equals(handle, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        ///     Removes a client from the client dictionary
        /// </summary>
        /// <param name="client"></param>
        public void RemoveClient(ConnectionBase client)
        {
            if (!OnBeforeRemoveClient(client))
                return;
            if (client == null)
                return;
            lock (_connectedClients)
                if (_connectedClients.ContainsKey(client.EndPoint))
                {
                    LogMessage(string.Format(Resources.Message_ClientRemoved, client));
                    _connectedClients.Remove(client.EndPoint);
                    ClientVersionIncrement();
                }
            OnAfterRemoveClient(client);
        }

        /// <summary>
        ///     Sends the byte array of data to the destination endpoint
        /// </summary>
        /// <param name="data"></param>
        /// <param name="destination"></param>
        public void Send(IList<byte> data, IPEndPoint destination)
        {
            if (!OnBeforeSend(data, destination))
                return;
            _serverUdpListener.Send(data, destination);
            OnAfterSend(data, destination);
        }

        /// <summary>
        ///     Starts the server.
        /// </summary>
        public void StartServer()
        {
            if (Run)
                return;
            OnBeforeServerStart();

            LogMessage(string.Format(Resources.Message_ServerStarted, _port));
            LogMessage(string.Format(Resources.Message_PingDelay, _pingInterval));

            Run = true;

            foreach (var dcController in Controllers.Values)
                dcController.StartController();

            _serverUdpListener = new UdpListener(_port);

            Task.Factory.StartNew(async () =>
            {
                //Start the connection monitoring thread.
                BackgroundWorker bgwClientMonitor = new BackgroundWorker();
                bgwClientMonitor.DoWork += ClientMonitor;
                bgwClientMonitor.RunWorkerAsync();

                //Loop while running.
                while (Run)
                {
                    ReceivedData received;
                    try
                    {
                        received = await _serverUdpListener.Receive();
                    }
                    catch (Exception er)
                    {
                        LogMessage("Error:" + er.Message);
                        continue;
                    }

                    //Add new connection
                    lock (_connectedClients)
                    {
                        if (!_connectedClients.ContainsKey(received.Sender))
                        {

                            AddClient(CreateNewConnection(received.Sender, this));
                            
                        }
                    }

                    //Process new packet.
                    lock (_connectedClients)
                    {
                        NewPacket(_connectedClients[received.Sender], received.Data);

                        //  new Thread(() => NewPacket(_connectedClients[received.Sender], received.Data)).Start();
                    }
                }
            });
            OnAfterServerStart();
        }

        /// <summary>
        ///     Stops the server.
        /// </summary>
        public void StopServer()
        {
            Run = false;
        }

        #endregion

        #region Private Variables

        /// <summary>
        ///     Dictionary keyed by the endpoint address for each ClientConnectionBase attached to the server.
        /// </summary>
        private readonly Dictionary<IPEndPoint, ConnectionBase> _connectedClients;

        private volatile int _connectedClientsVersion = 0;

        public void ClientVersionIncrement()
        {
            if (_connectedClientsVersion + 1 == int.MaxValue)
                _connectedClientsVersion = 0;
            _connectedClientsVersion++;
        }

        public int ClientVersion()
        {
            return _connectedClientsVersion;
        }


        /// <summary>
        ///     The millesecond interval delay between sending pings to the client
        /// </summary>
        private readonly int _pingInterval;

        /// <summary>
        ///     The UDP Port the server is listening on.
        /// </summary>
        private readonly int _port;

        /// <summary>
        ///     Server UDP Listener
        /// </summary>
        private UdpListener _serverUdpListener;

        /// <summary>
        ///     Used to store all the Datagram Packet Controllers
        /// </summary>
        private Dictionary<byte, DatagramControllerBase> Controllers {
            get;
        }

        #endregion

        #region Private Functions

        /// <summary>
        ///     Function which runs in it's own thread that pings every connected
        ///     client.  The client will respond with a Pong.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientMonitor(object sender, DoWorkEventArgs e)
        {
            while (Run)
            {
                Parallel.ForEach(ConnectedClients, client => client.SendPing());
                Thread.Sleep(_pingInterval);
                Parallel.ForEach(ConnectedClients.Where(client => !client.GotPong && client.PingTime != null), MarkClientMissed);
                Parallel.ForEach(ConnectedClients.Where(client => client.GotPong && client.PingTime != null), MarkClientHit);
                Parallel.ForEach(ConnectedClients.Where(client => client.NumberOfMissedPongs > 10), RemoveClient);
            }
        }

        /// <summary>
        ///     Finds a controller by Id
        /// </summary>
        /// <param name="controllerId"></param>
        /// <returns></returns>
        private DatagramControllerBase GetHandler(byte controllerId)
        {
            return Controllers.ContainsKey(controllerId) ? Controllers[controllerId] : null;
        }

        /// <summary>
        ///     Called to add a client to the client dictionary.
        /// </summary>
        /// <param name="client"></param>
        private void AddClient(ConnectionBase client)
        {
            lock (_connectedClients)
                if (!_connectedClients.ContainsKey(client.EndPoint))
                {
                    LogMessage(string.Format(Resources.Message_AddingEndpoint, client.EndPoint));
                    _connectedClients.Add(client.EndPoint, client);
                    ClientVersionIncrement();
                }
        }

        /// <summary>
        ///     Marks a client connection as active
        /// </summary>
        /// <param name="client"></param>
        private void MarkClientHit(ConnectionBase client)
        {
            if (client == null)
                return;
            lock (_connectedClients)
                if (_connectedClients.ContainsKey(client.EndPoint))
                    client.NumberOfMissedPongs = 0;
        }

        /// <summary>
        ///     Marks a client connection to indicate that it isn't responding.
        /// </summary>
        /// <param name="client"></param>
        private void MarkClientMissed(ConnectionBase client)
        {
            if (client == null)
                return;
            lock (_connectedClients)
                if (_connectedClients.ContainsKey(client.EndPoint))
                    client.NumberOfMissedPongs++;
        }

        /// <summary>
        ///     Called when a new packet is received
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        private void NewPacket(ConnectionBase connection, IList<byte> data)
        {
            var dataStream = data as byte[] ?? data.ToArray();
            if (dataStream.Length == 0)
                return;

            var handler = GetHandler(dataStream.ToArray()[0]);

            if (handler != null)
                handler.ProcessNewData(connection, dataStream);
            else
                LogMessage(string.Format(Resources.Message_NoHandler, dataStream[0]));
        }

        #endregion

        #region IDisposable

        private bool _disposed;

        /// <summary>
        ///     Dispose.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;
            // Dispose of resources held by this instance.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            // ReSharper disable once UseNullPropagation
            if (_serverUdpListener != null)
            {
                _serverUdpListener.Dispose();
                _serverUdpListener = null;
            }

            // Dispose of resources held by this instance.

            // Violates rule: DisposableFieldsShouldBeDisposed.
            // Should call aFieldOfADisposableType.Dispose();

            _disposed = true;
        }

        // Disposable types implement a finalizer.
        ~UdpServerBase()
        {
            Dispose(false);
        }

        #endregion

        #region Virtual Functins

        /// <summary>
        ///     Abstract Function which is called when a logging message is fired.
        /// </summary>
        /// <param name="message"></param>
        public virtual void LogMessage(string message)
        {
        }

        /// <summary>
        ///     Called after a client is diconnected
        /// </summary>
        /// <param name="client"></param>
        public virtual void OnAfterRemoveClient(ConnectionBase client)
        {
        }

        /// <summary>
        ///     Called after a datagram is sent to a client
        /// </summary>
        /// <param name="data"></param>
        /// <param name="destination"></param>
        public virtual void OnAfterSend(IList<byte> data, IPEndPoint destination)
        {
        }

        /// <summary>
        ///     Called after the server starts.
        /// </summary>
        public virtual void OnAfterServerStart()
        {
        }

        /// <summary>
        ///     Called before a client is removed.
        /// </summary>
        /// <param name="client"></param>
        /// <returns>If false, the client isn't removed.</returns>
        public virtual bool OnBeforeRemoveClient(ConnectionBase client)
        {
            return true;
        }

        /// <summary>
        ///     Called before a datagram is sent to a client
        /// </summary>
        /// <param name="data"></param>
        /// <param name="destination"></param>
        /// <returns>If false, the datagram isn't sent to the client.</returns>
        public virtual bool OnBeforeSend(IList<byte> data, IPEndPoint destination)
        {
            return true;
        }

        /// <summary>
        ///     Called before the server is started.
        /// </summary>
        /// <returns></returns>
        public virtual bool OnBeforeServerStart()
        {
            return true;
        }

        #endregion
    }
}