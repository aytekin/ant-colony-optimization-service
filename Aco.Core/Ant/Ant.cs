using System;
using System.Collections.Generic;
using System.Text;

namespace Aco.Core.Ant
{
    public class Ant
    {
        private  Random random = new Random(0);

        //
        // influence of pheromone on direction
        private float alpha;
        //
        // influence of adjacent node distance
        private float beta;


        public Ant(float alpha,float beta)
        {
            this.alpha = alpha;
            this.beta = beta;
        }


        public void UpdateAnts(int[][] ants, float[][] pheromones, int[][] _dists)
        {
            int _citiesNum = pheromones.Length;
            for (int k = 0; k < ants.Length; k++)
            {
                int start = random.Next(0, _citiesNum);
                int[] newTrail = BuildTrail(k, start, pheromones, _dists);
                ants[k] = newTrail;
            }
        }

        public int[][] InitAnts(int _antsNum, int _citiesNum)
        {
            int[][] ants = new int[_antsNum][];
            for (int k = 0; k < _antsNum; k++)
            {
                int start = random.Next(0, _citiesNum);
                ants[k] = RandomTrail(start, _citiesNum);
            }

            return ants;
        }

        private int[] RandomTrail(int start, int _citiesNum)
        {
            // Helper for InitAnts
            int[] trail = new int[_citiesNum];

            // Sequential
            for (int i = 0; i < _citiesNum; i++)
            {
                trail[i] = i;
            }

            // Fisher-Yates shuffle
            for (int i = 0; i < _citiesNum; i++)
            {
                int r = random.Next(i, _citiesNum);

                int tmp = trail[r];
                trail[r] = trail[i];
                trail[i] = tmp;
            }

            int idx = IndexOfTarget(trail, start);

            // Put start at [0]
            int temp = trail[0];
            trail[0] = trail[idx];
            trail[idx] = temp;

            return trail;
        }

        private int[] BuildTrail(int k, int start, float[][] pheromones, int[][] _dists)
        {
            int _citiesNum = pheromones.Length;
            int[] trail = new int[_citiesNum];
            bool[] visited = new bool[_citiesNum];
            trail[0] = start;
            visited[start] = true;
            for (int i = 0; i < _citiesNum - 1; i++)
            {
                int cityX = trail[i];
                int next = NextCity(k, cityX, visited, pheromones, _dists);
                trail[i + 1] = next;
                visited[next] = true;
            }

            return trail;
        }

        private int NextCity(int k, int cityX, bool[] visited, float[][] pheromones, int[][] _dists)
        {
            // for ant k (with visited[]), at nodeX, what is next node in trail?
            float[] probs = MoveProbs(k, cityX, visited, pheromones, _dists);

            float[] cumul = new float[probs.Length + 1];
            for (int i = 0; i < probs.Length; i++)
            {
                cumul[i + 1] = cumul[i] + probs[i];
                // consider setting cumul[cuml.Length-1] to 1.00
            }

            float p = (float)random.NextDouble();

            for (int i = 0; i < cumul.Length - 1; i++)
            {
                if (p >= cumul[i] && p < cumul[i + 1])
                {
                    return i;
                }
            }
            throw new Exception("Failure to return valid city in NextCity");
        }

        private  float[] MoveProbs(int k, int cityX, bool[] visited, float[][] pheromones, int[][] _dists)
        {
            // For ant k, located at nodeX, with visited[], return the prob of moving to each city
            int _citiesNum = pheromones.Length;
            float[] taueta = new float[_citiesNum];
            // Inclues cityX and visited cities
            float sum = 0f;
            // Sum of all tauetas
            // i is the adjacent city
            for (int i = 0; i < taueta.Length; i++)
            {
                if (i == cityX)
                {
                    taueta[i] = 0f;
                    // Prob of moving to self is 0
                }
                else if (visited[i] == true)
                {
                    taueta[i] = 0f;
                    // Prob of moving to a visited city is 0
                }
                else
                {
                    taueta[i] = (float)Math.Pow(pheromones[cityX][i], alpha) * (float)Math.Pow((1f / Distance(cityX, i, _dists)), beta);
                    // Could be huge when pheromone[][] is big
                    if (taueta[i] < 0.0001f)
                        taueta[i] = 0.0001f;

                    else if (taueta[i] > (float.MaxValue / (_citiesNum * 100)))
                        taueta[i] = float.MaxValue / (_citiesNum * 100);
                }
                sum += taueta[i];
            }

            float[] probs = new float[_citiesNum];
            for (int i = 0; i < probs.Length; i++)
            {
                probs[i] = taueta[i] / sum;
                // Big trouble if sum = 0.0
            }

            return probs;
        }


        public int IndexOfTarget(int[] trail, int target)
        {
            // Helper for RandomTrail
            for (int i = 0; i < trail.Length; i++)
            {
                if (trail[i] == target)
                {
                    return i;
                }
            }
            throw new Exception("Target not found in IndexOfTarget");
        }


        public float Distance(int cityX, int cityY, int[][] _dists)
        {
            return _dists[cityX][cityY];
        }
    }
}
