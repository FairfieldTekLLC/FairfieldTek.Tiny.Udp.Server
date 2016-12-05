# FairfieldTek.Tiny.Udp.Server

This project implements a fully functional Udp Sockets Server with examples of how to use it.
The software is owned and copyrighted by Fairfield Tek L.L.C. (http://www.FairfieldTek.com)

## License ##
Copyright [2016] Fairfield Tek L.L.C.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

 Fairfield Tek L.L.C.
 Copyright (c) 2016, Fairfield Tek L.L.C.
 
 
 THIS SOFTWARE IS PROVIDED BY WINTERLEAF ENTERTAINMENT LLC ''AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
 INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
 PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL WINTERLEAF ENTERTAINMENT LLC BE LIABLE FOR ANY DIRECT, INDIRECT, 
 INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
 SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
 ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR 
 OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH 
 DAMAGE. 

 ##How To Use##

 1. Create a new project
 2. create a new class and inherit from FairfieldTekLLC.Tiny.Udp.Server.BaseClass.UdpServerBase
 3. Define datagrams to hold your data FairfieldTekLLC.Tiny.Udp.Server.Common.BaseClass.DatagramBase
 4. Create any UDP Packet Controllers to handle different datagrams from step 3 from  FairfieldTekLLC.Tiny.Udp.Server.BaseClass.DatagramControllerBase
 5. Create a new server, passing your new controllers
 6. Finished.