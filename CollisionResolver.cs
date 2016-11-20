using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk
{
    public class CollisionResolver
    {
        private const int StuckDistance = 20;
        private const int resolveSteps = 5;

        public List<Point> history { get; set; }
        public bool isResolving { get; set; }
        public int resolveCount { get; set; }
        public int currStrafe { get; set; }

        public CollisionResolver()
        {
            history = new List<Point>();
            isResolving = false;
            resolveCount = CollisionResolver.resolveSteps;
            currStrafe = 0;
        }

        public void RecordAndAnalyzeStep(Wizard self, Move move)
        {
            history.Add(new Point(self.X, self.Y));

            if (history.Count > 10)
            {
                var oldPoint = history[history.Count - 10];
                if (Helpers.Helpers.DistTo(self.X, self.Y, oldPoint.X, oldPoint.Y) < StuckDistance)
                {
                    if (isResolving == false && resolveCount <= 0)
                    {
                        isResolving = true;
                        resolveCount = resolveSteps;
                    }
                }
            }
            if (isResolving)
            {
                if (resolveCount <= 0)
                {
                    resolveCount = resolveSteps;
                    isResolving = false;
                    currStrafe = 0;
                }
                if (currStrafe == 0)
                {
                    currStrafe = Math.Sign(Helpers.Helpers.rand.Next(-1, 2));
                }
                move.StrafeSpeed = 1000 * currStrafe;
                resolveCount--;
            }
            else
            {
                resolveCount--;
            }
        }
    }
}