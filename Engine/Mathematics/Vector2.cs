namespace MinigameIdle.Engine.Mathematics
{
    public struct Vector2 : IEquatable<Vector2>
    {
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 Zero => new(0, 0);

        public static Vector2 One => new(1, 1);

        public static Vector2 Half => new(0.5f, 0.5f);

        public static Vector2 Left => new(-1, 0);

        public static Vector2 Right => new(1, 0);

        public static Vector2 Up => new(0, -1);

        public static Vector2 Down => new(0, 1);

        public float X { get; }

        public float Y { get; }

        public float Length => (float)Math.Sqrt((X * X) + (Y * Y));

        public static float CrossProduct(Vector2 v1, Vector2 v2)
        {
            return (v1.X * v2.Y) - (v2.X * v1.Y);
        }

        public static float CrossProduct(Vector2 sub, Vector2 v1, Vector2 v2)
        {
            return CrossProduct(v1 - sub, v2 - sub);
        }

        bool IEquatable<Vector2>.Equals(Vector2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public Vector2 Rotate(Vector2 origin, float r)
        {
            r *= (float)Math.PI / 180;

            return new Vector2(
                (float)((Math.Cos(r) * (X - origin.X)) - (Math.Sin(r) * (Y - origin.Y))),
                (float)((Math.Sin(r) * (X - origin.X)) + (Math.Cos(r) * (Y - origin.Y)))) + origin;
        }

        public Vector2 Normalize()
        {
            float len = Length;

            if (len == 0)
            {
                return this;
            }

            return new Vector2(X / len, Y / len);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Vector2 vec)
            {
                return ((IEquatable<Vector2>)this).Equals(vec);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return new { X, Y }.GetHashCode();
        }

        public override string ToString()
            => $"(X: {X}, Y: {Y})";

        public static implicit operator Vector2(PointF self) => new(self.X, self.Y);
        public static implicit operator PointF(Vector2 self) => new(self.X, self.Y);

        public static implicit operator Vector2(Point self) => new(self.X, self.Y);
        public static explicit operator Point(Vector2 self) => new((int)self.X, (int)self.Y);

        public static implicit operator Vector2(SizeF self) => new(self.Width, self.Height);
        public static implicit operator SizeF(Vector2 self) => new(self.X, self.Y);

        public static implicit operator Vector2(Size self) => new(self.Width, self.Height);
        public static explicit operator Size(Vector2 self) => new((int)self.X, (int)self.Y);

        public static bool operator ==(Vector2 l, Vector2 r) => ((IEquatable<Vector2>)l).Equals(r);
        public static bool operator !=(Vector2 l, Vector2 r) => !(l == r);

        public static Vector2 operator -(Vector2 l, Vector2 r) => new(l.X - r.X, l.Y - r.Y);
        public static Vector2 operator -(Vector2 self) => Zero - self;
        public static Vector2 operator +(Vector2 l, Vector2 r) => new(l.X + r.X, l.Y + r.Y);
        public static Vector2 operator /(Vector2 l, Vector2 r) => new(l.X / r.X, l.Y / r.Y);
        public static Vector2 operator *(Vector2 l, Vector2 r) => new(l.X * r.X, l.Y * r.Y);

        public static Vector2 operator /(Vector2 l, float r) => new(l.X / r, l.Y / r);
        public static Vector2 operator *(Vector2 l, float r) => new(l.X * r, l.Y * r);
    }
}
