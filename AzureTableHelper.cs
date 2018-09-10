using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using ServiceStack;
using System.Linq;

namespace Common
{
	public static class AzureTableHelper<T> where T : TableEntityExt, new()
	{
		public static List<T> LoadEverything()
		{
			var query = new TableQuery<T>();
			return new T().GetCloudTable().ExecuteQuery(query).ToList();
		}

		public static List<T> LoadPartition(string partitionKey)
		{
			var query = new TableQuery<T>().Where(
				TableQuery.GenerateFilterCondition(
					"PartitionKey",
					QueryComparisons.GreaterThanOrEqual,
					partitionKey
				)
			);
			return new T().GetCloudTable().ExecuteQuery(query).ToList();
		}

		public static IEnumerable<T> Query(Func<TableQuery<T>, TableQuery<T>> query)
		{
			var tbl = new T().GetCloudTable();
			return tbl.ExecuteQuery(query(tbl.CreateQuery<T>()));
		}

		public static T Get(string partitionKey, string rowKey)
		{
			var tbl = new T().GetCloudTable();
			try
			{
				var query = TableOperation.Retrieve<T>(partitionKey, rowKey);
				var result = tbl.Execute(query);
				var item = (T) result.Result;
				return item;
			}
			catch (WebException we)
			{
				if (we.GetStatus() == HttpStatusCode.NotFound)
				{
					return null;
				}

				throw;
			}
		}

		public static async Task<TableResult> SetAsync(T item)
		{
			var tbl = item.GetCloudTable();
			var query = TableOperation.InsertOrReplace(item);
			return await tbl.ExecuteAsync(query);
		}

		public static TableResult Insert(T item)
		{
			var tbl = item.GetCloudTable();
			var query = TableOperation.Insert(item);
			return tbl.Execute(query);
		}
	}
}
