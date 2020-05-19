using Aco.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aco.Core.Ant
{
    public class AcoRunner
    {

        Random rand = new Random();

        List<List<int>> lengthMatr = new List<List<int>>();

        List<City> nodesLocs = new List<City>();

        AcoTsp tsp;

        float alpha = 1;
        float beta = 1;
        int iterNum = 1000;
        int antsNum = 200;
        float rho = 0.5f;
        float q = 20;

        void AddNodeToLengthMatrix()
        {
            for (int i = 0; i < lengthMatr.Count; i++)
                lengthMatr[i].Add(rand.Next(1, 50));
            lengthMatr.Add(new List<int>());
            for (int i = 0; i < lengthMatr.Count - 1; i++)
                lengthMatr[lengthMatr.Count - 1].Add(lengthMatr[i][lengthMatr.Count - 1]);
            lengthMatr[lengthMatr.Count - 1].Add(0);

            for (int i = 0; i < lengthMatr.Count; i++)
                lengthMatr[i][i] = 0;
        }

        void SetCities(List<City> cities)
        {
            foreach (var city in cities)
            {
                nodesLocs.Add(city);
                AddNodeToLengthMatrix();
            }
        }

        public AntResult Run(List<City> cities)
        {
            AntResult antResult = new AntResult();

            SetCities(cities);

            tsp = new AcoTsp();
            tsp.Calculate(lengthMatr, nodesLocs.Count, antsNum, iterNum, alpha, beta, rho, q);

            var bestTrl = tsp.GetBestTrail();

            List<string> routes = new List<string>();

            //will be 2-0-1 
            for (int i = 0; i < bestTrl.Count; i++)
            {
                routes.Add(bestTrl[i].ToString());
                Console.Write(bestTrl[i].ToString() + " - ");
            }


            Console.WriteLine();
            Console.WriteLine("********************************");
            Console.WriteLine("path length : " + tsp.GetBestTrailLength().ToString()); //will be 37
            antResult.OptimalDistance = tsp.GetBestTrailLength().ToString();
            antResult.OptimalRoutes = routes;
            return antResult;
        }
    }
}

