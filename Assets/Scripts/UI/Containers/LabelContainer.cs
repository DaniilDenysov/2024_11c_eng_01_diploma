using DesignPatterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace UI
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="I">item</typeparam>
    /// <typeparam name="T">self</typeparam>
    public abstract class LabelContainer<I,T> : Singleton<T> where I : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] protected Transform container;
        [SerializeField] protected List<I> items;

        public virtual void Add (I item)
        {
            Vector3 scaleBefore = item.transform.localScale;
            item.transform.SetParent(container);
            item.transform.localScale = scaleBefore;
            items.Add(item);
        }

        public List<I> GetItems ()
        {
            items.RemoveAll((i)=>i==null);
            return items;
        }
        
        public virtual bool Remove(I item)
        {
            item.transform.SetParent(null);
            return items.Remove(item);
        }

        public int GetAmount()
        {
            return items.Count;
        }
    }
}
