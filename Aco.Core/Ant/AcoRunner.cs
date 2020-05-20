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

        List<List<int>> lengthMatr = new List<List<int>>();

        List<City> nodesLocs = new List<City>();

        AcoTsp tsp;



        private void calculatePath()
        {

            List<List<int>> lengthMatr = new List<List<int>>();
            var index = 0;
            this.nodesLocs.ForEach(i =>
            {
                List<int> list = new List<int>();
                this.nodesLocs.ForEach(y =>
                {
                    var a = (i.X - y.X) * (i.X - y.X);
                    var b = (i.Y - y.Y) * (i.Y - y.Y);
                    var res = a - b;
                    var val = Math.Sqrt(Math.Abs((res)));
                    list.Add(Convert.ToInt32(val));
                });
                lengthMatr.Add(list);
                index++;
            });

            this.lengthMatr = lengthMatr;
        }

        void SetCities(List<City> cities)
        {
            foreach (var city in cities)
            {
                nodesLocs.Add(city);
                calculatePath();
            }
        }

        public AntResult Run(List<City> cities, AcoOptions acoOptions)
        {
            AntResult antResult = new AntResult();

            SetCities(cities);

            tsp = new AcoTsp();
            tsp.Calculate(lengthMatr, nodesLocs.Count, acoOptions.AntsNum,acoOptions.IterNum,acoOptions.Alpha,acoOptions.Beta,acoOptions.Rho, acoOptions.Q);

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

