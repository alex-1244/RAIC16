using System;
using System.Collections.Generic;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Helpers;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk
{
    public class Point
    {
        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        public double X;
        public double Y;
    }
    public sealed class MyStrategy : IStrategy
    {
        //public List<Point> history = new List<Point>();
        //public int changeTick = 0;
        //public double speed=0;

        public static bool isInitialized = false;
        public static Collider collisionDetector;

        public void Move(Wizard self, World world, Game game, Move move)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                collisionDetector = new Collider();
            }
            else
            {
                var nextPoint = collisionDetector.GetNextPoint(self, world, game, move);
                move.Turn=
            }
            //if (world.TickIndex <= changeTick)
            //{
            //    move.StrafeSpeed = speed;
            //}
            //history.Add(new Point() { x = self.X, y = self.Y });
            //if (history.Count > 20)
            //{
            //    if (self.GetDistanceTo(history[history.Count - 10].x, history[history.Count - 10].y) < 5)
            //    {
            //        changeTick = world.TickIndex + 30;
            //        move.StrafeSpeed = game.WizardStrafeSpeed;
            //        speed = game.WizardStrafeSpeed;
            //    }
            //}
            var angleTo = self.GetAngleTo(4000, 0);
            var selfAngle = self.Angle;
            var neededAngle = -1 * (selfAngle - angleTo) + 0.25;
            if (!(Math.Abs(selfAngle - angleTo) < 0.5))
            {
                move.Turn = neededAngle;
            }

            move.Speed = game.WizardForwardSpeed;
            //move.StrafeSpeed = game.WizardStrafeSpeed;
            //move.Turn = game.WizardMaxTurnAngle;
            move.Action = ActionType.MagicMissile;
        }
    }
}