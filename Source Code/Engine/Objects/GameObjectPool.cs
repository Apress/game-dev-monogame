using System;
using System.Collections.Generic;

namespace Engine2D.Objects
{
    public interface IGameObjectPool<T> where T : BaseGameObject
    {
        List<T> ActiveObjects { get; }

        T GetOrCreate(Func<T> createNbObjectFn);

        void DeactivateObject(T gameObject, Action<T> postDeactivateFn);
        void DeactivateAllObjects(Action<T> postDeactivateFn);
        void DeactivateAllObjects();
    }

    public class GameObjectPool<T> : IGameObjectPool<T> where T : BaseGameObject
    {
        private LinkedList<T> _activePool = new LinkedList<T>();
        private LinkedList<T> _inactivePool = new LinkedList<T>();

        public List<T> ActiveObjects 
        { 
            get 
            {
                var list = new List<T>();
                foreach (var gameObject in _activePool)
                {
                    list.Add(gameObject);
                }

                return list;
            } 
        }

        public GameObjectPool() { }

        public T GetOrCreate(Func<T> createNbObjectFn)
        {
            T activatedObject;

            if (_inactivePool.Count > 0)
            {
                var gameObject = _inactivePool.First.Value;
                Debug.Instance.Log($"reusing old instance of {gameObject}");
                gameObject.Initialize();
                gameObject.Activate();
                activatedObject = gameObject;

                _activePool.AddLast(gameObject);
                _inactivePool.RemoveFirst();
            }
            else
            {
                var gameObject = createNbObjectFn();
                Debug.Instance.Log($"creating new instance of {gameObject}");
                gameObject.Activate();
                activatedObject = gameObject;

                _activePool.AddLast(gameObject);
            }

            return activatedObject;
        }

        public void DeactivateObject(T gameObject, Action<T> postDeactivateFn)
        {
            var activeObject = _activePool.Find(gameObject);
            if (activeObject != null)
            {
                gameObject.Deactivate();

                _activePool.Remove(gameObject);
                _inactivePool.AddLast(gameObject);
            }

            postDeactivateFn(gameObject);
        }

        public void DeactivateObject(T gameObject)
        {
            DeactivateObject(gameObject, _ => { });
        }

        public void DeactivateAllObjects(Action<T> postDeactivateFn)
        {
            foreach (var gameObject in ActiveObjects)
            {
                DeactivateObject(gameObject, postDeactivateFn);
            }
        }

        public void DeactivateAllObjects()
        {
            foreach (var gameObject in ActiveObjects)
            {
                DeactivateObject(gameObject);
            }
        }

    }
}
