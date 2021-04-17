#pragma warning disable 618
using System;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using model;
using networking.dto;
using services;

namespace networking
{
    public class ContestMgmtClientRpcWorker : IContestMgmtObserver
    {
        private readonly IContestMgmtServices _server;
        
        private readonly TcpClient _connection;

        private readonly NetworkStream _stream = null!;

        private readonly IFormatter _formatter = null!;

        private volatile bool _connected;

        public ContestMgmtClientRpcWorker(IContestMgmtServices server, TcpClient connection)
        {
            _server = server;
            _connection = connection;
            try
            {
                _stream = connection.GetStream();
                _formatter = new BinaryFormatter();
                _connected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public void Run()
        {
            while (_connected)
            {
                try
                {
                    var request = _formatter!.Deserialize(_stream);
                    var response = HandleRequest((Request) request);
                    if (response == null)
                        throw new NullReferenceException("Response is null!");
                    SendResponse(response);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }

                try
                {
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }

            try
            {
                _stream.Close();
                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public void NewRegistration(Registration registration)
        {
            var registrationDto = (RegistrationDTO) registration;
            Console.WriteLine("New Registration " + registration);
            try
            {
                SendResponse(new Response {Type = ResponseType.NewRegistration, Data = registrationDto});
            }
            catch (Exception e)
            {
                throw new ContestMgmtException("Error sending " + e);
            }
        }

        private Response? HandleRequest(Request request)
        {
            Response? response = null;

            string handlerName = $"Handle{request.Type}";
            Console.WriteLine($"Handler Name: {handlerName}");

            try
            {
                var handlerMethod = GetType().GetMethod(handlerName, BindingFlags.NonPublic |
                                                                     BindingFlags.Instance);
                Console.WriteLine($"{handlerMethod?.Name} request...");
                response = (Response?) handlerMethod?.Invoke(this, new object?[] {request});
                Console.WriteLine($"Method {handlerName} invoked");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return response;
        }

        private void SendResponse(Response response)
        {
            Console.WriteLine("Sending response " + response);
            _formatter.Serialize(_stream, response);
            _stream.Flush();
        }

        private Response HandleLogin(Request request)
        {
            if (request.Data == null)
                throw new NullReferenceException();
            var organiserDto = (OrganiserDTO) request.Data;
            var organiser = (Organiser) organiserDto;
            try
            {
                lock (_server)
                {
                    _server.Login(organiser, this);
                }

                return new Response {Type = ResponseType.Ok};
            }
            catch (ContestMgmtException e)
            {
                _connected = false;
                return new Response {Type = ResponseType.Error, Data = e.Message};
            }
        }

        private Response HandleLogout(Request request)
        {
            if (request.Data == null)
                throw new NullReferenceException();
            var organiserDto = (OrganiserDTO) request.Data;
            var organiser = (Organiser) organiserDto;
            try
            {
                lock (_server)
                {
                    _server.Logout(organiser, this);
                }

                _connected = false;
                return new Response {Type = ResponseType.Ok};
            }
            catch (ContestMgmtException e)
            {
                return new Response {Type = ResponseType.Error, Data = e.Message};
            }
        }

        private Response HandleGetCompetitionTypes(Request request)
        {
            if (request.Data == null)
                throw new NullReferenceException();
            var competitionType = (string) request.Data;
            try
            {
                string[] comps;
                lock (_server)
                {
                    var res = _server.GetCompetitionTypes(competitionType);
                    comps = res.ToArray();
                }

                return new Response
                {
                    Type = ResponseType.GetCompetitionTypes,
                    Data = comps
                };
            }
            catch (ContestMgmtException e)
            {
                return new Response {Type = ResponseType.Error, Data = e.Message};
            }
        }

        private Response HandleGetParticipantsByCompetition(Request request)
        {
            if (request.Data == null)
                throw new NullReferenceException();
            var competitionDto = (CompetitionDTO) request.Data;
            var competition = (Competition) competitionDto;
            try
            {
                ParticipantDTO[] parts;
                lock (_server)
                {
                    var res = _server.GetParticipantsByCompetition(competition);
                    parts = (from part in res
                            select (ParticipantDTO) part).ToArray();
                }

                return new Response
                {
                    Type = ResponseType.GetParticipantsByCompetition,
                    Data = parts
                };
            }
            catch (ContestMgmtException e)
            {
                return new Response {Type = ResponseType.Error, Data = e.Message};
            }
        }

        private Response HandleGetCompetitionsAndCountsByString(Request request)
        {
            if (request.Data == null)
                throw new NullReferenceException();
            var (competitionType, ageCategory) = ((string, string)) request.Data;
            try
            {
                CompetitionCountDTO[] res;
                lock (_server)
                {
                    res = _server.GetCompetitionsAndCounts(competitionType, ageCategory).Values
                        .Select(t => (CompetitionCountDTO) t)
                        .ToArray();
                }
                
                return new Response
                {
                    Type = ResponseType.GetCompetitionsAndCountsByString,
                    Data = res
                };
            }
            catch (ContestMgmtException e)
            {
                return new Response {Type = ResponseType.Error, Data = e.Message};
            }
        }

        private Response HandleGetParticipantIdsByName(Request request)
        {
            if (request.Data == null)
                throw new NullReferenceException();
            var (firstName, lastName) = ((string, string)) request.Data;
            try
            {
                long[] ids;
                lock (_server)
                {
                    ids = _server.GetParticipantIdsByName(firstName, lastName).ToArray();
                }

                return new Response
                {
                    Type = ResponseType.GetParticipantIdsByName,
                    Data = ids
                };
            }
            catch (ContestMgmtException e)
            {
                return new Response {Type = ResponseType.GetParticipantIdsByName, Data = e.Message};
            }
        }

        private Response HandleGetAgeCategoriesForParticipant(Request request)
        {
            if (request.Data == null)
                throw new NullReferenceException();
            var psDto = (ParticipantStringDTO) request.Data;
            var (participant, competitionType) = ((Participant, string)) psDto;
            try
            {
                string[] ageCats;
                lock (_server)
                {
                    ageCats = _server.GetAgeCategoriesForParticipant(participant, competitionType).ToArray();
                }

                return new Response
                {
                    Type = ResponseType.GetAgeCategoriesForParticipant,
                    Data = ageCats
                };
            }
            catch (ContestMgmtException e)
            {
                return new Response {Type = ResponseType.GetAgeCategoriesForParticipant, Data = e.Message};
            }
        }

        private Response HandleAddRegistration(Request request)
        {
            if (request.Data == null)
                throw new NullReferenceException();
            var registrationDto = (RegistrationDTO) request.Data;
            var registration = (Registration) registrationDto;
            try
            {
                lock (_server)
                {
                    _server.AddRegistration(registration);
                }

                return new Response {Type = ResponseType.Ok};
            }
            catch (ContestMgmtException e)
            {
                return new Response {Type = ResponseType.Error, Data = e.Message};
            }
        }
    }
}