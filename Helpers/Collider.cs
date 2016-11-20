using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Helpers
{
    public class Comp : IComparer<PointWithHistAngle>
    {
        public LivingUnit unit;
        public Comp(LivingUnit unit)
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

    public class Collider
    {
        public readonly int NumOfTurns = 20;
        public readonly int DEPTH = 20;

        public List<double> XHist { get; set; }
        public List<double> YHist { get; set; }
        public Collider()
        {
            XHist = new List<double>();
            YHist = new List<double>();
        }
        public Point GetNextPoint(Wizard self, World world, Game game, Move move)
        {
            double minDist = 10000;
            LivingUnit closestEnemy = null;
            foreach (var building in world.Buildings.Where(x => x.Faction != self.Faction))
            {
                var distTo = self.GetDistanceTo(building.X, building.Y);
                if (distTo < minDist)
                {
                    minDist = distTo;
                    closestEnemy = building;
                }
            }

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
            var closestE = possibleNextPoints.OrderByDescending(x => x, new Comp(closestEnemy)).ToList()[0];// Helpers.DistTo(x.Point.X, x.Point.Y, closestEnemy.X, closestEnemy.Y));
            while (closestE.PrevPoint != null)
            {
                closestE = closestE.PrevPoint;
            }
            return closestE.Point;
        }

        private List<PointWithHistAngle> filterOutUnpossiblePoints(List<PointWithHistAngle> possibleNextPoints, Game game)
        {
            return possibleNextPoints.Where(x => (x.Point.X > 0 && x.Point.X < game.MapSize && x.Point.Y > 0 && x.Point.Y < game.MapSize)).ToList();
        }

        private List<PointWithHistAngle> GetPossiblePoints(Wizard self, LivingUnit closestEnemy, Game game, PointWithHistAngle prevPoint)
        {
            var X = prevPoint?.Point?.X ?? self.X;
            var Y = prevPoint?.Point?.Y ?? self.Y;
            var Angle = prevPoint?.Angle ?? self.Angle;

            var res = new List<PointWithHistAngle>();
            var maxDist = Math.Max(game.WizardForwardSpeed, Helpers.DistTo(closestEnemy.X, closestEnemy.Y, X, Y));
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

        //rx = x0 - xz
        //ry = y0 - y;
        //c = Math.cos(alpha);
        //s = Math.sin(alpha);
        //x1 = x + rx* c - ry* s;
        //y1 = y + rx* s + ry* c;

        //internal bool CheckCollision(Wizard self, World world, Game game, Move move)
        //{
        //    XHist.Add(self.X);
        //    YHist.Add(self.Y);
        //    if (XHist.Count > 10)
        //    {
        //        if(self.GetDistanceTo(XHist[XHist.Count-10], YHist[YHist.Count - 10]) < 20)
        //        {

        //        }
        //    }
        //    return true;
        //}
    }
}
