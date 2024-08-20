using HidSharp.Reports;
using HWMonitorWebApi.Hubs;
using LibreHardwareMonitor.Hardware;
using Microsoft.AspNetCore.SignalR;

namespace HWMonitorWebApi.Services
{
    public class DataItem
    {
        public string? CPUTemp { get; set; }
        public string? CPULoad { get; set; }
        public string? GPUTemp { get; set; }
        public string? GPULoad { get; set; }
        public string? GPUPower { get; set; }
        public string? CPUFan { get; set; }
        public string? SystemFan1 { get; set; }
        public string? SystemFan2 { get; set; }
        public string? GPUFan1 { get; set; }
        public string? GPUFan2 { get; set; }
        public string? Gram { get; set; }
        public string? RAM  { get; set; }
    }

    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }

    public class HardwareMonitorService : BackgroundService
    {
        private readonly IHubContext<HardwareHub> _hubContext;

        public HardwareMonitorService(IHubContext<HardwareHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var data = GetHardwareData();
                await _hubContext.Clients.All.SendAsync("ReceiveHardwareData", data, stoppingToken);
                await Task.Delay(800, stoppingToken);
            }
        }

        private object GetHardwareData()
        {
            var dataItem = new DataItem();
            Computer computer = new Computer();
            computer.IsCpuEnabled = true;
            computer.IsMotherboardEnabled = true;
            computer.IsGpuEnabled = true;
            computer.IsMemoryEnabled = true;

            computer.Open();
            computer.Accept(new UpdateVisitor());

            foreach (var item in computer.Hardware)
            {
                foreach (var sub in item.SubHardware)
                {
                    foreach (ISensor sensor in sub.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Fan && sensor.Name == "CPU Fan")
                        {
                            dataItem.CPUFan = sensor.Value.ToString();
                        }
                        if (sensor.SensorType == SensorType.Fan && sensor.Name == "System Fan #1")
                        {
                            dataItem.SystemFan1 = sensor.Value.ToString();
                        }
                        if (sensor.SensorType == SensorType.Fan && sensor.Name == "System Fan #2")
                        {
                            dataItem.SystemFan2 = sensor.Value.ToString();
                        }
                    }
                }
                foreach (var sensor in item.Sensors)
                {
                    if (item.HardwareType == HardwareType.Cpu)
                    {
                        if (sensor.SensorType == SensorType.Load && sensor.Name == "CPU Total")
                        {
                            dataItem.CPULoad = Convert.ToInt32(sensor.Value).ToString();
                        }
                        if (sensor.SensorType == SensorType.Temperature && sensor.Name == "CPU Package")
                        {
                            dataItem.CPUTemp = Convert.ToInt32(sensor.Value).ToString();
                        }
                    }
                    if (item.HardwareType==HardwareType.Memory)
                    {
                        if (sensor.SensorType==SensorType.Data && sensor.Name == "Memory Available")
                        {
                            dataItem.RAM = Convert.ToInt32(sensor.Value).ToString();
                        }
                    }
                    if (item.HardwareType == HardwareType.GpuNvidia)
                    {
                        if (sensor.SensorType == SensorType.Temperature && sensor.Name == "GPU Core")
                        {
                            dataItem.GPUTemp = Convert.ToInt32(sensor.Value).ToString();
                        }
                        if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Core")
                        {
                            dataItem.GPULoad = Convert.ToInt32(sensor.Value).ToString();
                        }
                        if (sensor.SensorType == SensorType.Fan && sensor.Name == "GPU Fan 1")
                        {
                            dataItem.GPUFan1 = sensor.Value.ToString();
                        }
                        if (sensor.SensorType == SensorType.Fan && sensor.Name == "GPU Fan 2")
                        {
                            dataItem.GPUFan2 = sensor.Value.ToString();
                        }
                        if (sensor.SensorType == SensorType.Power && sensor.Name == "GPU Package")
                        {
                            dataItem.GPUPower = Convert.ToInt32(sensor.Value).ToString();
                        }
                        if (sensor.SensorType == SensorType.SmallData && sensor.Name == "GPU Memory Free")
                        {
                            dataItem.Gram = sensor.Value.ToString();
                        }
                    }
                   
                }
            }
            computer.Close();
            return dataItem;
        }
    }
}
