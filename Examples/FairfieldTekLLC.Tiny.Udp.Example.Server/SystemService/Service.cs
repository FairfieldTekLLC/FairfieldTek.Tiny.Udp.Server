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
using System.Linq;
using System.ServiceProcess;
using FairfieldTekLLC.Tiny.Udp.Example.Server.SystemService.Framework;

namespace FairfieldTekLLC.Tiny.Udp.Example.Server.SystemService
{
    /*
        [WindowsService("Official Name Of Service",
        DisplayName = "Name Displayed in Services panel",
        Description = "Description",
        EventLogSource = "Event Log Source",
        StartMode = ServiceStartMode.Automatic)]
     */

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Service<T> where T : IWindowsService, new()
    {
        public static void Main(string[] args)
        {
            // if install was a command line flag, then run the installer at runtime.
            if (args.Contains("-install", StringComparer.InvariantCultureIgnoreCase))
                WindowsServiceInstaller.RuntimeInstall<T>();
            // if uninstall was a command line flag, run uninstaller at runtime.
            else if (args.Contains("-uninstall", StringComparer.InvariantCultureIgnoreCase))
                WindowsServiceInstaller.RuntimeUnInstall<T>();
            // otherwise, fire up the service as either console or windows service based on UserInteractive property.
            else
            {
                T implementation = new T();
                // if started from console, file explorer, etc, run as console app.
                if (Environment.UserInteractive)
                    ConsoleHarness.Run(args, implementation);
                // otherwise run as a windows service
                else
                    ServiceBase.Run(WindowsServiceHarness.GetNew(implementation));
            }
        }
    }
}