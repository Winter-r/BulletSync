﻿using System.Collections.Generic;
using System.Linq;

namespace BulletSync.Character
{
    public static class LoopUtils
    {
        public static bool IsFilled<T>(this IEnumerable<T> enumerable)
        {
            //No null enumerables.
            if (enumerable == null)
                return false;

            //Return for empty enumerables.
            if (enumerable.Count() == 0)
                return false;

            return true;
        }
    }
}