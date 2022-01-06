using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SFML.Graphics;
using SFML.System;

namespace WarpSpeed
{
    class StarManager
    {
        private readonly List<Star> _stars = new List<Star>();
        private readonly Random _rand = new Random();

        private readonly int _starsToSpawnPerSecond;
        private readonly float _distancePerSecond;
        private readonly int _framerate;
        private readonly float _minDistance;
        private readonly float _maxDistance;
        private readonly uint _screenWidth;
        private readonly uint _screenHeight;

        public StarManager(int starsToSpawnPerSecond, float distancePerSecond, int framerate, float minDistance, float maxDistance, uint screenWidth, uint screenHeight)
        {
            _starsToSpawnPerSecond = starsToSpawnPerSecond;
            _distancePerSecond = distancePerSecond;
            _framerate = framerate;
            _minDistance = minDistance;
            _maxDistance = maxDistance;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
        }

        public void Update()
        {
            DespawnStars();
            SpawnNewStars();
            MoveStars();
        }

        private void SpawnNewStars()
        {
            var starsToSpawnPerFrame = ((float)_starsToSpawnPerSecond / _framerate);

            if (starsToSpawnPerFrame >= 1)
            {
                for (int i = 0; i < starsToSpawnPerFrame; i++)
                {
                    SpawnNewStar();
                }
            }
            else
            {
                if (_rand.NextDouble() < starsToSpawnPerFrame)
                {
                    SpawnNewStar();
                }
            }
        }

        private void SpawnNewStar()
        {
            _stars.Add(
                new Star
                {
                    NewCoord =
                        new Vector2((float) _rand.NextDouble() - 0.5f, (float) _rand.NextDouble() - 0.5f),
                    NewDistance = (float) (_rand.NextDouble() * (_maxDistance - _minDistance) + _minDistance),
                });
        }

        private void DespawnStars()
        {
            for (int i = _stars.Count - 1; i >= 0; i--)
            {
                var star = _stars[i];
                if (star.NewCoord.Length() > Math.Sqrt(2) || star.NewDistance <= 0.0001f)
                {
                    // could make this more efficient by reshuffling stars while removing
                    //_stars.RemoveAt(i);
                    _stars[i] = _stars[_stars.Count - 1];
                    _stars.RemoveAt(_stars.Count - 1);
                }
            }
        }

        private void MoveStars()
        {
            foreach (var star in _stars)
            {
                star.Advance(_distancePerSecond / _framerate);
            }
        }

        public void Draw(RenderWindow window)
        {
            //var vertexArray = new VertexArray(PrimitiveType.Lines);
            var vertexList = new List<Vertex>();
            foreach (var star in _stars)
            {
                //star.Draw(window);
                //star.AddToVertexArray(window, vertexArray);
                star.AddToVertexList(_screenWidth, _screenHeight, vertexList);
            }
            //window.Draw(vertexArray);
            window.Draw(vertexList.ToArray(), PrimitiveType.Lines);
        }
    }

    class Star
    {
        public float OldDistance;
        public Vector2 OldCoord;
        public float NewDistance;
        public Vector2 NewCoord;

        public void Advance(float distance)
        {
            OldDistance = NewDistance;
            OldCoord = NewCoord;
            NewDistance = Math.Max(0.0001f, OldDistance - distance);
            float scaleFactor = OldDistance / NewDistance;
            NewCoord = OldCoord * scaleFactor;
        }

        public void Draw(RenderWindow window)
        {
            var oldCoordScreen = WorldToScreen(OldCoord, window.Size.X, window.Size.Y);
            var newCoordScreen = WorldToScreen(NewCoord, window.Size.X, window.Size.Y);

            var diff = newCoordScreen - oldCoordScreen;
            var normalizedDiff = diff / diff.Length();
            newCoordScreen += normalizedDiff * 1f;

            var line = new VertexArray(PrimitiveType.Lines);
            line.Append(new Vertex(new Vector2f(oldCoordScreen.X, oldCoordScreen.Y), Color.White));
            line.Append(new Vertex(new Vector2f(newCoordScreen.X, newCoordScreen.Y), Color.White));
            window.Draw(line);
        }

        public void AddToVertexArray(RenderWindow window, VertexArray vertexArray)
        {
            var oldCoordScreen = WorldToScreen(OldCoord, window.Size.X, window.Size.Y);
            var newCoordScreen = WorldToScreen(NewCoord, window.Size.X, window.Size.Y);

            var diff = newCoordScreen - oldCoordScreen;
            var normalizedDiff = diff / diff.Length();
            newCoordScreen += normalizedDiff * 1f;

            vertexArray.Append(new Vertex(new Vector2f(oldCoordScreen.X, oldCoordScreen.Y), Color.White));
            vertexArray.Append(new Vertex(new Vector2f(newCoordScreen.X, newCoordScreen.Y), Color.White));
        }
        public void AddToVertexList(uint screenWidth, uint screenHeight, List<Vertex> vertexList)
        {
            var oldCoordScreen = WorldToScreen(OldCoord, screenWidth, screenHeight);
            var newCoordScreen = WorldToScreen(NewCoord, screenWidth, screenHeight);

            var diff = newCoordScreen - oldCoordScreen;
            var normalizedDiff = diff / diff.Length();
            newCoordScreen += normalizedDiff * 1f;

            vertexList.Add(new Vertex(new Vector2f(oldCoordScreen.X, oldCoordScreen.Y), Color.White));
            vertexList.Add(new Vertex(new Vector2f(newCoordScreen.X, newCoordScreen.Y), Color.White));
        }

        private static Vector2 WorldToScreen(Vector2 coord, uint width, uint height)
        {
            coord *= width;
            coord += new Vector2(width/2, height/2);
            return coord;
        }
    }
}
