using System;

namespace Diablerie.Engine.Entities
{
    public class StaticObjectMode
    {
        public static readonly StaticObjectMode Neutral = new StaticObjectMode(0, "NU");
        public static readonly StaticObjectMode Operating = new StaticObjectMode(1, "OP");
        public static readonly StaticObjectMode On = new StaticObjectMode(2, "ON");
        public static readonly StaticObjectMode Special1 = new StaticObjectMode(3, "S1");
        public static readonly StaticObjectMode Special2 = new StaticObjectMode(4, "S2");
        public static readonly StaticObjectMode Special3 = new StaticObjectMode(5, "S3");
        public static readonly StaticObjectMode Special4 = new StaticObjectMode(6, "S4");
        public static readonly StaticObjectMode Special5 = new StaticObjectMode(7, "S5");

        public readonly int index;
        public readonly string token;

        public static StaticObjectMode GetByToken(string token, StaticObjectMode defaultMode = null)
        {
            if (token == "NU") return Neutral;
            if (token == "OP") return Operating;
            if (token == "ON") return On;
            if (token == "S1") return Special1;
            if (token == "S2") return Special2;
            if (token == "S3") return Special3;
            if (token == "S4") return Special4;
            if (token == "S5") return Special5;
            return defaultMode;
        }

        private StaticObjectMode(int index, string token)
        {
            this.index = index;
            this.token = token;
        }

        public override string ToString()
        {
            return token;
        }
    }
}