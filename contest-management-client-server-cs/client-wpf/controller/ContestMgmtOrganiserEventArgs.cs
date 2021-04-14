using System;

namespace client_wpf.controller
{
    public enum ContestMgmtOrganiserEvent
    {
        NewRegistration
    }
    
    public class ContestMgmtOrganiserEventArgs : EventArgs
    {
        public ContestMgmtOrganiserEventArgs(ContestMgmtOrganiserEvent organiserEvent, object data)
        {
            OrganiserEventType = organiserEvent;
            Data = data;
        }

        public ContestMgmtOrganiserEvent OrganiserEventType { get; }

        public object Data { get; }
    }
}