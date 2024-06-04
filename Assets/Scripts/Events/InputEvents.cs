
using Components;
using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    public class InputEvents
    {
        public UnityAction<Tile, Vector3> MouseDownGrid;
        public UnityAction<Vector3> MouseUpGrid;
    }
}