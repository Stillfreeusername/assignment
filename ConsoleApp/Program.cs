using System;
using System.Collections.Generic;
using System.Numerics;

namespace ConsoleApp
{
	public class Product
	{
		public BigInteger Reference { get; set; }
		public string Name { get; set; }
		public BigInteger CompanyPrefix { get; set; }
		public string CompanyName { get; set; }
	}

	public class Inventory
	{
		public string ID { get; set; }
		public string Location { get; set; }
		public DateTimeOffset DateOfInventory { get; set; }
		public List<string> ItemTags { get; set; }
	}

	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
		}
	}
}
