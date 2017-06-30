namespace ionix.Data.MongoDB.Migration
{
	using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ExperimentalAttribute : Attribute
    {
    }
}