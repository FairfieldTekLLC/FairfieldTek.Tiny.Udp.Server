﻿//  Fairfield Tek L.L.C.
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
    [AttributeUsage(AttributeTargets.Class)]
    public class WindowsServiceAttribute : Attribute
    {
        /// <summary>
        ///     Marks an IWindowsService with configuration and installation attributes.
        /// </summary>
        /// <param name="name">The name of the windows service.</param>
        public WindowsServiceAttribute(string name)
        {
            // set name and default description and display name to name.
            Name = name;
            Description = name;
            DisplayName = name;

            // default all other attributes.
            CanStop = true;
            CanShutdown = true;
            CanPauseAndContinue = true;
            StartMode = ServiceStartMode.Manual;
            EventLogSource = null;
            Password = null;
            UserName = null;
        }

        /// <summary>
        ///     True if service supports pause and continue (defaults to true).
        /// </summary>
        public bool CanPauseAndContinue { get; set; }

        /// <summary>
        ///     True if service supports shutdown event (defaults to true).
        /// </summary>
        public bool CanShutdown { get; set; }

        /// <summary>
        ///     True if service supports stop event (defaults to true).
        /// </summary>
        public bool CanStop { get; set; }

        /// <summary>
        ///     A textural description of the service name (defaults to Name).
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The displayable name that shows in service manager (defaults to Name).
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     Specifies the event log source to set the service's EventLog to.  If this is
        ///     empty or null (the default) no event log source is set.  If set, will auto-log
        ///     start and stop events.
        /// </summary>
        public string EventLogSource { get; set; }

        /// <summary>
        ///     The name of the service.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The password to run the service under (defaults to null).  Ignored
        ///     if the UserName is empty or null, this property is ignored.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     The method to start the service when the machine reboots (defaults to Manual).
        /// </summary>
        public ServiceStartMode StartMode { get; set; }

        /// <summary>
        ///     The user to run the service under (defaults to null).  A null or empty
        ///     UserName field causes the service to run as ServiceAccount.LocalService.
        /// </summary>
        public string UserName { get; set; }
    }
}