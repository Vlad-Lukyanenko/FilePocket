namespace FilePocket.Contracts.Storage.Responses
{
    public class GetUserStorageConsumptionResponse
    {
        public double Used { get; set; }
        public double Total { get; set; }
        public double RemainingSizeMb { get; set; }

    }
}
