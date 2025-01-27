using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("¡Bienvenidos al juego de carreras de autos!");

        // Lista de autos
        string[] autos = { "Auto 1", "Auto 2", "Auto 3", "Auto 4" };

        // Tareas para cada auto
        List<Task> tareasAutos = new List<Task>();
        CancellationTokenSource cts = new CancellationTokenSource();

        foreach (var auto in autos)
        {
            // Crear tarea para cada auto
            var tareaAuto = Task.Factory.StartNew(async () =>
            {
                Random rnd = new Random();
                int progreso = 0;

                while (progreso < 100)
                {
                    await Task.Delay(rnd.Next(200, 500)); // Simula el avance del auto
                    progreso += rnd.Next(10, 20); // Incrementa el progreso
                    Console.WriteLine($"{auto} ha avanzado al {progreso}%");
                }

                if (progreso >= 100)
                {
                    Console.WriteLine($"{auto} ha terminado la carrera.");
                }

            }, cts.Token, TaskCreationOptions.AttachedToParent, TaskScheduler.Default);

            // Continuaciones para manejar resultados
            tareaAuto.Unwrap().ContinueWith(t =>
            {
                Console.WriteLine($"{auto} terminó exitosamente.");
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            tareaAuto.Unwrap().ContinueWith(t =>
            {
                Console.WriteLine($"{auto} fue cancelado.");
            }, TaskContinuationOptions.OnlyOnCanceled);

            tareasAutos.Add(tareaAuto.Unwrap()); // Usamos Unwrap para manejar tareas anidadas
        }

        // Continuación para detectar el primer auto que termina
        Task.Factory.ContinueWhenAny(tareasAutos.ToArray(), ganador =>
        {
            Console.WriteLine("¡Tenemos un ganador!");
        });

        // Esperar todas las tareas
        try
        {
            await Task.WhenAll(tareasAutos);
        }
        catch
        {
            Console.WriteLine("Ocurrió un error durante la carrera.");
        }

       
        Console.WriteLine("La carrera ha terminado.");
    }
}
