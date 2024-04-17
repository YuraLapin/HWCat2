namespace HWCat
{
    public static class Program
    {
        public static int Main()
        {
            var sim = new CatSimulator();

            sim.Simulate(10);

            return 0;
        }
    }
}