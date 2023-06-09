﻿using Microsoft.Data.SqlClient;
using Multitenancy.Common.Data.Helpers;
using Multitenancy.Data.Master.Entities;
using Multitenancy.Data.Master.Models;
using Newtonsoft.Json;

namespace Multitenancy.Data.Master.Helpers;

public static class ConnectionHelper
{
    public static SqlConnectionStringBuilder GetConnectionBuilder(string key, TenantStorage tenantStorage)
    {
        var dataSource = SecurityHelper.Decrypt(key, tenantStorage.Server);
        var initialCatalog = SecurityHelper.Decrypt(key, tenantStorage.Database);
        var userId = string.IsNullOrWhiteSpace(tenantStorage.Username) ? "" : SecurityHelper.Decrypt(key, tenantStorage.Username);
        var password = string.IsNullOrWhiteSpace(tenantStorage.Password) ? "" : SecurityHelper.Decrypt(key, tenantStorage.Password);

        var connectionBuilder = new SqlConnectionStringBuilder
        {
            DataSource = dataSource,
            InitialCatalog = initialCatalog,
            UserID = userId,
            Password = password
        };

        if (tenantStorage.ConnectionParameters != null)
        {
            var parameters = JsonConvert.DeserializeObject<TenantStorageConnectionParameter>(tenantStorage.ConnectionParameters);
            connectionBuilder.PersistSecurityInfo = parameters.PersistSecurityInfo ?? connectionBuilder.PersistSecurityInfo;
            connectionBuilder.MultipleActiveResultSets = parameters.MultipleActiveResultSets ?? connectionBuilder.MultipleActiveResultSets;
            connectionBuilder.Encrypt = parameters.Encrypt ?? connectionBuilder.Encrypt;
            connectionBuilder.TrustServerCertificate = parameters.TrustServerCertificate ?? connectionBuilder.TrustServerCertificate;
            connectionBuilder.ConnectTimeout = parameters.ConnectTimeout ?? connectionBuilder.ConnectTimeout;
            connectionBuilder.LoadBalanceTimeout = parameters.LoadBalanceTimeout ?? connectionBuilder.LoadBalanceTimeout;
            connectionBuilder.Pooling = parameters.Pooling ?? connectionBuilder.Pooling;
            connectionBuilder.MinPoolSize = parameters.MinPoolSize ?? connectionBuilder.MinPoolSize;
            connectionBuilder.MaxPoolSize = parameters.MaxPoolSize ?? connectionBuilder.MaxPoolSize;
        }

        return connectionBuilder;
    }
}
