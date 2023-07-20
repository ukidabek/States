using System.Collections;
using Utilities.General;

namespace Utilities.States
{
    public abstract class CoroutineStateLogic : StateLogic
    {
        protected CoroutineManager m_coroutineManager;

        protected virtual void Awake()
        {
        }

        public override void Activate()
        {
            base.Activate();
            
            if(m_coroutineManager == null)
				m_coroutineManager = new CoroutineManager(this);

			m_coroutineManager.Run(Coroutine());
        }

        public abstract IEnumerator Coroutine();
    }
}