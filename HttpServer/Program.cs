using System;
using System.Net;
using Newtonsoft.Json;
using HttpServer.Core.Request;
using HttpServer.Core.Routing;
using HttpServer.Core.Server;
using HttpServer.DAL;
using HttpServer.Models;
using HttpServer.RouteCommands.Messages;
using HttpServer.RouteCommands.Users;
using HttpServer.RouteCommands.Transactions;
using HttpServer.RouteCommands.Cards;
using HttpServer.RouteCommands.Deck;
using HttpServer.RouteCommands.Battle;
using HttpMethod = HttpServer.Core.Request.HttpMethod;
//using HttpMethod = HttpServer.Core.Request.HttpMethod;

namespace HttpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //var messageRepository = new InMemoryMessageRepository();
            //var userRepository = new InMemoryUserRepository();

            // use the DB connection instead
            // better: fetch the connection string from a config file -> see next semester ;)
            // we use an extra Database class to coordinate the creation of the repositories and manage the DB connection
            var db = new Database("Host=localhost;Port=5431;Username=postgres;Password=postgres;Database=mtcgdb");

            var messageManager = new MessageManager(db.MessageRepository, db.UserRepository);
            var manager = new Manager(db.CardRepository, db.UserRepository, db.TradeRepository);
            var battleManager = new BattleManager(db.CardRepository, db.UserRepository);

            var identityProvider = new MessageIdentityProvider(db.UserRepository);
            var routeParser = new IdRouteParser();

            var router = new Router(routeParser, identityProvider);
            RegisterRoutes(router, messageManager, manager, battleManager);

            var httpServer = new HttpServer.Core.Server.HttpServer(IPAddress.Any, 10001, router);
            httpServer.Start();
        }

        private static void RegisterRoutes(Router router, IMessageManager messageManager, Imanager manager, BattleManager battleManager)
        {
            // public routes
            router.AddRoute(HttpMethod.Post, "/sessions", (r, p) => new LoginCommand(manager, Deserialize<Credentials>(r.Payload)));
            router.AddRoute(HttpMethod.Post, "/users", (r, p) => new RegisterCommand(messageManager, Deserialize<Credentials>(r.Payload)));

            // protected routes
            router.AddProtectedRoute(HttpMethod.Post, "/packages", (r, p) => new AddPackageCommand(manager, Deserialize<List<PreCard>>(r.Payload)));
            router.AddProtectedRoute(HttpMethod.Post, "/transactions/packages", (r, p) => new AcquirePackageCommand(manager));
            router.AddProtectedRoute(HttpMethod.Get, "/cards", (r, p) => new ShowAcquiredCardsCommand(manager));
            router.AddProtectedRoute(HttpMethod.Get, "/deck", (r, p) => new ShowMainDeckCommand(manager));
            router.AddProtectedRoute(HttpMethod.Get, "/deck?format=plain", (r, p) => new ShowMainDeckPlainCommand(manager));
            router.AddProtectedRoute(HttpMethod.Put, "/deck", (r, p) => new ConfigureMainDeckCommand(manager, Deserialize<List<string>>(r.Payload)));
            router.AddProtectedRoute(HttpMethod.Get, "/score", (r, p) => new GetScoreboardCommand(manager));
            router.AddProtectedRoute(HttpMethod.Post, "/battles", (r, p) => new BattleCommand(manager, battleManager));

            router.AddProtectedRoute(HttpMethod.Get, "/messages", (r, p) => new ListMessagesCommand(messageManager));
            router.AddProtectedRoute(HttpMethod.Post, "/messages", (r, p) => new AddMessageCommand(messageManager, r.Payload));
            router.AddProtectedRoute(HttpMethod.Get, "/messages/{id}", (r, p) => new ShowMessageCommand(messageManager, int.Parse(p["id"])));
            router.AddProtectedRoute(HttpMethod.Put, "/messages/{id}", (r, p) => new UpdateMessageCommand(messageManager, int.Parse(p["id"]), r.Payload));
            router.AddProtectedRoute(HttpMethod.Delete, "/messages/{id}", (r, p) => new RemoveMessageCommand(messageManager, int.Parse(p["id"])));
        }

        private static T Deserialize<T>(string payload) where T : class
        {
            var deserializedData = JsonConvert.DeserializeObject<T>(payload);
            return deserializedData;
        }
    }
}
