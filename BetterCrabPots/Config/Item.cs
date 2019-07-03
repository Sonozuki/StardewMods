using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCrabPots.Config
{
    class Item
    {
        public int Id { get; set; }
        public int Chance { get; set; }
        public int Quantity { get; set; }
        public int Quality { get; set; } = 5;
        public string Comment { get; set; }

        public Item(int id = -1, int chance = 1, int quantity = 1, int quality = 5, string comment = "")
        {
            Id = id;
            Chance = chance;
            Quantity = quantity;
            Quality = quality;
            Comment = comment;
        }
    }
}
