
//    This file is part of Dynamical Systems Net.
//
//    Dynamical Systems Net is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Dynamical Systems Netis distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public License
//    along with Dynamical Systems Net.  If not, see <http://www.gnu.org/licenses/>.

using DynamicalSystemsNet.Core;
using DynamicalSystemsNet.Examples.Systems;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create program.
            Program program = new Program();

            // Create a system.
            LinearCoupleSystem system = new LinearCoupleSystem(new RungeKuttaIntegrator());

            // Run program.
            program.Run(system);
        }


        private volatile DynamicalSystem _system;
        private double _timeStepSize;
        private Semaphore _exitPrintLoop;
        private SemaphoreSlim _systemLock;

        public Program()
        {
            _system = null;
            _timeStepSize = 0.00001;
            _exitPrintLoop = new Semaphore(0, 1);
            _systemLock = new SemaphoreSlim(1, 1);
        }

        public void Run(DynamicalSystem system)
        {
            // Store system.
            _system = system;

            if (system == null)
            {
                Console.WriteLine("Warning: System is null.");
            }

            // Inform user.
            Console.WriteLine("Initializing tasks for new system...");

            // Create tasks.
            Task integratorTask = new Task(ThreadMainIntegrator);
            Task printerTask = new Task(ThreadMainPrintState);

            // Run tasks.
            Console.WriteLine("Starting tasks... press any key to exit early.");
            Thread.Sleep(1000);
            printerTask.Start();
            integratorTask.Start();

            // Block this thread until input.
            ConsoleKeyInfo input = Console.ReadKey(true);
            Console.WriteLine("Shutting down tasks...");

            // Tell print task to quit.
            _exitPrintLoop.Release();

            // Tell integrator task to quit.
            _system = null;

            // Join on tasks.
            integratorTask.Wait();
            printerTask.Wait();

            // Done.
            Thread.Sleep(1000);
            Console.WriteLine("Application finished.");
        }

        private void ThreadMainPrintState()
        {
            while (!_exitPrintLoop.WaitOne(400))
            {
                var system = _system;
                if (system != null)
                {
                    // Grab system state.
                    _systemLock.Wait();

                    // Convert to string.
                    string systemAsString = system.ToString();

                    // Release lock on system state.
                    _systemLock.Release();

                    // Print to console.
                    System.Console.WriteLine(systemAsString);
                }
            }

        }

        private void ThreadMainIntegrator()
        {
            while (true)
            {
                var system = this._system;
                if (system == null)
                {
                    // Quit thread if no system is alive.
                    break;
                }

                //
                // Advance the system.
                //
                _systemLock.Wait();
                system.Step(_timeStepSize);
                _systemLock.Release();

            }

        }
    }
}
