using System.Collections.Generic;

namespace Example
{
    namespace Contracts
    {
        namespace Uniqueness
        {
            public interface ICommands
            {
                void AddUniqueValue(string category, object uniquevalue);
                void RemoveUniqueValue(string category, object uniquevalue);
            }
            public interface IEvents
            {
                void UniqueValueAdded(string category, object value);
                void UniqueValueRemoved(string category, object value);
            }

            public interface IState
            {
                bool HasUniqueValue(string category, object value);
            }
        }
    }

    public class UniquenessImpl : Contracts.Uniqueness.ICommands
    {

        Contracts.Uniqueness.IState state;
        public Contracts.Uniqueness.IEvents Modifier;

        public UniquenessImpl()
        {
            var stateImpl = new UniquenessState();
            this.state = stateImpl;
            this.Modifier = new UniquenessModifier(stateImpl);
        }

        public void AddUniqueValue(string category, object uniquevalue)
        {
            Guard.Against(state.HasUniqueValue(category, uniquevalue), "This value is not unique");
            Modifier.UniqueValueAdded(category, uniquevalue);
        }

        public void RemoveUniqueValue(string category, object uniquevalue)
        {
            Guard.That(state.HasUniqueValue(category, uniquevalue), "The unique value is missing");
            Modifier.UniqueValueRemoved(category, uniquevalue);
        }
    }

    public class UniquenessState : Contracts.Uniqueness.IState
    {
        public Dictionary<string, List<object>> UniqueValues { get; private set; }

        public UniquenessState()
        {
            UniqueValues = new Dictionary<string, List<object>>();
        }

        public bool HasUniqueValue(string category, object value)
        {
            return UniqueValues.ContainsKey(category) && UniqueValues[category].Contains(value);
        }
    }

    public class UniquenessModifier : Contracts.Uniqueness.IEvents
    {
        UniquenessState state;

        public UniquenessModifier(UniquenessState state)
        { this.state = state; }

        public void UniqueValueAdded(string category, object value)
        {
            if (!state.UniqueValues.ContainsKey(category))
                state.UniqueValues.Add(category, new List<object>());
            state.UniqueValues[category].Add(value);
        }

        public void UniqueValueRemoved(string category, object value)
        {
            state.UniqueValues[category].Remove(value);
            if (state.UniqueValues[category].Count == 0)
                state.UniqueValues.Remove(category);
        }
    }
}
