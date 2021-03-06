﻿using System;
using System.Collections.Generic;
using IP_Framework.InternalDbHandler;
using MongoDB.Bson;

namespace IP_Framework
{
    class EpidemyAlert : IModule
    {
        private EventHandler fatherHandler;

        public static double AreaAroundYuu = 0.2;
        public static int AreaAroundYuuCases = 8;

        public static double NeighourHood = 1.5;
        public static int NeighourHoodCases = 20;

        public static double Town = 10;
        public static int TownCases = 150;

        public static double Country = 100;
        public static int CountryCases = 1000;

        public EpidemyAlert(EventHandler father)
        {
            fatherHandler = father;
        }

        public String CreateConvexHauls(List<Point> points)
        {

            List<List<Point>> finalResultList = new List<List<Point>>();

            foreach (List<Point> list in ConvexHaul.CalculateHull(points, AreaAroundYuu))
            {
                finalResultList.Add(list);
            }
            foreach (List<Point> list in ConvexHaul.CalculateHull(points, NeighourHood))
            {
                finalResultList.Add(list);
            }
            foreach (List<Point> list in ConvexHaul.CalculateHull(points, Town))
            {
                finalResultList.Add(list);
            }
            foreach (List<Point> list in ConvexHaul.CalculateHull(points, Country))
            {
                finalResultList.Add(list);
            }

            String JSON = "{result : [";

            foreach (var list in finalResultList)
            {
                JSON = JSON + "[";
                foreach (var point in list)
                {
                    JSON = JSON + "{" + point.x + ", " + point.y + "},";
                }
                JSON = JSON.TrimEnd(',');
                JSON = JSON + "],";
            }

            JSON = JSON.TrimEnd(',');
            JSON = JSON + "]}";

            Console.WriteLine(JSON);

            return JSON;
        }

        public String CheckIfPointsCauseAlert(List<Point> points, Point user)
        {
            String JSON = "{areas : [";

            int counterForAreaAroundYuu = 0;
            int counterForNeighourHood = 0;
            int counterForTown = 0;
            int counterForCountry = 0;

            foreach (Point point in points)
            {
                if (ConvexHaul.Distance(point, user) < AreaAroundYuu)
                    counterForAreaAroundYuu++;
                if (ConvexHaul.Distance(point, user) < NeighourHood)
                    counterForNeighourHood++;
                if (ConvexHaul.Distance(point, user) < Town)
                    counterForTown++;
                if (ConvexHaul.Distance(point, user) < Country)
                    counterForCountry++;
            }

            if (counterForAreaAroundYuu >= AreaAroundYuuCases)
            {
                JSON = JSON + "{\"AreaAroundYou\" : 1},";
            }
            else
            {
                JSON = JSON + "{\"AreaAroundYou\" : 0},";
            }

            if (counterForNeighourHood >= NeighourHoodCases)
            {
                JSON = JSON + "{\"NeighourHood\" : 1},";
            }
            else
            {
                JSON = JSON + "{\"NeighourHood\" : 0},";
            }

            if (counterForTown >= TownCases)
            {
                JSON = JSON + "{\"Town\" : 1},";
            }
            else
            {
                JSON = JSON + "{\"Town\" : 0},";
            }

            if (counterForCountry >= CountryCases)
            {
                JSON = JSON + "{\"Country\" : 1}]}";
            }
            else
            {
                JSON = JSON + "{\"Country\" : 0}]}";
            }

            return JSON;
        }

        public override bool InvokeCommand(SubModuleFunctions command, IContext contextHandler)
        {
            Console.WriteLine("InvokeCommand execution for EpidemyAlert subModule");

            EpidemyContext subModuleContextHandler = contextHandler as EpidemyContext;

            DBModule instance = Utils.Singleton<DBModule>.Instance;
            UserHandler userHandler = instance.GetUserHandler();
            List<Point> points;

            switch (command)
            {

                case SubModuleFunctions.EpidemyCheckForAreas:

                    points = userHandler.GetPoints();
                    subModuleContextHandler.json = CreateConvexHauls(points);
                    return true;

                case SubModuleFunctions.EpidemyCheckForSpecificAlert:

                    points = userHandler.GetPointsForDisease(subModuleContextHandler.specificSearch);
                    subModuleContextHandler.json = CreateConvexHauls(points);
                    return true;

                case SubModuleFunctions.EpidemyCheckForAlert:

                    points = userHandler.GetPointsForDisease(subModuleContextHandler.specificSearch);
                    Point user = new Point();
                    subModuleContextHandler.json = CheckIfPointsCauseAlert(points, user);
                    return true;

                default:

                    return false;
            }
        }

        public override bool Init(byte[] context, int sizeOfContext)
        {
            Console.WriteLine("Init execution");
            return true;
        }

        public override bool UnInit()
        {
            Console.WriteLine("UnInit execution");
            return true;
        }
    }
}
