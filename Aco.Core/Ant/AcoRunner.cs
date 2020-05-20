using Aco.Entity;
using Aco.Entity.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aco.Core.Ant
{
    public class AcoRunner
    {

        Random rand = new Random();

        int[][] distances;

        AcoTsp tsp;



        void SetDistances(List<City> cities)
        {
            int length = cities.Count;

            int[][] temp = new int[length][];
                
            for (int i = 0; i < length; i++)
            {
                temp[i] = new int[length];
                for (int j = 0; j < length; j++)
                {
                    var a = (cities[i].X - cities[j].X) * (cities[i].X - cities[j].X);
                    var b = (cities[i].Y - cities[j].Y) * (cities[i].Y - cities[j].Y);
                    var res = a - b;
                    var val = Math.Sqrt(Math.Abs((res)));
                    temp[i][j] = Convert.ToInt32(val);
                }
            }
            this.distances = temp;

        }

        public AntResult Run(List<City> cities, AcoOptions acoOptions)
        {
            AntResult antResult = new AntResult();

            SetDistances(cities);

            tsp = new AcoTsp(acoOptions);
            tsp.Calculate(distances, cities.Count);


            antResult.OptimalDistance = tsp.GetBestTrailLength().ToString();
            antResult.OptimalRoutes = tsp.GetBestTrail();
            return antResult;
        }
    }
}

