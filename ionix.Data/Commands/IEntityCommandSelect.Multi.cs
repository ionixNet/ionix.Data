﻿namespace ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;
    using Utils.Extensions;
    using Utils.Reflection;

    public enum MapBy
    {
        Name,
        Sequence
    }

    partial class EntityCommandSelect
    {
        private object[] MapMulti(IEntityMetaDataProvider provider, MapBy by, Type[] types, IDataReader dr)
        {
            object[] ret = new object[types.Length];
            Tuple<object, IEntityMetaData>[] arr = new Tuple<object, IEntityMetaData>[types.Length];
            for (int j = 0; j < types.Length; ++j)
            {
                Type type = types[j];
                arr[j] = new Tuple<object, IEntityMetaData>(Activator.CreateInstance(type), provider.CreateEntityMetaData(type));
            }

            switch (by)
            {
                case MapBy.Name://
                    for (int j = 0; j < types.Length; ++j)
                    {
                        var t = arr[j];
                        this.Map(t.Item1, t.Item2, dr, MapType.Query);
                        ret[j] = t.Item1;
                    }
                    break;
                case MapBy.Sequence://Çakışan Kolon İismlerinde tüm alanları çekerek ilgili problemin kısmen giderilebilmesi için eklendi.
                    int drIndex = 0;
                    for (int j = 0; j < types.Length; ++j)
                    {
                        IEntityMetaData metaData = arr[j].Item2;
                        object entity = arr[j].Item1;
                        foreach (PropertyMetaData md in metaData.Properties)
                        {
                            PropertyInfo pi = md.Property;
                            if (pi.GetSetMethod() != null)
                            {
                                try
                                {
                                    object dbValue = dr[drIndex];
                                    if (dbValue == DBNull.Value)
                                    {
                                        pi.SetValue(entity, null, null);
                                    }
                                    else
                                    {
                                        if (this.ConvertType)
                                            pi.SetValueSafely(entity, dbValue);
                                        else
                                            pi.SetValue(entity, dbValue, null);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new InvalidOperationException($"MapBy.Sequence is not ordered correctly.", ex);
                                }
                            }
                            ++drIndex;
                        }
                        ret[j] = entity;
                    }
                    break;
                default:
                    throw new NotSupportedException(by.ToString());
            }
            return ret;
        }

        private void CheckParams(IEntityMetaDataProvider provider, SqlQuery query, params Type[] types)
        {
            if (null == provider)
                throw new ArgumentNullException(nameof(provider));
            if (null == query && query.IsEmpty())
                throw new ArgumentNullException(nameof(query));
            if (types.IsEmptyList())
                throw new ArgumentNullException(nameof(types));
        }
        protected internal virtual object[] QueryTemplateSingle(IEntityMetaDataProvider provider, SqlQuery query, MapBy by, params Type[] types)
        {
            this.CheckParams(provider, query, types);

            IDataReader dr = null;
            try
            {
                dr = this.DataAccess.CreateDataReader(query, CommandBehavior.SingleRow);

                if (dr.Read())
                {
                    return this.MapMulti(provider, by, types, dr);
                }
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return null;
        }

        protected internal virtual IList<object[]> QueryTemplate(IEntityMetaDataProvider provider, SqlQuery query, MapBy by, params Type[] types)
        {
            this.CheckParams(provider, query, types);

            List<object[]> ret = new List<object[]>();
            IDataReader dr = null;
            try
            {
                dr = this.DataAccess.CreateDataReader(query, CommandBehavior.Default);

                while (dr.Read())
                {
                    ret.Add(this.MapMulti(provider, by, types, dr));
                }
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return ret;
        }


        public Tuple<TEntity1, TEntity2> QuerySingle<TEntity1,TEntity2>(IEntityMetaDataProvider provider,  SqlQuery query, MapBy by)
        {
            var result = this.QueryTemplateSingle(provider, query, by, typeof (TEntity1), typeof (TEntity2));
            if (null != result)
            {
                return new Tuple<TEntity1, TEntity2>((TEntity1)result[0], (TEntity2)result[1]);
            }
            return null;
        }
        public Tuple<TEntity1, TEntity2, TEntity3> QuerySingle<TEntity1, TEntity2, TEntity3>(IEntityMetaDataProvider provider, SqlQuery query, MapBy by)
        {
            var result = this.QueryTemplateSingle(provider, query, by, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3));
            if (null != result)
            {
                return new Tuple<TEntity1, TEntity2, TEntity3>((TEntity1)result[0], (TEntity2)result[1], (TEntity3)result[2]);
            }
            return null;
        }
        public Tuple<TEntity1, TEntity2, TEntity3, TEntity4> QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4>(IEntityMetaDataProvider provider, SqlQuery query, MapBy by)
        {
            var result = this.QueryTemplateSingle(provider, query, by, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4));
            if (null != result)
            {
                return new Tuple<TEntity1, TEntity2, TEntity3, TEntity4>((TEntity1)result[0], (TEntity2)result[1], (TEntity3)result[2], (TEntity4)result[3]);
            }
            return null;
        }
        public Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5> QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(IEntityMetaDataProvider provider, SqlQuery query, MapBy by)
        {
            var result = this.QueryTemplateSingle(provider, query, by, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4), typeof(TEntity5));
            if (null != result)
            {
                return new Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>((TEntity1)result[0], (TEntity2)result[1], (TEntity3)result[2], (TEntity4)result[3], (TEntity5)result[4]);
            }
            return null;
        }
        public Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6> QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(IEntityMetaDataProvider provider, SqlQuery query, MapBy by)
        {
            var result = this.QueryTemplateSingle(provider, query, by, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4), typeof(TEntity5), typeof(TEntity6));
            if (null != result)
            {
                return new Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>((TEntity1)result[0], (TEntity2)result[1], (TEntity3)result[2], (TEntity4)result[3], (TEntity5)result[4], (TEntity6)result[5]);
            }
            return null;
        }
        public Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7> QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(IEntityMetaDataProvider provider, SqlQuery query, MapBy by)
        {
            var result = this.QueryTemplateSingle(provider, query, by, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4), typeof(TEntity5), typeof(TEntity6), typeof(TEntity7));
            if (null != result)
            {
                return new Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>((TEntity1)result[0], (TEntity2)result[1], (TEntity3)result[2], (TEntity4)result[3], (TEntity5)result[4], (TEntity6)result[5], (TEntity7)result[6]);
            }
            return null;
        }



        public IList<Tuple<TEntity1, TEntity2>> Query<TEntity1, TEntity2>(IEntityMetaDataProvider provider, SqlQuery query, MapBy by)
        {
            var result = this.QueryTemplate(provider, query, by, typeof(TEntity1), typeof(TEntity2));
            var ret = new List<Tuple<TEntity1, TEntity2>>(result.Count);
            if (result.Count != 0)
            {        
                foreach (object[] arr in result)
                {
                    ret.Add(new Tuple<TEntity1, TEntity2>((TEntity1)arr[0], (TEntity2)arr[1]));
                }
            }
            return ret;
        }
        public IList<Tuple<TEntity1, TEntity2, TEntity3>> Query<TEntity1, TEntity2, TEntity3>(IEntityMetaDataProvider provider, SqlQuery query, MapBy by)
        {
            var result = this.QueryTemplate(provider, query, by, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3));
            var ret = new List<Tuple<TEntity1, TEntity2, TEntity3>>(result.Count);
            if (result.Count != 0)
            {
                foreach (object[] arr in result)
                {
                    ret.Add(new Tuple<TEntity1, TEntity2, TEntity3>((TEntity1)arr[0], (TEntity2)arr[1], (TEntity3)arr[2]));
                }
            }
            return ret;
        }
        public IList<Tuple<TEntity1, TEntity2, TEntity3, TEntity4>> Query<TEntity1, TEntity2, TEntity3, TEntity4>(IEntityMetaDataProvider provider, SqlQuery query, MapBy by)
        {
            var result = this.QueryTemplate(provider, query, by, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4));
            var ret = new List<Tuple<TEntity1, TEntity2, TEntity3, TEntity4>>(result.Count);
            if (result.Count != 0)
            {
                foreach (object[] arr in result)
                {
                    ret.Add(new Tuple<TEntity1, TEntity2, TEntity3, TEntity4>((TEntity1)arr[0], (TEntity2)arr[1], (TEntity3)arr[2], (TEntity4)arr[3]));
                }
            }
            return ret;
        }
        public IList<Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>> Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(IEntityMetaDataProvider provider, SqlQuery query, MapBy by)
        {
            var result = this.QueryTemplate(provider, query, by, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4), typeof(TEntity5));
            var ret = new List<Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>>(result.Count);
            if (result.Count != 0)
            {
                foreach (object[] arr in result)
                {
                    ret.Add(new Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>((TEntity1)arr[0], (TEntity2)arr[1], (TEntity3)arr[2], (TEntity4)arr[3], (TEntity5)arr[4]));
                }
            }
            return ret;
        }
        public IList<Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>> Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(IEntityMetaDataProvider provider, SqlQuery query, MapBy by)
        {
            var result = this.QueryTemplate(provider, query, by, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4), typeof(TEntity5), typeof(TEntity6));
            var ret = new List<Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>>(result.Count);
            if (result.Count != 0)
            {
                foreach (object[] arr in result)
                {
                    ret.Add(new Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>((TEntity1)arr[0], (TEntity2)arr[1], (TEntity3)arr[2], (TEntity4)arr[3], (TEntity5)arr[4], (TEntity6)arr[5]));
                }
            }
            return ret;
        }
        public IList<Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>> Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(IEntityMetaDataProvider provider, SqlQuery query, MapBy by)
        {
            var result = this.QueryTemplate(provider, query, by, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4), typeof(TEntity5), typeof(TEntity6));
            var ret = new List<Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>>(result.Count);
            if (result.Count != 0)
            {
                foreach (object[] arr in result)
                {
                    ret.Add(new Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>((TEntity1)arr[0], (TEntity2)arr[1], (TEntity3)arr[2], (TEntity4)arr[3], (TEntity5)arr[4], (TEntity6)arr[5], (TEntity7)arr[6]));
                }
            }
            return ret;
        }
    }
}
