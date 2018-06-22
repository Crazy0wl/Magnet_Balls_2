using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB_Engine
{
    public class LightningBall : BaseBonus
    {
        private const int MAX_COUNT = 100;
        private int iterCounter;

        public override BallType BonusType { get { return BallType.Lightning; } }

        public override void Emit(Ball ball, FastPool pool, Vector2 force, float delay)
        {
            iterCounter = 0;
            base.Emit(ball, pool, force, delay);
            StartCoroutine(EmitRoutine(delay));
        }

        private IEnumerator EmitRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            transform.position = ball.Pos;
            ForceBalls();
            PlaySound("lightning", Random.Range(0.9f, 1.1f), 0f, 1f);
            Ball b = null;
            while ((b = GetBallForDestroy()) && iterCounter++ < MAX_COUNT)
            {
              //  if (ball.WasJoin Joints.Count > 0)
                {
                    Stack<Ball> path = ball.GetPathTo(b);
                    if (path.Contains(ball))
                    {
                        Lightning lightning = Balls.EmitLightning();
                        b.Targeted = true;
                        Ball[] pathArray = path.ToArray();
                        lightning.Emit(b, pathArray);
                        yield return new WaitForSeconds(0.15f);
                    }
                }
            }
            if (ball.BallType == BallType.Lightning)
            {
                ball.BallType = BallType.Simple;
                ball.Kill(0, Vector2.zero, 0.1f);
            }
        }

        private Ball GetBallForDestroy()
        {
            Ball result = Balls.main.AllBalls.Find(b => 
            b.BallColor == ball.BallColor && 
            b != ball && 
            (!b.IsKinematic || b.BallType == BallType.Anchored) && 
            !b.Destroying && 
            !b.Targeted && 
            b.BallType != BallType.Lightning);
            return result;
        }

        private List<Ball> GetBallsForDestroy()
        {
            List<Ball> result = new List<Ball>();
            result = Balls.main.AllBalls.FindAll(b => b.BallColor == ball.BallColor && b != ball && !b.IsKinematic && !b.Destroying && !b.Targeted);
            return result;
        }
    }
}