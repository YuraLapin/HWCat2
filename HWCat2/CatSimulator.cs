namespace HWCat
{
    enum States
    {
        sleep,
        play,
        eat,
    }

    internal class CatSimulator
    {
        public const int DAY = 1440;

        private int lambdaSleepSleep = 100;
        private int lambdaSleepPlay = 20;
        private int lambdaSleepEat = 25;

        private int lambdaPlaySleep = 25;
        private int lambdaPlayEat = 15;

        private int lambdaEatSleep = 15;
        private int lambdaEatPlay = 25;

        private Random rnd = new Random();

        private double CalculateTau(int lambda)
        {
            double r = rnd.NextSingle();
            double ln = Math.Log(r);
            double ans = (-1.0 / lambda) * ln * 100.0;
            return ans;
        }

        private States GetStartingState()
        {
            var lst = new List<States>()
            {
                States.sleep,
                States.play,
                States.eat,
            };
            return lst[rnd.Next(lst.Count)];
        }

        private float Sum(List<float> list)
        {
            float ans = 0f;

            foreach (var item in list)
            {
                ans += item;
            }

            return ans;
        }

        private float GetAvg(List<float> list)
        {
            if (list.Count == 0)
            {
                return 0f;
            }

            return Sum(list) / list.Count;
        }

        public void Simulate(int experimentsCount)
        {
            var sums = new Dictionary<States, double>
            {
                { States.sleep, 0 },
                { States.play, 0 },
                { States.eat, 0 }
            };

            var sleepParts = new List<float>();
            var playParts = new List<float>();
            var eatParts = new List<float>();

            var chainLengths = new List<float>();

            for (int i = 0; i < experimentsCount; i++)
            {
                var curChain = new List<States>();
                var startingState = GetStartingState();
                curChain.Add(startingState);
                double timePassed = 0;
                var prevState = startingState;

                double sleepTime = 0;
                double playTime = 0;
                double eatTime = 0;

                while (timePassed < DAY)
                {
                    States newState;
                    double tau;

                    if (prevState == States.sleep)
                    {
                        var tauSleep = CalculateTau(lambdaSleepSleep);
                        var tauPlay = CalculateTau(lambdaSleepPlay);
                        var tauEat = CalculateTau(lambdaSleepEat);

                        tau = Math.Min(tauSleep, Math.Min(tauPlay, tauEat));

                        if (tau == tauSleep)
                        {
                            newState = States.sleep;
                            sleepTime += tau;
                        }

                        else if (tau == tauPlay)
                        {
                            newState = States.play;
                            playTime += tau;
                        }

                        else
                        {
                            newState = States.eat;
                            eatTime += tau;
                        }
                    }

                    else if (prevState == States.play)
                    {
                        var tauSleep = CalculateTau(lambdaPlaySleep);
                        var tauEat = CalculateTau(lambdaPlayEat);

                        tau = Math.Min(tauSleep, tauEat);

                        if (tau == tauSleep)
                        {
                            newState = States.sleep;
                            sleepTime += tau;
                        }

                        else
                        {
                            newState = States.eat;
                            eatTime += tau;
                        }
                    } 
                    
                    else
                    {
                        var tauSleep = CalculateTau(lambdaEatSleep);
                        var tauPlay = CalculateTau(lambdaEatPlay);

                        tau = Math.Min(tauSleep, tauPlay);

                        if (tau == tauSleep)
                        {
                            newState = States.sleep;
                            sleepTime += tau;
                        }

                        else
                        {
                            newState = States.play;
                            playTime += tau;
                        }
                    }

                    curChain.Add(newState);
                    prevState = newState;
                    timePassed += tau;
                }

                var totalTime = sleepTime + playTime + eatTime;
                sums[States.sleep] += sleepTime;
                sums[States.play] += playTime;
                sums[States.eat] += eatTime;

                sleepParts.Add((float)(sleepTime / totalTime));
                playParts.Add((float)(playTime / totalTime));
                eatParts.Add((float)(eatTime / totalTime));

                chainLengths.Add(curChain.Count);
            }

            var sleepAvg = GetAvg(sleepParts);
            var playAvg = GetAvg(playParts);
            var eatAvg = GetAvg(eatParts);

            var chainAvg = GetAvg(chainLengths);

            foreach (var sum in sums)
            {
                Console.WriteLine($"{sum.Value} {sum.Key}");
            }

            Console.WriteLine($"Спит за день: {sleepAvg}");
            Console.WriteLine($"Играет за день: {playAvg}");
            Console.WriteLine($"Ест за день: {eatAvg}");
        }
    }
}
