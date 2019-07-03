using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persona4GoldenFusionCalculator.Data.Entities
{
    public class Persona
    {
        public string Name { get; set; }
        public string Arcana { get; set; }
        public int Level { get; set; }
        public int[] Stats { get; set; }
        public string[] Elements { get; set; }
        public List<Skill> Skills { get; set; }

        public Persona(string name, string arcana, int level, int[] stats, string[] elements, List<Skill> skills)
        {
            Name = name;
            Arcana = arcana;
            Level = level;
            Stats = stats;
            Elements = elements;
            Skills = skills;
        }
    }
}
