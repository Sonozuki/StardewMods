using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persona4GoldenFusionCalculator.Data.Entities
{
    public class Skill
    {
        public string Name { get; set; }
        public int LevelRequired { get; set; }

        public Skill(string name, int levelRequired)
        {
            Name = name;
            LevelRequired = levelRequired;
        }
    }
}
