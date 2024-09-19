using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Counter:IEntity
    {
        public int Id { get; set; }
        public int Up { get; set; }
        public int Down { get; set; }
        public int Difference { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
