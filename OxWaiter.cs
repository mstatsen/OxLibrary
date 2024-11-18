namespace OxLibrary
{
    public class OxWaiter
    {
        private Thread? thread;
        private readonly Func<int> Function;

        public bool Ready;
        private bool enabled = false;
        public bool Enabled 
        { 
            get => enabled;
            set
            {
                enabled = value;

                if (enabled)
                    Start();
                else
                    Stop();
            }
        }

        public OxWaiter(Func<int> function) =>
            Function = function;

        public void Start()
        {
            if (enabled 
                && thread is not null 
                && thread.IsAlive)
            {
                Ready = true;
                return;
            }

            thread = new(Waitfunction);
            thread.Start();
        }

        public void Stop()
        {
            Ready = false;
            enabled = false;
        }

        public void Waitfunction()
        {
            Ready = true;
            enabled = true;

            while (enabled)
            {
                Ready = true;
                Thread.Sleep(400);

                if (Ready)
                {
                    Function.Invoke();
                    Ready = false;
                }
            }
        }
    }
}