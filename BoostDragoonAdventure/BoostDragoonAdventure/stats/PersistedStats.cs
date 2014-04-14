using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.stats
{
    public class PersistedStats //totally could have been struct
    {
        public int maxHP, hp, maxCharge;
        
        public PersistedStats(int maxHP, int currentHP, int maxCharge)
        {
            this.maxHP = maxHP;
            this.hp = currentHP;
            this.maxCharge = maxCharge;
        }
    }
}
