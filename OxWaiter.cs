namespace OxLibrary
{
    public class OxWaiter
    {
        private Thread? thread;
        private readonly Func<int> Function;

        public bool Ready;
        public bool Enabled;

        public OxWaiter(Func<int> function) =>
            Function = function;

        public void Start()
        {
            thread = new(Waitfunction);
            thread.Start();
        }

        public void Stop()
        {
            Ready = false;
            Enabled = false;
        }

        public void Waitfunction()
        {
            Ready = true;
            Enabled = true;

            while (true)
            {
                Ready = true;
                Thread.Sleep(400);

                if (Ready)
                {
                    Function.Invoke();
                    Ready = false;
                }

                if (!Enabled)
                    break;
            }
        }
    }


    public class OxStepWaiter
    {
        private Thread? thread;
        private readonly IProgress<int> Step;
        private readonly Func<int> StepCalcer;

        public bool Enabled;

        public OxStepWaiter(IProgress<int> step, Func<int> stepCalcer)
        {
            Step = step;
            StepCalcer = stepCalcer;
        }

        public void Start()
        {
            thread = new(Waitfunction);
            //thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        public void Stop() => 
            Enabled = false;

        public void Waitfunction()
        {
            Enabled = true;

            while (true)
            {
                Step.Report(StepCalcer());
                Task.Delay(100).Wait();

                if (!Enabled)
                    break;
            }
        }
    }
}