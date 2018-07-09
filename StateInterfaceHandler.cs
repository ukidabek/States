namespace BaseGameLogic.States
{
    public class StateInterfaceHandler
    {
        public IState CurrentState { get; private set; }
        private IOnSleep OnSleepInterface { get; set; }
        private IOnAwake OnAwakeInterface { get; set; }
        private IOnUpdate OnUpdateInterface { get; set; }
        private IOnLateUpdate OnLateUpdateInterface { get; set; }
        private IOnFixedUpdate OnFixedUpdateInterface { get; set; }

        public StateInterfaceHandler(IState state)
        {
            CurrentState = state;
            OnSleepInterface = GetInterface<IOnSleep>(state);
            OnAwakeInterface = GetInterface<IOnAwake>(state);
            OnUpdateInterface = GetInterface<IOnUpdate>(state);
            OnLateUpdateInterface = GetInterface<IOnLateUpdate>(state);
            OnFixedUpdateInterface = GetInterface<IOnFixedUpdate>(state);
        }

        private T GetInterface<T>(object state) where T : class
        {
            if (state is T)
                return state as T;

            return null;
        }

        public void Enter()
        {
            CurrentState.OnEnter();
        }

        public void Exit()
        {
            CurrentState.OnExit();
        }

        public void Sleep()
        {
            if (OnSleepInterface != null)
                OnSleepInterface.OnSleep();
        }

        public void Awake()
        {
            if (OnAwakeInterface != null)
                OnAwakeInterface.OnAwake();
        }

        public void Update()
        {
            if (OnUpdateInterface != null)
                OnUpdateInterface.OnUpdate();
        }

        public void LateUpdate()
        {
            if (OnLateUpdateInterface != null)
                OnLateUpdateInterface.OnLateUpdate();
        }

        public void FixedUpdate()
        {
            if (OnFixedUpdateInterface != null)
                OnFixedUpdateInterface.OnFixedUpdate();
        }
    }
}