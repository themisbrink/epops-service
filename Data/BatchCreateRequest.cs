namespace EpopsService.Data
{
    public class BatchCreateRequest
    {
        public string SourceItemId { get; set; } = "";
        public string ISBN { get; set; } = "";
        public int PrintQuantity { get; set; }
    }

}
