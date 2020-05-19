using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aco.Core.Ant
{
    public class AcoTsp
    {
        private Random random = new Random(0);
        //
        // influence of pheromone on direction
        private float alpha;
        //
        // influence of adjacent node distance
        private float beta;
        //
        // pheromone decrease factor
        private float rho;
        //
        // pheromone increase factor
        private float Q;

        private int citiesNum;
        private int antsNum;
        private int itersNum;

        private int[][] dists;

        private int[] bestTrail;
        private float bestLength;

        public void Calculate(List<List<int>> distances, int citiesNum, int antsNum, int iterationsNum, float alpha, float beta, float rho, float Q)
        {
            itersNum = iterationsNum;

            // Initialize ants to random trails
            int[][] ants = InitAnts(antsNum, citiesNum);

            dists = new int[distances.Count][];
            for (int i = 0; i < distances.Count; i++)
                dists[i] = new int[distances.Count];

            for (int i = 0; i < distances.Count; i++)
                for (int j = 0; j < distances.Count; j++)
                    dists[i][j] = distances[i][j];

            // Determine the best initial trail
            bestTrail = BestTrail(ants, dists);
            // The length of the best trail
            bestLength = Length(bestTrail, dists);

            float[][] pheromones = InitPheromones(citiesNum);

            for (int i = 0; i < itersNum; i++)
            {
                UpdateAnts(ants, pheromones, dists);
                UpdatePheromones(pheromones, ants, dists);

                int[] currBestTrail = BestTrail(ants, dists);
                float currBestLength = Length(currBestTrail, dists);
                if (currBestLength < bestLength)
                {
                    bestLength = currBestLength;
                    bestTrail = currBestTrail;
                }
            }
        }

        public List<int> GetBestTrail()
        {
            if (bestTrail != null)
                return bestTrail.ToList();
            else
                return null;
        }

        public float GetBestTrailLength()
        {
            if (bestTrail != null)
                return bestLength;
            else
                return -1f;
        }

        private int[][] InitAnts(int _antsNum, int _citiesNum)
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

        private int IndexOfTarget(int[] trail, int target)
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

        private float Length(int[] trail, int[][] _dists)
        {
            // Total length of a trail
            float result = 0f;
            for (int i = 0; i < trail.Length - 1; i++)
            {
                result += Distance(trail[i], trail[i + 1], _dists);
            }

            return result;
        }

        private int[] BestTrail(int[][] ants, int[][] _dists)
        {
            // Best trail has shortest total length
            float _bestLength = Length(ants[0], _dists);
            int idxBestLength = 0;
            for (int k = 1; k < ants.Length; k++)
            {
                float len = Length(ants[k], _dists);
                if (len < _bestLength)
                {
                    _bestLength = len;
                    idxBestLength = k;
                }
            }
            int _citiesNum = ants[0].Length;

            int[] _bestTrail = new int[_citiesNum];
            ants[idxBestLength].CopyTo(_bestTrail, 0);

            return _bestTrail;
        }

        private float[][] InitPheromones(int _citiesNum)
        {
            float[][] pheromones = new float[_citiesNum][];
            for (int i = 0; i < _citiesNum; i++)
            {
                pheromones[i] = new float[_citiesNum];
            }
            for (int i = 0; i < pheromones.Length; i++)
            {
                for (int j = 0; j < pheromones[i].Length; j++)
                {
                    pheromones[i][j] = 0.01f;
                    // Otherwise first call to UpdateAnts -> BuildTrail -> NextNode -> MoveProbs => all 0.0 => throws
                }
            }

            return pheromones;
        }

        private void UpdateAnts(int[][] ants, float[][] pheromones, int[][] _dists)
        {
            int _citiesNum = pheromones.Length;
            for (int k = 0; k < ants.Length; k++)
            {
                int start = random.Next(0, _citiesNum);
                int[] newTrail = BuildTrail(k, start, pheromones, _dists);
                ants[k] = newTrail;
            }
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

        private float[] MoveProbs(int k, int cityX, bool[] visited, float[][] pheromones, int[][] _dists)
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

        private void UpdatePheromones(float[][] pheromones, int[][] ants, int[][] _dists)
        {
            for (int i = 0; i < pheromones.Length; i++)
            {
                for (int j = i + 1; j < pheromones[i].Length; j++)
                {
                    for (int k = 0; k < ants.Length; k++)
                    {
                        float length = Length(ants[k], _dists);
                        // length of ant k trail
                        float decrease = (1f - rho) * pheromones[i][j];
                        float increase = 0f;
                        if (EdgeInTrail(i, j, ants[k]) == true)
                        {
                            increase = (Q / length);
                        }

                        pheromones[i][j] = decrease + increase;

                        if (pheromones[i][j] < 0.0001f)
                        {
                            pheromones[i][j] = 0.0001f;
                        }
                        else if (pheromones[i][j] > 100000f)
                        {
                            pheromones[i][j] = 100000f;
                        }

                        pheromones[j][i] = pheromones[i][j];
                    }
                }
            }
        }

        private bool EdgeInTrail(int cityX, int cityY, int[] trail)
        {
            // Are cityX and cityY adjacent to each other in trail[]?
            int lastIndex = trail.Length - 1;
            int idx = IndexOfTarget(trail, cityX);

            if ((idx == 0 && trail[1] == cityY) ||
                (idx == 0 && trail[lastIndex] == cityY))
            {
                return true;
            }
            else if (idx == 0)
            {
                return false;
            }
            else if ((idx == lastIndex && trail[lastIndex - 1] == cityY) ||
                    (idx == lastIndex && trail[0] == cityY))
            {
                return true;
            }
            else if (idx == lastIndex)
            {
                return false;
            }
            else if ((trail[idx - 1] == cityY) ||
                    (trail[idx + 1] == cityY))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private float Distance(int cityX, int cityY, int[][] _dists)
        {
            return _dists[cityX][cityY];
        }
    }

}
