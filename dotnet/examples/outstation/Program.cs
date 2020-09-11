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
            //�@�Ӻ޲z�� 
            var channel = mgr.AddTCPServer("server", LogLevels.NORMAL, ServerAcceptMode.CloseExisting, new IPEndpoint("127.0.0.1", 20000), ChannelListener.Print());
            //�гy�@��Channel 
            var config = new OutstationStackConfig();

            // configure the various measurements in our database     �гy��ƶq���I        
            config.databaseTemplate.binary.Add(3, new BinaryConfig());
            config.databaseTemplate.binary.Add(7, new BinaryConfig());
            config.databaseTemplate.analog.Add(0, new AnalogConfig());
            // ....           ���\�۰ʰe��� 
            config.outstation.config.allowUnsolicited = true;

            // Optional: setup your stack configuration here�]�w���a�a�}�A���ݦa�} 
            config.link.localAddr = 10;// outstation ip 
            config.link.remoteAddr = 1; //master addr

            var outstation = channel.AddOutstation("outstation", RejectingCommandHandler.Instance, DefaultOutstationApplication.Instance, config);
            //��]�w�ɥ��Channel����̭� 
            outstation.Enable(); // enable communications

            Console.WriteLine("Press <Enter> to change a value");
            //�n��i�h�Ȧs�����ƭ� 
            bool binaryValue = false;
            double analogValue = 0;
            while (true)
            { //���U Enter �|���ܤG���� ��W�[����ƭ� 
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

                            //��s�Ȧs�����ƭ� 
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
