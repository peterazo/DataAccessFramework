using System;
using System.Collections;
using System.Collections.Generic;

namespace DAF
{
    public interface IQuery
    {
        IQuery SetParameter<V>(string name, V val);

        int Execute();
    }

    public interface IQuery<T> : IQuery
    {
        IQuery<T> SetFirstFetched(int index);
        IQuery<T> SetFetchSize(int size);
        new IQuery<T> SetParameter<V>(string name, V val);

        IList<T> List();
        T First();
        T FirstOrDefault();

        void Clear();
    }
}
