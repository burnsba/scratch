// see sm.png for graphical representation

var fsm = new FiniteStateMachine();
var s1 = new State();
s1.Value = "state 1";

var s2 = new State();
s2.Value = "state 2";

var s3 = new State();
s3.Value = "fin";

s3.CanTransition = false;

var s1_c = new StateTransitionCriteria();
s1_c.NextState = s2;
s1_c.Predicate = x => (int)x == 1;
s1.AddCritera(s1_c);

var s2_c = new StateTransitionCriteria();
s2_c.NextState = s3;
s2_c.Predicate = x => (int)x == 2;
s2.AddCritera(s2_c);

var s2_c2 = new StateTransitionCriteria();
s2_c2.NextState = s1;
s2_c2.Predicate = x => (int)x == 1;
s2.AddCritera(s2_c2);

fsm.BeginState = s1;

/*
. fsm.BeginState = s1;
> fsm.CurrentState
null
> fsm.TryTransition(1)
true
> fsm.CurrentState
[state 2]
> fsm.TryTransition(1)
true
> fsm.CurrentState
[state 1]
> fsm.TryTransition(1)
true
> fsm.CurrentState
[state 2]
> fsm.TryTransition(2)
true
> fsm.CurrentState
[fin]
> 
*/
