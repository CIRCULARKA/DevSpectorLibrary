using System;
using System.Collections.Generic;

namespace DevSpector.SDK.Models
{
	public class Device
	{
		public Device(Guid id, string inventoryNumber, string modelName, string type,
			string networkName, string housing, string cabinet,
			List<string> ipAddresses, List<Software> software)
		{
			ID = id;
			InventoryNumber = inventoryNumber;
			Type = type;
			NetworkName = networkName;
			Housing = housing;
			Cabinet = cabinet;
			IPAddresses = ipAddresses;
			Software = software;
			ModelName = modelName;
		}

		public Guid ID { get; }

		public string InventoryNumber { get; }

		public string Type { get; }

		public string NetworkName { get; }

		public string ModelName { get; }

		public string Housing { get; }

		public string Cabinet { get; }

		public List<string> IPAddresses { get; }

		public List<Software> Software { get; }
	}
}
