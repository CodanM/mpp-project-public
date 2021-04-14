#pragma warning disable 618
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using model;
using networking.dto;
using networking.utils;
using services;

namespace networking
{
    public class ContestMgmtServerRpcProxy : IContestMgmtServices
    {
        private readonly string _host;

        private readonly int _port;

        private IContestMgmtObserver? _client;

        private NetworkStream _stream = null!;

        private IFormatter _formatter = null!;

        private TcpClient _connection = null!;

        private BlockingQueue<Response> _responseQueue = new();

        private volatile bool _finished;

        public ContestMgmtServerRpcProxy(string host, int port)
        {
            _host = host;
            _port = port;
        }
        
        public void Login(Organiser organiser, IContestMgmtObserver client)
        {
            InitializeConnection();
            
            var organiserDto = (OrganiserDTO) organiser;
            SendRequest(new Request {Type = RequestType.Login, Data = organiserDto});

            var response = ReadResponse() ?? throw new NullReferenceException();

            if (response.Type == ResponseType.Error)
            {
                CloseConnection();
                throw new ContestMgmtException(response.Data?.ToString());
            }

            _client = client;
        }

        public void Logout(Organiser organiser, IContestMgmtObserver client)
        {
            var organiserDto = (OrganiserDTO) organiser;
            SendRequest(new Request {Type = RequestType.Logout, Data = organiserDto});

            var response = ReadResponse() ?? throw new NullReferenceException();
            CloseConnection();

            if (response.Type == ResponseType.Error)
                throw new ContestMgmtException(response.Data?.ToString());
        }

        public IList<Competition> GetCompetitionsByString(string competitionType, string ageCategory)
        {
            var stringTuple = (competitionType, ageCategory);
            SendRequest(new Request {Type = RequestType.GetCompetitionsByString, Data = stringTuple});

            var response = ReadResponse();
            if (response?.Data == null)
                throw new NullReferenceException();
            var compDtos = (CompetitionDTO[]) response.Data;

            return compDtos.Select(compDto => (Competition) compDto).ToList();
        }

        public IList<Participant> GetParticipantsByCompetition(Competition competition)
        {
            var competitionDto = (CompetitionDTO) competition;
            SendRequest(new Request {Type = RequestType.GetParticipantsByCompetition, Data = competitionDto});

            var response = ReadResponse();
            if (response?.Data == null)
                throw new NullReferenceException();
            var partDtos = (ParticipantDTO[]) response.Data;

            return partDtos.Select(partDto => (Participant) partDto).ToList();
        }

        public IDictionary<long, (Competition, int)> GetCompetitionsAndCountsByString(string competitionType, 
            string ageCategory)
        {
            var strings = (competitionType, ageCategory);
            SendRequest(new Request {Type = RequestType.GetCompetitionsAndCountsByString, Data = strings});

            var response = ReadResponse();
            if (response?.Data == null)
                throw new NullReferenceException();
            var ccDtos = (CompetitionCountDTO[]) response.Data;
            
            return ccDtos.Select(ccDto => ((Competition, int)) ccDto).ToDictionary(t => t.Item1.Id);
        }

        public IList<long> GetParticipantIdsByName(string firstName, string lastName)
        {
            var strings = (firstName, lastName);
            SendRequest(new Request {Type = RequestType.GetParticipantIdsByName, Data = strings});

            var response = ReadResponse();
            if (response?.Data == null)
                throw new NullReferenceException();
            var ids = (long[]) response.Data;
            
            return ids.ToList();
        }

        public IList<string> GetAgeCategoriesForParticipant(Participant participant, string competitionType)
        {
            var psDto = (ParticipantStringDTO) (participant, competitionType);
            SendRequest(new Request {Type = RequestType.GetAgeCategoriesForParticipant, Data = psDto});

            var response = ReadResponse();
            if (response?.Data == null)
                throw new NullReferenceException();
            var ageCategories = (string[]) response.Data;
            
            return ageCategories.ToList();
        }

        public void AddRegistration(Registration registration)
        {
            throw new NotImplementedException();
        }

        private void Run()
        {
            while (!_finished)
            {
                try
                {
                    Response response;
                    if (!_finished)
                        response = _formatter.Deserialize(_stream) as Response ?? throw new NullReferenceException();
                    else
                        break;
                    if (response.IsUpdate())
                        HandleUpdate(response);
                    else
                    {
                        _responseQueue.Enqueue(response);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Reading error {e}");
                }
            }
        }

        private void InitializeConnection()
        {
            try
            {
                _connection = new TcpClient(_host, _port);
                _stream = _connection.GetStream();
                _formatter = new BinaryFormatter();
                _responseQueue = new BlockingQueue<Response>();
                _finished = false;
                StartReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private void CloseConnection()
        {
            _finished = true;
            try
            {
                _stream.Close();
                _connection.Close();
                _responseQueue.Close();
                _client = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private void StartReader()
        {
            var t = new Thread(Run);
            t.Start();
        }

        private void SendRequest(Request request)
        {
            try
            {
                _formatter.Serialize(_stream, request);
                _stream.Flush();
            }
            catch (Exception e)
            {
                throw new ContestMgmtException($"Error sending object {e}");
            }
        }

        private Response? ReadResponse()
        {
            Response? response = null;

            try
            {
                response = _responseQueue.Dequeue();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return response;
        }

        private void HandleUpdate(Response response)
        {
            if (response.Type == ResponseType.NewRegistration)
            {
                
            }
        }
    }
}