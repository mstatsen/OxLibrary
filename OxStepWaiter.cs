namespace OxLibrary
{
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