using Newtonsoft.Json;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using System.Threading.Tasks;
using Kongo.Core.Helpers;
using Kongo.Core.DataServices;
using Microsoft.EntityFrameworkCore;
using System;

namespace Kongo.Core.DataProcessors
{


	/// <summary>
	/// StdErr Log processor
	/// </summary>
	public class StdErrorProcessor : IProcessStdError
	{
		private readonly KongoDataStorage _db;

		public StdErrorProcessor(KongoDataStorage databaseName)
		{
			_db = databaseName;
			//_db.Database.EnsureCreated();
			_db.Database.Migrate();			
		}

		~StdErrorProcessor()
		{
			_db.Database.EnsureDeleted();
			_db.Dispose();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="logLine"></param>
		/// <param name="lineNumber"></param>
		/// <returns></returns>
		public Task IngestLogEntry(string logLine, int lineNumber)
		{
			Exceptions.ThrowIfNotJson(logLine, "logLine");
			Exceptions.ThrowIfNegative(lineNumber, "lineNumber");

			var logEntry = JsonConvert.DeserializeObject<LogIngestionModel>(logLine);

			_db.LogEntries.Add(logEntry);
			_db.SaveChanges();
			return Task.FromResult(true);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Task<ProcessedLogModel> ProcessIngestedLogs()
		{
			var result = new ProcessedLogModel();
			foreach (var msg in _db.LogEntries)
			{
				Console.WriteLine($"MsgID={msg.Id}\tLevel: {msg.Level}\tMsg: {msg.Msg}\tNode id: {msg.Node_id}\tPeer: {msg.Peer_addr}\tReason: {msg.Reason}\tTask: {msg.Task}");
			}
			return Task.FromResult(result);
		}

		public async Task<bool> DeleteDatabaseFile()
		{
			return await _db.Database.EnsureDeletedAsync();
		}
	}
}
