﻿namespace ionix.Data.SQLite
{
    using System;

    internal static class GlobalInternal
    {
        internal const char Prefix = '@';

        internal static string BeginStatement = "BEGIN;" + Environment.NewLine;
        internal static string EndStatement = Environment.NewLine + "END;";

    }

    internal sealed class ValueSetter : DbValueSetter
    {
        internal static readonly ValueSetter Instance = new ValueSetter();

        private ValueSetter()
        {

        }

        public override char Prefix => GlobalInternal.Prefix;
    }
}
