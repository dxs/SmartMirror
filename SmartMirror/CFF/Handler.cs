﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace SmartMirror.CFF
{
	public class Handler
	{
		private bool DEBUG = false;
		private StationBoard currentConnectionsLausanne = new StationBoard();
		public StationBoard CurrentConnectionsLausanne { get { return currentConnectionsLausanne; } set { currentConnectionsLausanne = value; } }
		private StationBoard currentConnectionsGenf = new StationBoard();
		public StationBoard CurrentConnectionsGenf { get { return currentConnectionsGenf; } set { currentConnectionsGenf = value; } }

		public Handler()
		{
			CurrentConnectionsLausanne.StationBoards = new ObservableCollection<StationBoards>();
			CurrentConnectionsGenf.StationBoards = new ObservableCollection<StationBoards>();
		}

		public async Task<Station> GetStation(string city)
		{
			HttpClient http = new HttpClient();
			string response = await http.GetStringAsync(@"http://transport.opendata.ch/v1/locations?query=" + city);
			Location tmp = JsonConvert.DeserializeObject<Location>(response);
			if (tmp == null)
				return null;
			Station station = tmp.Stations[0];
			return station;
		}

		public async Task<StationBoard> GetStationBoard(string city)
		{
			HttpClient http = new HttpClient();
			string response = await http.GetStringAsync($@"http://transport.opendata.ch/v1/stationboard?station={city}&limit=4");
			StationBoard tmp = JsonConvert.DeserializeObject<StationBoard>(response);
			foreach (var s in tmp.StationBoards)
				if (s.To != "Allaman")
					CurrentConnectionsLausanne.StationBoards.Add(s);
				else
					CurrentConnectionsGenf.StationBoards.Add(s);

			return tmp;
		}

		public async Task<Connections> GetConnection(string from, string to)
		{
			var http = new HttpClient();
			var response = await http.GetStringAsync($"http://transport.opendata.ch/v1/connections?from={from}&to={to}");
			Connections data = JsonConvert.DeserializeObject<Connections>(response);
			return data;
		}

		private async void GetBase()
		{
			
		}

		public DateTime TimeConverter(string ISO)
		{
			DateTime d = DateTime.Parse(ISO, null, DateTimeStyles.RoundtripKind);
			return d;
		}
	}
}
