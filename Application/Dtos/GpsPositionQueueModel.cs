public class GpsPositionQueueModel
{
    public string Id { get; set; }
    public string Plate { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public double Speed { get; set; }
    public double Km { get; set; }
    public DateTime TimeUpdate { get; set; }
}