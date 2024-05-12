using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


class Program
{
    private static int GetStationIndex(string stationName, List<string> stationNames)
    {
        return stationNames.IndexOf(stationName);
    }

    private static string GetCommonLine(List<string> line1, List<string> line2)
    {
        return line1.Intersect(line2).FirstOrDefault();
    }
    private static void PrintPath(List<int> parent, int destination, List<string> stationNames, List<List<string>> stationLines, List<List<int>> minutesMatrix, List<List<int>> problemsMatrix)
    {
        int[] path = new int[parent.Count];
        int pathIndex = 0;
        int current = destination;

        while (parent[current] != -1)
        {
            path[pathIndex++] = current;
            current = parent[current];
        }
        path[pathIndex] = current;

        int printCount = 1;

        int nextStationIndex = path[pathIndex - 1];
        string nextStationName = stationNames[nextStationIndex];
        string commonLine = GetCommonLine(stationLines[current], stationLines[nextStationIndex]);
        //Get Station Name at Index in List
        string stationName = stationNames[current];

        //Debug
        /*string stationNameTest = stationNames[0];
        Console.WriteLine($"TEST: Station 1: {stationNameTest}");*/

        Console.WriteLine($"({printCount}) Start: {stationName} ({commonLine})");
        printCount++;

        string latestLine = commonLine;
        int minutesCount = 0;

        //Iterate through path
        for (int i = pathIndex; i >= 0; i--, printCount++)
        {
            int stationIndex = path[i];
            stationName = stationNames[stationIndex];

            if (i > 0)
            {
                nextStationIndex = path[i - 1];
                nextStationName = stationNames[nextStationIndex];
                commonLine = GetCommonLine(stationLines[stationIndex], stationLines[nextStationIndex]);
                int minutes = minutesMatrix[stationIndex][nextStationIndex] + +problemsMatrix[stationIndex][nextStationIndex];
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
        }
        Console.WriteLine($"Total Journey Time: {minutesCount} minutes");
    }

    private static int GetClosestVertex(List<int> distances, List<bool> visited)
    {
        int minDistance = int.MaxValue;
        int minIndex = -1;

        for (int v = 0; v < distances.Count; v++)
        {
            if (!visited[v] && distances[v] <= minDistance)
            {
                minDistance = distances[v];
                minIndex = v;
            }
        }

        return minIndex;
    }

    public static void Dijkstra(List<List<int>> minutesMatrix, List<List<int>> problemsMatrix, List<string> stationNames, List<List<string>> stationLines, int sourceStation, int destinationStation)
    {
        // Ensure all Lists have the same number of elements (stations)
        if (minutesMatrix.Count != problemsMatrix.Count || minutesMatrix.Count != stationNames.Count || stationNames.Count != stationLines.Count)
        {
            throw new ArgumentException("All input Lists must have the same number of elements (stations)");
        }
        //Console.WriteLine($"minutesMatrix: {minutesMatrix.Count}, problemsMatrix: {problemsMatrix.Count}, stationNames: {stationNames.Count}, stationLines: {stationLines.Count}"); //Default: 63 by 63

        int numVertices = minutesMatrix.Count;
        //Console.WriteLine(numVertices);

        List<int> distances = new List<int>(numVertices);
        for (int i = 0; i < numVertices; i++)
        {
            distances.Add(0); 
        }
        List<bool> visited = new List<bool>(numVertices);
        for (int i = 0; i < numVertices; i++)
        {
            visited.Add(true);
        }
        List<int> parent = new List<int>(numVertices);
        for (int i = 0; i < numVertices; i++)
        {
            parent.Add(0);
        }
        //Console.WriteLine(distances[0]);

        for (int i = 0; i < numVertices; i++)
        {
            //Console.WriteLine(distances[i]);
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
                // Handle potential uneven List lengths within a matrix
                if (v < minutesMatrix[u].Count && minutesMatrix[u][v] != -1 && problemsMatrix[u][v] != -1 && (distances[u] + minutesMatrix[u][v] + problemsMatrix[u][v]) < distances[v])
                {
                    distances[v] = distances[u] + minutesMatrix[u][v] + problemsMatrix[u][v];
                    parent[v] = u;
                }
            }
        }

        Console.WriteLine($"Route: {stationNames[sourceStation]} to {stationNames[destinationStation]}");

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
        //Matrix creation
        List<List<int>> adjMatrix = new List<List<int>>();
        for (int x = 0; x < 63; x++)
        {
            adjMatrix.Add(new List<int>(Enumerable.Repeat(-1, 63)));
        }


        //Assign weights new - based on ADJ Matrix excel spreadsheet across x axis (A,B,C,...)
        //Aldgate (Circle, Metropolitan)
        adjMatrix[0][34] = 2;
        adjMatrix[0][57] = 1;
        //Aldgate East (District, Hammersmith & City)
        adjMatrix[1][34] = 2;
        adjMatrix[1][57] = 2;
        //Angel (Northern)
        adjMatrix[2][29] = 2;
        adjMatrix[2][42] = 2;
        //Baker Street (Bakerloo, Circle, Hammersmith & City, Jubilee, Metropolitan)
        adjMatrix[3][8] = 2;
        adjMatrix[3][16] = 1;
        adjMatrix[3][24] = 2;
        adjMatrix[3][38] = 1;
        adjMatrix[3][57] = 2;
        //Bank (Central, Northern, Waterloo & City)
        adjMatrix[4][34] = 10;
        adjMatrix[4][35] = 2;
        adjMatrix[4][40] = 2;
        adjMatrix[4][54] = 2;
        adjMatrix[4][61] = 5;
        //Barbican (Circle, Hammersmith & City, Metropolitan)
        adjMatrix[5][21] = 1;
        adjMatrix[5][40] = 1;
        //Bayswater (Circle, District)
        adjMatrix[6][41] = 2;
        adjMatrix[6][44] = 2;
        //Blackfriars (Circle, District)
        adjMatrix[7][36] = 1;
        adjMatrix[7][55] = 1;
        //Bond Street (Central, Jubilee)
        adjMatrix[8][3] = 2;
        adjMatrix[8][25] = 2;
        adjMatrix[8][37] = 1;
        adjMatrix[8][43] = 1;
        //Borough (Northern)
        adjMatrix[9][17] = 2;
        adjMatrix[9][35] = 1;
        //Cannon Street (Circle, District)
        adjMatrix[10][36] = 1;
        adjMatrix[10][39] = 1;
        //Chancery Lane (Central)
        adjMatrix[11][27] = 1;
        adjMatrix[11][54] = 2;
        //Charing Cross (Bakerloo, Northern)
        adjMatrix[12][18] = 1;
        adjMatrix[12][33] = 1;
        adjMatrix[12][45] = 1;
        //Covent Garden (Piccadilly)
        adjMatrix[13][27] = 2;
        adjMatrix[13][33] = 1;
        //Earl's Court (District, Piccadilly)
        adjMatrix[14][22] = 2;
        adjMatrix[14][26] = 3;
        //Edgware Road (Circle, District, Hammersmith & City)
        adjMatrix[15][3] = 2;
        adjMatrix[15][44] = 2;
        //Edgware Road (Bakerloo)
        adjMatrix[16][38] = 1;
        adjMatrix[16][44] = 2;
        //Elephant & Castle (Bakerloo, Northern)
        adjMatrix[17][9] = 2;
        adjMatrix[17][31] = 3;
        //Embankment (Bakerloo, Circle, District, Northern)
        adjMatrix[18][12] = 1;
        adjMatrix[18][55] = 1;
        adjMatrix[18][61] = 1;
        adjMatrix[18][62] = 1;
        //Euston (Northern, Victoria)
        adjMatrix[19][29] = 1;
        adjMatrix[19][60] = 1;
        //Euston Square (Circle, Metropolitan, Hammersmith & City)
        adjMatrix[20][24] = 1;
        adjMatrix[20][29] = 2;
        //Farringdon (Circle, Hammersmith & City, Metropolitan)
        adjMatrix[21][5] = 1;
        adjMatrix[21][29] = 3;
        //Gloucester Road (Circle, District, Piccadilly)
        adjMatrix[22][14] = 2;
        adjMatrix[22][26] = 2;
        adjMatrix[22][51] = 1;
        //Goodge Street (Northern)
        adjMatrix[23][56] = 1;
        adjMatrix[23][60] = 1;
        //Great Portland Street (Circle, Hammersmith & City, Metropolitan)
        adjMatrix[24][3] = 2;
        adjMatrix[24][20] = 1;
        //Green Park (Jubilee, Piccadilly, Victoria)
        adjMatrix[25][8] = 2;
        adjMatrix[25][28] = 2;
        adjMatrix[25][43] = 2;
        adjMatrix[25][45] = 1;
        adjMatrix[25][59] = 2;
        adjMatrix[25][62] = 2;
        //High Street Kensington (Circle, District)
        adjMatrix[26][14] = 3;
        adjMatrix[26][22] = 2;
        adjMatrix[26][40] = 2;
        //Holborn (Central, Piccadilly)
        adjMatrix[27][11] = 1;
        adjMatrix[27][13] = 1;
        adjMatrix[27][49] = 1;
        adjMatrix[27][56] = 2;
        //Hyde Park Corner (Piccadilly)
        adjMatrix[28][25] = 2;
        adjMatrix[28][30] = 1;
        //King's Cross St Pancras (Circle, Hammersmith & City, Metropolitan, Northern, Piccadilly, Victoria)
        adjMatrix[29][2] = 2;
        adjMatrix[29][19] = 1;
        adjMatrix[29][20] = 2;
        adjMatrix[29][21] = 3;
        adjMatrix[29][49] = 2;
        //Knightsbridge (Piccadilly)
        adjMatrix[30][28] = 1;
        adjMatrix[30][51] = 2;
        //Lambeth North (Bakerloo)
        adjMatrix[31][17] = 2;
        adjMatrix[31][61] = 1;
        //Lancaster Gate (Central)
        adjMatrix[32][37] = 2;
        adjMatrix[32][47] = 1;
        //Leicester Square (Northern, Piccadilly)
        adjMatrix[33][12] = 1;
        adjMatrix[33][13] = 1;
        adjMatrix[33][45] = 1;
        adjMatrix[33][56] = 1;
        //Liverpool Street (Central, Circle, Hammersmith & City, Metropolitan)
        adjMatrix[34][0] = 2;
        adjMatrix[34][1] = 2;
        adjMatrix[34][4] = 2;
        adjMatrix[34][40] = 1;
        //London Bridge (Jubilee, Northern)
        adjMatrix[35][4] = 2;
        adjMatrix[35][9] = 1;
        adjMatrix[35][52] = 2;
        //Mansion House (Circle, District)
        adjMatrix[36][7] = 2;
        adjMatrix[36][10] = 1;
        //Marble Arch (Central)
        adjMatrix[37][8] = 1;
        adjMatrix[37][32] = 2;
        //Marylebone (Bakerloo)
        adjMatrix[38][3] = 1;
        adjMatrix[38][16] = 1;
        //Monument (Circle, District)
        adjMatrix[39][4] = 1;
        adjMatrix[39][10] = 1;
        //Moorgate (Circle, Hammersmith & City, Metropolitan, Northern)
        adjMatrix[40][4] = 2;
        adjMatrix[40][5] = 1;
        adjMatrix[40][34] = 1;
        adjMatrix[40][42] = 1;
        //Notting Hill Gate (Central, Circle, District)
        adjMatrix[41][6] = 1;
        adjMatrix[41][26] = 2;
        adjMatrix[41][47] = 1;
        //Old Street (Northern)
        adjMatrix[42][2] = 3;
        adjMatrix[42][40] = 1;
        //Oxford Circus (Bakerloo, Central, Victoria)
        adjMatrix[43][8] = 1;
        adjMatrix[43][25] = 2;
        adjMatrix[43][45] = 2;
        adjMatrix[43][48] = 2;
        adjMatrix[43][56] = 1;
        adjMatrix[43][60] = 2;
        //Paddington (Bakerloo, Circle, District, Hammersmith & City)
        adjMatrix[44][6] = 2;
        adjMatrix[44][15] = 2;
        adjMatrix[44][16] = 2;
        //Piccadilly Circus(Bakerloo, Piccadilly)
        adjMatrix[45][12] = 2;
        adjMatrix[45][25] = 1;
        adjMatrix[45][33] = 1;
        adjMatrix[45][43] = 2;
        //Pimlico(Victoria)
        adjMatrix[46][58] = 1;
        adjMatrix[46][59] = 2;
        //Queensway (Central)
        adjMatrix[47][32] = 2;
        adjMatrix[47][41] = 1;
        //Regent's Park (Bakerloo)
        adjMatrix[48][3] = 2;
        adjMatrix[48][43] = 2;
        //Russell Square (Piccadilly)
        adjMatrix[49][27] = 2;
        adjMatrix[49][29] = 2;
        //Sloane Square (Circle, District)
        adjMatrix[50][51] = 2;
        adjMatrix[50][59] = 2;
        //South Kensington(Circle, District, Piccadilly)
        adjMatrix[51][22] = 1;
        adjMatrix[51][30] = 2;
        adjMatrix[51][50] = 2;
        //Southwark (Jubilee)
        adjMatrix[52][35] = 2;
        adjMatrix[52][61] = 1;
        //St James's Park (Circle, District)
        adjMatrix[53][59] = 1;
        adjMatrix[53][62] = 2;
        //St Paul's (Central)
        adjMatrix[54][4] = 2;
        adjMatrix[54][11] = 2;
        //Temple (Circle, District)
        adjMatrix[55][7] = 1;
        adjMatrix[55][18] = 1;
        //Tottenham Court Road(Central, Northern)
        adjMatrix[56][23] = 1;
        adjMatrix[56][27] = 1;
        adjMatrix[56][33] = 1;
        adjMatrix[56][43] = 1;
        //Tower Hill (Circle, District)
        adjMatrix[57][0] = 1;
        adjMatrix[57][1] = 2;
        adjMatrix[57][39] = 2;
        //Vauxhall (Victoria)
        adjMatrix[58][46] = 1;
        //Victoria (Circle, District, Victoria)
        adjMatrix[59][25] = 2;
        adjMatrix[59][46] = 2;
        adjMatrix[59][50] = 2;
        adjMatrix[59][53] = 1;
        //Warren Street (Northern, Victoria)
        adjMatrix[60][19] = 1;
        adjMatrix[60][23] = 1;
        adjMatrix[60][43] = 2;
        //Waterloo (Bakerloo, Jubilee, Northern, Waterloo & City)
        adjMatrix[61][4] = 4;
        adjMatrix[61][18] = 1;
        adjMatrix[61][31] = 1;
        adjMatrix[61][52] = 1;
        adjMatrix[61][62] = 1;
        //Westminster (Circle, District, Jubilee)
        adjMatrix[62][18] = 1;
        adjMatrix[62][25] = 2;
        adjMatrix[62][53] = 2;
        adjMatrix[62][61] = 1;


        //Problems Matrix
        List<List<int>> problemsMatrix = new List<List<int>>();
        for (int x = 0; x < 63; x++)
        {
            problemsMatrix.Add(new List<int>(Enumerable.Repeat(0, 63)));
        }


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
        Console.WriteLine("Welcome to the TfL tube route application!");

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
                    Console.WriteLine("1. List of impossible routes");
                    Console.WriteLine("2. List of delayed routes");
                    Console.WriteLine("3. Add time delays");
                    Console.WriteLine("4. Remove time delays");
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
                                if (problemsMatrix[x][y] == -1)
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
                                if (problemsMatrix[x][y] > 0)
                                {
                                    String station1 = stationNames[x];
                                    String station2 = stationNames[y];
                                    String commonLine = GetCommonLine(stationLines[x], stationLines[y]);
                                    int oldTime = adjMatrix[x][y];
                                    int newTime = oldTime + problemsMatrix[x][y];
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
                        problemsMatrix[source][destination] = delay;
                        problemsMatrix[destination][source] = delay;
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
                        problemsMatrix[source][destination] = 0;
                        problemsMatrix[destination][source] = 0;
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
                        problemsMatrix[source][destination] = -1;
                        problemsMatrix[destination][source] = -1;
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
                        problemsMatrix[source][destination] = 0;
                        problemsMatrix[destination][source] = 0;
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
                    Console.WriteLine("1. Find the fastest route");
                    Console.WriteLine("2. Display Tube Information about a station");
                    Console.WriteLine("3. Go Back");
                    choice = Convert.ToInt32(Console.ReadLine());
                    if (choice == 1)
                    {
                        Console.WriteLine("Please enter starting station name:");
                        int source = GetStationIndex(Console.ReadLine(), stationNames);
                        //Console.WriteLine($"Source Index: {source}"); //Debug
                        if (source == -1)
                        {
                            Console.WriteLine("INVALID INPUT: Station not found!");
                            continue;
                        }
                        Console.WriteLine("Please enter destination station name:");
                        int destination = GetStationIndex(Console.ReadLine(), stationNames);
                        //Console.WriteLine($"Destination Index: {destination}"); //Debug
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