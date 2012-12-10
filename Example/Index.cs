using System.Collections.Generic;

namespace Example
{
    namespace Contracts
    {
        namespace Index
        {
            public interface ICommands
            {
                void AddUniqueValue(string indexName, object uniquevalue);
                void RemoveUniqueValue(string indexName, object uniquevalue);
            }
            public interface IHandleEvents
            {
                void ValueAdded(string indexName, object value);
                void ValueRemoved(string indexName, object value);
            }

            public interface IQuery
            {
                bool ContainsValue(string indexName, object value);
            }
        }
    }

    public class Index : Contracts.Index.ICommands
    {

        Contracts.Index.IQuery Query;
        public Contracts.Index.IHandleEvents Handle;

        public Index(Contracts.Index.IQuery Query,Contracts.Index.IHandleEvents Handle)
        {
            this.Query = Query;
            this.Handle = Handle;
        }

        public void AddUniqueValue(string indexName, object uniquevalue)
        {
            Guard.Against(Query.ContainsValue(indexName, uniquevalue), "This value is not unique");
            Handle.ValueAdded(indexName, uniquevalue);
        }

        public void RemoveUniqueValue(string indexName, object uniquevalue)
        {
            Guard.That(Query.ContainsValue(indexName, uniquevalue), "The unique value is missing");
            Handle.ValueRemoved(indexName, uniquevalue);
        }
    }

    public class IndexState 
    {
        public Dictionary<string, List<object>> IndexDictionary { get; private set; }

        public IndexState()
        {
            IndexDictionary = new Dictionary<string, List<object>>();
        }

    }

    public class IndexQueries : Contracts.Index.IQuery
    {
        IndexState state;

        public IndexQueries(IndexState state)
        {
            this.state = state;
        }

        public bool ContainsValue(string indexName, object value)
        {
            return state.IndexDictionary.ContainsKey(indexName) && state.IndexDictionary[indexName].Contains(value);
        }
    }

    public class IndexEventhandlers : Contracts.Index.IHandleEvents
    {
        IndexState state;

        public IndexEventhandlers(IndexState state)
        { this.state = state; }

        public void ValueAdded(string indexName, object value)
        {
            if (!state.IndexDictionary.ContainsKey(indexName))
                state.IndexDictionary.Add(indexName, new List<object>());
            state.IndexDictionary[indexName].Add(value);
        }

        public void ValueRemoved(string indexName, object value)
        {
            state.IndexDictionary[indexName].Remove(value);
            if (state.IndexDictionary[indexName].Count == 0)
                state.IndexDictionary.Remove(indexName);
        }
    }
}
