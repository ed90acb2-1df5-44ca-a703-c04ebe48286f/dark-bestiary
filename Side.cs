using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DarkBestiary
{
    [Flags]
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum Side
    {
        None = 0,
        Left = 1 << 1,
        Right = 1 << 2,
        Top = 1 << 3,
        Bottom = 1 << 4,
    }

    internal static class SideMethods
    {

        public static IEnumerable<Side> ToEnumerable(this Side side)
        {
            return new[] {Side.Top, Side.Right, Side.Bottom, Side.Left}
                .Where(s => side.HasFlag(s))
                .ToList();
        }
    }

}