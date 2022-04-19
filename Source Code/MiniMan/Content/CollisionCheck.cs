//using System;
//using System.Collections.Generic;
//using Microsoft.Xna.Framework;

//namespace Chapter6Game.Content
//{
//    public static class CollisionCheck
//    {
//        public static bool AABB(Rectangle A, Rectangle B)
//        {
//            return A.Left < B.Right
//            && A.Right > B.Left &&
//            A.Top < B.Bottom && A.Bottom > A.Top;
//        }


//        public static float Distance(this Circle A, Circle B)
//        {
//            //Gets distance between X and Y Coordinates of Each Circle
//            int dx = A.Center.X - B.Center.X;
//            int dy = A.Center.Y - B.Center.Y;

//            //Uses Pythagorean theorem to calculate distance between the 2 circles
//            return (float)Math.Sqrt((dx * dx) + (dy * dy));
//        }

//        public static bool CircleCollision(Circle A, Circle B)
//        {
//            //Gets sum of Radii to detect collision
//            int radii = A.Radius + B.Radius;

//            // Gets Distance from center of each circle
//            float distance = Distance(A, B);
//            // If distance is less than radii sum, it will collide
//            return distance < radii;
//        }

//        public static bool CircleRectangleCollision(Circle A, Rectangle B)
//        {
//            float distX = A.Center.X - B.X;
//            float distY = A.Center.Y - B.Y;

//            float distance =  (float)Math.Sqrt((distX * distX) + (distY * distY));

//            return distance < A.Radius;
//        }


//    }
//}
