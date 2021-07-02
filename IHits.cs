using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scalability
{
    public enum HitType
    {
        Melee,
        Projectile,
    }

    public interface IHits
    {
        bool HitsWithType( HitType type );
    }
}
