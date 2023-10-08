namespace backend.Data;

    public class DatabaseSetting
    {
        public string ConnectionString {get;set;} = null!;
        public string DatabaseName {get;set;} = null!;
        public string AccountCollectionName {get;set;} = null!;
        public string TrainCollectionName {get;set;} = null!;
        public string ScheduleCollectionName {get;set;} = null!;
        public string ReservationCollectionName {get;set;} = null!;
    }
