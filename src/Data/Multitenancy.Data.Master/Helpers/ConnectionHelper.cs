using Microsoft.Data.SqlClient;
using Plat.Analytics.Data.Master;
using Plat.Analytics.Services.Models.Tenant;

namespace Plat.Analytics.Services.Helpers
{
    public static class ConnectionHelper
    {
        public static SqlConnectionStringBuilder GetConnectionBuilder(string key, TenantStorage connectionInfo)
        {
            var dataSource = SecurityHelper.Decrypt(key, connectionInfo.Server);
            var initialCatalog = SecurityHelper.Decrypt(key, connectionInfo.Database);
            // ToDo: Remove string.IsNullOrWhiteSpace
            var userId = string.IsNullOrWhiteSpace(connectionInfo.Username) ? "" : SecurityHelper.Decrypt(key, connectionInfo.Username);
            var password = string.IsNullOrWhiteSpace(connectionInfo.Password) ? "" : SecurityHelper.Decrypt(key, connectionInfo.Password);

            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = dataSource,
                InitialCatalog = initialCatalog,
                UserID = userId,
                Password = password
            };
            if (connectionInfo.ConnectionParameters != null)
            {
                var parameters = Newtonsoft.Json.JsonConvert.DeserializeObject<TenantStorageConnectionParameter>(connectionInfo.ConnectionParameters);
                sqlConnectionStringBuilder.PersistSecurityInfo = parameters.PersistSecurityInfo ?? sqlConnectionStringBuilder.PersistSecurityInfo;
                sqlConnectionStringBuilder.MultipleActiveResultSets = parameters.MultipleActiveResultSets ?? sqlConnectionStringBuilder.MultipleActiveResultSets;
                sqlConnectionStringBuilder.Encrypt = parameters.Encrypt ?? sqlConnectionStringBuilder.Encrypt;
                sqlConnectionStringBuilder.TrustServerCertificate = parameters.TrustServerCertificate ?? sqlConnectionStringBuilder.TrustServerCertificate;
                sqlConnectionStringBuilder.ConnectTimeout = parameters.ConnectTimeout ?? sqlConnectionStringBuilder.ConnectTimeout;
                sqlConnectionStringBuilder.LoadBalanceTimeout = parameters.LoadBalanceTimeout ?? sqlConnectionStringBuilder.LoadBalanceTimeout;
                sqlConnectionStringBuilder.Pooling = parameters.Pooling ?? sqlConnectionStringBuilder.Pooling;
                sqlConnectionStringBuilder.MinPoolSize = parameters.MinPoolSize ?? sqlConnectionStringBuilder.MinPoolSize;
                sqlConnectionStringBuilder.MaxPoolSize = parameters.MaxPoolSize ?? sqlConnectionStringBuilder.MaxPoolSize;
            }
            return sqlConnectionStringBuilder;
        }
    }
}
