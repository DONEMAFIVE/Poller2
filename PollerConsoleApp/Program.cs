using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace PollerConsoleApp
{
    class Program
    {
        public static string period = null; //this shit

        static async Task Main(string[] args)
        {
            using (dbService db = new dbService())
            {
                var dt = db.csharp_test
                    .FromSqlRaw($"SELECT * FROM `csharp_test` WHERE `id`=(SELECT MAX(`id`)FROM `csharp_test`)")
                    .ToList();
                Console.WriteLine("Последняя сохраненная группа:");

                foreach (table_csharp_test u in dt)
                {
                    Console.Write(
                        $"момент нажатия:\n{u.moment_presses}нажатая клавиша:\n{u.keystroke}интервал опроса:{u.duration_sec} сек.");
                }
            }

            var pollingPeriod = TimeSpan.FromSeconds(1);

            var greetings = Task.Run(async () =>
            {
                Console.WriteLine(
                    "\n\nДобро пожаловать в бесполезное приложение для опроса!"); //Welcome to useless polling application!

                // display latest previous batch of polled units here (loaded from database)

                Console.WriteLine(
                    "Выберите интервал опроса измеряемый в секундах и нажмите клавишу Enter"); //Select pollling interval measured in seconds and press Enter

                pollingPeriod = await SelectPollingPeriod(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
                Console.WriteLine(
                    "Нажмите ESC для выхода или любую другую клавишу для входа в систему\n"); //Press ESC to exit or any other key to log it
            });
            await greetings;

            var cancelSource = new CancellationTokenSource();
            var transmitter = new Subject<Data.PollerUnit>();

            var poller = new PolledConsumerWrapper(transmitter, pollingPeriod, onNewBatchObserved, cancelSource);
            period = pollingPeriod.Seconds.ToString();
            var polleach = Task.Run(() =>
            {
                while (!cancelSource.IsCancellationRequested)
                {
                    var key = Console.ReadKey();

                    if (key.Key == ConsoleKey.Escape)
                    {
                        cancelSource.Cancel();
                        break;
                    }

                    transmitter.OnNext(new Data.PollerUnit()
                    {
                        Timestamp = DateTimeOffset.Now,
                        Content = key.KeyChar.ToString()
                    });
                }
            });
            await polleach;
            cancelSource.Cancel();
        }

        private static async Task<TimeSpan> SelectPollingPeriod(TimeSpan defaultPeriod, TimeSpan awaitUserInput)
        {
            var pollingPeriod = defaultPeriod;

            var intervalSelectionCanclellation = new CancellationTokenSource();

            var waitIntervalSelection = Task.Run(() =>
            {
                var parsedSucessfully = false;
                while (!parsedSucessfully & !intervalSelectionCanclellation.IsCancellationRequested)
                {
                    var intervalAsString = Console.ReadLine();
                    parsedSucessfully = Int32.TryParse(intervalAsString, out var parsedInterval);
                    if (parsedSucessfully)
                        pollingPeriod = TimeSpan.FromSeconds(parsedInterval);
                }

                ;
                intervalSelectionCanclellation.Cancel();
            });


            var delayForUserEnter = Task.Delay(awaitUserInput, intervalSelectionCanclellation.Token);

            await Task.WhenAny(waitIntervalSelection, delayForUserEnter);
            intervalSelectionCanclellation.Cancel();

            Console.WriteLine($"Период опроса {pollingPeriod.Seconds} cекунд");

            if (delayForUserEnter.IsCompleted)
                Console.WriteLine("Нажмите клавишу Enter, чтобы продолжить"); //Press Enter to continue
            return pollingPeriod;
        }

        private static void onNewBatchObserved(List<Data.IPollerUnit> polled)
        {
            Console.WriteLine(
                $"\nЗагрузка...\n{polled.Aggregate(new StringBuilder(), (sb, item) => sb.Append($"{item.Timestamp.ToString("HH:mm:ss:ffff")} {item.Content} \n"))}");

            var _moment = polled.Aggregate(new StringBuilder(),
                (sb, item) => sb.Append($"{item.Timestamp.ToString("HH:mm:ss:ffff")}\n"));
            var _keystroke = polled.Aggregate(new StringBuilder(), (sb, item) => sb.Append($"{item.Content}\n"));

            using (dbService db = new dbService())
            {
                table_csharp_test group = new table_csharp_test
                    {moment_presses = _moment.ToString(), keystroke = _keystroke.ToString(), duration_sec = period};
                db.csharp_test.Add(group);
                db.SaveChanges();
            }
        }
    }
}