using System;
using System.Collections;
using System.Collections.Generic;


class Program
{
    private static int GetStationIndex(string stationName, List<string> stationNames)
    {
        return stationNames.IndexOf(stationName);
    }

    private static string GetCommonLine(List<string> line1, List<string> line2)
    {
        foreach (string station in line1)
        {
            if (line2.Contains(station))
            {
                return station;
            }
        }
        return "";
    }
    private static void PrintPath(int[] parent, int destination, List<string> stationNames, List<List<string>> stationLines, int[,] minutesMatrix, int[,] problemsMatrix)
    {
        //build the path using a Stack (LIFO)
        Stack<int> path = new Stack<int>();
        int current = destination;

        while (parent[current] != -1)
        {
            path.Push(current);
            current = parent[current];
        }

        path.Push(current);

        int printCount = 1;

        string nextStationName = stationNames[path.Peek()];
        string commonLine = GetCommonLine(stationLines[current], stationLines[path.Peek()]);
        string stationName = stationNames[current];

        Console.WriteLine($"({printCount}) Start: {stationName} ({commonLine})");
        printCount++;

        string latestLine = commonLine;
        int minutesCount = 0;

        //Iterate through path
        foreach (int stationIndex in path)
        {
            stationName = stationNames[stationIndex];

            //Check for non-starting station
            if (path.Count > 1 && stationIndex != path.Peek())
            {
                int nextStationIndex = path.Pop();
                nextStationName = stationNames[nextStationIndex];
                commonLine = GetCommonLine(stationLines[stationIndex], stationLines[nextStationIndex]);
                int minutes = minutesMatrix[stationIndex, nextStationIndex] + problemsMatrix[stationIndex, nextStationIndex];
                minutesCount += minutes;

                if (!commonLine.Equals(latestLine))
                {
                    Console.WriteLine($"({printCount}) Change: {stationName} ({latestLine}) to {stationName} ({commonLine})");
                    latestLine = commonLine;
                    printCount++;
                }
                Console.WriteLine($"({printCount})      {stationName} ({commonLine}) to {nextStationName} ({commonLine})     {minutes} mins");
            }
            else
            {
                Console.WriteLine($"({printCount}) End: {stationName} ({commonLine})");
            }
            Console.WriteLine($"Total Journey Time: {minutesCount} minutes");
        }

    }

    private static int GetClosestVertex(int[] distances, bool[] visited)
    {
        int minDistance = int.MaxValue;
        int minIndex = -1;

        for (int v = 0; v < distances.Length; v++)
        {
            if (!visited[v] && distances[v] <= minDistance)
            {
                minDistance = distances[v];
                minIndex = v;
            }
        }

        return minIndex;
    }

