using MultiQueueModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;



namespace MultiQueueModels
{
    public class SimulationSystem
    {
        public SimulationSystem()
        {
            this.Servers = new List<Server>();
            this.InterarrivalDistribution = new List<TimeDistribution>();
            this.PerformanceMeasures = new PerformanceMeasures();
            this.SimulationTable = new List<SimulationCase>();
        }

        ///////////// INPUTS ///////////// 
        public int NumberOfServers { get; set; }
        public int StoppingNumber { get; set; }
        public List<Server> Servers { get; set; }
        public List<TimeDistribution> InterarrivalDistribution { get; set; }
        public Enums.StoppingCriteria StoppingCriteria { get; set; }
        public Enums.SelectionMethod SelectionMethod { get; set; }

        ///////////// OUTPUTS /////////////
        public List<SimulationCase> SimulationTable { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }


        public SimulationCase[] modelCalculation(SimulationSystem system, string[] lines)
        {
            system.NumberOfServers = Convert.ToInt32(lines[1]);
            system.StoppingNumber = Convert.ToInt32(lines[4]);
            if (Convert.ToInt32(lines[7]) == 1)
            {
                system.StoppingCriteria = Enums.StoppingCriteria.NumberOfCustomers;
            }
            else
            {
                system.StoppingCriteria = Enums.StoppingCriteria.SimulationEndTime;
            }

            if (Convert.ToInt32(lines[10]) == 1)
            {
                system.SelectionMethod = Enums.SelectionMethod.HighestPriority;
            }
            else if (Convert.ToInt32(lines[10]) == 2)
            {
                system.SelectionMethod = Enums.SelectionMethod.Random;
            }
            else
            {
                system.SelectionMethod = Enums.SelectionMethod.LeastUtilization;
            }
            int loopVar = 13;
            int counter = 0;
            decimal tmpProbability = 0;
            int minRange = 1;
            int probCounters = 0;
            while (lines[loopVar] != "")
            {
                string[] temp = lines[loopVar].Split(',');
                system.InterarrivalDistribution.Add(new TimeDistribution());
                system.InterarrivalDistribution[counter].Time = Convert.ToInt32(temp[0]);
                system.InterarrivalDistribution[counter].Probability = Convert.ToDecimal(temp[1]);
                system.InterarrivalDistribution[counter].CummProbability = tmpProbability + system.InterarrivalDistribution[counter].Probability;
                if (counter == 0)
                {
                    system.InterarrivalDistribution[counter].MinRange = minRange;
                }
                else
                {
                    system.InterarrivalDistribution[counter].MinRange = Convert.ToInt32(system.InterarrivalDistribution[counter - 1].MaxRange + 1);
                }
                system.InterarrivalDistribution[counter].MaxRange = Convert.ToInt32(system.InterarrivalDistribution[counter].CummProbability * 100);
                tmpProbability = system.InterarrivalDistribution[counter].CummProbability;
                loopVar++;
                counter++;
            }
            for (int i = 0; i < system.NumberOfServers; i++)
            {
                loopVar += 2;
                tmpProbability = 0;
                counter = 0;
                system.Servers.Add(new Server());
                while (lines[loopVar] != "")
                {
                    string[] temp = lines[loopVar].Split(',');
                    system.Servers[i].TimeDistribution.Add(new TimeDistribution());
                    system.Servers[i].TimeDistribution[counter].Time = Convert.ToInt32(temp[0]);
                    system.Servers[i].TimeDistribution[counter].Probability = Convert.ToDecimal(temp[1]);
                    system.Servers[i].TimeDistribution[counter].CummProbability = tmpProbability + system.Servers[i].TimeDistribution[counter].Probability;
                    if (counter == 0)
                    {
                        system.Servers[i].TimeDistribution[counter].MinRange = minRange;
                    }
                    else
                    {
                        system.Servers[i].TimeDistribution[counter].MinRange = Convert.ToInt32(system.Servers[i].TimeDistribution[counter - 1].MaxRange + 1);
                    }
                    system.Servers[i].TimeDistribution[counter].MaxRange = Convert.ToInt32(system.Servers[i].TimeDistribution[counter].CummProbability * 100);
                    tmpProbability = system.Servers[i].TimeDistribution[counter].CummProbability;
                    counter++;
                    if (loopVar + 1 == lines.Length)
                    {
                        break;
                    }
                    loopVar++;
                }
                probCounters = counter;
            }
            Random ramdomVar = new Random();
            SimulationCase[] sim = new SimulationCase[101];
            for (int i = 0; i < system.NumberOfServers; i++)
            {
                system.Servers[i].ID = i+1;
                system.Servers[i].FinishTime = 0;
            }
            // highestPriority
            if (system.SelectionMethod == Enums.SelectionMethod.HighestPriority)  
            {
                for (int i = 1; i <= system.StoppingNumber; i++)
                {
                    sim[i] = new SimulationCase();
                    sim[i].CustomerNumber = i;
                    if (i == 1)
                    {
                        sim[i].RandomInterArrival =1;
                        sim[i].InterArrival = 0;
                        sim[i].ArrivalTime = 0;
                        sim[i].RandomService = ramdomVar.Next(1, 100);
                        sim[i].StartTime = 0;
                        sim[i].AssignedServer = system.Servers[0];
                        for (int j = 0; j < probCounters; j++)
                        {
                            if (system.Servers[0].TimeDistribution[j].MinRange <= sim[i].RandomService &&
                                system.Servers[0].TimeDistribution[j].MaxRange >= sim[i].RandomService)
                            {
                                sim[i].ServiceTime = system.Servers[0].TimeDistribution[j].Time;
                            }
                        }
                        sim[i].EndTime = sim[i].StartTime + sim[i].ServiceTime;
                        sim[i].TimeInQueue = sim[i].StartTime - sim[i].ArrivalTime;
                        system.Servers[0].FinishTime = sim[i].EndTime;
                        system.Servers[0].TotalWorkingTime = system.Servers[0].TotalWorkingTime+ sim[i].ServiceTime;
                    }
                    else
                    {
                        bool finish = false;
                        sim[i].RandomInterArrival = ramdomVar.Next(1, 100);
                        for (int j = 0; j < probCounters; j++)
                        {
                            if (system.InterarrivalDistribution[j].MinRange <= sim[i].RandomInterArrival &&
                                system.InterarrivalDistribution[j].MaxRange >= sim[i].RandomInterArrival)
                            {
                                sim[i].InterArrival = system.InterarrivalDistribution[j].Time;
                            }
                        }
                        sim[i].ArrivalTime = sim[i - 1].ArrivalTime + sim[i].InterArrival;
                        sim[i].RandomService = ramdomVar.Next(1, 100);
                        for (int f = 0; f < system.NumberOfServers; f++)
                        {
                            if (sim[i].ArrivalTime >= system.Servers[f].FinishTime)
                            {
                                sim[i].AssignedServer = system.Servers[f];
                                for (int j = 0; j < probCounters ; j++)
                                {
                                    if (system.Servers[f].TimeDistribution[j].MinRange <= sim[i].RandomService &&
                                        system.Servers[f].TimeDistribution[j].MaxRange >= sim[i].RandomService)
                                    {
                                        sim[i].ServiceTime = system.Servers[f].TimeDistribution[j].Time;
                                    }
                                }
                                sim[i].StartTime = sim[i].ArrivalTime;
                                sim[i].EndTime = sim[i].StartTime + sim[i].ServiceTime;
                                sim[i].TimeInQueue = 0;
                                system.Servers[f].FinishTime = sim[i].EndTime;
                                system.Servers[f].TotalWorkingTime = system.Servers[f].TotalWorkingTime + sim[i].ServiceTime;
                                finish = true;
                                break;
                            } 
                        }
                        if (!finish)
                        {
                            int serverFinish = 0;
                            for (int f = 0; f < system.NumberOfServers; f++)
                            {
                                if (system.Servers[serverFinish].FinishTime > system.Servers[f].FinishTime)
                                {
                                    serverFinish = f;
                                }
                            }
                             sim[i].AssignedServer = system.Servers[serverFinish];
                             for (int j = 0; j < probCounters; j++)
                             {
                                if (system.Servers[serverFinish].TimeDistribution[j].MinRange <= sim[i].RandomService &&
                                    system.Servers[serverFinish].TimeDistribution[j].MaxRange >= sim[i].RandomService)
                                {
                                    sim[i].ServiceTime = system.Servers[serverFinish].TimeDistribution[j].Time;
                                }
                             }
                             sim[i].StartTime = system.Servers[serverFinish].FinishTime;
                             sim[i].EndTime = sim[i].StartTime + sim[i].ServiceTime;
                             sim[i].TimeInQueue = system.Servers[serverFinish].FinishTime - sim[i].ArrivalTime;
                             system.Servers[serverFinish].FinishTime = sim[i].EndTime;
                             system.Servers[serverFinish].TotalWorkingTime = system.Servers[serverFinish].TotalWorkingTime + sim[i].ServiceTime ;
                        }
                    }
                }
                decimal totalTimeSpentInQueue = 0;
                decimal numberOfCustomersInQueue = 0;
                decimal countOfUsersJoinedTheServer = 0;
                decimal simulationFinishTime = -1;
                for (int i = 1; i <= system.StoppingNumber; i++)
                {
                    if (sim[i].TimeInQueue != 0) 
                    {
                        totalTimeSpentInQueue = totalTimeSpentInQueue+ sim[i].TimeInQueue;
                        numberOfCustomersInQueue++;
                    }

                }
                system.PerformanceMeasures.AverageWaitingTime = totalTimeSpentInQueue / system.StoppingNumber;
                system.PerformanceMeasures.WaitingProbability = numberOfCustomersInQueue / system.StoppingNumber;
                for (int i = 0; i < system.NumberOfServers; i++)
                {
                    countOfUsersJoinedTheServer = 0;
                    simulationFinishTime = -1;
                    for (int l = 0; l < system.NumberOfServers ; l++)
                    {
                        if (system.Servers[l].FinishTime >simulationFinishTime )
                        {
                            simulationFinishTime = system.Servers[l].FinishTime;
                        }
                    }
                    system.Servers[i].IdleProbability = (simulationFinishTime - system.Servers[i].TotalWorkingTime) / simulationFinishTime;
                    for (int j = 1; j <= system.StoppingNumber; j++)
                    {
                        if (sim[j].AssignedServer.ID == system.Servers[i].ID)
                        {
                            countOfUsersJoinedTheServer++;
                        }
                    }
                    if (countOfUsersJoinedTheServer != 0)
                    {
                        system.Servers[i].AverageServiceTime = system.Servers[i].TotalWorkingTime / countOfUsersJoinedTheServer;
                    }
                    else
                    {
                        system.Servers[i].AverageServiceTime = 0;
                    }
                    system.Servers[i].Utilization = system.Servers[i].TotalWorkingTime / simulationFinishTime ;
                }
                int[] countTimeAtTime = new int[Convert.ToInt32(simulationFinishTime)];
                for (int l = 1; l <= system.StoppingNumber; l++)
                {
                    if (sim[l].TimeInQueue != 0)
                    {
                        for (int n = sim[l].ArrivalTime; n < sim[l].StartTime; n++)
                        {
                            countTimeAtTime[n]++;
                        }
                    }
                }
                system.PerformanceMeasures.MaxQueueLength = countTimeAtTime.Max();
            } 
            //ran dom method
            else if (system.SelectionMethod == Enums.SelectionMethod.Random) 
            {
                for (int i = 1; i <= system.StoppingNumber; i++)
                {
                    sim[i] = new SimulationCase();
                    sim[i].CustomerNumber = i;
                    bool checkIfOneISFree = false;
                    bool[] arrayOfServersCheckAvailablity = new bool[system.NumberOfServers];
                    int choosenServer = ramdomVar.Next(0 , system.NumberOfServers);
                    for (int l = 0; l < system.NumberOfServers; l++)
                    {
                        arrayOfServersCheckAvailablity[l] = false;
                    }
                    if (i ==1)
                    {
                        sim[i].RandomInterArrival = 1;
                        sim[i].InterArrival = 0;
                        sim[i].ArrivalTime = 0;
                    }
                    else
                    {
                        sim[i].RandomInterArrival = ramdomVar.Next(1, 100);
                        for (int j = 0; j < probCounters; j++)
                        {
                            if (system.InterarrivalDistribution[j].MinRange <= sim[i].RandomInterArrival &&
                                system.InterarrivalDistribution[j].MaxRange >= sim[i].RandomInterArrival)
                            {
                                sim[i].InterArrival = system.InterarrivalDistribution[j].Time;
                            }
                        }
                        sim[i].ArrivalTime = sim[i - 1].ArrivalTime + sim[i].InterArrival;
                    }
                    for (int l = 0; l < system.NumberOfServers; l++)
                    {
                        if (sim[i].ArrivalTime >= system.Servers[l].FinishTime)
                        {
                            arrayOfServersCheckAvailablity[l] = true;
                            checkIfOneISFree = true; 
                        }
                    }
                    if (checkIfOneISFree)
                    {
                        while (arrayOfServersCheckAvailablity[choosenServer] != true )
                        {
                            choosenServer = ramdomVar.Next(0, system.NumberOfServers);
                        }
                        if (arrayOfServersCheckAvailablity[choosenServer] == true)
                        {
                            if (i == 1)
                            {
                                sim[i].RandomService = ramdomVar.Next(1, 100);
                                sim[i].StartTime = 0;
                                sim[i].AssignedServer = system.Servers[choosenServer];
                                for (int j = 0; j < probCounters; j++)
                                {
                                    if (system.Servers[choosenServer].TimeDistribution[j].MinRange <= sim[i].RandomService &&
                                        system.Servers[choosenServer].TimeDistribution[j].MaxRange >= sim[i].RandomService)
                                    {
                                        sim[i].ServiceTime = system.Servers[choosenServer].TimeDistribution[j].Time;
                                    }
                                }
                                sim[i].EndTime = sim[i].StartTime + sim[i].ServiceTime;
                                sim[i].TimeInQueue =0;
                                system.Servers[choosenServer].FinishTime = sim[i].EndTime;
                                system.Servers[choosenServer].TotalWorkingTime = system.Servers[choosenServer].TotalWorkingTime + sim[i].ServiceTime;
                            }
                            else
                            {
                                sim[i].AssignedServer = system.Servers[choosenServer];
                                sim[i].RandomService = ramdomVar.Next(1, 100);
                                sim[i].StartTime = system.Servers[choosenServer].FinishTime;
                                for (int j = 0; j < probCounters; j++)
                                {
                                    if (system.Servers[choosenServer].TimeDistribution[j].MinRange <= sim[i].RandomService &&
                                        system.Servers[choosenServer].TimeDistribution[j].MaxRange >= sim[i].RandomService)
                                    {
                                        sim[i].ServiceTime = system.Servers[choosenServer].TimeDistribution[j].Time;
                                    }
                                }
                                sim[i].EndTime = sim[i].StartTime + sim[i].ServiceTime;
                                sim[i].TimeInQueue = 0;
                                system.Servers[choosenServer].FinishTime = sim[i].EndTime;
                                system.Servers[choosenServer].TotalWorkingTime = system.Servers[choosenServer].TotalWorkingTime + sim[i].ServiceTime;
                            }

                        }
                    }
                    else
                    {
                        int[] arrayOfServersCheckTime = new int[system.NumberOfServers];
                        int indexOfNearestFinishServer = 0;
                        sim[i].StartTime = system.Servers[choosenServer].FinishTime;
                        for (int l = 0; l < system.NumberOfServers; l++)
                        {
                            arrayOfServersCheckTime[l] = system.Servers[l].FinishTime;
                        }
                        for (int l = 0; l < system.NumberOfServers; l++)
                        {
                            if (arrayOfServersCheckTime[indexOfNearestFinishServer] > system.Servers[l].FinishTime)
                            {
                                indexOfNearestFinishServer = l;
                            }
                        }
                        while (arrayOfServersCheckTime[indexOfNearestFinishServer] != arrayOfServersCheckTime[choosenServer] )
                        {
                            choosenServer = ramdomVar.Next(0, system.NumberOfServers);
                        }
                        sim[i].AssignedServer = system.Servers[choosenServer];
                        sim[i].RandomService = ramdomVar.Next(1, 100);
                        for (int j = 0; j < probCounters; j++)
                        {
                            if (system.Servers[choosenServer].TimeDistribution[j].MinRange <= sim[i].RandomService &&
                                system.Servers[choosenServer].TimeDistribution[j].MaxRange >= sim[i].RandomService)
                            {
                                sim[i].ServiceTime = system.Servers[choosenServer].TimeDistribution[j].Time;
                            }
                        }
                        sim[i].EndTime = sim[i].StartTime + sim[i].ServiceTime;
                        sim[i].TimeInQueue = sim[i].StartTime - sim[i].ArrivalTime;
                        system.Servers[choosenServer].FinishTime = sim[i].EndTime;
                        system.Servers[choosenServer].TotalWorkingTime = system.Servers[choosenServer].TotalWorkingTime + sim[i].ServiceTime;
                    }
                }
                decimal totalTimeSpentInQueue = 0;
                decimal numberOfCustomersInQueue = 0;
                decimal countOfUsersJoinedTheServer = 0;
                decimal simulationFinishTime = -1;
                for (int i = 1; i <= system.StoppingNumber; i++)
                {
                    if (sim[i].TimeInQueue != 0)
                    {
                        totalTimeSpentInQueue = totalTimeSpentInQueue + sim[i].TimeInQueue;
                        numberOfCustomersInQueue++;
                        
                    }
                }
                system.PerformanceMeasures.AverageWaitingTime = totalTimeSpentInQueue / system.StoppingNumber;
                system.PerformanceMeasures.WaitingProbability = numberOfCustomersInQueue / system.StoppingNumber;
                system.PerformanceMeasures.MaxQueueLength = 0;
                for (int i = 0; i < system.NumberOfServers; i++)
                {
                    countOfUsersJoinedTheServer = 0;
                    simulationFinishTime = -1;
                    for (int l = 0; l < system.NumberOfServers; l++)
                    {
                        if (system.Servers[l].FinishTime > simulationFinishTime)
                        {
                            simulationFinishTime = system.Servers[l].FinishTime;
                        }
                    }
                    system.Servers[i].IdleProbability = (simulationFinishTime - system.Servers[i].TotalWorkingTime) / simulationFinishTime;
                    for (int j = 1; j <= system.StoppingNumber; j++)
                    {
                        if (sim[j].AssignedServer.ID == system.Servers[i].ID)
                        {
                            countOfUsersJoinedTheServer++;
                        }
                    }
                    if (countOfUsersJoinedTheServer != 0)
                    {
                        system.Servers[i].AverageServiceTime = system.Servers[i].TotalWorkingTime / countOfUsersJoinedTheServer;
                    }
                    else
                    {
                        system.Servers[i].AverageServiceTime = 0;
                    }
                    system.Servers[i].Utilization = system.Servers[i].TotalWorkingTime / simulationFinishTime;
                }
                int[] countTimeAtTime = new int[Convert.ToInt32(simulationFinishTime)];
                for (int l = 1; l <= system.StoppingNumber; l++)
                {
                    if (sim[l].TimeInQueue != 0)
                    {
                        for (int n = sim[l].ArrivalTime; n < sim[l].StartTime; n++)
                        {
                            countTimeAtTime[n]++;
                        }
                    }
                }
                system.PerformanceMeasures.MaxQueueLength = countTimeAtTime.Max();
            }
            return sim;
        }

    }
}
