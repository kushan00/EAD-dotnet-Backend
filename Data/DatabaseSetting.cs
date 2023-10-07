namespace backend.Data;

    public class DatabaseSetting
    {
        public string ConnectionString {get;set;} = null!;
        public string DatabaseName {get;set;} = null!;
        public string AccountCollectionName {get;set;} = null!;
    }
