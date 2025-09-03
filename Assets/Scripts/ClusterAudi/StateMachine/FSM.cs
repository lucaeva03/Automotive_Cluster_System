using System.Collections.Generic;

namespace ClusterAudi {
    public class FSM<TState> where TState : IState
    {
        // Dizionario che associa Stato a Oggetto
        protected Dictionary<string, TState> _mystatesDictionary = new();

        // Stato attivo e stato precedente
        protected TState _currentState;
        protected TState _previousState;

        // Aggiunta stato
        public void AddState(string stateName, TState state)
        {
            _mystatesDictionary[stateName] = state;
        }

        // Rimozione stato
        public void RemoveState(string stateName)
        {
            if (_mystatesDictionary.ContainsKey(stateName))
            {
                _mystatesDictionary.Remove(stateName);
            }
        }

        // Cambio Stato
        public void GoTo(string newState)
        {
            if (_currentState != null)
            {
                _previousState = _currentState;
                _currentState.StateOnExit();
            }

            _currentState = _mystatesDictionary[newState];

            _currentState.StateOnEnter();
        }

        // Chiamato ogni frame
        public void UpdateState()
        {
            if(_currentState != null)
            {
                _currentState.StateOnUpdate(); //Aggiorna stato corrente
            }
        }

        // Restituisce stato corrente
        public TState GetCurrentState()
        {
            return _currentState;
        }
    }
}
