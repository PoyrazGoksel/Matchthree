using Zenject;

namespace Extensions.Unity.MonoHelper
{
    public abstract class EventListenerInstaller<T> : MonoInstaller<T> where T: MonoInstaller<T>
    {
        protected virtual void OnEnable() => RegisterEvents();

        protected virtual void OnDisable() => UnRegisterEvents();

        protected abstract void RegisterEvents();
        protected abstract void UnRegisterEvents();
    }
}