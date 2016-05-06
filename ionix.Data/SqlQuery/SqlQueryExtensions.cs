namespace ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;

    public static class SqlQueryExtensions
    {
        //i.e. "update Perseon set Name=@0, No=@1 where Id=@2".ToQuery("Memo", 12, 1);
        public static SqlQuery ToQuery(this string sql, params object[] parameters)
        {
            SqlQuery query = new SqlQuery();
            query.Sql(sql, parameters);
            return query;
        }
        public static SqlQuery ToQuery(this string sql)
        {
            return new SqlQuery(sql);
            // return ConversionExtensions.ToQuery(sql, null);
        }

        //Arama Yapısından Sık Kullanılan İç Sorgular İçin Eklendi
        public static SqlQuery ToInnerQuery(this SqlQuery query, string alias)
        {
            if (null != query)
            {
                if (String.IsNullOrEmpty(alias))
                    alias = "T";

                SqlQuery inner = new SqlQuery();
                inner.Text.Append("SELECT * FROM (");
                inner.Combine(query);
                inner.Text.Append(") ");
                inner.Text.Append(alias);
                return inner;
            }
            return null;
        }
        public static SqlQuery ToInnerQuery(this SqlQuery query)
        {
            return ToInnerQuery(query, "T");
        }


        //public static SqlQuery Where(this SqlQuery query, ExpandoObject where, char prefix)
        //{
        //    if (null != query && null != where)
        //    {
        //        IDictionary<string, object> dic = where;
        //        FilterCriteriaList criterias = new FilterCriteriaList(prefix);
        //        foreach (KeyValuePair<string, object> kvp in dic)
        //        {
        //            criterias.Add(kvp.Key, null, ConditionOperator.Equals, kvp.Value);
        //        }

        //        query.Combine(criterias.ToQuery());
        //    }
        //    return query;
        //}
    }
}
