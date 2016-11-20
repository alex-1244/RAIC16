using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Helpers
{
    public class ClosestComparer : IComparer<PointWithHistAngle>
    {
        public Unit unit;
        public ClosestComparer(Unit unit)
        {
            this.unit = unit;
        }
        public int Compare(PointWithHistAngle a, PointWithHistAngle b)
        {
            var distA = Helpers.DistTo(a.Point.X, a.Point.Y, unit.X, unit.Y);
            var distB = Helpers.DistTo(b.Point.X, b.Point.Y, unit.X, unit.Y);
            return Math.Sign(distA - distB);
        }
    }

    public class PathDetector
    {
        public readonly int NumOfTurns = 20;
        public readonly int DEPTH = 20;

        public List<double> XHist { get; set; }
        public List<double> YHist { get; set; }
        public PathDetector()
        {
            XHist = new List<double>();
            YHist = new List<double>();
        }
        public Point GetNextPoint(Wizard self, World world, Game game, Move move)
        {
            double minDist = 10000;
            Unit closestEnemy = null;
            foreach (var building in world.Buildings.Where(x => x.Faction != self.Faction))
            {
                var distTo = self.GetDistanceTo(building.X, building.Y);
                if (distTo < minDist)
                {
                    minDist = distTo;
                    closestEnemy = building;
                }
            }
            closestEnemy = closestEnemy ?? new Point(4000, 0);

            var angle = self.GetAngleTo(closestEnemy.X, closestEnemy.Y);
            move.Turn = self.Angle + angle;
            List<PointWithHistAngle> possibleNextPoints = GetPossiblePoints(self, closestEnemy, game, null);
            for (int i = 0; i < DEPTH; i++)
            {
                possibleNextPoints = filterOutUnpossiblePoints(possibleNextPoints, game);
                foreach (var point in possibleNextPoints)
                {
                    possibleNextPoints.Concat(GetPossiblePoints(self, closestEnemy, game, point));
                }
            }
            var closestE = possibleNextPoints.OrderByDescending(x => x, new ClosestComparer(closestEnemy)).ToList()[0];
            var DO = true;
            while (DO)
            {
                if (closestE.PrevPoint == null)
                {
                    DO = false;
                }
                else if (closestE.PrevPoint.PrevPoint == null)
                {
                    DO = false;
                    closestE = closestE.PrevPoint;
                }
                else
                {
                    closestE = closestE.PrevPoint;
                }

            }
            return closestE.Point;
        }

        private List<PointWithHistAngle> filterOutUnpossiblePoints(List<PointWithHistAngle> possibleNextPoints, Game game)
        {
            return possibleNextPoints.Where(x => (x.Point.X > 0 && x.Point.X < game.MapSize && x.Point.Y > 0 && x.Point.Y < game.MapSize)).ToList();
        }

        private List<PointWithHistAngle> GetPossiblePoints(Wizard self, Unit closestEnemy, Game game, PointWithHistAngle prevPoint)
        {
            var X = prevPoint?.Point?.X ?? self.X;
            var Y = prevPoint?.Point?.Y ?? self.Y;
            var Angle = prevPoint?.Angle ?? self.Angle;

            var res = new List<PointWithHistAngle>();
            var maxDist = Math.Min(game.WizardForwardSpeed, Helpers.DistTo(closestEnemy.X, closestEnemy.Y, X, Y));
            var minAngle = Angle - game.WizardMaxTurnAngle;
            var maxAngle = Angle + game.WizardMaxTurnAngle;
            for (var i = 0; i < this.NumOfTurns; i++)
            {
                var angle = minAngle + (game.WizardMaxTurnAngle * 2 * i / this.NumOfTurns);
                Point nextPoint = new Point(X + Math.Sin(angle) * maxDist, Y + Math.Cos(angle) * maxDist);
                res.Add(new PointWithHistAngle()
                {
                    Point = nextPoint,
                    PrevPoint = prevPoint,
                    Angle = angle
                });
            }
            return res;
        }
    }
}