    public static void Dijkstra(int[,] minutesMatrix, int[,] problemsMatrix, List<string> stationNames, List<List<string>> stationLines, int sourceStation, int destinationStation)
    {
        Dictionary<string, int> stations = new Dictionary<string, int>();
        for (int i = 0; i < stationNames.Count; i++)
        {
            stations.Add(stationNames[i], i);
        }

        int numVertices = stations.Count;

        int[] distances = new int[numVertices];
        bool[] visited = new bool[numVertices];
        int[] parent = new int[numVertices];

        for (int i = 0; i < numVertices; i++)
        {
            distances[i] = int.MaxValue;
            visited[i] = false;
            parent[i] = -1;
        }

        distances[sourceStation] = 0;

        for (int count = 0; count < numVertices - 1; count++)
        {
            int u = GetClosestVertex(distances, visited);
            visited[u] = true;

            for (int v = 0; v < numVertices; v++)
            {
                if (!visited[v] && minutesMatrix[u, v] != -1 && problemsMatrix[u, v] != -1 && (distances[u] + minutesMatrix[u, v] + problemsMatrix[u, v]) < distances[v])
                {
                    distances[v] = distances[u] + minutesMatrix[u, v] + problemsMatrix[u, v];
                    parent[v] = u;
                }
            }
        }

        Console.WriteLine($"Route: {sourceStation} to {destinationStation}");

        if (distances[destinationStation] == int.MaxValue)
        {
            Console.WriteLine("No path exists.");
        }
        else
        {
            PrintPath(parent, destinationStation, stationNames, stationLines, minutesMatrix, problemsMatrix);
        }
    }
    public static void Main()
    {
        //Matrx creation
        int[,] adjMatrix = new int[63, 63];
        for (int x = 0; x < 63; x++)
        {
            for (int y = 0; y < 63; y++)
            {
                adjMatrix[x, y] = -1;
            }
        }
        //Assign weights
        adjMatrix[0, 35] = 9;
        adjMatrix[10, 22] = 10;
        adjMatrix[11, 23] = 5;
        adjMatrix[12, 24] = 6;
        adjMatrix[13, 25] = 7;
        adjMatrix[14, 26] = 8;
        adjMatrix[15, 27] = 9;
        adjMatrix[2, 6] = 5;
        adjMatrix[3, 7] = 4;
        adjMatrix[4, 8] = 40;
        adjMatrix[15, 9] = 10;
        adjMatrix[2, 10] = 11;
        adjMatrix[3, 15] = 5;
        adjMatrix[34, 27] = 10;
        adjMatrix[25, 25] = 20;
        adjMatrix[37, 48] = 30;
        adjMatrix[3, 38] = 10;



        //Problems Matrix
        int[,] problemsMatrix = new int[63, 63];

        for (int x = 0; x < 63; x++)
        {
            for (int y = 0; y < 63; y++)
            {
                problemsMatrix[x, y] = 0;
            }
        }

        //Debug only
        /*for (int y = 0; y < 63; y++)
        {
            for (int x = 0; x < 63; x++)
            {
                Console.Write(adjMatrix[x, y]);
            }
            Console.WriteLine();
        }*/

        List<string> stationNames = new List<string>
        {
            "Aldgate",
            "Aldgate East",
            "Angel",
            "Baker Street",
            "Bank",
            "Barbican",
            "Bayswater",
            "Blackfriars",
            "Bond Street",
            "Borough",
            "Cannon Street",
            "Chancery Lane",
            "Charing Cross",
            "Covent Garden",
            "Earl's Court",
            "Edgware Road CDHC",
            "Edgware Road Bakerloo",
            "Elephant & Castle",
            "Embankment",
            "Euston",
            "Euston Square",
            "Farringdon",
            "Gloucester Road",
            "Goodge Street",
            "Great Portland Street",
            "Green Park",
            "High Street Kensington",
            "Holborn",
            "Hyde Park Corner",
            "King's Cross St Pancras",
            "Knightsbridge",
            "Lambeth North",
            "Lancaster Gate",
            "Leicester Square",
            "Liverpool Street",
            "London Bridge",
            "Mansion House",
            "Marble Arch",
            "Marylebone",
            "Monument",
            "Moorgate",
            "Notting Hill Gate",
            "Old Street",
            "Oxford Circus",
            "Paddington",
            "Piccadilly Circus",
            "Pimlico",
            "Queensway",
            "Regent's Park",
            "Russell Square",
            "Sloane Square",
            "South Kensington",
            "Southwark",
            "St James's Park",
            "St Paul's",
            "Temple",
            "Tottenham Court Road",
            "Tower Hill",
            "Vauxhall",
            "Victoria",
            "Warren Street",
            "Waterloo",
            "Westminster"
        };

        List<List<string>> stationLines = new List<List<string>>
        {
            new List<string> { "Circle", "Metropolitan" }, // Aldgate
            new List < string > { "District", "Hammersmith & City" }, // Aldgate East
            new List<string> { "Northern" }, // Angel
            new List<string> { "Bakerloo", "Circle", "Hammersmith & City", "Jubilee", "Metropolitan" }, // Baker Street
            new List<string> { "Central", "Northern", "Waterloo & City" }, // Bank
            new List < string > { "Circle", "Hammersmith & City", "Metropolitan" }, // Barbican
            new List < string > { "Circle", "District" }, // Bayswater
            new List < string > { "Circle", "District" }, // Blackfriars
            new List < string > { "Central", "Jubilee" }, // Bond Street
            new List < string > { "Northern" }, // Borough
            new List < string > { "Circle", "District" }, // Cannon Street
            new List < string > { "Central" }, // Chancery Lane
            new List < string > { "Bakerloo", "Northern" }, // Charing Cross
            new List < string > { "Piccadilly" }, // Covent Garden
            new List < string > { "District", "Piccadilly" }, // Earl's Court
            new List < string > { "Circle", "District", "Hammersmith & City" }, // Edgware Road
            new List < string > { "Bakerloo" }, // Edgware Road
            new List < string > { "Bakerloo", "Northern" }, // Elephant & Castle
            new List<string> { "Bakerloo", "Circle", "District", "Northern" }, // Embankment
            new List < string > { "Northern", "Victoria" }, // Euston
            new List < string > { "Circle", "Metropolitan", "Hammersmith & City" }, // Euston Square
            new List < string > { "Circle", "Hammersmith & City", "Metropolitan" }, // Farringdon
            new List < string > { "Circle", "District", "Piccadilly" }, // Gloucester Road
            new List < string > { "Northern" }, // Goodge Street
            new List<string> { "Circle", "Hammersmith & City", "Metropolitan" }, // Great Portland Street
            new List<string> { "Jubilee", "Piccadilly", "Victoria" }, // Green Park
            new List<string> { "Circle", "District" }, // High Street Kensington
            new List < string > { "Central", "Piccadilly" }, // Holborn
            new List<string> { "Piccadilly" }, // Hyde Park Corner
            new List<string> { "Circle", "Hammersmith & City", "Metropolitan", "Northern", "Piccadilly", "Victoria" }, // King's Cross St Pancras
            new List < string > { "Piccadilly" }, // Knightsbridge
            new List<string> { "Bakerloo" }, // Lambeth North
            new List<string> { "Central" }, // Lancaster Gate
            new List < string > { "Northern", "Piccadilly" }, // Leicester Square
            new List<string> { "Central", "Circle", "Hammersmith & City", "Metropolitan" }, // Liverpool Street
            new List < string > { "Jubilee", "Northern" }, // London Bridge
            new List<string> { "Circle", "District" }, // Mansion House
            new List<string> { "Central" }, // Marble Arch
            new List<string> { "Bakerloo" }, // Marylebone
            new List<string> { "Circle", "District" }, // Monument
            new List < string > { "Circle", "Hammersmith & City", "Metropolitan", "Northern" }, // Moorgate
            new List<string> { "Central", "Circle", "District" }, // Notting Hill Gate
            new List < string > { "Northern" }, // Old Street
            new List < string > { "Bakerloo", "Central", "Victoria" }, // Oxford Circus
            new List < string > { "Bakerloo", "Circle", "District", "Hammersmith & City" }, // Paddington
            new List < string > { "Bakerloo", "Piccadilly" }, // Piccadilly Circus
            new List < string > { "Victoria" }, // Pimlico
            new List < string > { "Central" }, // Queensway
            new List<string> { "Bakerloo" }, // Regent's Park
            new List < string > { "Piccadilly" }, // Russell Square
            new List < string > { "Circle", "District" }, // Sloane Square
            new List < string > { "Circle", "District", "Piccadilly" }, // South Kensington
            new List < string > { "Jubilee" }, // Southwark
            new List < string > { "Circle", "District" }, // St James's Park
            new List < string > { "Central" }, // St Paul's
            new List < string > { "Circle", "District" }, // Temple
            new List < string > { "Central", "Northern" }, // Tottenham Court Road
            new List < string > { "Circle", "District" }, // Tower Hill
            new List < string > { "Victoria" }, // Vauxhall
            new List < string > { "Circle", "District", "Victoria" }, // Victoria
            new List < string > { "Northern", "Victoria" }, // Warren Street
            new List < string > { "Bakerloo", "Jubilee", "Northern", "Waterloo & City" }, // Waterloo
            new List < string > { "Circle", "District", "Jubilee" } // Westminster
        };


        //MENUS------------------------------------------------------------------
        int choice = 0;
        Console.WriteLine("Welcome to the TfL tube walking route application!");

        while (choice != 3)
        {
            Console.WriteLine("Are you a customer or TfL Manager? Please enter 1 or 2:");
            Console.WriteLine("1. TfL Manager");
            Console.WriteLine("2. Customer");
            Console.WriteLine("3. Exit");
            choice = Convert.ToInt32(Console.ReadLine());
            if (choice == 1)
            {
                while (choice != 7)
                {
                    Console.WriteLine("Please select one of the options below:");
                    Console.WriteLine("1. List of impossible walking routes");
                    Console.WriteLine("2. List of delayed walking routes");
                    Console.WriteLine("3. Add walking time delays");
                    Console.WriteLine("4. Remove walking time delays");
                    Console.WriteLine("5. Set route is now impossible");
                    Console.WriteLine("6. Set route is now possible");
                    Console.WriteLine("7. Go Back");
                    choice = Convert.ToInt32(Console.ReadLine());
                    if (choice == 1)
                    {
                        // Closed routes:
                        // Northern Line: London Bridge - Monument : bridge closed
                        Console.WriteLine("Closed routes:");
                        for (int x = 0; x < 63; x++)
                        {
                            for (int y = 0; y < 63; y++)
                            {
                                if (problemsMatrix[x, y] == -1)
                                {
                                    String station1 = stationNames[x];
                                    String station2 = stationNames[y];
                                    String commonLine = GetCommonLine(stationLines[x], stationLines[y]);
                                    Console.WriteLine($"{commonLine} Line: {station1} - {station2} : route closed");
                                }
                            }
                        }


                    }
                    else if (choice == 2)
                    {
                        //Delayed routes:
                        //Victoria Line:  Oxford Circus - Warren Street : 18 min now 23 min
                        Console.WriteLine("Delayed routes:");
                        for (int x = 0; x < 63; x++)
                        {
                            for (int y = 0; y < 63; y++)
                            {
                                if (problemsMatrix[x, y] > 0)
                                {
                                    String station1 = stationNames[x];
                                    String station2 = stationNames[y];
                                    String commonLine = GetCommonLine(stationLines[x], stationLines[y]);
                                    int oldTime = adjMatrix[x, y];
                                    int newTime = oldTime + problemsMatrix[x, y];
                                    Console.WriteLine($"{commonLine} Line: {station1} - {station2} : {oldTime} min now {newTime} min");
                                }
                            }
                        }

                    }
                    else if (choice == 3)
                    {
                        Console.WriteLine("Please enter starting station name:");
                        int source = GetStationIndex(Console.ReadLine(), stationNames);
                        if (source == -1)
                        {
                            Console.WriteLine("INVALID INPUT: Station not found!");
                            continue;
                        }
                        Console.WriteLine("Please enter destination station name:");
                        int destination = GetStationIndex(Console.ReadLine(), stationNames);
                        if (destination == -1)
                        {
                            Console.WriteLine("INVALID INPUT: Station not found!");
                            continue;
                        }
                        Console.WriteLine("Please enter delay amount in minutes:");
                        int delay = Convert.ToInt32(Console.ReadLine());
                        problemsMatrix[source, destination] = delay;
                        problemsMatrix[destination, source] = delay;
                        Console.WriteLine("Delay added!");
                        Console.WriteLine("");
                    }
                    else if (choice == 4)
                    {
                        Console.WriteLine("Please enter starting station name:");
                        int source = GetStationIndex(Console.ReadLine(), stationNames);
                        if (source == -1)
                        {
                            Console.WriteLine("INVALID INPUT: Station not found!");
                            continue;
                        }
                        Console.WriteLine("Please enter destination station name:");
                        int destination = GetStationIndex(Console.ReadLine(), stationNames);
                        if (destination == -1)
                        {
                            Console.WriteLine("INVALID INPUT: Station not found!");
                            continue;
                        }
                        problemsMatrix[source, destination] = 0;
                        problemsMatrix[destination, source] = 0;
                        Console.WriteLine("Delay removed!");
                        Console.WriteLine("");

                    }
                    else if (choice == 5)
                    {
                        Console.WriteLine("Please enter starting station name:");
                        int source = GetStationIndex(Console.ReadLine(), stationNames);
                        if (source == -1)
                        {
                            Console.WriteLine("INVALID INPUT: Station not found!");
                            continue;
                        }
                        Console.WriteLine("Please enter destination station name:");
                        int destination = GetStationIndex(Console.ReadLine(), stationNames);
                        if (destination == -1)
                        {
                            Console.WriteLine("INVALID INPUT: Station not found!");
                            continue;
                        }
                        problemsMatrix[source, destination] = -1;
                        problemsMatrix[destination, source] = -1;
                        Console.WriteLine("Route Closed!");
                        Console.WriteLine("");



                    }
                    else if (choice == 6)
                    {
                        Console.WriteLine("Please enter starting station name:");
                        int source = GetStationIndex(Console.ReadLine(), stationNames);
                        if (source == -1)
                        {
                            Console.WriteLine("INVALID INPUT: Station not found!");
                            continue;
                        }
                        Console.WriteLine("Please enter destination station name:");
                        int destination = GetStationIndex(Console.ReadLine(), stationNames);
                        if (destination == -1)
                        {
                            Console.WriteLine("INVALID INPUT: Station not found!");
                            continue;
                        }
                        problemsMatrix[source, destination] = 0;
                        problemsMatrix[destination, source] = 0;
                        Console.WriteLine("Route Opened!");
                        Console.WriteLine("");

                    }
                    else if (choice != 7)
                    {
                        Console.WriteLine("INVALID INPUT: Please enter 1, 2 or 3.");
                    }
                }
            }
            else if (choice == 2)
            {
                while (choice != 3)
                {
                    Console.WriteLine("Please select one of the options below:");
                    Console.WriteLine("1. Find the fastest walking route");
                    Console.WriteLine("2. Display Tube Information about a station");
                    Console.WriteLine("3. Go Back");
                    choice = Convert.ToInt32(Console.ReadLine());
                    if (choice == 1)
                    {
                        Console.WriteLine("Please enter starting station name:");
                        int source = GetStationIndex(Console.ReadLine(), stationNames);
                        if (source == -1)
                        {
                            Console.WriteLine("INVALID INPUT: Station not found!");
                            continue;
                        }
                        Console.WriteLine("Please enter destination station name:");
                        int destination = GetStationIndex(Console.ReadLine(), stationNames);
                        if (destination == -1)
                        {
                            Console.WriteLine("INVALID INPUT: Station not found!");
                            continue;
                        }
                        Console.WriteLine("");
                        Dijkstra(adjMatrix, problemsMatrix, stationNames, stationLines, source, destination);
                        Console.WriteLine("");
                    }
                    else if (choice == 2)
                    {
                        Console.WriteLine("Please enter station name:");
                        int stationIndex = GetStationIndex(Console.ReadLine(), stationNames);
                        if (stationIndex == -1)
                        {
                            Console.WriteLine("INVALID INPUT: Station not found!");
                            continue;
                        }
                        string stationName = stationNames[stationIndex];
                        List<string> currentStationLines = stationLines[stationIndex];
                        Console.WriteLine("");
                        Console.WriteLine($"Station Name: {stationName}");
                        string joinedLines = string.Join(", ", currentStationLines);
                        Console.WriteLine($"Station Lines: {joinedLines}");
                        Console.WriteLine("");

                    }
                    else if (choice != 3)
                    {
                        Console.WriteLine("INVALID INPUT: Please enter 1, 2 or 3.");
                    }
                }
                choice = 0;
            }
            else if (choice != 3)
            {
                Console.WriteLine("INVALID INPUT: Please enter 1, 2 or 3.");
            }
        }

    }
}