using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudumDare38.Characters
{
    interface ISuicidable
    {
        int ContactDamage();
        void Explode();
        bool NeedCollectExplosionDamage();
        void CollectExplosionDamage();
    }
}
