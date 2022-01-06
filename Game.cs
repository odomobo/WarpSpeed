using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Threading;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WarpSpeed;

namespace Orbits
{
    class Game
    {
        private readonly RenderWindow _window;
        private readonly int _frameRate;

        private readonly StarManager _starManager;

        public Game(uint width, uint height, int framerate)
        {
            _window = new RenderWindow(new VideoMode(width, height), "Warp Speed", Styles.Titlebar | Styles.Close);

            _window.KeyPressed += KeyPressed;
            _window.Closed += Closed;
            _window.SetMouseCursor(new Cursor(Cursor.CursorType.Arrow));
            _window.MouseButtonPressed += MouseButtonPressed;
            _window.MouseButtonReleased += MouseButtonReleased;

            _frameRate = framerate;

            _starManager = new StarManager(50000, 400, framerate, 100, 200, width, height);
        }

        public void Run()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (_window.IsOpen)
            {
                UpdateState();
                DrawWindow();

                GC.Collect();

                var elapsed = stopwatch.Elapsed;
                var frameTime = TimeSpan.FromSeconds(1) / _frameRate;
                if (elapsed < frameTime)
                    Thread.Sleep(frameTime-elapsed);

                stopwatch.Restart();
            }
        }

        private void UpdateState()
        {
            _starManager.Update();
        }

        private void DrawWindow()
        {
            _window.DispatchEvents();

            _window.Clear(Color.Black);

            _starManager.Draw(_window);

            _window.Display();
        }

        #region Callbacks

        private void MouseButtonPressed(object? sender, MouseButtonEventArgs e)
        {
            //if (e.Button == Mouse.Button.Left)
            // TODO
        }

        private void MouseButtonReleased(object? sender, MouseButtonEventArgs e)
        {
            //if (e.Button == Mouse.Button.Left)
            // TODO
        }

        private void KeyPressed(object? sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                _window.Close();
            }
        }

        private void Closed(object? sender, EventArgs e) => _window.Close();

        #endregion Callbacks

    }
}
