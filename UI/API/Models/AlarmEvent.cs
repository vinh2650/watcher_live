using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{
    public class AlarmEvent
    {
        public string CellId { get; set; }

        public string Alarm { get; set; }

        public DateTime CreateDateTimeUtc { get; set; }
    }
}
