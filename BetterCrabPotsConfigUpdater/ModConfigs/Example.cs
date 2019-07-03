using Persona4GoldenFusionCalculator.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCrabPotsConfigUpdater.ModConfigs
{
    public class CalculatorContext
    {
        Persona Persona = new Persona("Izanagi", "Fool", 1, new int[5] { 3, 2, 2, 3, 2 }, new string[7] { "", "", "", "rs", "wk", "", "nu" }, new List<Skill> { new Skill("Zio", 0), new Skill("Cleave", 0), new Skill("Rakukaja", 0), new Skill("Rakunda", 3), new Skill("Tarukaja", 5) });
    };
}
