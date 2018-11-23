using System;

namespace Scribs {
    public static class Utils {
        public static DateTime DateFromJs(long time) => new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(time);
    }
}