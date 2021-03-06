﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Common
{
	public abstract class TableEntityExt : TableEntity
	{
		[JsonIgnore, IgnoreProperty]
		public abstract string TableName { get; }

		public CloudTable GetCloudTable()
		{
			if (AzureConfiguration.StorageAccount == null)
				throw new Exception("AzureConfiguration.StorageAccount is not set.");
			var storageAccount = AzureConfiguration.StorageAccount;
			var tblClient = storageAccount.CreateCloudTableClient();
			var tbl = tblClient.GetTableReference(TableName);
			return tbl;
		}
	}
}
