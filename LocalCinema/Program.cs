using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;

namespace LocalCinema
{
    internal class Program
    {
        [MTAThread]  // Change from [STAThread] to [MTAThread]
        static void Main(string[] args)
        {
            // Explicitly create the DispatcherQueue on the current (main) thread
            DispatcherQueueController.CreateOnCurrentThread();

            Application.Start(p =>
            {
                var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
                System.Threading.SynchronizationContext.SetSynchronizationContext(context);
                new App();
            });
        }
    }
}