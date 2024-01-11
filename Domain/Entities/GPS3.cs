using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class GPS3
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string VehicleCode { get; set; }
        public Int32 Date { get; set; }

        [Column(TypeName = "jsonb")]
        public string Detail { get; set; }

        public int Total { get; set; }

        public DateTimeOffset CreatedTime { get; set; }
    }
}
