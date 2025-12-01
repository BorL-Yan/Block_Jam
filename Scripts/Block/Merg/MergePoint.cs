using System;
using UnityEngine;


    public class MergePoint : MonoBehaviour
    {
        public bool Active { get; set; }
        public Vector3 Pos => transform.position;
        public BlockColor BlockColor {get; set;}
        public IMovable Movable {get; set;}
        public IDisposable Disposable {get; set;}
        public bool IsStanding { get; set;}
        public Action<MergePoint> OnChangePosition;

        public void CopyFrom(MergePoint other)
        {
            Active = true;
            BlockColor = other.BlockColor;
            Disposable = other.Disposable;
            Movable = other.Movable;
            IsStanding = other.IsStanding;  
            OnChangePosition = other.OnChangePosition;
            
        }

        public void Clear()
        {
            Active = false;
            BlockColor = default;
            Disposable = null;
            Movable = null;
            IsStanding = false;
            OnChangePosition = null;
        }

    }
