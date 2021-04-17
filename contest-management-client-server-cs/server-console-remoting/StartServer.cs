using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using persistence;
using persistence.database;
using services;

namespace server_console_remoting
{
    class StartServer
    {
        private static void Main(string[] args)
        {
            var serverProvider = new BinaryServerFormatterSinkProvider {TypeFilterLevel = TypeFilterLevel.Full};
            var clientProvider = new BinaryClientFormatterSinkProvider();
            IDictionary props = new Hashtable();

            props["port"] = 2602;
            var channel = new TcpChannel(props, clientProvider, serverProvider);
            ChannelServices.RegisterChannel(channel, false);

            IParticipantRepository participantRepository = new ParticipantDbRepository();
            ICompetitionRepository competitionRepository = new CompetitionDbRepository();
            IOrganiserRepository organiserRepository = new OrganiserDbRepository();
            IRegistrationRepository registrationRepository = new RegistrationDbRepository();

            var server = new ContestMgmtServerImpl(participantRepository, competitionRepository,
                organiserRepository, registrationRepository);

            RemotingServices.Marshal(server, "ContestMgmt");
            
            Console.WriteLine("Server started...");
            Console.WriteLine("Press <Enter> to exit...");
            Console.ReadLine();
        }
    }
    
    // public class SerialContestMgmtServer : ConcurrentServer
    // {
    //     private readonly IContestMgmtServices _server;
    //
    //     private ContestMgmtClientRpcWorker _worker;
    //     
    //     public SerialContestMgmtServer(string host, int port, IContestMgmtServices server) : base(host, port)
    //     {
    //         _server = server;
    //         Console.WriteLine("SerialContestMgmtServer...");
    //     }
    //
    //     protected override Thread CreateWorker(TcpClient client)
    //     {
    //         _worker = new ContestMgmtClientRpcWorker(_server, client);
    //         return new Thread(_worker.Run);
    //     }
    // }
}