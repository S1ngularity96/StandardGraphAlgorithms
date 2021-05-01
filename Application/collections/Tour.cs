using System.Collections.Generic;
using MA.Exceptions;
namespace MA.Collections
{
    public class Tour
    {
        private HashSet<int> visited;
        private List<int> stations;
        private int BEGINS_WITH;
        private bool FINISHED;
        private float costs = 0.0f;
        private int LAST_STATION;
        private Tour()
        {

        }
        public Tour(int station)
        {
            this.stations = new List<int>();
            this.visited = new HashSet<int>();

            visited.Add(station);
            stations.Add(station);
            BEGINS_WITH = station;
            FINISHED = false;
            LAST_STATION = BEGINS_WITH;
        }

        public Tour Copy()
        {
            Tour tour = new Tour();
            tour.stations = new List<int>(this.stations);
            tour.visited = new HashSet<int>(this.visited);
            tour.BEGINS_WITH = this.BEGINS_WITH;
            tour.FINISHED = this.FINISHED;
            tour.costs = this.costs;
            tour.LAST_STATION = this.LAST_STATION;
            return tour;
        }

        public void AddStation(int station, float costs)
        {
            if (!FINISHED)
            {
                if (BEGINS_WITH == station)
                {
                    stations.Add(station);
                    visited.Add(station);
                    this.LAST_STATION = station;
                    FINISHED = true;
                }
                else if (visited.Add(station))
                {
                    this.LAST_STATION = station;
                    stations.Add(station);
                }
                else
                {
                    throw new GraphException("Tour must be distinct");
                }
                this.costs += costs;
            }
            else
            {
                throw new GraphException("Tour already finished");
            }



        }
        public float GetCosts()
        {
            return this.costs;
        }

        public bool IsFinished()
        {
            return this.FINISHED;
        }

        public int GetLastStation()
        {
            return this.LAST_STATION;
        }

        public int CountStations()
        {
            return this.stations.Count;
        }

        public bool ContainsStation(int station)
        {
            return visited.Contains(station);
        }

        public override string ToString()
        {
            string text = "";
            text = $"Tour with the cost of {this.costs} and {this.stations.Count} stations\n";
            text += "{ ";
            List<int> copy = new List<int>(stations);

            while (copy.Count > 1)
            {
                text += $"{copy[0]} -> ";
                copy.RemoveAt(0);
            }
            if (copy.Count == 1)
            {
                text += $"{copy[0]}" + " }";
            }
            return text;
        }
    }
}