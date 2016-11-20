using System;
using System.Collections.Generic;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Helpers;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk
{
    public class Point : Unit
    {
        public Point(double x, double y) : base(-1, x, y, 0, 0, 0, Faction.Other)
        {
            this.X = x;
            this.Y = y;
        }
        public new double X;
        public new double Y;
    }
    public sealed class MyStrategy : IStrategy
    {
        //public List<Point> history = new List<Point>();
        //public int changeTick = 0;
        //public double speed=0;

        public static bool isInitialized = false;
        public static PathDetector pathDetector;
        public static CollisionResolver collisionResolver;

        public void Move(Wizard self, World world, Game game, Move move)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                Helpers.Helpers.rand = new Random();
                pathDetector = new PathDetector();
                collisionResolver = new CollisionResolver();
            }
            else
            {
                var nextPoint = pathDetector.GetNextPoint(self, world, game, move);
                move.Turn = -1 * self.GetAngleTo(nextPoint.X, nextPoint.Y);
                move.Speed = game.WizardForwardSpeed;
                collisionResolver.RecordAndAnalyzeStep(self, move);
                move.Action = ActionType.MagicMissile;
            }
        }
    }
}