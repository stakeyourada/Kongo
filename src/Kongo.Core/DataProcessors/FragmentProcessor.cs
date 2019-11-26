using Kongo.Core.DataServices;
using Kongo.Core.Helpers;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
		}

		/// <summary>
		/// Process Network Fragments and summarize into ProcessedFragmentsModel
		/// </summary>
		/// <param name="jsonContent"></param>
		/// <returns></returns>
		public Task<ProcessedFragmentsModel> ProcessFragments(string jsonContent)
		{
			Exceptions.ThrowIfNotJson(jsonContent, "jsonContent");

			var fragments = JsonConvert.DeserializeObject<List<FragmentModel>>(jsonContent).ToList();

			var restFragments = fragments.Where(fragment => fragment.Received_from.Equals("Rest"));
			var networkFragments = fragments.Where(fragment => fragment.Received_from.Equals("Network"));

			List<FragmentModel> unknownFragments = new List<FragmentModel>();
			List<PendingFragmentModel> pendingFragments = new List<PendingFragmentModel>();
			List<InBlockFragmentModel> inBlockFragments = new List<InBlockFragmentModel>();
			List<RejectedFragmentModel> rejectedFragments = new List<RejectedFragmentModel>();

			foreach (var fragment in fragments)
			{
				var statusType = fragment.Status.GetType();

				if (statusType == typeof(string))
				{
					pendingFragments.Add(
						new PendingFragmentModel
						{
							Fragment_id = fragment.Fragment_id,
							Last_updated_at = fragment.Last_updated_at,
							Received_at = fragment.Received_at,
							Received_from = fragment.Received_from,
							Status = (string)fragment.Status
						});
					continue;
				}
				if (statusType == typeof(JObject))
				{
					var json = fragment.Status.ToString(Formatting.None);
					
					var blockStatus = JsonConvert.DeserializeObject<InABlockStatus>(json);
					var rejectedStatus = JsonConvert.DeserializeObject<RejectedStatus>(json);

					if ( blockStatus.InABlock != null)
					{
						inBlockFragments.Add(
							new InBlockFragmentModel
							{
								Fragment_id = fragment.Fragment_id,
								Last_updated_at = fragment.Last_updated_at,
								Received_at = fragment.Received_at,
								Received_from = fragment.Received_from,
								Status = blockStatus.InABlock
							});
					} 
					else
					{
						if ( rejectedStatus.Rejected != null)
						{
							rejectedFragments.Add(
								new RejectedFragmentModel
								{
									Fragment_id = fragment.Fragment_id,
									Last_updated_at = fragment.Last_updated_at,
									Received_at = fragment.Received_at,
									Received_from = fragment.Received_from,
									Status = rejectedStatus.Rejected
								});
						} else
						{
							unknownFragments.Add(fragment);
						}
					}
				}
			}

			var totalFragments = fragments.Count();
			var fragmentsReceviedFromRest = restFragments.Count();
			var fragmentsReceviedFromNetwork = networkFragments.Count();

			var fragmentsInBlock = inBlockFragments.Count();
			var fragmentsRejected = rejectedFragments.Count();
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
				FragmentsRejected = fragmentsRejected,
				BlockFragments = inBlockFragments,
				FragmentsPending = fragmentsPending,
				PendingFragments = pendingFragments
			};

			// SQL Lite can't map IEnum or dynamic objects, so trim to just the aggregate counts
			_database.FragmentStatistics.Add(new StoredFragmentsModel
			{
				Timestamp = result.Timestamp,
				TotalFragments = result.TotalFragments,
				FragmentsReceviedFromRest = result.FragmentsReceviedFromRest,
				FragmentsReceviedFromNetwork = result.FragmentsReceviedFromNetwork,
				FragmentsInBlock = result.FragmentsInBlock,
				FragmentsRejected = result.FragmentsRejected,
				FragmentsPending = result.FragmentsPending,
			});
			
			_database.SaveChanges();

			// return ProcessedFragmentsModel result
			return Task.FromResult(result);
		}
	}
}
