using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scalability
{
    public static class Constants
    {
        public const int RoomSize = 800;

        public const int PhysicsMask_Solids = 0x01;
        public const int PhysicsMask_Player = 0x02;
        public const int PhysicsMask_Enemies = 0x04;
        public const int PhysicsMask_Collectables = 0x08;
    }
}
