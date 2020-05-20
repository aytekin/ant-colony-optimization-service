using Aco.Entity.Dto;
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
        // pheromone decrease factor
        private float rho;
        //
        // pheromone increase factor
        private float Q;

        private int antsNum;
        private int iterationsNum;

        private int[][] dists;

        private int[] bestTrail;
        private float bestLength;

        private Ant ant;

        public AcoTsp(AcoOptions acoOptions)
        {
            this.rho = acoOptions.Rho;
            this.Q = acoOptions.Q;
            this.antsNum = acoOptions.AntsNum;
            this.iterationsNum = acoOptions.IterNum;
            ant = new Ant(acoOptions.Alpha, acoOptions.Beta);
            
        }

        public void Calculate(int[][] distances, int citiesNum)
        {

            // Initialize ants to random trails
            int[][] ants = ant.InitAnts(antsNum, citiesNum);

            dists = distances;

            // Determine the best initial trail
            bestTrail = BestTrail(ants, dists);
            // The length of the best trail
            bestLength = Length(bestTrail, dists);

            float[][] pheromones = InitPheromones(citiesNum);

            for (int i = 0; i < iterationsNum; i++)
            {
                ant.UpdateAnts(ants, pheromones, dists);
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

      

        private float Length(int[] trail, int[][] _dists)
        {
            // Total length of a trail
            float result = 0f;
            for (int i = 0; i < trail.Length - 1; i++)
            {
                result += ant.Distance(trail[i], trail[i + 1], _dists);
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
            int idx = ant.IndexOfTarget(trail, cityX);

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

      
    }

}
