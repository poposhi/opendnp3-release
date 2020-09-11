// Copyright 2013-2020 Automatak, LLC
//
// Licensed to Green Energy Corp (www.greenenergycorp.com) and Automatak
// LLC (www.automatak.com) under one or more contributor license agreements. 
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership. Green Energy Corp and Automatak LLC license
// this file to you under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License. You may obtain
// a copy of the License at:
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Automatak.DNP3.Adapter;
using Automatak.DNP3.Interface;

namespace DotNetOutstationDemo
{   
    class Program
    {
        static int Main(string[] args)
        {
            IDNP3Manager mgr = DNP3ManagerFactory.CreateManager(1, new PrintingLogAdapter());
            //一個管理者 
            var channel = mgr.AddTCPServer("server", LogLevels.NORMAL, ServerAcceptMode.CloseExisting, new IPEndpoint("127.0.0.1", 20000), ChannelListener.Print());
            //創造一個Channel 
            var config = new OutstationStackConfig();

            // configure the various measurements in our database     創造資料量測點        
            config.databaseTemplate.binary.Add(3, new BinaryConfig());
            config.databaseTemplate.binary.Add(7, new BinaryConfig());
            config.databaseTemplate.analog.Add(0, new AnalogConfig());
            // ....           允許自動送資料 
            config.outstation.config.allowUnsolicited = true;

            // Optional: setup your stack configuration here設定本地地址，遠端地址 
            config.link.localAddr = 10;// outstation ip 
            config.link.remoteAddr = 1; //master addr

            var outstation = channel.AddOutstation("outstation", RejectingCommandHandler.Instance, DefaultOutstationApplication.Instance, config);
            //把設定檔丟到Channel物件裡面 
            outstation.Enable(); // enable communications

            Console.WriteLine("Press <Enter> to change a value");
            //要放進去暫存器的數值 
            bool binaryValue = false;
            double analogValue = 0;
            while (true)
            { //按下 Enter 會改變二元值 跟增加類比數值 
                switch (Console.ReadLine())
                {
                    case("x"):
                        return 0;
                    default:
                        {
                            binaryValue = !binaryValue;
                            ++analogValue;

                            // create a changeset and load it 
                            var changeset = new ChangeSet();

                            var binaryFlags = new Flags();
                            binaryFlags.Set(BinaryQuality.ONLINE);

                            var analogFlags = new Flags();
                            analogFlags.Set(AnalogQuality.ONLINE);

                            //更新暫存器的數值 
                            changeset.Update(new Binary(binaryValue, binaryFlags, new DNPTime(DateTime.UtcNow)), 3);
                            changeset.Update(new Analog(analogValue, analogFlags, DNPTime.Now), 0);
                            outstation.Load(changeset);
                        }
                        break;
                }                              
            }
        }
    }
}
