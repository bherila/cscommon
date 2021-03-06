﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using ServiceStack;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Nito.AsyncEx;

namespace Common
{
	public static class AzureTableHelper<T> where T : TableEntityExt, new()
	{
		public static List<T> ExecuteQuery<T>(CloudTable table, TableQuery<T> query) where T : ITableEntity, new()
		{
			TableContinuationToken continuationToken = null;

			TableQuerySegment<T> tableQueryResult =
				AsyncContext.Run(() => table.ExecuteQuerySegmentedAsync(query, continuationToken));

			return tableQueryResult.ToList();
		}

        public static IEnumerable<T> EnumerateEverything()
        {
            var table = new T().GetCloudTable();
            TableContinuationToken token = null;
            do
            {
                var q = new TableQuery<T>();
                var queryResult = Task.Run(() => table.ExecuteQuerySegmentedAsync(q, token)).GetAwaiter().GetResult();
                foreach (var item in queryResult.Results)
                {
                    yield return item;
                }
                token = queryResult.ContinuationToken;
            } while (token != null);
        }

        public static List<T> LoadEverything()
		{
			try {
                return EnumerateEverything().ToList();
			} catch (StorageException) {
				var storageAccount = AzureConfiguration.StorageAccount;
				var tblClient = storageAccount.CreateCloudTableClient();
				var tbl = tblClient.GetTableReference(new T().TableName);
				AsyncContext.Run(() => tbl.CreateIfNotExistsAsync());
				return new List<T>();
			}
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
			return ExecuteQuery(new T().GetCloudTable(), (query));
		}

		public static IEnumerable<T> Query(Func<TableQuery<T>, TableQuery<T>> query)
		{
			var tbl = new T().GetCloudTable();
			return ExecuteQuery(tbl, query(new TableQuery<T>()));
		}

		public static T Get(string partitionKey, string rowKey)
		{
			var tbl = new T().GetCloudTable();
			try
			{
				var query = TableOperation.Retrieve<T>(partitionKey, rowKey);
				TableResult result = AsyncContext.Run(() => tbl.ExecuteAsync(query));
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

		public static TableResult Set(T item)
		{
			var tbl = item.GetCloudTable();
			var query = TableOperation.InsertOrReplace(item);
			return AsyncContext.Run(() => tbl.ExecuteAsync(query));
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
			return AsyncContext.Run(() => tbl.ExecuteAsync(query));
		}

		public static async Task<TableResult> DeleteAsync(T item) {
			var tbl = item.GetCloudTable();
			var query = TableOperation.Delete(item);
			return await tbl.ExecuteAsync(query);
		}
	}
}
