namespace BaseGameLogic.States
{
    public class StateInterfaceHandler
    {
        public IState CurrentState { get; private set; }
        public IOnSleep OnSleepInterface { get; private set; }
        public IOnAwake OnAwakeInterface { get; private set; }
        public IOnUpdate OnUpdateInterface { get; private set; }
        public IOnLateUpdate OnLateUpdateInterface { get; private set; }
        public IOnFixedUpdate OnFixedUpdateInterface { get; private set; }

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