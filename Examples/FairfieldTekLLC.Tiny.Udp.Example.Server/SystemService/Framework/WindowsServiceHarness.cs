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
using System.ServiceProcess;

namespace FairfieldTekLLC.Tiny.Udp.Example.Server.SystemService.Framework
{
    /// <summary>
    ///     A generic Windows Service that can handle any assembly that
    ///     implements IWindowsService (including AbstractWindowsService)
    /// </summary>
    public partial class WindowsServiceHarness : ServiceBase
    {
        /// <summary>
        ///     Constructor a generic windows service from the given class
        /// </summary>
        /// <param name="serviceImplementation">Service implementation.</param>
        private WindowsServiceHarness(IWindowsService serviceImplementation)
        {
            // make sure service passed in is valid
            if (serviceImplementation == null)
                throw new ArgumentNullException(nameof(serviceImplementation), "IWindowsService cannot be null in call to GenericWindowsService");

            // set instance and backward instance
            ServiceImplementation = serviceImplementation;
        }

        /// <summary>
        ///     Get the class implementing the windows service
        /// </summary>
        public IWindowsService ServiceImplementation { get; }

        /// <summary>
        ///     Set configuration data
        /// </summary>
        /// <param name="serviceImplementation">The service with configuration settings.</param>
        private void ConfigureServiceFromAttributes(IWindowsService serviceImplementation)
        {
            WindowsServiceAttribute attribute = serviceImplementation.GetType().GetAttribute<WindowsServiceAttribute>();

            if (attribute != null)
            {
                // wire up the event log source, if provided
                if (!string.IsNullOrWhiteSpace(attribute.EventLogSource))
                {
                    // assign to the base service's EventLog property for auto-log events.
                    EventLog.Source = attribute.EventLogSource;
                }

                CanStop = attribute.CanStop;
                CanPauseAndContinue = attribute.CanPauseAndContinue;
                CanShutdown = attribute.CanShutdown;

                // we don't handle: laptop power change event
                CanHandlePowerEvent = false;

                // we don't handle: Term Services session event
                CanHandleSessionChangeEvent = false;

                // always auto-event-log 
                AutoLog = true;
            }
            else
                throw new InvalidOperationException(
                    $"IWindowsService implementer {serviceImplementation.GetType().FullName} must have a WindowsServiceAttribute.");
        }

        public static WindowsServiceHarness GetNew(IWindowsService serviceImplementation)
        {
            var window = new WindowsServiceHarness(serviceImplementation);

            // configure our service
            window.ConfigureServiceFromAttributes(serviceImplementation);
            return window;
        }

        /// <summary>
        ///     Override service control on continue
        /// </summary>
        protected override void OnContinue()
        {
            // perform class specific behavior 
            ServiceImplementation.OnContinue();
        }

        /// <summary>
        ///     Called when service is paused
        /// </summary>
        protected override void OnPause()
        {
            // perform class specific behavior 
            ServiceImplementation.OnPause();
        }

        /// <summary>
        ///     Called when the Operating System is shutting down
        /// </summary>
        protected override void OnShutdown()
        {
            // perform class specific behavior
            ServiceImplementation.OnShutdown();
        }

        /// <summary>
        ///     Called when service is requested to start
        /// </summary>
        /// <param name="args">The startup arguments array.</param>
        protected override void OnStart(string[] args)
        {
            ServiceImplementation.OnStart(args);
        }

        /// <summary>
        ///     Called when service is requested to stop
        /// </summary>
        protected override void OnStop()
        {
            ServiceImplementation.OnStop();
        }
    }
}