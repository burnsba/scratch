public class StateTransitionCriteria
{
    public Func<object, bool> Predicate { get; set; }
    public State NextState { get; set; }
}

public class State
{
    private List<StateTransitionCriteria> _acceptCriteria = new List<StateTransitionCriteria>();
    
    public bool OptionDefaultSelfNext { get; set; }
    public bool CanTransition { get; set; }
    
    public State()
    {
        OptionDefaultSelfNext = true;
        CanTransition = true;
    }
    
    public void AddCritera(StateTransitionCriteria criteria)
    {
        _acceptCriteria.Add(criteria);
    }

    public State TryTransition(Object arg)
    {
        if (!CanTransition)
        {
            return this;
        }
        
        foreach (var c in _acceptCriteria)
        {
            if (c.Predicate(arg))
            {
                return c.NextState;
            }
        }
        
        if (OptionDefaultSelfNext)
        {
            return this;
        }
        else
        {
            return null;
        }
    }
    
    public string Value { get; set; }
    
    public override string ToString()
    {
        return Value;
    }
}

public class FiniteStateMachine
{
    private bool _hasBegun = false;
    
    public State BeginState { get; set; }
    public State CurrentState { get; set; }
    
    public bool TryTransition(Object arg)
    {
        if (!_hasBegun)
        {
            Reset();
            _hasBegun = true;
        }
        
        if (Object.ReferenceEquals(null, CurrentState))
        {
            return false;
        }
        
        State prior = CurrentState;
        
        CurrentState = CurrentState.TryTransition(arg);
        
        if (Object.ReferenceEquals(null, CurrentState))
        {
            return false;
        }
        
        if (Object.ReferenceEquals(prior, CurrentState))
        {
            return false;
        }
        
        return true;
    }
    
    public void Reset()
    {
        CurrentState = BeginState;
    }
}
