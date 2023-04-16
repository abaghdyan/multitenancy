namespace Plat.Analytics.Services.Options
{
    public class MasterDbOptions
    {
        public const string Section = "MasterDb";

        public string ConnectionString { get; set; }
        public string EncryptionKey { get; set; }
    }
}
