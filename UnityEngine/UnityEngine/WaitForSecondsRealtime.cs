namespace UnityEngine
{
    using System;

    public class WaitForSecondsRealtime : CustomYieldInstruction
    {
        private float waitTime;

        public WaitForSecondsRealtime(float time)
        {
            this.waitTime = Time.realtimeSinceStartup + time;
        }

        public override bool keepWaiting
        {
            get
            {
                return (Time.realtimeSinceStartup < this.waitTime);
            }
        }
    }
}

