using System;
using System.Net.Sockets;
using System.Threading;
using networking;
using persistence;
using persistence.database;
using services;

namespace server_console
{
    class StartServer
    {
        private static void Main(string[] args)
        {
            IParticipantRepository participantRepository = new ParticipantDbRepository();
            ICompetitionRepository competitionRepository = new CompetitionDbRepository();
            IOrganiserRepository organiserRepository = new OrganiserDbRepository();
            IRegistrationRepository registrationRepository = new RegistrationDbRepository();

            IContestMgmtServices services = new ContestMgmtServerImpl(participantRepository, competitionRepository,
                organiserRepository, registrationRepository);

            var server = new SerialContestMgmtServer("127.0.0.1", 2602, services);
            server.Start();
            Console.WriteLine("Server started...");
            Console.ReadLine();
        }
    }
    
    public class SerialContestMgmtServer : ConcurrentServer
    {
        private readonly IContestMgmtServices _server;

        private ContestMgmtClientRpcWorker _worker;
        
        public SerialContestMgmtServer(string host, int port, IContestMgmtServices server) : base(host, port)
        {
            _server = server;
            Console.WriteLine("SerialContestMgmtServer...");
        }

        protected override Thread CreateWorker(TcpClient client)
        {
            _worker = new ContestMgmtClientRpcWorker(_server, client);
            return new Thread(_worker.Run);
        }
    }
}