using System.Collections.Generic;
using Unigine;

namespace UnigineApp.data.Scripts.AI
{
    internal class NavigationMaker {

        private int Radius;
        private NavigationMesh NavMesh;
        private List<Obstacle> ObstaclesList;
        private List<List<dvec3>> PathPoints;

        public NavigationMaker(int Radius, Node NavMesh, List<Node> Obstacles, List<List<Node>> Pathways) {

            this.Radius = Radius;
            this.NavMesh = NavMesh as NavigationMesh;
            PathCreate(Obstacles, Pathways);
        }

        public void RenderNavigation() => NavMesh.RenderVisualizer();
        public void RenderObstacles() => ObstaclesList.ForEach(Obj => Obj.RenderVisualizer());

        public List<List<dvec3>> GetAllPaths() => PathPoints;
        public List<dvec3> GetPath(int Num) => PathPoints[Num];
        public List<dvec3> GetPath(List<dvec3> list) {

            List<dvec3> Paths = new();
            PathRoute Path = new();
            Path.Radius = Radius;

            for (int i = 0; i < list.Count - 1; i++) {
                Path.Create2D(list[i], list[i + 1]);
                for (int j = 0; j < Path.NumPoints - 1; j++) {
                    Paths.Add(Path.GetPoint(i));
                }
            }
            return Paths;
        }

        private void PathCreate(List<Node> Obstacles, List<List<Node>> Pathways) {

            Obstacles.ForEach(Obj => ObstaclesList.Add(Obj as Obstacle));
            foreach (var Paths in Pathways) {
                List<dvec3> Pathing = new();
                Paths.ForEach(Obj => Pathing.Add(Obj.WorldPosition));
                PathPoints.Add(Pathing);
            }    
        }
    }
}
