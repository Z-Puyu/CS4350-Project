using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataStructuresForUnity.Runtime.Utilities {
    [DisallowMultipleComponent, RequireComponent(typeof(BoxCollider2D))]
    public sealed class BoundingRect : MonoBehaviour {
        public enum Alignment {
            Centre,
            Top,
            Bottom,
            Left,
            Right,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }
        
        private Transform Transform { get; set; }
        private BoxCollider2D BoxCollider { get; set; }
        private Lazy<(Vector3 centre, float radius)> CachedBoundingCircle { get; set; }
        public Bounds Bounds => this.BoxCollider.bounds;
        public float Width => this.Bounds.size.x;
        public float Height => this.Bounds.size.y;

        // World-space coordinates (Bounds already in world space)
        public Vector3 Centre => this.Bounds.center;
        public Vector3 TopCentre => this.Centre + this.Bounds.extents.y * Vector3.up;
        public Vector3 BottomCentre => this.Centre + this.Bounds.extents.y * Vector3.down;
        public Vector3 LeftCentre => this.Centre + this.Bounds.extents.x * Vector3.left;
        public Vector3 RightCentre => this.Centre + this.Bounds.extents.x * Vector3.right;
        public Vector3 TopLeft => this.TopCentre + this.Bounds.extents.x * Vector3.left;
        public Vector3 TopRight => this.TopCentre + this.Bounds.extents.x * Vector3.right;
        public Vector3 BottomLeft => this.BottomCentre + this.Bounds.extents.x * Vector3.left;
        public Vector3 BottomRight => this.BottomCentre + this.Bounds.extents.x * Vector3.right;

        public (Vector3 centre, float radius) BoundingCircle => this.CachedBoundingCircle.Value;

        private void Awake() {
            this.Transform = this.transform;
            this.BoxCollider = this.GetComponent<BoxCollider2D>();
            this.CachedBoundingCircle = new Lazy<(Vector3 centre, float radius)>(this.GetBoundingCircle);
        }

        public void ResizeTo(double width, double height) {
            double widthScaler = width / this.Bounds.size.x;
            double heightScaler = height / this.Bounds.size.y;
            double scaler = Math.Min(widthScaler, heightScaler);
            Vector3 currScale = this.Transform.localScale;
            float x = currScale.x * (float)scaler;
            float y = currScale.y * (float)scaler;
            this.Transform.localScale = new Vector3(x, y, currScale.z);
            this.CachedBoundingCircle = new Lazy<(Vector3 centre, float radius)>(this.GetBoundingCircle);
        }

        public void ResizeTo(BoundingRect other) {
            this.ResizeTo(other.Width, other.Height);
        }
        
        public void AlignTo(BoundingRect other, Alignment alignment = Alignment.Centre) {
            Vector3 translation = alignment switch {
                Alignment.Centre => other.Centre - this.Centre,
                Alignment.Top => other.TopCentre - this.TopCentre,
                Alignment.Bottom => other.BottomCentre - this.BottomCentre,
                Alignment.Left => other.LeftCentre - this.LeftCentre,
                Alignment.Right => other.RightCentre - this.RightCentre,
                Alignment.TopLeft => other.TopLeft - this.TopLeft,
                Alignment.TopRight => other.TopRight - this.TopRight,
                Alignment.BottomLeft => other.BottomLeft - this.BottomLeft,
                Alignment.BottomRight => other.BottomRight - this.BottomRight,
                var _ => throw new ArgumentException("Invalid alignment", nameof(alignment))
            };
            
            this.Transform.position += translation;
        }

        public IEnumerable<Vector3> EvenlyDistributeOnBoundingCircle(int n) {
            if (n <= 0) {
                return Enumerable.Empty<Vector3>();
            }

            List<Vector3> points = new List<Vector3> {
                this.BoundingCircle.centre + this.BoundingCircle.radius * Vector3.up
            };

            float step = 360f / n;
            for (int i = 1; i < n; i += 1) {
                Vector3 dir = Quaternion.Euler(0, 0, step * i) * Vector3.up;
                points.Add(this.BoundingCircle.centre + this.BoundingCircle.radius * dir);
            }

            return points;
        }

        public IEnumerable<Vector3> SymmetricallyDistributeOnBoundingCircle(int n, Vector3 axis, float angleExtent) {
            if (n < 0 || angleExtent < 0) {
                return Enumerable.Empty<Vector3>();
            }

            angleExtent %= 180f;
            List<Vector3> points = new List<Vector3> {
                this.BoundingCircle.centre + this.BoundingCircle.radius * axis.normalized
            };

            if (Mathf.Approximately(angleExtent, 0)) {
                return points;
            }

            float step = angleExtent / n;
            for (int i = 1; i < n; i += 1) {
                Vector3 dir = Quaternion.Euler(0, 0, step * i) * axis;
                points.Add(this.BoundingCircle.centre + this.BoundingCircle.radius * dir);
            }

            return points;
        }

        private (Vector3 centre, float radius) GetBoundingCircle() {
            Vector3 centre = this.Bounds.center;
            double radius = Math.Sqrt(Math.Pow(this.Bounds.extents.x, 2) + Math.Pow(this.Bounds.extents.y, 2));
            return (centre, (float)radius);
        }
    }
}
