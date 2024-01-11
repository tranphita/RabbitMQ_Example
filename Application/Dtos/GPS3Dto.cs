using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

public class GPS3Dto
{
    public int Id { get; set; }
    public string VehicleCode { get; set; }
    public Int32 Date { get; set; }
    public List<GpsPositionQueueModel> Detail { get; set; }

    public int Total { get; set; }

    public DateTimeOffset CreatedTime { get; set; }
}


