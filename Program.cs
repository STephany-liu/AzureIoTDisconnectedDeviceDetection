using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using Microsoft.Azure.Devices.Shared;

namespace IoTSimulatedDeviceRemover
{
    class DeviceRemoverProgram
    {
        static RegistryManager registryManager;
        static string connectionString = "{your connect string}";
        static void Main(string[] args)
        {
            List<Twin> disconnectedDevicesPrev = new List<Twin> { };

            while (true)
            {
                disconnectedDevicesPrev =  CheckDisconnection(disconnectedDevicesPrev).Result;
                System.Threading.Thread.Sleep(1000*15); // wait for 15 minutes
            }
        }

        private static async System.Threading.Tasks.Task<List<Twin>> CheckDisconnection(List<Twin> disconnectedDevicesPrev)
        {
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            List<Twin> disconnectedDevices = new List<Twin> { };
            List<Twin> disconnectedDevicesOver15min = new List<Twin> { };

            var query = registryManager.CreateQuery("SELECT * FROM devices WHERE connectionState = 'Disconnected'");
            while (query.HasMoreResults)
            {
                var page = await query.GetNextAsTwinAsync();
                foreach (var twin in page)
                {
                    disconnectedDevices.Add(twin);
                    if (disconnectedDevicesPrev.Exists((Twin obj) => obj.DeviceId == twin.DeviceId)){
                        disconnectedDevicesOver15min.Add(twin);
                    }
                }
            }

            Console.WriteLine("start printing devices disconnected over 15min");
            foreach (Twin device in disconnectedDevicesOver15min)
            {
                Console.WriteLine(device.DeviceId);
            }
            return disconnectedDevices;
        }
    }
}