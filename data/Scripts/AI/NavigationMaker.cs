using System.Collections.Generic;
using Unigine;

namespace UnigineApp.data.Scripts.AI
{
    internal class NavigationMaker {

        private int Radius;
        private NavigationMesh NavMesh;
        private List<Obstacle> ObstaclesList;
        private List<dvec3> PathPoints;

        public NavigationMaker(int Radius, Node NavMesh, List<Node> Obstacles, List<Node> Pathways) {

            this.Radius = Radius;
            this.NavMesh = NavMesh as NavigationMesh;
            PathCreate(Obstacles, Pathways);
        }

        public void RenderNavigation() => NavMesh.RenderVisualizer();
        public void RenderObstacles() => ObstaclesList.ForEach(Obj => Obj.RenderVisualizer());


        public List<dvec3> GetPath() => PathPoints;
        public List<dvec3> GetModifiedPath(List<dvec3> list) {

            List<dvec3> Paths = new();
            PathRoute Path = new();
            Path.Radius = Radius;

            for (int i = 0; i < list.Count - 1; i++) {
                Path.Create2D(list[i], list[i + 1]);
                for (int j = 0; j < Path.NumPoints - 1; j++) {

                    Paths.Add(Path.GetPoint(j));
                }
            }
            return Paths;
        }

        private void PathCreate(List<Node> Obstacles, List<Node> Pathways) {

            PathPoints = new();
            ObstaclesList = new();

            Obstacles.ForEach(Obj => ObstaclesList.Add(Obj as Obstacle));
            Pathways.ForEach(Paths => PathPoints.Add(Paths.WorldPosition));
        }
    }
}
