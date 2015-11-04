namespace ionix.Data
{
    using System;

    [Serializable]
    public enum SortDirection : int
    {
        Asc = 0,
        Desc
    }

    [Serializable]
    public enum ConditionOperator : int
    {
        Equals = 0,
        NotEquals = 1,
        GreaterThan = 2,
        LessThan = 3,
        GreaterThanOrEqualsTo = 4,
        LessThanOrEqualsTo = 5,
        In = 6,
        Between = 7,
        Contains = 8,
        StartsWith = 9,
        EndsWith = 10
    }
}
