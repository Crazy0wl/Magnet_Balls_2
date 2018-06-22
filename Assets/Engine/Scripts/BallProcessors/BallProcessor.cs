namespace MB_Engine
{
    public class BallProcessor
    {
        private Ball ball;
        public BallProcessor(Ball ball)
        {
            this.ball = ball;

        }

        public virtual void ProcessJoinWith(Ball ball)
        {

        }
    }
}
