using Kongo.Core.DataServices;
using Kongo.Core.Helpers;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kongo.Core.DataProcessors
{
	/// <summary>
	/// Network Fragments processor
	/// </summary>
	public class FragmentProcessor : IProcessFragments
	{
		private readonly KongoDataStorage _database;

		public FragmentProcessor(KongoDataStorage database)
		{
			_database = database;
			_database.Database.EnsureCreated();
			_database.Database.Migrate();
		}

		/// <summary>
		/// Process Network Fragments and summarize into ProcessedFragmentsModel
		/// </summary>
		/// <param name="jsonFragments"></param>
		/// <returns></returns>
		public Task<ProcessedFragmentsModel> ProcessFragments(string jsonFragments)
		{
			Exceptions.ThrowIfNotJson(jsonFragments, "jsonFragments");

			var fragments = JsonConvert.DeserializeObject<List<FragmentModel>>(jsonFragments);

			var restFragments = fragments.Where(fragment => fragment.Received_from.Equals("Rest"));
			var networkFragments = fragments.Where(fragment => fragment.Received_from.Equals("Network"));

			// hotdog or not a hotdog - either a string (Pending) or object (InBlock)
			var blockFragments = fragments.Where(f => f.Status.GetType() != typeof(string));
			var pendingFragments = fragments.Where(f => f.Status.GetType() == typeof(string));

			var totalFragments = fragments.Count();
			var fragmentsReceviedFromRest = restFragments.Count();
			var fragmentsReceviedFromNetwork = networkFragments.Count();

			// hotdog or not a hotdog - either a string or object (InBlock)
			var fragmentsInBlock = blockFragments.Count();
			var fragmentsPending = pendingFragments.Count();

			var result = new ProcessedFragmentsModel()
			{
				Timestamp = DateTimeOffset.UtcNow,
				TotalFragments = totalFragments,
				FragmentsReceviedFromRest = fragmentsReceviedFromRest,
				RestFragments = restFragments,
				FragmentsReceviedFromNetwork = fragmentsReceviedFromNetwork,
				NetworkFragments = networkFragments,
				FragmentsInBlock = fragmentsInBlock,
				BlockFragments = blockFragments,
				FragmentsPending = fragmentsPending,
				PendingFragments = pendingFragments
			};

			// SQL Lite can't map IEnum or dynamic objects, so trim to just the aggregate counts
			_database.Fragments.Add(new StoredFragmentsModel
			{
				Timestamp = result.Timestamp,
				TotalFragments = result.TotalFragments,
				FragmentsReceviedFromRest = result.FragmentsReceviedFromRest,
				FragmentsReceviedFromNetwork = result.FragmentsReceviedFromNetwork,
				FragmentsInBlock = result.FragmentsInBlock,
				FragmentsPending = result.FragmentsPending,
			});
			
			_database.SaveChanges();

			// return ProcessedFragmentsModel result
			return Task.FromResult(result);
		}
	}
}
