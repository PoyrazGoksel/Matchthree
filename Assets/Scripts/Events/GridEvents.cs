using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    public class GridEvents
    {
        public UnityAction<Bounds> GridLoaded;
        public UnityAction InputStart;
        public UnityAction InputStop;
    }
}