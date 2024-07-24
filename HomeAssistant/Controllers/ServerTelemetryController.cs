using HomeAssistant.Filters;
using LibreHardwareMonitor.Hardware;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HomeAssistant.Controllers
{
	[ApiController]
	[SkipStatusCodePages]
	[NoUserLogging]
	[Route("api/[controller]/[action]")]
	public class ServerTelemetryController : Controller
	{
		private readonly Computer _computer;

		public ServerTelemetryController()
		{
			_computer = new Computer
			{
				IsCpuEnabled = true,
				IsGpuEnabled = true,
				IsMotherboardEnabled = true,
				IsMemoryEnabled = true,
				IsNetworkEnabled = true,
				IsStorageEnabled = true,
				IsControllerEnabled = true
			};
			_computer.Open();
		}

		[HttpGet]
		public IActionResult CpuTemperature()
		{
			var temperatureData = new List<string>();

			foreach (IHardware hardware in _computer.Hardware)
			{
				if (hardware.HardwareType == HardwareType.Cpu)
				{
					hardware.Update();

					foreach (ISensor sensor in hardware.Sensors)
					{
						if (sensor.SensorType == SensorType.Temperature)
						{
							temperatureData.Add($"{sensor.Name}: {sensor.Value} °C");
						}
					}
				}
			}

			return Ok(JsonSerializer.Serialize(temperatureData));
		}

		[HttpGet]
		public IActionResult AllSensors()
		{
			_computer.Open();
			_computer.Accept(new UpdateVisitor());

			List<string> sensorData = new();

			foreach (IHardware hardware in _computer.Hardware)
			{
				sensorData.Add("Hardware: "+ hardware.Name);

				foreach (IHardware subhardware in hardware.SubHardware)
				{
					sensorData.Add("Hardware: " + subhardware.Name);

					foreach (ISensor sensor in subhardware.Sensors)
					{
						sensorData.Add($"Sensor: {sensor.Name}, value: {sensor.Value}");
					}
				}

				foreach (ISensor sensor in hardware.Sensors)
				{
					sensorData.Add($"\tSensor: {sensor.Name}, value: {sensor.Value}");
				}
			}

			_computer.Close();

			return Ok(JsonSerializer.Serialize(sensorData));
		}

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
}
